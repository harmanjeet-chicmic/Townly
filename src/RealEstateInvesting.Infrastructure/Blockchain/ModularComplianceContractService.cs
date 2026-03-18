using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using RealEstateInvesting.Application.Common.Interfaces;

namespace RealEstateInvesting.Infrastructure.Blockchain;

/// <summary>
/// Modular Compliance: isModuleBound(module), addModule(module).
/// </summary>
public sealed class ModularComplianceContractService : IModularComplianceContractService
{
    private const string ComplianceAbi = @"[
      {""inputs"":[{""internalType"":""address"",""name"":""module"",""type"":""address""}],""name"":""isModuleBound"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""stateMutability"":""view"",""type"":""function""},
      {""inputs"":[{""internalType"":""address"",""name"":""module"",""type"":""address""}],""name"":""addModule"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""}
    ]";

    private readonly TRexOptions _options;
    private readonly ILogger<ModularComplianceContractService> _logger;

    public ModularComplianceContractService(IOptions<TRexOptions> options, ILogger<ModularComplianceContractService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> IsModuleBoundAsync(string complianceAddress, string moduleAddress, CancellationToken cancellationToken = default)
    {
        var web3 = new Web3(_options.RpcUrl);
        var contract = web3.Eth.GetContract(ComplianceAbi, NormalizeAddress(complianceAddress));
        return await contract.GetFunction("isModuleBound").CallAsync<bool>(NormalizeAddress(moduleAddress)).ConfigureAwait(false);
    }

    public async Task<string> AddModuleAsync(string complianceAddress, string moduleAddress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.PrivateKey))
            throw new InvalidOperationException("TRex:PrivateKey is required for addModule.");

        var web3 = CreateWeb3WithSigner();
        var contract = web3.Eth.GetContract(ComplianceAbi, NormalizeAddress(complianceAddress));
        var txHash = await contract.GetFunction("addModule").SendTransactionAsync(
            web3.TransactionManager.Account!.Address,
            new HexBigInteger(0),
            new HexBigInteger(200000),
            null,
            NormalizeAddress(moduleAddress)).ConfigureAwait(false);
        _logger.LogInformation("ModularCompliance addModule tx sent: {TxHash}", txHash);
        return txHash;
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
