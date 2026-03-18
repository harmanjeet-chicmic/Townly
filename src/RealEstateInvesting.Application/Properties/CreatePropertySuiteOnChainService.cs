using System.Numerics;
using Microsoft.Extensions.Logging;
using RealEstateInvesting.Application.Common.Interfaces;

namespace RealEstateInvesting.Application.Properties;

/// <summary>
/// Runs the full property creation flow in one go: deploy suite, vault, register, identities, mint, bind compliance.
/// </summary>
public sealed class CreatePropertySuiteOnChainService : ICreatePropertySuiteOnChainService
{
    private readonly ITREXFactoryContractService _trexFactory;
    private readonly IRealEstateVaultFactoryContractService _vaultFactory;
    private readonly IRealEstateRegistryContractService _registry;
    private readonly IIdentityRegistryContractService _identityRegistry;
    private readonly ITokenReaderContractService _tokenReader;
    private readonly IComplianceTokenContractService _complianceToken;
    private readonly IModularComplianceContractService _modularCompliance;
    private readonly IBlockchainSettings _blockchainSettings;
    private readonly IKycClaimTopicProvider _kycClaimTopic;
    private readonly ILogger<CreatePropertySuiteOnChainService> _logger;

    private const int DelayMs = 2000;
    private const byte TokenDecimals = 6;

    public CreatePropertySuiteOnChainService(ITREXFactoryContractService trexFactory,
                                             IRealEstateVaultFactoryContractService vaultFactory,
                                             IRealEstateRegistryContractService registry,
                                             IIdentityRegistryContractService identityRegistry,
                                             ITokenReaderContractService tokenReader,
                                             IComplianceTokenContractService complianceToken,
                                             IModularComplianceContractService modularCompliance,
                                             IBlockchainSettings blockchainSettings,
                                             IKycClaimTopicProvider kycClaimTopic,
                                             ILogger<CreatePropertySuiteOnChainService> logger)
    {
        _trexFactory = trexFactory;
        _vaultFactory = vaultFactory;
        _registry = registry;
        _identityRegistry = identityRegistry;
        _tokenReader = tokenReader;
        _complianceToken = complianceToken;
        _modularCompliance = modularCompliance;
        _blockchainSettings = blockchainSettings;
        _kycClaimTopic = kycClaimTopic;
        _logger = logger;
    }

