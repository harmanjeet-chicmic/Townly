using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Notifications.Interfaces;
using System.Security.Claims;

namespace RealEstateInvesting.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationRepository _repo;

    public NotificationController(INotificationRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyNotifications()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var notifications = await _repo.GetByUserAsync(userId);
        return Ok(notifications);
    }

    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var notification = await _repo.GetByIdAsync(id);

        if (notification == null || notification.UserId != userId)
            return NotFound();

        notification.MarkAsRead();
        await _repo.SaveChangesAsync();

        return Ok();
    }
}
