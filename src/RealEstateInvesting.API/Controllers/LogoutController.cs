using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.API.Contracts;
using RealEstateInvesting.Application.Common.Interfaces;

namespace RealEstateInvesting.API.Controllers;

[ApiController]
[Route("api/auth")]
[Authorize]
public class LogoutController : ControllerBase
{
    private readonly IUserDeviceTokenRepository _deviceTokenRepo;
    private readonly ICurrentUserService _currentUserService;

    public LogoutController(
        IUserDeviceTokenRepository deviceTokenRepo,
        ICurrentUserService currentUserService)
    {
        _deviceTokenRepo = deviceTokenRepo;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Logout endpoint for both Users and Admins.
    /// Since the system uses JWT, the client should delete the token.
    /// This endpoint allows the client to signal the logout intent and 
    /// deactivates all device tokens for the user.
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = _currentUserService.UserId;
        await _deviceTokenRepo.DeactivateAllByUserIdAsync(userId);
        await _deviceTokenRepo.SaveChangesAsync();

        return Ok(RealEstateInvesting.API.Contracts.ApiResponse<object>.Success(null, "Logged out successfully and device tokens deactivated"));
    }
}
