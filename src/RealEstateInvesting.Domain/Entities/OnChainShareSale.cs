using RealEstateInvesting.Domain.Common;

namespace RealEstateInvesting.Domain.Entities;

/// <summary>
/// Audit record for Flow 6: on-chain share sale (approve property token + sellShares).
/// </summary>
public class OnChainShareSale : BaseEntity
{
    /// <summary>Our user ID who requested the sale (if known).</summary>
    public Guid? UserId { get; private set; }

    /// <summary>Wallet associated with the sale (for audit).</summary>
    public string? UserWalletAddress { get; private set; }

    /// <summary>Property token address (ERC-20) that was sold.</summary>
    public string PropertyTokenAddress { get; private set; } = default!;

    /// <summary>Amount of shares sold (in token smallest units, as string).</summary>
    public string AmountOfSharesRaw { get; private set; } = default!;

    /// <summary>Transaction hash of the approve(propertyToken) call.</summary>
    public string? ApproveTxHash { get; private set; }

    /// <summary>Transaction hash of the sellShares call.</summary>
    public string SellTxHash { get; private set; } = default!;

    private OnChainShareSale() { }

    /// <summary>Creates an audit record for a completed share sale.</summary>
    public static OnChainShareSale Create(
        string propertyTokenAddress,
        string amountOfSharesRaw,
        string? approveTxHash,
        string sellTxHash,
        Guid? userId,
        string? userWalletAddress)
    {
        return new OnChainShareSale
        {
            PropertyTokenAddress = Normalize(propertyTokenAddress),
            AmountOfSharesRaw = amountOfSharesRaw ?? "0",
            ApproveTxHash = approveTxHash,
            SellTxHash = sellTxHash,
            UserId = userId,
            UserWalletAddress = userWalletAddress != null ? Normalize(userWalletAddress) : null
        };
    }

    private static string Normalize(string address)
    {
        var a = (address ?? "").Trim();
        return a.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? a.ToLowerInvariant() : "0x" + a.ToLowerInvariant();
    }
}
