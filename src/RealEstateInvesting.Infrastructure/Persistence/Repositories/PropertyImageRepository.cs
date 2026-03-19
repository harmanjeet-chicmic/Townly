using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

public class PropertyImageRepository : IPropertyImageRepository
{
    private readonly AppDbContext _context;

    public PropertyImageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(IEnumerable<PropertyImage> images)
    {
        _context.PropertyImages.AddRange(images);
        await _context.SaveChangesAsync();
    }

    public async Task<List<PropertyImage>> GetByPropertyIdAsync(Guid propertyId)
    {
        return await _context.PropertyImages
            .Where(i => i.PropertyId == propertyId && !i.IsDeleted)
            .OrderByDescending(i => i.UploadedAt)
            .ToListAsync();
    }

    public async Task<List<PropertyImage>> GetByPropertyIdsAsync(IEnumerable<Guid> propertyIds)
    {
        return await _context.PropertyImages
            .Where(i => propertyIds.Contains(i.PropertyId) && !i.IsDeleted)
            .OrderByDescending(i => i.UploadedAt)
            .ToListAsync();
    }

    public async Task SoftDeleteByPropertyIdAsync(
        Guid propertyId,
        Guid? deletedBy = null)
    {
        var images = await _context.PropertyImages
            .Where(i => i.PropertyId == propertyId && !i.IsDeleted)
            .ToListAsync();

        foreach (var image in images)
        {
            image.SoftDelete(deletedBy);
        }

        await _context.SaveChangesAsync();
    }
}
