namespace RealEstateInvesting.API.Contracts;

/// <summary>
/// Request for the full property creation flow (deploy suite, vault, register, identities, mint, bind).
/// </summary>
public class CreatePropertySuiteApiRequest
{
    /// <summary>Property identifier (e.g. AMOY-RWA-013).</summary>
    public string PropertyId { get; set; } = string.Empty;

    /// <summary>Price per share in USDC (e.g. "150").</summary>
    public string PropertyPriceUsdc { get; set; } = string.Empty;

    /// <summary>Total supply to mint to vault (e.g. "2000").</summary>
    public string MintAmount { get; set; } = string.Empty;

    /// <summary>Optional. Defaults to ipfs://meta-{PropertyId}.</summary>
    public string? MetadataUri { get; set; }

    /// <summary>Identity contract address for the vault (for token's IR).</summary>
    public string VaultIdentityAddress { get; set; } = string.Empty;

    /// <summary>Identity contract address for the marketplace (for token's IR).</summary>
    public string MarketplaceIdentityAddress { get; set; } = string.Empty;

    /// <summary>Country code for identity registration (e.g. 42).</summary>
    public ushort IdentityCountryCode { get; set; } = 42;

    /// <summary>Optional: additional (wallet, identityAddress, countryCode) to verify on the new token's IR.</summary>
    public List<IdentityEntryDto>? AdditionalIdentities { get; set; }
}

public class IdentityEntryDto
{
    public string WalletAddress { get; set; } = string.Empty;
    public string IdentityContractAddress { get; set; } = string.Empty;
    public ushort CountryCode { get; set; } = 42;
}
