namespace RealEstateInvesting.API.Contracts;

/// <summary>
/// Request body for Add Property Step 4: Supply the Vault (set vault identity and mint tokens to vault).
/// </summary>
public class SupplyVaultOnChainRequest
{
    /// <summary>Vault contract address.</summary>
    public string VaultAddress { get; set; } = string.Empty;

    /// <summary>Property token (ERC-3643) contract address.</summary>
    public string TokenAddress { get; set; } = string.Empty;

    /// <summary>Amount to mint to vault (raw units string, e.g. "1000000000000000000").</summary>
    public string AmountMintedRaw { get; set; } = string.Empty;

    /// <summary>Optional: identity contract address for the vault; if set, updateIdentity(vault, this) is called before mint.</summary>
    public string? VaultIdentityAddress { get; set; }

    /// <summary>Optional: internal property ID to link to this supply.</summary>
    public Guid? PropertyId { get; set; }
}
