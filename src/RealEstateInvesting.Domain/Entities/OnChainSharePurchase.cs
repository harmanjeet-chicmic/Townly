using RealEstateInvesting.Domain.Common;

namespace RealEstateInvesting.Domain.Entities;

/// <summary>
/// Audit record for Flow 5: on-chain share purchase (approve stablecoin + buyShares).
/// </summary>
public class OnChainSharePurchase : BaseEntity
{
    /// <summary>Our user ID who requested the purchase (if known).</summary>
    public Guid? UserId { get; private set; }

    /// <summary>Wallet that was KYC-checked (and may be the same as executor in future).</summary>
    public string? UserWalletAddress { get; private set; }

    /// <summary>Property token address (ERC-20) that was bought.</summary>
    public string PropertyTokenAddress { get; private set; } = default!;

    /// <summary>Amount of shares bought (in token smallest units, as string).</summary>
    public string AmountOfSharesRaw { get; private set; } = default!;

    /// <summary>Stablecoin amount approved (in smallest units, as string).</summary>
    public string AmountStablecoinApprovedRaw { get; private set; } = default!;

    /// <summary>Transaction hash of the approve(stablecoin) call.</summary>
    public string? ApproveTxHash { get; private set; }

    /// <summary>Transaction hash of the buyShares call.</summary>
    public string BuyTxHash { get; private set; } = default!;

    private OnChainSharePurchase() { }

    /// <summary>Creates an audit record for a completed share purchase.</summary>
    public static OnChainSharePurchase Create(
        string propertyTokenAddress,
        string amountOfSharesRaw,
        string amountStablecoinApprovedRaw,
        string? approveTxHash,
        string buyTxHash,
        Guid? userId,
        string? userWalletAddress)
    {
        return new OnChainSharePurchase
        {
            PropertyTokenAddress = Normalize(propertyTokenAddress),
            AmountOfSharesRaw = amountOfSharesRaw ?? "0",
            AmountStablecoinApprovedRaw = amountStablecoinApprovedRaw ?? "0",
            ApproveTxHash = approveTxHash,
            BuyTxHash = buyTxHash,
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
