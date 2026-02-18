using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Notifications.Interfaces;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);
    Task SaveChangesAsync();

    Task<List<Notification>> GetByUserAsync(Guid userId);
    Task<Notification?> GetByIdAsync(Guid id);
    Task<List<Notification>> GetUnreadByUserAsync(Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task MarkAllAsReadAsync(Guid userId);
    Task DeleteAsync(Notification notification);

}
