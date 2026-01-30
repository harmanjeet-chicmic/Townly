using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;

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
}
