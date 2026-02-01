using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Notifications.Interfaces;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);
    Task SaveChangesAsync();

    Task<List<Notification>> GetByUserAsync(Guid userId);
    Task<Notification?> GetByIdAsync(Guid id);
}
