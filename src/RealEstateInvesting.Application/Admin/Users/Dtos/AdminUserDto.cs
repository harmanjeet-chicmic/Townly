using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Admin.Users.DTOs;

public class AdminUserDto
{
    public Guid UserId { get; set; }

    public string WalletAddress { get; set; } = default!;

    public long ChainId { get; set; }

    public UserRole Role { get; set; }

    public KycStatus KycStatus { get; set; }

    public bool IsBlocked { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateTime CreatedAt { get; set; }
}