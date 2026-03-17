using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Admin.Users.DTOs;

public class AdminUserQuery
{
    public bool? IsBlocked { get; set; }

    public KycStatus? KycStatus { get; set; }

    public UserRole? Role { get; set; }

    public string? Search { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}