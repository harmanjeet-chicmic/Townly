using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using RealEstateInvesting.Application.Common.Interfaces;
using System.Numerics;

namespace RealEstateInvesting.Infrastructure.Blockchain;

/// <summary>
/// ERC20 contract implementation (approve, balanceOf). Used for Flow 5 (stablecoin) and Flow 6 (property token).
/// </summary>
public sealed class ERC20ContractService : IERC20ContractService
{
    private const string Erc20Abi = "[{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";

    private readonly TRexOptions _options;
    private readonly ILogger<ERC20ContractService> _logger;

    public ERC20ContractService(IOptions<TRexOptions> options, ILogger<ERC20ContractService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> ApproveAsync(string tokenAddress, string spender, string amountRaw, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.PrivateKey))
            throw new InvalidOperationException("TRex:PrivateKey is required for approve.");

        var amount = ParseAmount(amountRaw);
        var web3 = CreateWeb3WithSigner();
        var contract = web3.Eth.GetContract(Erc20Abi, NormalizeAddress(tokenAddress));
        var approveFn = contract.GetFunction("approve");

        var txHash = await approveFn.SendTransactionAsync(
            web3.TransactionManager.Account!.Address,
            new HexBigInteger(0),
            new HexBigInteger(100000),
            null,
            NormalizeAddress(spender),
            new HexBigInteger(amount)).ConfigureAwait(false);

        _logger.LogInformation("ERC20 approve tx sent: {TxHash}", txHash);
        return txHash;
    }

    /// <inheritdoc />
    public async Task<string> BalanceOfAsync(string tokenAddress, string accountAddress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.RpcUrl))
            return "0";

        var web3 = new Web3(_options.RpcUrl);
        var contract = web3.Eth.GetContract(Erc20Abi, NormalizeAddress(tokenAddress));
        var balanceFn = contract.GetFunction("balanceOf");
        var balance = await balanceFn.CallAsync<BigInteger>(NormalizeAddress(accountAddress)).ConfigureAwait(false);
        return balance.ToString();
    }

    private static Web3 CreateWeb3WithSigner(string privateKey, string rpcUrl)
    {
        var key = privateKey.Trim();
        if (key.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) key = key[2..];
        var account = new Nethereum.Web3.Accounts.Account("0x" + key);
        return new Web3(account, rpcUrl);
    }

    private Web3 CreateWeb3WithSigner()
    {
        return CreateWeb3WithSigner(_options.PrivateKey!, _options.RpcUrl);
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
