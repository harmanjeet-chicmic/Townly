using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Notifications.Interfaces;
using RealEstateInvesting.Domain.Enums;
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
    public async Task<IActionResult> GetMyNotifications(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 9,
    [FromQuery] string? search = null,
    [FromQuery] string? notificationType = null)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _repo.GetByUserAsync(
            userId, page, pageSize, search, notificationType);

        return Ok(result);
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
    public async Task<IActionResult> GetUnread(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 9)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _repo.GetUnreadByUserAsync(
            userId, page, pageSize);

        return Ok(result);
    }
    [HttpGet("me/unread/count")]
    public async Task<IActionResult> GetCount()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var count = await _repo.GetUnreadCountAsync(userId);
        return Ok(count);
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
