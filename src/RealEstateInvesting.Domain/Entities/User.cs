using RealEstateInvesting.Domain.Common;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Domain.Entities;

public class User : BaseEntity
{
    
    public string WalletAddress { get; private set; } = default!;
    public long ChainId { get; private set; }

    // Roles & access
    public UserRole Role { get; private set; } = UserRole.Investor;

    // Compliance
    public KycStatus KycStatus { get; private set; } = KycStatus.NotStarted;

    // Account state
    public bool IsBlocked { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    // EF Core requires a private parameterless constructor
    private User() { }

    // Factory method (best practice)
    public static User Create(string walletAddress, long chainId)
    {
        return new User
        {
            WalletAddress = walletAddress.ToLowerInvariant(),
            ChainId = chainId,
            Role = UserRole.Investor,
            KycStatus = KycStatus.NotStarted,
            IsBlocked = false
        };
    }

    // Domain behaviors (NOT setters)

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        MarkUpdated();
    }

    public void UpdateKycStatus(KycStatus status)
    {
        KycStatus = status;
        MarkUpdated();
    }

    public void Block()
    {
        IsBlocked = true;
        MarkUpdated();
    }

    public void Unblock()
    {
        IsBlocked = false;
        MarkUpdated();
    }

    public void ChangeRole(UserRole role)
    {
        Role = role;
        MarkUpdated();
    }
}
