using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Admin.Properties.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Infrastructure.Persistence;

namespace RealEstateInvesting.Infrastructure.Admin.Properties;

public class AdminPropertyRepository : IAdminPropertyRepository
{
    private readonly AppDbContext _db;

    public AdminPropertyRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Property>> GetPendingAsync()
    {
        return await _db.Properties
            .Where(p => p.Status == PropertyStatus.PendingApproval)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Property?> GetByIdAsync(Guid id)
    {
        return await _db.Properties.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
