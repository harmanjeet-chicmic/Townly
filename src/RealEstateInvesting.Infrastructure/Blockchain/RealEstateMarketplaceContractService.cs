using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using RealEstateInvesting.Application.Common.Interfaces;
using System.Numerics;

namespace RealEstateInvesting.Infrastructure.Blockchain;

/// <summary>
/// T-REX Real Estate Marketplace contract. Flow 5: buyShares. Flow 6: sellShares.
/// </summary>
public sealed class RealEstateMarketplaceContractService : IRealEstateMarketplaceContractService
{
    private const string MarketplaceAbi = "[{\"inputs\":[{\"internalType\":\"address\",\"name\":\"tokenAddress\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"buyShares\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"tokenAddress\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"sellShares\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";

    private readonly TRexOptions _options;
    private readonly ILogger<RealEstateMarketplaceContractService> _logger;

    public RealEstateMarketplaceContractService(IOptions<TRexOptions> options, ILogger<RealEstateMarketplaceContractService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> BuySharesAsync(string propertyTokenAddress, string amountOfSharesRaw, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.PrivateKey))
            throw new InvalidOperationException("TRex:PrivateKey is required for buyShares.");

        var amount = ParseAmount(amountOfSharesRaw);
        var web3 = CreateWeb3WithSigner();
        var contract = web3.Eth.GetContract(MarketplaceAbi, _options.RealEstateMarketplaceAddress);
        var buyFn = contract.GetFunction("buyShares");

        var txHash = await buyFn.SendTransactionAsync(
            web3.TransactionManager.Account!.Address,
            new HexBigInteger(0),
            new HexBigInteger(500000),
            null,
            NormalizeAddress(propertyTokenAddress),
            new HexBigInteger(amount)).ConfigureAwait(false);

        _logger.LogInformation("Marketplace buyShares tx sent: {TxHash}", txHash);
        return txHash;
    }

    /// <inheritdoc />
    public async Task<string> SellSharesAsync(string propertyTokenAddress, string amountOfSharesRaw, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.PrivateKey))
            throw new InvalidOperationException("TRex:PrivateKey is required for sellShares.");

        var amount = ParseAmount(amountOfSharesRaw);
        var web3 = CreateWeb3WithSigner();
        var contract = web3.Eth.GetContract(MarketplaceAbi, _options.RealEstateMarketplaceAddress);
        var sellFn = contract.GetFunction("sellShares");

        var txHash = await sellFn.SendTransactionAsync(
            web3.TransactionManager.Account!.Address,
            new HexBigInteger(0),
            new HexBigInteger(500000),
            null,
            NormalizeAddress(propertyTokenAddress),
            new HexBigInteger(amount)).ConfigureAwait(false);

        _logger.LogInformation("Marketplace sellShares tx sent: {TxHash}", txHash);
        return txHash;
    }

    private Web3 CreateWeb3WithSigner()
    {
        var key = _options.PrivateKey!.Trim();
        if (key.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) key = key[2..];
        var account = new Nethereum.Web3.Accounts.Account("0x" + key);
        return new Web3(account, _options.RpcUrl);
    }

    private static BigInteger ParseAmount(string amountRaw)
    {
        if (string.IsNullOrWhiteSpace(amountRaw) || !BigInteger.TryParse(amountRaw, out var value))
            return BigInteger.Zero;
        return value;
    }

    private static string NormalizeAddress(string address)
    {
        var a = (address ?? "").Trim();
        return a.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? a : "0x" + a;
    }
}
