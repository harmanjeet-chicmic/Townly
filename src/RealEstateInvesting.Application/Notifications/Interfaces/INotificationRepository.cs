using RealEstateInvesting.Application.Common.Dtos;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Notifications.Interfaces;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);
    Task SaveChangesAsync();

    Task<PagedResult<Notification>> GetByUserAsync(
        Guid userId,
        int page,
        int pageSize,
        string? search,
        string? notificationType);

    Task<PagedResult<Notification>> GetUnreadByUserAsync(
        Guid userId,
        int page,
        int pageSize);

    Task<int> GetUnreadCountAsync(Guid userId);

    Task<Notification?> GetByIdAsync(Guid id);

    Task MarkAllAsReadAsync(Guid userId);

    Task DeleteAsync(Notification notification);
}