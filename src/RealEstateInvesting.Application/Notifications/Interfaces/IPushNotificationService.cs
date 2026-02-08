namespace RealEstateInvesting.Application.Notifications.Interfaces;

public interface IPushNotificationService
{
    Task SendToUserAsync(
        Guid userId,
        string title,
        string body,
        string type,
        Guid? referenceId = null);
}
