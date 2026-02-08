using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.API.Contracts;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using System.Security.Claims;

namespace RealEstateInvesting.API.Controllers.Notifications;

[ApiController]
[Authorize]
[Route("api/notifications/device-token")]
public class DeviceTokensController : ControllerBase
{
    private readonly IUserDeviceTokenRepository _repo;

    public DeviceTokensController(IUserDeviceTokenRepository repo)
    {
        _repo = repo;
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDeviceTokenRequest request)
    {   
        Console.WriteLine("=========FCM TOKEN ======="+request.DeviceToken);
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var existing = await _repo.GetByTokenAsync(request.DeviceToken);

        if (existing != null)
        {
            existing.Refresh();
        }
        else
        {
            var token = UserDeviceToken.Create(
                userId,
                request.DeviceToken,
                request.Platform);

            await _repo.AddAsync(token);
        }

        await _repo.SaveChangesAsync();
        return Ok();
    }
}
