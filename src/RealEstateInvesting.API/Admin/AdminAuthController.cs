using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.API.Contracts;
using RealEstateInvesting.Application.AdminAuth.DTOs;
using RealEstateInvesting.Application.AdminAuth.Interfaces;
namespace RealEstateInvesting.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/auth")]
public class AdminAuthController : ControllerBase
{
    private readonly IAdminAuthService _authService;

    public AdminAuthController(IAdminAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AdminLoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        var response = AdminApiResponse<AdminAuthResponse>.Success(
            result,
            "Admin login successful"
        );

        return Ok(response);
    }
}