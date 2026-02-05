using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Notifications.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Infrastructure.Persistence;

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

    public async Task<List<Notification>> GetByUserAsync(Guid userId)
    {
        return await _db.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        return await _db.Notifications.FirstOrDefaultAsync(n => n.Id == id);
    }
    public async Task<List<Notification>> GetUnreadByUserAsync(Guid userId)
    {
        return await _db.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
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
}
