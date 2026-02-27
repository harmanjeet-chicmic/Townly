using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

public class PropertyDocumentRepository : IPropertyDocumentRepository
{
    private readonly AppDbContext _context;

    public PropertyDocumentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(IEnumerable<PropertyDocument> documents)
    {
        _context.PropertyDocuments.AddRange(documents);
        await _context.SaveChangesAsync();
    }
    public async Task<List<PropertyDocument>> GetByPropertyIdAsync(Guid propertyId)
{
    return await _context.PropertyDocuments
        .Where(d => d.PropertyId == propertyId)
        .OrderByDescending(d => d.UploadedAt)
        .ToListAsync();
}
}
