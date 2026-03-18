using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using RealEstateInvesting.Application.Common.Interfaces;

namespace RealEstateInvesting.Infrastructure.Blockchain;

/// <summary>
/// T-REX Real Estate Registry contract implementation (Flow 4: Register property).
/// </summary>
public sealed class RealEstateRegistryContractService : IRealEstateRegistryContractService
{
    private const string RegistryAbi = "[" +
        "{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"string\",\"name\":\"uri\",\"type\":\"string\"},{\"internalType\":\"address\",\"name\":\"tokenAddress\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"vaultAddress\",\"type\":\"address\"}],\"name\":\"registerProperty\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}," +
        "{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"propertyId\",\"type\":\"uint256\"}],\"name\":\"propertyTokens\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"}," +
        "{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"propertyId\",\"type\":\"uint256\"}],\"name\":\"propertyVaults\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"}" +
        "]";

    private readonly TRexOptions _options;
    private readonly ILogger<RealEstateRegistryContractService> _logger;

    public RealEstateRegistryContractService(IOptions<TRexOptions> options, ILogger<RealEstateRegistryContractService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> RegisterPropertyAsync(string to, string uri, string tokenAddress, string vaultAddress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.PrivateKey))
            throw new InvalidOperationException("TRex:PrivateKey is required for registerProperty (REGISTER_ROLE wallet).");

        var web3 = CreateWeb3WithSigner();
        var contract = web3.Eth.GetContract(RegistryAbi, _options.RealEstateRegistryAddress);
        var registerFunction = contract.GetFunction("registerProperty");

        var txHash = await registerFunction.SendTransactionAsync(
            web3.TransactionManager.Account!.Address,
            new HexBigInteger(0),
            new HexBigInteger(500000),
            null,
            NormalizeAddress(to),
            uri ?? "",
            NormalizeAddress(tokenAddress),
            NormalizeAddress(vaultAddress)).ConfigureAwait(false);

        _logger.LogInformation("RealEstateRegistry registerProperty tx sent: {TxHash}", txHash);
        return txHash;
    }

    /// <inheritdoc />
    public async Task<string> GetPropertyTokenAsync(long onChainPropertyId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.RpcUrl) || string.IsNullOrWhiteSpace(_options.RealEstateRegistryAddress))
        {
            _logger.LogWarning("TRex RpcUrl or RealEstateRegistryAddress not configured.");
            return string.Empty;
        }

        var web3 = new Web3(_options.RpcUrl);
        var contract = web3.Eth.GetContract(RegistryAbi, _options.RealEstateRegistryAddress);
        var fn = contract.GetFunction("propertyTokens");
        var address = await fn.CallAsync<string>(onChainPropertyId).ConfigureAwait(false);
        return address ?? string.Empty;
    }

    /// <inheritdoc />
    public async Task<string> GetPropertyVaultAsync(long onChainPropertyId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.RpcUrl) || string.IsNullOrWhiteSpace(_options.RealEstateRegistryAddress))
        {
            _logger.LogWarning("TRex RpcUrl or RealEstateRegistryAddress not configured.");
            return string.Empty;
        }

        var web3 = new Web3(_options.RpcUrl);
        var contract = web3.Eth.GetContract(RegistryAbi, _options.RealEstateRegistryAddress);
        var fn = contract.GetFunction("propertyVaults");
        var address = await fn.CallAsync<string>(onChainPropertyId).ConfigureAwait(false);
        return address ?? string.Empty;
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
