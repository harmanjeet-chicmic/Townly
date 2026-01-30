using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface ICurrentUser
{
    Guid UserId { get; }
    string WalletAddress { get; }
    UserRole Role { get; }
    KycStatus KycStatus { get; }
    bool IsBlocked { get; }
}
