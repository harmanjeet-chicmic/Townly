using RealEstateInvesting.Domain.Common;

namespace RealEstateInvesting.Domain.Entities;

/// <summary>
/// Audit record for "Supply the Vault" (Add Property Step 4 in T-REX Developer Guide):
/// set identity for vault and/or mint property tokens to vault.
/// </summary>
public class OnChainVaultSupply : BaseEntity
{
    /// <summary>Vault contract address that receives tokens and (optionally) identity.</summary>
    public string VaultAddress { get; private set; } = default!;

    /// <summary>Property token (ERC-3643) contract address.</summary>
    public string TokenAddress { get; private set; } = default!;

    /// <summary>Amount minted to vault (raw string, e.g. "1000000000000000000").</summary>
    public string AmountMintedRaw { get; private set; } = default!;

    /// <summary>Transaction hash for identity update (optional; if vault identity was set).</summary>
    public string? IdentityTxHash { get; private set; }

    /// <summary>Transaction hash for mint to vault.</summary>
    public string MintTxHash { get; private set; } = default!;

    /// <summary>Our internal property ID (if linked).</summary>
    public Guid? PropertyId { get; private set; }

    /// <summary>Admin who performed the supply.</summary>
    public Guid? PerformedByAdminId { get; private set; }

    private OnChainVaultSupply() { }

    /// <summary>
    /// Creates an audit record for a vault supply operation.
    /// </summary>
    public static OnChainVaultSupply Create(
        string vaultAddress,
        string tokenAddress,
        string amountMintedRaw,
        string? identityTxHash,
        string mintTxHash,
        Guid? propertyId,
        Guid? performedByAdminId)
    {
        return new OnChainVaultSupply
        {
            VaultAddress = Normalize(vaultAddress),
            TokenAddress = Normalize(tokenAddress),
            AmountMintedRaw = amountMintedRaw ?? "0",
            IdentityTxHash = identityTxHash,
            MintTxHash = mintTxHash,
            PropertyId = propertyId,
            PerformedByAdminId = performedByAdminId
        };
    }

    private static string Normalize(string address)
    {
        var a = (address ?? "").Trim();
        return a.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? a.ToLowerInvariant() : "0x" + a.ToLowerInvariant();
    }
}
