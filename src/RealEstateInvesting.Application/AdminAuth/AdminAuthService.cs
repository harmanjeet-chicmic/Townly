using RealEstateInvesting.Application.AdminAuth.DTOs;
using RealEstateInvesting.Application.AdminAuth.Interfaces;
using RealEstateInvesting.Application.Common.Interfaces;

namespace RealEstateInvesting.Application.AdminAuth;

public class AdminAuthService : IAdminAuthService
{
    private readonly IAdminRepository _adminRepo;
    private readonly IJwtService _jwtService;
    private readonly IAdminPasswordHasher _passwordHasher;

    public AdminAuthService(
        IAdminRepository adminRepo,
        IJwtService jwtService,
        IAdminPasswordHasher passwordHasher)
    {
        _adminRepo = adminRepo;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AdminAuthResponse> LoginAsync(AdminLoginRequest request)
    {
        var admin = await _adminRepo.GetByEmailAsync(request.Email);
        if (admin == null || !admin.IsActive)
            throw new InvalidOperationException("Invalid admin credentials");

        if (!_passwordHasher.Verify(admin.PasswordHash, request.Password))
            throw new InvalidOperationException("Invalid admin credentials");

        return new AdminAuthResponse
        {
            AccessToken = _jwtService.GenerateAdminToken(admin),
            ExpiresAt = DateTime.UtcNow.AddHours(6)
        };
    }
}
