// namespace RealEstateInvesting.Application.Auth.DTOs;

// public class AuthResponse
// {
//     public string AccessToken { get; set; } = default!;
//     public DateTime ExpiresAt { get; set; }
// }
namespace RealEstateInvesting.Application.Auth.DTOs;

public class AuthResponse
{
    public string AccessToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
