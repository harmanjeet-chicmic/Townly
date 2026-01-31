namespace RealEstateInvesting.Application.AdminAuth.DTOs;

public class AdminLoginRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
