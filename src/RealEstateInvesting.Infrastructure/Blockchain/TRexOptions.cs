namespace RealEstateInvesting.Infrastructure.Blockchain;

/// <summary>
/// T-REX / Amoy network configuration.
/// Set via appsettings "TRex" section or environment variables (RPC_URL, PRIVATE_KEY).
/// </summary>
public class TRexOptions
{
    public const string SectionName = "TRex";

    /// <summary>
    /// JSON-RPC endpoint (e.g. https://rpc-amoy.polygon.technology).
    /// </summary>
    public string RpcUrl { get; set; } = string.Empty;

    /// <summary>
    /// Private key of the wallet that has AGENT_ROLE (for updateIdentity / updateCountry).
    /// Optional if you only need read-only isVerified.
    /// </summary>
    public string? PrivateKey { get; set; }

    /// <summary>
    /// Identity Registry contract address (Amoy: 0x192738Fb0FF12Aa5564FAF18dEa315ccdF5A98ec).
    /// </summary>
    public string IdentityRegistryAddress { get; set; } = "0x192738Fb0FF12Aa5564FAF18dEa315ccdF5A98ec";

    /// <summary>
    /// Real Estate Registry contract address (Amoy: 0xB79a38247D66369fD9A5dF844Eac0fe94a88abd1).
    /// Used for Flow 4: Register property on chain (requires REGISTER_ROLE).
    /// </summary>
    public string RealEstateRegistryAddress { get; set; } = "0xB79a38247D66369fD9A5dF844Eac0fe94a88abd1";

    /// <summary>
    /// Stablecoin (e.g. USDC) contract address (Amoy: 0xF39906465Ac54E2370c4a189Af83b143f99F7010).
    /// Flow 5: Buyer approves this token for the marketplace before buyShares.
    /// </summary>
    public string StablecoinAddress { get; set; } = "0xF39906465Ac54E2370c4a189Af83b143f99F7010";

    /// <summary>
    /// Real Estate Marketplace contract address (Amoy: 0x4f087f31e47F6EC53e3eAaE7Eb7234B7A459DfBA).
    /// Flow 5: buyShares(propertyTokenAddress, amount). Flow 6: sellShares(propertyTokenAddress, amount).
    /// </summary>
    public string RealEstateMarketplaceAddress { get; set; } = "0x4f087f31e47F6EC53e3eAaE7Eb7234B7A459DfBA";

    /// <summary>
    /// T-REX Factory contract address (deployTREXSuite). Required for full property-suite creation API.
    /// </summary>
    public string? TrexFactoryAddress { get; set; }

    /// <summary>
    /// Real Estate Vault Factory contract address (deployVault). Required for full property-suite creation API.
    /// </summary>
    public string? RealEstateVaultFactoryAddress { get; set; }

    /// <summary>
    /// Claim Issuer contract address (used in deployTREXSuite claim config). Required for full property-suite creation API.
    /// </summary>
    public string? ClaimIssuerAddress { get; set; }
}
