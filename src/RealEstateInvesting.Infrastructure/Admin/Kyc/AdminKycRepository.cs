using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Admin.Kyc.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Infrastructure.Persistence;

namespace RealEstateInvesting.Infrastructure.Admin.Kyc;

public class AdminKycRepository : IAdminKycRepository
{
    private readonly AppDbContext _db;

    public AdminKycRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<KycRecord>> GetPendingAsync()
    {
        return await _db.KycRecords
            .Where(x => x.Status == KycStatus.Pending)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<KycRecord?> GetByIdAsync(Guid id)
    {
        return await _db.KycRecords.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
