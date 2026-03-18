using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using RealEstateInvesting.Application.Common.Interfaces;
using System.Numerics;

namespace RealEstateInvesting.Infrastructure.Blockchain;

/// <summary>
/// Real Estate Vault Factory: deployVault(token, stablecoin, pricePerShare, admin), vaults(token).
/// </summary>
public sealed class RealEstateVaultFactoryContractService : IRealEstateVaultFactoryContractService
{
    private const string VaultFactoryAbi = @"[
      {
        ""inputs"": [
          {""internalType"": ""address"", ""name"": ""_token"", ""type"": ""address""},
          {""internalType"": ""address"", ""name"": ""_stablecoin"", ""type"": ""address""},
          {""internalType"": ""uint256"", ""name"": ""_pricePerShare"", ""type"": ""uint256""},
          {""internalType"": ""address"", ""name"": ""_admin"", ""type"": ""address""}
        ],
        ""name"": ""deployVault"",
        ""outputs"": [],
        ""stateMutability"": ""nonpayable"",
        ""type"": ""function""
      },
      {
        ""inputs"": [{""internalType"": ""address"", ""name"": """", ""type"": ""address""}],
        ""name"": ""vaults"",
        ""outputs"": [{""internalType"": ""address"", ""name"": """", ""type"": ""address""}],
        ""stateMutability"": ""view"",
        ""type"": ""function""
      }
    ]";

    private readonly TRexOptions _options;
    private readonly ILogger<RealEstateVaultFactoryContractService> _logger;

    public RealEstateVaultFactoryContractService(IOptions<TRexOptions> options, ILogger<RealEstateVaultFactoryContractService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> DeployVaultAsync(string tokenAddress, string stablecoinAddress, string pricePerShareRaw, string adminAddress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.PrivateKey) || string.IsNullOrWhiteSpace(_options.RealEstateVaultFactoryAddress))
            throw new InvalidOperationException("TRex:PrivateKey and TRex:RealEstateVaultFactoryAddress are required for deployVault.");

        var price = BigInteger.TryParse(pricePerShareRaw, out var p) ? p : BigInteger.Zero;
        var web3 = CreateWeb3WithSigner();
        var contract = web3.Eth.GetContract(VaultFactoryAbi, NormalizeAddress(_options.RealEstateVaultFactoryAddress));
        var deployFn = contract.GetFunction("deployVault");

        var txHash = await deployFn.SendTransactionAsync(
            web3.TransactionManager.Account!.Address,
            new HexBigInteger(0),
            new HexBigInteger(500000),
            null,
            NormalizeAddress(tokenAddress),
            NormalizeAddress(stablecoinAddress),
            new HexBigInteger(price),
            NormalizeAddress(adminAddress)).ConfigureAwait(false);

        _logger.LogInformation("VaultFactory deployVault tx sent: {TxHash}", txHash);
        return txHash;
    }

    public async Task<string> GetVaultAsync(string tokenAddress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.RpcUrl) || string.IsNullOrWhiteSpace(_options.RealEstateVaultFactoryAddress))
            return "0x0000000000000000000000000000000000000000";

        var web3 = new Web3(_options.RpcUrl);
        var contract = web3.Eth.GetContract(VaultFactoryAbi, NormalizeAddress(_options.RealEstateVaultFactoryAddress));
        var vaultAddress = await contract.GetFunction("vaults").CallAsync<string>(NormalizeAddress(tokenAddress)).ConfigureAwait(false);
        return vaultAddress ?? "0x0000000000000000000000000000000000000000";
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
