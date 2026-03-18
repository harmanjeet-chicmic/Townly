using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using RealEstateInvesting.Application.Common.Interfaces;

namespace RealEstateInvesting.Infrastructure.Blockchain;

/// <summary>
/// T-REX Token: identityRegistry(), paused(), unpause(), compliance().
/// </summary>
public sealed class TokenReaderContractService : ITokenReaderContractService
{
    private const string TokenAbi = @"[
      {""inputs"":[],""name"":""identityRegistry"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""stateMutability"":""view"",""type"":""function""},
      {""inputs"":[],""name"":""paused"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""stateMutability"":""view"",""type"":""function""},
      {""inputs"":[],""name"":""unpause"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},
      {""inputs"":[],""name"":""compliance"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""stateMutability"":""view"",""type"":""function""}
    ]";

    private readonly TRexOptions _options;
    private readonly ILogger<TokenReaderContractService> _logger;

    public TokenReaderContractService(IOptions<TRexOptions> options, ILogger<TokenReaderContractService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> GetIdentityRegistryAsync(string tokenAddress, CancellationToken cancellationToken = default)
    {
        var web3 = new Web3(_options.RpcUrl);
        var contract = web3.Eth.GetContract(TokenAbi, NormalizeAddress(tokenAddress));
        var ir = await contract.GetFunction("identityRegistry").CallAsync<string>().ConfigureAwait(false);
        return ir ?? "0x0000000000000000000000000000000000000000";
    }

    public async Task<bool> GetPausedAsync(string tokenAddress, CancellationToken cancellationToken = default)
    {
        var web3 = new Web3(_options.RpcUrl);
        var contract = web3.Eth.GetContract(TokenAbi, NormalizeAddress(tokenAddress));
        return await contract.GetFunction("paused").CallAsync<bool>().ConfigureAwait(false);
    }

    public async Task<string> UnpauseAsync(string tokenAddress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.PrivateKey))
            throw new InvalidOperationException("TRex:PrivateKey is required for unpause.");

        var web3 = CreateWeb3WithSigner();
        var contract = web3.Eth.GetContract(TokenAbi, NormalizeAddress(tokenAddress));
        var txHash = await contract.GetFunction("unpause").SendTransactionAsync(
            web3.TransactionManager.Account!.Address,
            new HexBigInteger(0),
            new HexBigInteger(100000),
            new HexBigInteger(0),
            Array.Empty<object>()).ConfigureAwait(false);
        _logger.LogInformation("Token unpause tx sent: {TxHash}", txHash);
        return txHash;
    }

    public async Task<string> GetComplianceAsync(string tokenAddress, CancellationToken cancellationToken = default)
    {
        var web3 = new Web3(_options.RpcUrl);
        var contract = web3.Eth.GetContract(TokenAbi, NormalizeAddress(tokenAddress));
        var comp = await contract.GetFunction("compliance").CallAsync<string>().ConfigureAwait(false);
        return comp ?? "0x0000000000000000000000000000000000000000";
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
