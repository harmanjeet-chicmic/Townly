using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Infrastructure.Persistence;
using System.Linq;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

public class PropertyRegistrationJobRepository : IPropertyRegistrationJobRepository
{
    private readonly AppDbContext _context;

    public PropertyRegistrationJobRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyDictionary<Guid, PropertyRegistrationJob>> GetLatestByPropertyIdsAsync(
     IEnumerable<Guid> propertyIds,
     CancellationToken cancellationToken = default)
    {
        var ids = propertyIds.ToList();
        if (ids.Count == 0)
            return new Dictionary<Guid, PropertyRegistrationJob>();

        var stringIds = ids.Select(x => x.ToString()).ToList();

        var rows = await _context.PropertyRegistrationJobs
            .Where(x => stringIds.Contains(x.PropertyId)) // ✅ string match
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return rows
            .Where(x => Guid.TryParse(x.PropertyId, out _)) // ✅ avoid crash
            .GroupBy(x => Guid.Parse(x.PropertyId))
            .ToDictionary(g => g.Key, g => g.First());
    }
    
}
