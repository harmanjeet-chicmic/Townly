using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Notifications.Interfaces;

public interface INotificationService
{
    Task CreateAsync(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        Guid? referenceId = null);
}
