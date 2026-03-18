using RealEstateInvesting.Domain.Common;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Domain.Entities;

/// <summary>
/// Audit record for T-REX Identity Registry on-chain updates (identity/country).
/// Stored for future use: reporting, reconciliation, support.
/// </summary>
public class OnChainKycAction : BaseEntity
{
    /// <summary>User's wallet address that was updated on chain.</summary>
    public string WalletAddress { get; private set; } = default!;

    /// <summary>Our user id if we have it (resolved from wallet).</summary>
    public Guid? UserId { get; private set; }

    public OnChainKycActionType ActionType { get; private set; }

    /// <summary>Set when ActionType = IdentityUpdate.</summary>
    public string? IdentityContractAddress { get; private set; }

    /// <summary>Set when ActionType = CountryUpdate. ISO 3166-1 numeric.</summary>
    public ushort? CountryCode { get; private set; }

    /// <summary>Transaction hash returned by the chain.</summary>
    public string TransactionHash { get; private set; } = default!;

    /// <summary>Admin user id who triggered the on-chain call (if any).</summary>
    public Guid? PerformedByAdminId { get; private set; }

    private OnChainKycAction() { }

    public static OnChainKycAction ForIdentityUpdate(
        string walletAddress,
        Guid? userId,
        string identityContractAddress,
        string transactionHash,
        Guid? performedByAdminId)
    {
        return new OnChainKycAction
        {
            WalletAddress = NormalizeWallet(walletAddress),
            UserId = userId,
            ActionType = OnChainKycActionType.IdentityUpdate,
            IdentityContractAddress = identityContractAddress,
            TransactionHash = transactionHash,
            PerformedByAdminId = performedByAdminId
        };
    }

    public static OnChainKycAction ForCountryUpdate(
        string walletAddress,
        Guid? userId,
        ushort countryCode,
        string transactionHash,
        Guid? performedByAdminId)
    {
        return new OnChainKycAction
        {
            WalletAddress = NormalizeWallet(walletAddress),
            UserId = userId,
            ActionType = OnChainKycActionType.CountryUpdate,
            CountryCode = countryCode,
            TransactionHash = transactionHash,
            PerformedByAdminId = performedByAdminId
        };
    }

    /// <summary>Identity + country in one transaction (registerIdentity).</summary>
    public static OnChainKycAction ForRegisterIdentity(
        string walletAddress,
        Guid? userId,
        string identityContractAddress,
        ushort countryCode,
        string transactionHash,
        Guid? performedByAdminId)
    {
        return new OnChainKycAction
        {
            WalletAddress = NormalizeWallet(walletAddress),
            UserId = userId,
            ActionType = OnChainKycActionType.RegisterIdentity,
            IdentityContractAddress = identityContractAddress,
            CountryCode = countryCode,
            TransactionHash = transactionHash,
            PerformedByAdminId = performedByAdminId
        };
    }

    private static string NormalizeWallet(string address)
    {
        var a = (address ?? "").Trim();
        return a.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? a.ToLowerInvariant() : "0x" + a.ToLowerInvariant();
    }
}
