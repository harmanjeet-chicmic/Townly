using System.Numerics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using RealEstateInvesting.Application.Common.Interfaces;

namespace RealEstateInvesting.Infrastructure.Blockchain;

/// <summary>
/// T-REX Factory: deployTREXSuite(salt, tokenDetails, claimDetails). Parses TREXSuiteDeployed event for token and IR addresses.
/// </summary>
public sealed class TREXFactoryContractService : ITREXFactoryContractService
{
    // deployTREXSuite(string,(address,string,string,uint8,address,address,address[],address[],address[],bytes[]),(uint256[],address[],uint256[][]))
    private const string TrexFactoryAbi = @"[
      {
        ""inputs"": [
          {""internalType"": ""string"", ""name"": ""_salt"", ""type"": ""string""},
          {
            ""components"": [
              {""internalType"": ""address"", ""name"": ""owner"", ""type"": ""address""},
              {""internalType"": ""string"", ""name"": ""name"", ""type"": ""string""},
              {""internalType"": ""string"", ""name"": ""symbol"", ""type"": ""string""},
              {""internalType"": ""uint8"", ""name"": ""decimals"", ""type"": ""uint8""},
              {""internalType"": ""address"", ""name"": ""irs"", ""type"": ""address""},
              {""internalType"": ""address"", ""name"": ""ONCHAINID"", ""type"": ""address""},
              {""internalType"": ""address[]"", ""name"": ""irAgents"", ""type"": ""address[]""},
              {""internalType"": ""address[]"", ""name"": ""tokenAgents"", ""type"": ""address[]""},
              {""internalType"": ""address[]"", ""name"": ""complianceModules"", ""type"": ""address[]""},
              {""internalType"": ""bytes[]"", ""name"": ""complianceSettings"", ""type"": ""bytes[]""}
            ],
            ""internalType"": ""struct ITREXFactory.TokenDetails"",
            ""name"": ""_tokenDetails"",
            ""type"": ""tuple""
          },
          {
            ""components"": [
              {""internalType"": ""uint256[]"", ""name"": ""claimTopics"", ""type"": ""uint256[]""},
              {""internalType"": ""address[]"", ""name"": ""issuers"", ""type"": ""address[]""},
              {""internalType"": ""uint256[][]"", ""name"": ""issuerClaims"", ""type"": ""uint256[][]""}
            ],
            ""internalType"": ""struct ITREXFactory.ClaimDetails"",
            ""name"": ""_claimDetails"",
            ""type"": ""tuple""
          }
        ],
        ""name"": ""deployTREXSuite"",
        ""outputs"": [],
        ""stateMutability"": ""nonpayable"",
        ""type"": ""function""
      },
      {
        ""anonymous"": false,
        ""inputs"": [
          {""indexed"": true, ""internalType"": ""address"", ""name"": ""_token"", ""type"": ""address""},
          {""indexed"": false, ""internalType"": ""address"", ""name"": ""_ir"", ""type"": ""address""},
          {""indexed"": false, ""internalType"": ""address"", ""name"": ""_irs"", ""type"": ""address""},
          {""indexed"": false, ""internalType"": ""address"", ""name"": ""_tir"", ""type"": ""address""},
          {""indexed"": false, ""internalType"": ""address"", ""name"": ""_ctr"", ""type"": ""address""},
          {""indexed"": false, ""internalType"": ""address"", ""name"": ""_mc"", ""type"": ""address""},
          {""indexed"": true, ""internalType"": ""string"", ""name"": ""_salt"", ""type"": ""string""}
        ],
        ""name"": ""TREXSuiteDeployed"",
        ""type"": ""event""
      }
    ]";

    private readonly TRexOptions _options;
    private readonly ILogger<TREXFactoryContractService> _logger;

    public TREXFactoryContractService(IOptions<TRexOptions> options, ILogger<TREXFactoryContractService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<TREXSuiteDeployResult> DeployTREXSuiteAsync(TREXSuiteDeployRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.PrivateKey) || string.IsNullOrWhiteSpace(_options.TrexFactoryAddress))
            throw new InvalidOperationException("TRex:PrivateKey and TRex:TrexFactoryAddress are required for deployTREXSuite.");

        var web3 = CreateWeb3WithSigner();
        var factoryAddress = NormalizeAddress(_options.TrexFactoryAddress);
        var contract = web3.Eth.GetContract(TrexFactoryAbi, factoryAddress);
        var deployFn = contract.GetFunction("deployTREXSuite");

        var owner = NormalizeAddress(request.OwnerAddress);
        var irs = NormalizeAddress(request.IrsAddress);
        var onchainId = NormalizeAddress(request.OnChainIdAddress);
        var irAgents = (request.IrAgents ?? Array.Empty<string>()).Select(NormalizeAddress).ToArray();
        var tokenAgents = (request.TokenAgents ?? Array.Empty<string>()).Select(NormalizeAddress).ToArray();
        var complianceModules = (request.ComplianceModules ?? Array.Empty<string>()).Select(NormalizeAddress).ToArray();
        var complianceSettings = (request.ComplianceSettings ?? Array.Empty<byte[]>()).ToList();
        if (complianceSettings.Count == 0 && complianceModules.Length > 0)
            complianceSettings = complianceModules.Select(_ => Array.Empty<byte>()).ToList();

        var claimTopic = string.IsNullOrWhiteSpace(request.ClaimTopicHex)
            ? BigInteger.Zero
            : BigInteger.Parse(request.ClaimTopicHex.Trim().Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
        var claimTopics = new[] { claimTopic };
        var issuers = new[] { NormalizeAddress(request.ClaimIssuerAddress) };
        var issuerClaims = new[] { claimTopics };

        object tokenDetails = new object[]
        {
            owner,
            request.TokenName ?? "",
            request.TokenSymbol ?? "",
            request.Decimals,
            irs,
            onchainId,
            irAgents,
            tokenAgents,
            complianceModules,
            complianceSettings
        };
        object claimDetails = new object[] { claimTopics, issuers, issuerClaims };

        var txHash = await deployFn.SendTransactionAsync(
            web3.TransactionManager.Account!.Address,
            new HexBigInteger(0),
            new HexBigInteger(8000000),
            null,
            request.Salt ?? "",
            tokenDetails,
            claimDetails).ConfigureAwait(false);

        _logger.LogInformation("TREXFactory deployTREXSuite tx sent: {TxHash}", txHash);

        var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash).ConfigureAwait(false);
        var waitCount = 0;
        while (receipt == null && waitCount < 60)
        {
            await Task.Delay(2000, cancellationToken).ConfigureAwait(false);
            receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash).ConfigureAwait(false);
            waitCount++;
        }
        if (receipt == null)
            throw new InvalidOperationException("Transaction receipt not found for " + txHash);

        var (tokenAddress, irAddress) = ParseTREXSuiteDeployed(receipt);
        if (string.IsNullOrEmpty(tokenAddress) || string.IsNullOrEmpty(irAddress))
            throw new InvalidOperationException("TREXSuiteDeployed event not found in receipt for " + txHash);

        return new TREXSuiteDeployResult
        {
            TokenAddress = tokenAddress,
            IdentityRegistryAddress = irAddress,
            TransactionHash = txHash
        };
    }

    private static (string? TokenAddress, string? IrAddress) ParseTREXSuiteDeployed(TransactionReceipt receipt)
    {
        if (receipt.Logs == null) return (null, null);
        foreach (var log in receipt.Logs)
        {
            if (log.Topics == null || log.Topics.Length < 2) continue;
            var tokenHex = log.Topics[1].ToString();
            if (tokenHex.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) tokenHex = tokenHex[2..];
            tokenHex = tokenHex.TrimStart('0').PadLeft(40, '0');
            var tokenAddress = "0x" + tokenHex.ToLowerInvariant();
            var data = log.Data ?? "";
            if (data.Length < 2 + 32 * 5 * 2) continue; // hex: 0x + 160 hex chars
            var hex = data.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? data[2..] : data;
            byte[] dataBytes = Convert.FromHexString(hex);
            if (dataBytes.Length < 32 * 5) continue;
            var irBytes = dataBytes.AsSpan(12, 20).ToArray();
            var irAddress = "0x" + Convert.ToHexString(irBytes).ToLowerInvariant();
            return (tokenAddress, irAddress);
        }
        return (null, null);
    }

    private Web3 CreateWeb3WithSigner()
    {
        var key = _options.PrivateKey!.Trim();
        if (key.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) key = key[2..];
        var account = new Nethereum.Web3.Accounts.Account("0x" + key);
        return new Web3(account, _options.RpcUrl);
    }

    private static string NormalizeAddress(string address)
    {
        var a = (address ?? "").Trim();
        return a.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? a : "0x" + a;
    }
}
