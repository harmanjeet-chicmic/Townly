using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Notifications.Interfaces;
namespace RealEstateInvesting.API.Controllers;

[ApiController]
[Route("api/test/push")]
public class TestPushController : ControllerBase
{
    private readonly IPushNotificationService _push;

    public TestPushController(IPushNotificationService push)
    {
        _push = push;
    }

    [HttpPost("{userId}")]
    public async Task<IActionResult> Test(Guid userId)
    {
        await _push.SendToUserAsync(
            userId,
            "Test Notification",
            "If you see this, backend push works",
            "test"
        );

        return Ok("Sent");
    }
}