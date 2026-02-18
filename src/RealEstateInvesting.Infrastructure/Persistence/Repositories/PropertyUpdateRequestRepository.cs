using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

public class PropertyUpdateRequestRepository : IPropertyUpdateRequestRepository
{
    private readonly AppDbContext _context;

    public PropertyUpdateRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PropertyUpdateRequest request)
    {
        _context.PropertyUpdateRequests.Add(request);
        await _context.SaveChangesAsync();
    }

    public async Task<PropertyUpdateRequest?> GetPendingByPropertyIdAsync(Guid propertyId)
    {
        return await _context.PropertyUpdateRequests
            .FirstOrDefaultAsync(x =>
                x.PropertyId == propertyId &&
                x.Status == PropertyUpdateStatus.Pending);
    }

    public async Task<PropertyUpdateRequest?> GetByIdAsync(Guid id)
    {
        return await _context.PropertyUpdateRequests.FindAsync(id);
    }

    public async Task UpdateAsync(PropertyUpdateRequest request)
    {
        _context.PropertyUpdateRequests.Update(request);
        await _context.SaveChangesAsync();
    }
    public async Task<List<PropertyUpdateRequest>> GetAllPendingAsync()
    {
        return await _context.PropertyUpdateRequests
            .Where(x => x.Status == PropertyUpdateStatus.Pending)
            .OrderByDescending(x => x.RequestedAt)
            .ToListAsync();
    }

}
