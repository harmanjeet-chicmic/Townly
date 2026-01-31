using RealEstateInvesting.Application.AdminAuth.DTOs;

namespace RealEstateInvesting.Application.AdminAuth.Interfaces;

public interface IAdminAuthService
{
    Task<AdminAuthResponse> LoginAsync(AdminLoginRequest request);
}
