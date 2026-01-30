using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Infrastructure.Identity;

public class CurrentUser : ICurrentUser
{
    public Guid UserId { get; }
    public string WalletAddress { get; }
    public UserRole Role { get; }
    public KycStatus KycStatus { get; }
    public bool IsBlocked { get; }

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        var principal = httpContextAccessor.HttpContext?.User
            ?? throw new InvalidOperationException("No active HTTP context.");

        UserId = Guid.Parse(
            principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("UserId claim missing.")
        );

        WalletAddress =
            principal.FindFirst("walletAddress")?.Value
            ?? throw new InvalidOperationException("Wallet address claim missing.");

        Role = Enum.Parse<UserRole>(
            principal.FindFirst(ClaimTypes.Role)?.Value
            ?? UserRole.Investor.ToString()
        );

        KycStatus = Enum.Parse<KycStatus>(
            principal.FindFirst("kycStatus")?.Value
            ?? KycStatus.NotStarted.ToString()
        );

        IsBlocked = bool.Parse(
            principal.FindFirst("isBlocked")?.Value
            ?? "false"
        );
    }
}
