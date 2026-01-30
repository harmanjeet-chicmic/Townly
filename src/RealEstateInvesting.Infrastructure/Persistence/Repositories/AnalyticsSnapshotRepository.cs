using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

public class AnalyticsSnapshotRepository : IAnalyticsSnapshotRepository
{
    private readonly AppDbContext _context;

    public AnalyticsSnapshotRepository(AppDbContext context)
    {
        _context = context;
    }

    // -------------------------------
    // Property analytics
    // -------------------------------
    public async Task AddPropertySnapshotAsync(PropertyAnalyticsSnapshot snapshot)
    {
        _context.PropertyAnalyticsSnapshots.Add(snapshot);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<PropertyAnalyticsSnapshot>> GetPropertySnapshotsAsync(
        Guid propertyId,
        DateTime fromUtc)
    {
        return await _context.PropertyAnalyticsSnapshots
            .Where(s => s.PropertyId == propertyId && s.SnapshotAt >= fromUtc)
            .OrderBy(s => s.SnapshotAt)
            .ToListAsync();
    }

    // -------------------------------
    // User portfolio analytics
    // -------------------------------
    public async Task AddUserPortfolioSnapshotAsync(UserPortfolioSnapshot snapshot)
    {
        _context.UserPortfolioSnapshots.Add(snapshot);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<UserPortfolioSnapshot>> GetUserPortfolioSnapshotsAsync(
        Guid userId,
        DateTime fromUtc)
    {
        return await _context.UserPortfolioSnapshots
            .Where(s => s.UserId == userId && s.SnapshotAt >= fromUtc)
            .OrderBy(s => s.SnapshotAt)
            .ToListAsync();
    }
    public async Task<PropertyAnalyticsSnapshot?> GetLatestPropertySnapshotAsync(Guid propertyId)
    {
        return await _context.PropertyAnalyticsSnapshots
            .Where(x => x.PropertyId == propertyId)
            .OrderByDescending(x => x.SnapshotAt)
            .FirstOrDefaultAsync();
    }
    public async Task<IEnumerable<PropertyAnalyticsSnapshot>>
    GetLatestPropertySnapshotsAsync(IEnumerable<Guid> propertyIds)
    {
        return await _context.PropertyAnalyticsSnapshots
            .Where(s => propertyIds.Contains(s.PropertyId))
            .GroupBy(s => s.PropertyId)
            .Select(g => g
                .OrderByDescending(x => x.SnapshotAt)
                .First())
            .ToListAsync();
    }


}
