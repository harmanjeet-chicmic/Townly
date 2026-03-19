using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Infrastructure.Persistence;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

public class PropertyActivationRecordRepository : IPropertyActivationRecordRepository
{
    private readonly AppDbContext _context;

    public PropertyActivationRecordRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<Guid, Guid>> GetLatestJobIdByPropertyIdsAsync(IEnumerable<Guid> propertyIds, CancellationToken cancellationToken = default)
    {
        var idList = propertyIds.ToList();

        if (idList.Count == 0)
            return new Dictionary<Guid, Guid>();

        var records = await _context.PropertyActivationRecords.Where(r => idList.Contains(r.PropertyId))
                                                              .OrderByDescending(r => r.CreatedAt)
                                                              .ToListAsync(cancellationToken);
        return records.GroupBy(r => r.PropertyId).ToDictionary(g => g.Key, g => g.First().JobId);
    }

    /// <inheritdoc />
    public async Task<PropertyActivationRecord?> GetByJobIdAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return await _context.PropertyActivationRecords
            .FirstOrDefaultAsync(r => r.JobId == jobId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(PropertyActivationRecord record, CancellationToken cancellationToken = default)
    {
        _context.PropertyActivationRecords.Update(record);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
