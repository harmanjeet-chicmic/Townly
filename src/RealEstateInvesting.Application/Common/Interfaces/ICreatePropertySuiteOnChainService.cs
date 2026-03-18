namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// Single API that runs the full property creation flow: deploy T-REX suite, deploy vault, register property, setup identities, mint and bind compliance.
/// </summary>
public interface ICreatePropertySuiteOnChainService
{
    /// <summary>
    /// Executes the complete flow: 1) Deploy T-REX suite, 2) Deploy vault, 3) Register in registry, 4) Verify identities (vault, marketplace, optional users), 5) Unpause if needed, mint to vault, bind vault to compliance.
    /// </summary>
    Task<CreatePropertySuiteResult> CreatePropertySuiteAsync(CreatePropertySuiteRequest request, CancellationToken cancellationToken = default);
}

public class CreatePropertySuiteRequest
{
    /// <summary>Property identifier (e.g. AMOY-RWA-013).</summary>
    public string PropertyId { get; set; } = default!;

    /// <summary>Price per share in USDC (e.g. "150").</summary>
    public string PropertyPriceUsdc { get; set; } = default!;

    /// <summary>Total supply to mint to vault (e.g. "2000").</summary>
    public string MintAmount { get; set; } = default!;

    /// <summary>Optional. Defaults to ipfs://meta-{PropertyId}.</summary>
    public string? MetadataUri { get; set; }

    /// <summary>Identity contract address for the vault (for token's IR).</summary>
    public string VaultIdentityAddress { get; set; } = default!;

    /// <summary>Identity contract address for the marketplace (for token's IR).</summary>
    public string MarketplaceIdentityAddress { get; set; } = default!;

    /// <summary>Country code for identity registration (e.g. 42).</summary>
    public ushort IdentityCountryCode { get; set; } = 42;

    /// <summary>Optional: additional (userWallet, identityAddress) to verify on the new token's IR.</summary>
    public IReadOnlyList<IdentityEntry>? AdditionalIdentities { get; set; }
}

public class IdentityEntry
{
    public string WalletAddress { get; set; } = default!;
    public string IdentityContractAddress { get; set; } = default!;
    public ushort CountryCode { get; set; } = 42;
}

public class CreatePropertySuiteResult
{
    public string TokenAddress { get; init; } = default!;
    public string VaultAddress { get; init; } = default!;
    public string IdentityRegistryAddress { get; init; } = default!;
    public string DeploySuiteTxHash { get; init; } = default!;
    public string DeployVaultTxHash { get; init; } = default!;
    public string RegisterPropertyTxHash { get; init; } = default!;
    public IReadOnlyList<string> IdentityTxHashes { get; init; } = Array.Empty<string>();
    public string? UnpauseTxHash { get; init; }
    public string MintTxHash { get; init; } = default!;
    public string? BindComplianceTxHash { get; init; }
}
