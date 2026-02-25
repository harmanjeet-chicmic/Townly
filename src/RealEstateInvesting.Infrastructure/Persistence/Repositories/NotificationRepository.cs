using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Notifications.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Infrastructure.Persistence;
using RealEstateInvesting.Application.Common.Dtos;
namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _db;

    public NotificationRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Notification notification)
    {
        await _db.Notifications.AddAsync(notification);
    }

   public async Task<PagedResult<Notification>> GetByUserAsync(
    Guid userId,
    int page,
    int pageSize,
    string? search,
    string? notificationType)
{
    var query = _db.Notifications
        .Where(n => n.UserId == userId);

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(n =>
            n.Title.Contains(search) ||
            n.Message.Contains(search));
    }

    if (!string.IsNullOrWhiteSpace(notificationType))
    {
        query = query.Where(n =>
            n.Type.ToString() == notificationType);
    }

    var totalCount = await query.CountAsync();

    var items = await query
        .OrderByDescending(n => n.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new PagedResult<Notification>
    {
        Page = page,
        PageSize = pageSize,
        TotalCount = totalCount,
        HasMore = (page * pageSize) < totalCount,
        Items = items
    };
}

    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        return await _db.Notifications.FirstOrDefaultAsync(n => n.Id == id);
    }
    public async Task<PagedResult<Notification>> GetUnreadByUserAsync(
    Guid userId,
    int page,
    int pageSize)
{
    var query = _db.Notifications
        .Where(n => n.UserId == userId && !n.IsRead);

    var totalCount = await query.CountAsync();

    var items = await query
        .OrderByDescending(n => n.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new PagedResult<Notification>
    {
        Page = page,
        PageSize = pageSize,
        TotalCount = totalCount,
        HasMore = (page * pageSize) < totalCount,
        Items = items
    };
}

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _db.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var unread = await _db.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var n in unread)
            n.MarkAsRead();
    }


    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
    public async Task DeleteAsync(Notification notification)

    {
         _db.Notifications.Remove(notification);
         await _db.SaveChangesAsync();
    }
}
