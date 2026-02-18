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

    [HttpGet("me/unread")]
    public async Task<IActionResult> GetUnread()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var notifications = await _repo.GetUnreadByUserAsync(userId);
        return Ok(notifications);
    }
    [HttpGet("me/unread/count")]
    public async Task<IActionResult> GetCount()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var notifications = await _repo.GetUnreadByUserAsync(userId);
        return Ok(notifications.Count());
    }
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var notification = await _repo.GetByIdAsync(id);

        if (notification == null || notification.UserId != userId)
            return NotFound();

        notification.MarkAsRead();
        await _repo.SaveChangesAsync();

        return Ok(notification);
    }
    [HttpPost("me/read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _repo.MarkAllAsReadAsync(userId);
        await _repo.SaveChangesAsync();

        return Ok();
    }
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteNotification(Guid id)
    {
         
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var notification = await _repo.GetByIdAsync(id);

        if (notification == null || notification.UserId != userId)
            return NotFound();
        await _repo.DeleteAsync(notification);
        
        return Ok("notification deleted sucessfully");
    }


}
