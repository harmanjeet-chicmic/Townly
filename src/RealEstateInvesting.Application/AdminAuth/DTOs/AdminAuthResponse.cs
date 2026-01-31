namespace RealEstateInvesting.Application.AdminAuth.DTOs;

public class AdminAuthResponse
{
    public string AccessToken { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
}
