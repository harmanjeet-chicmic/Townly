using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using RealEstateInvesting.Application.Common.Interfaces;
using System.Numerics;

namespace RealEstateInvesting.Infrastructure.Blockchain;

/// <summary>
/// T-REX compliant token: mint(to, amount). Used for Add Property Step 4 (supply vault).
/// </summary>
public sealed class ComplianceTokenContractService : IComplianceTokenContractService
{
    // mint(address _to, uint256 _amount) — common in ERC-3643 / T-REX tokens
    private const string MintAbi = @"[
      {
        ""inputs"": [
          {""internalType"": ""address"", ""name"": ""_to"", ""type"": ""address""},
          {""internalType"": ""uint256"", ""name"": ""_amount"", ""type"": ""uint256""}
        ],
        ""name"": ""mint"",
        ""outputs"": [],
        ""stateMutability"": ""nonpayable"",
        ""type"": ""function""
      }
    ]";

    private readonly TRexOptions _options;
    private readonly ILogger<ComplianceTokenContractService> _logger;

    public ComplianceTokenContractService(IOptions<TRexOptions> options, ILogger<ComplianceTokenContractService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> MintAsync(string tokenAddress, string toAddress, string amountRaw, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.PrivateKey))
            throw new InvalidOperationException("TRex:PrivateKey is required for mint (agent/minter role on token).");

        var amount = ParseAmount(amountRaw);
        var web3 = CreateWeb3WithSigner();
        var contract = web3.Eth.GetContract(MintAbi, NormalizeAddress(tokenAddress));
        var mintFn = contract.GetFunction("mint");

        var txHash = await mintFn.SendTransactionAsync(
            web3.TransactionManager.Account!.Address,
            new HexBigInteger(0),
            new HexBigInteger(200000),
            null,
            NormalizeAddress(toAddress),
            new HexBigInteger(amount)).ConfigureAwait(false);

        _logger.LogInformation("Token mint tx sent: {TxHash} for token {Token}", txHash, tokenAddress);
        return txHash;
    }

    private static BigInteger ParseAmount(string amountRaw)
    {
        if (string.IsNullOrWhiteSpace(amountRaw) || !BigInteger.TryParse(amountRaw, out var value))
            return BigInteger.Zero;
        return value;
    }

    private Web3 CreateWeb3WithSigner()
    {
        var key = _options.PrivateKey!.Trim();
        if (key.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            key = key[2..];
        var account = new Nethereum.Web3.Accounts.Account("0x" + key);
        return new Web3(account, _options.RpcUrl);
    }

    private static string NormalizeAddress(string address)
    {
        var a = (address ?? "").Trim();
        return a.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? a : "0x" + a;
    }
}
