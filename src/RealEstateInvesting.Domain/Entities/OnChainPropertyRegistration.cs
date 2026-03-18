using RealEstateInvesting.Domain.Common;

namespace RealEstateInvesting.Domain.Entities;

/// <summary>
/// Audit record for T-REX Real Estate Registry on-chain property registration (Flow 4).
/// Stored for future use: reporting, reconciliation, linking on-chain property ID to our Property.
/// </summary>
public class OnChainPropertyRegistration : BaseEntity
{
    /// <summary>Our internal property ID (if linked to an existing Property entity).</summary>
    public Guid? PropertyId { get; private set; }

    /// <summary>Owner/operator wallet address (to) used in registerProperty.</summary>
    public string ToAddress { get; private set; } = default!;

    /// <summary>Property metadata URI (e.g. ipfs://Qm...).</summary>
    public string Uri { get; private set; } = default!;

    /// <summary>Property ERC-20 token address.</summary>
    public string TokenAddress { get; private set; } = default!;

    /// <summary>Property vault address.</summary>
    public string VaultAddress { get; private set; } = default!;

    /// <summary>On-chain property ID from the registry (if available from receipt/event).</summary>
    public long? OnChainPropertyId { get; private set; }

    /// <summary>Transaction hash of the registerProperty call.</summary>
    public string TransactionHash { get; private set; } = default!;

    /// <summary>Admin user ID who triggered the registration (if any).</summary>
    public Guid? PerformedByAdminId { get; private set; }

    private OnChainPropertyRegistration() { }

    /// <summary>
    /// Creates a new registration record for audit and future lookup.
    /// </summary>
    public static OnChainPropertyRegistration Create(
        string toAddress,
        string uri,
        string tokenAddress,
        string vaultAddress,
        string transactionHash,
        Guid? propertyId,
        long? onChainPropertyId,
        Guid? performedByAdminId)
    {
        return new OnChainPropertyRegistration
        {
            ToAddress = Normalize(toAddress),
            Uri = uri ?? "",
            TokenAddress = Normalize(tokenAddress),
            VaultAddress = Normalize(vaultAddress),
            TransactionHash = transactionHash,
            PropertyId = propertyId,
            OnChainPropertyId = onChainPropertyId,
            PerformedByAdminId = performedByAdminId
        };
    }

    private static string Normalize(string address)
    {
        var a = (address ?? "").Trim();
        return a.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? a.ToLowerInvariant() : "0x" + a.ToLowerInvariant();
    }
}
