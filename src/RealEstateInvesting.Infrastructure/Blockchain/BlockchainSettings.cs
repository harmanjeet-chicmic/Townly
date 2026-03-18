using Microsoft.Extensions.Options;
using RealEstateInvesting.Application.Common.Interfaces;

namespace RealEstateInvesting.Infrastructure.Blockchain;

/// <summary>Exposes TRex addresses to Application layer without referencing TRexOptions type.</summary>
public sealed class BlockchainSettings : IBlockchainSettings
{
    private readonly TRexOptions _options;
    private readonly string? _deployerAddress;

    public BlockchainSettings(IOptions<TRexOptions> options)
    {
        _options = options.Value;
        _deployerAddress = GetDeployerAddressFromKey(_options.PrivateKey);
    }

    public string StablecoinAddress => _options.StablecoinAddress;
    public string RealEstateMarketplaceAddress => _options.RealEstateMarketplaceAddress;
    public string? DeployerAddress => _deployerAddress;
    public string? ClaimIssuerAddress => _options.ClaimIssuerAddress;

    private static string? GetDeployerAddressFromKey(string? privateKey)
    {
        if (string.IsNullOrWhiteSpace(privateKey)) return null;
        var key = privateKey.Trim();
        if (key.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) key = key[2..];
        try
        {
            var account = new Nethereum.Web3.Accounts.Account("0x" + key);
            return account.Address;
        }
        catch
        {
            return null;
        }
    }
}
