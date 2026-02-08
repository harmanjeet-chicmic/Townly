using RealEstateInvesting.Application.Notifications.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Notifications;

public class NotificationService : INotificationService
{
    private readonly IPushNotificationService _pushService;

    private readonly INotificationRepository _repo;

    public NotificationService(INotificationRepository repo,
            IPushNotificationService pushService)
    {
        _repo = repo;
        _pushService = pushService;
    }

    public async Task CreateAsync(
    Guid userId,
    NotificationType type,
    string title,
    string message,
    Guid? referenceId = null)
    {
        var notification = Notification.Create(
            userId,
            type,
            title,
            message,
            referenceId);

        await _repo.AddAsync(notification);
        await _repo.SaveChangesAsync();

        // 🔔 Send push notification (best-effort)
        try
        {
            await _pushService.SendToUserAsync(
                userId,
                title,
                message,
                type.ToString(),
                referenceId);
        }
        catch
        {
            // swallow push failures (never break business flow)
        }
    }

}
