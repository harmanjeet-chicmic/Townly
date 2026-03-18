using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using RealEstateInvesting.Application.Common.Interfaces;

namespace RealEstateInvesting.Infrastructure.Blockchain;

/// <summary>
/// T-REX Identity Registry contract implementation. Flow 1: isVerified (read). Flow 2: updateIdentity. Flow 3: updateCountry.
/// </summary>
public sealed class IdentityRegistryContractService : IIdentityRegistryContractService
{
    private const string IdentityRegistryAbi = @"[
      {
        ""inputs"": [{""internalType"": ""address"", ""name"": ""_userAddress"", ""type"": ""address""}],
        ""name"": ""isVerified"",
        ""outputs"": [{""internalType"": ""bool"", ""name"": """", ""type"": ""bool""}],
        ""stateMutability"": ""view"",
        ""type"": ""function""
      },
      {
        ""inputs"": [
          {""internalType"": ""address"", ""name"": ""user"", ""type"": ""address""},
          {""internalType"": ""address"", ""name"": ""identity"", ""type"": ""address""}
        ],
        ""name"": ""updateIdentity"",
        ""outputs"": [],
        ""stateMutability"": ""nonpayable"",
        ""type"": ""function""
      },
      {
        ""inputs"": [
          {""internalType"": ""address"", ""name"": ""user"", ""type"": ""address""},
          {""internalType"": ""uint16"", ""name"": ""country"", ""type"": ""uint16""}
        ],
        ""name"": ""updateCountry"",
        ""outputs"": [],
        ""stateMutability"": ""nonpayable"",
        ""type"": ""function""
      },
      {
        ""inputs"": [
          {""internalType"": ""address"", ""name"": ""user"", ""type"": ""address""},
          {""internalType"": ""address"", ""name"": ""identity"", ""type"": ""address""},
          {""internalType"": ""uint16"", ""name"": ""country"", ""type"": ""uint16""}
        ],
        ""name"": ""registerIdentity"",
        ""outputs"": [],
        ""stateMutability"": ""nonpayable"",
        ""type"": ""function""
      }
    ]";

    private readonly TRexOptions _options;
    private readonly ILogger<IdentityRegistryContractService> _logger;

    public IdentityRegistryContractService(IOptions<TRexOptions> options,  ILogger<IdentityRegistryContractService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> IsVerified(string userAddress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.RpcUrl) || string.IsNullOrWhiteSpace(_options.IdentityRegistryAddress))
        {
            _logger.LogWarning("TRex RpcUrl or IdentityRegistryAddress not configured; returning false for IsVerified");
            return false;
        }

        try
        {
            var web3 = new Web3(_options.RpcUrl);
            var contract = web3.Eth.GetContract(IdentityRegistryAbi, _options.IdentityRegistryAddress);
            var isVerifiedFunction = contract.GetFunction("isVerified");

            var address = userAddress.Trim();
            if (!address.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                address = "0x" + address;

            return await isVerifiedFunction.CallAsync<bool>(address).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "IdentityRegistry isVerified call failed for {Address}", userAddress);
            throw;
        }
    }

    public async Task<string> UpdateIdentity(string userAddress, string identityContractAddress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.PrivateKey))
            throw new InvalidOperationException("TRex:PrivateKey is required for updateIdentity (AGENT_ROLE wallet).");

        var web3 = CreateWeb3WithSigner();
        var contract = web3.Eth.GetContract(IdentityRegistryAbi, _options.IdentityRegistryAddress);
        var updateIdentityFunction = contract.GetFunction("updateIdentity");

        var user = NormalizeAddress(userAddress);
        var identity = NormalizeAddress(identityContractAddress);

        var tx = await updateIdentityFunction.SendTransactionAsync(web3.TransactionManager.Account!.Address,
                                                                   new HexBigInteger(0),
                                                                   new HexBigInteger(300000),
                                                                   null,
                                                                   user,
                                                                   identity).ConfigureAwait(false);

        _logger.LogInformation("IdentityRegistry updateIdentity tx sent: {TxHash}", tx);
        return tx;
    }

    public async Task<string> UpdateCountry(string userAddress, ushort countryCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.PrivateKey))
            throw new InvalidOperationException("TRex:PrivateKey is required for updateCountry (AGENT_ROLE wallet).");

        var web3 = CreateWeb3WithSigner();
        var contract = web3.Eth.GetContract(IdentityRegistryAbi, _options.IdentityRegistryAddress);
        var updateCountryFunction = contract.GetFunction("updateCountry");

        var user = NormalizeAddress(userAddress);

        var tx = await updateCountryFunction.SendTransactionAsync(web3.TransactionManager.Account!.Address,
                                                                  new HexBigInteger(0),
                                                                  new HexBigInteger(300000),
                                                                  null,
                                                                  user,
                                                                  countryCode).ConfigureAwait(false);

        _logger.LogInformation("IdentityRegistry updateCountry tx sent: {TxHash}", tx);
        return tx;
    }

    public async Task<string> RegisterIdentity(string userAddress, string identityContractAddress, ushort countryCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.PrivateKey))
            throw new InvalidOperationException("TRex:PrivateKey is required for registerIdentity (AGENT_ROLE wallet).");

        var web3 = CreateWeb3WithSigner();
        var contract = web3.Eth.GetContract(IdentityRegistryAbi, _options.IdentityRegistryAddress);
        var registerIdentityFunction = contract.GetFunction("registerIdentity");

        var user = NormalizeAddress(userAddress);
        var identity = NormalizeAddress(identityContractAddress);

        var tx = await registerIdentityFunction.SendTransactionAsync(web3.TransactionManager.Account!.Address,
                                                                     new HexBigInteger(0),
                                                                     new HexBigInteger(350000),
                                                                     null,
                                                                     user,
                                                                     identity,
                                                                     countryCode).ConfigureAwait(false);

        _logger.LogInformation("IdentityRegistry registerIdentity tx sent: {TxHash}", tx);
        return tx;
    }

    public async Task<string> RegisterIdentityOnRegistryAsync(string identityRegistryAddress, string userAddress, string identityContractAddress, ushort countryCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.PrivateKey))
            throw new InvalidOperationException("TRex:PrivateKey is required for registerIdentity (AGENT_ROLE wallet).");

        var web3 = CreateWeb3WithSigner();
        var contract = web3.Eth.GetContract(IdentityRegistryAbi, NormalizeAddress(identityRegistryAddress));
        var registerIdentityFunction = contract.GetFunction("registerIdentity");

        var user = NormalizeAddress(userAddress);
        var identity = NormalizeAddress(identityContractAddress);

        var tx = await registerIdentityFunction.SendTransactionAsync(web3.TransactionManager.Account!.Address,
                                                                     new HexBigInteger(0),
                                                                     new HexBigInteger(350000),
                                                                     null,
                                                                     user,
                                                                     identity,
                                                                     countryCode).ConfigureAwait(false);

        _logger.LogInformation("IdentityRegistry registerIdentity on {Registry} tx sent: {TxHash}", identityRegistryAddress, tx);
        return tx;
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
        var a = address.Trim();
        return a.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? a : "0x" + a;
    }
}