    public async Task<CreatePropertySuiteResult> CreatePropertySuiteAsync(CreatePropertySuiteRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var deployer = _blockchainSettings.DeployerAddress;
            if (string.IsNullOrEmpty(deployer))
                throw new InvalidOperationException("Deployer address not available (TRex:PrivateKey required).");
            if (string.IsNullOrWhiteSpace(_blockchainSettings.ClaimIssuerAddress))
                throw new InvalidOperationException("TRex:ClaimIssuerAddress is required for property suite creation.");

            var identityTxHashes = new List<string>();

            // Deploy T-REX Suite
            _logger.LogInformation("Deploying T-REX suite for property {PropertyId}...", request.PropertyId);
            var salt = $"{request.PropertyId}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
            var symbolSuffix = request.PropertyId.Contains('-', StringComparison.Ordinal) ? request.PropertyId.Split('-').Last() : request.PropertyId;
            var trexRequest = new TREXSuiteDeployRequest
            {
                Salt = salt,
                OwnerAddress = deployer,
                TokenName = $"Property {request.PropertyId}",
                TokenSymbol = $"P{symbolSuffix}",
                Decimals = TokenDecimals,
                IrAgents = new[] { deployer },
                TokenAgents = new[] { deployer },
                ClaimTopicHex = _kycClaimTopic.GetKycClaimTopicHex(),
                ClaimIssuerAddress = _blockchainSettings.ClaimIssuerAddress!
            };
            var suiteResult = await _trexFactory.DeployTREXSuiteAsync(trexRequest, cancellationToken).ConfigureAwait(false);
            var tokenAddress = suiteResult.TokenAddress;
            var irAddress = suiteResult.IdentityRegistryAddress;

            await Task.Delay(DelayMs, cancellationToken).ConfigureAwait(false);

            #region  Deploy Vault

            _logger.LogInformation("Deploying vault for token {Token}...", tokenAddress);
            var pricePerShareRaw = (BigInteger.Parse(request.PropertyPriceUsdc ?? "0") * BigInteger.Pow(10, TokenDecimals)).ToString();
            var deployVaultTxHash = await _vaultFactory.DeployVaultAsync(tokenAddress, _blockchainSettings.StablecoinAddress, pricePerShareRaw, deployer, cancellationToken).ConfigureAwait(false);
            await Task.Delay(DelayMs, cancellationToken).ConfigureAwait(false);

            var vaultAddress = await _vaultFactory.GetVaultAsync(tokenAddress, cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrEmpty(vaultAddress) || vaultAddress == "0x0000000000000000000000000000000000000000")
                throw new InvalidOperationException("Vault address not found after deployVault.");

            #endregion

            #region  Register in Master Registry

            _logger.LogInformation("Registering property in registry...");
            var metadataUri = request.MetadataUri ?? $"ipfs://meta-{request.PropertyId}";
            var registerTxHash = await _registry.RegisterPropertyAsync(deployer, metadataUri, tokenAddress, vaultAddress, cancellationToken).ConfigureAwait(false);

            #endregion


            #region  Setup identities on the token's IR

            _logger.LogInformation("Verifying identities on token IR...");
            await EnsureVerifiedAsync(irAddress, vaultAddress, request.VaultIdentityAddress, request.IdentityCountryCode, identityTxHashes, cancellationToken).ConfigureAwait(false);
            await EnsureVerifiedAsync(irAddress, _blockchainSettings.RealEstateMarketplaceAddress, request.MarketplaceIdentityAddress, request.IdentityCountryCode, identityTxHashes, cancellationToken).ConfigureAwait(false);
            if (request.AdditionalIdentities != null)
            {
                foreach (var entry in request.AdditionalIdentities)
                    await EnsureVerifiedAsync(irAddress, entry.WalletAddress, entry.IdentityContractAddress, entry.CountryCode, identityTxHashes, cancellationToken).ConfigureAwait(false);
            }
            #endregion


            #region  Unpause, mint, bind compliance

            _logger.LogInformation("Unpause, mint, bind compliance...");
            string? unpauseTxHash = null;
            var isPaused = await _tokenReader.GetPausedAsync(tokenAddress, cancellationToken).ConfigureAwait(false);
            if (isPaused)
            {
                unpauseTxHash = await _tokenReader.UnpauseAsync(tokenAddress, cancellationToken).ConfigureAwait(false);
                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
            }

            var mintAmountRaw = (BigInteger.Parse(request.MintAmount ?? "0") * BigInteger.Pow(10, TokenDecimals)).ToString();
            var mintTxHash = await _complianceToken.MintAsync(tokenAddress, vaultAddress, mintAmountRaw, cancellationToken).ConfigureAwait(false);

            string? bindComplianceTxHash = null;
            var complianceAddress = await _tokenReader.GetComplianceAsync(tokenAddress, cancellationToken).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(complianceAddress) && complianceAddress != "0x0000000000000000000000000000000000000000")
            {
                var isBound = await _modularCompliance.IsModuleBoundAsync(complianceAddress, vaultAddress, cancellationToken).ConfigureAwait(false);
                if (!isBound)
                    bindComplianceTxHash = await _modularCompliance.AddModuleAsync(complianceAddress, vaultAddress, cancellationToken).ConfigureAwait(false);
            }

            #endregion

            return new CreatePropertySuiteResult
            {
                TokenAddress = tokenAddress,
                VaultAddress = vaultAddress,
                IdentityRegistryAddress = irAddress,
                DeploySuiteTxHash = suiteResult.TransactionHash,
                DeployVaultTxHash = deployVaultTxHash,
                RegisterPropertyTxHash = registerTxHash,
                IdentityTxHashes = identityTxHashes,
                UnpauseTxHash = unpauseTxHash,
                MintTxHash = mintTxHash,
                BindComplianceTxHash = bindComplianceTxHash
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating property suite");
            throw;
        }
       
    }

    private async Task EnsureVerifiedAsync(string irAddress, string walletAddress, string identityAddress, ushort countryCode, List<string> identityTxHashes, CancellationToken cancellationToken)
    {
        try
        {
            var txHash = await _identityRegistry.RegisterIdentityOnRegistryAsync(irAddress, walletAddress, identityAddress, countryCode, cancellationToken).ConfigureAwait(false);
            identityTxHashes.Add(txHash);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "registerIdentity failed for {Wallet} on IR {IR}; continuing.", walletAddress, irAddress);
        }
    }

}
