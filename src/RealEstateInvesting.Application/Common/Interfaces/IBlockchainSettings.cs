namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>Read-only blockchain addresses for Flow 5/6 (stablecoin, marketplace) and property-suite deployment.</summary>
public interface IBlockchainSettings
{
    string StablecoinAddress { get; }
    string RealEstateMarketplaceAddress { get; }
    /// <summary>Address of the wallet configured as TRex:PrivateKey (deployer for TREX suite and vault).</summary>
    string? DeployerAddress { get; }

    /// <summary>Claim Issuer address for deployTREXSuite claim config.</summary>
    string? ClaimIssuerAddress { get; }
}
