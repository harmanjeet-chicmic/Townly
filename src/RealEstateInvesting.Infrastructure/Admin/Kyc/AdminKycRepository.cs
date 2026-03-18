using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Admin.Kyc.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Infrastructure.Persistence;
using RealEstateInvesting.Application.Admin.Kyc.DTOs;

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

    public async Task<int> GetPendingKycCountAsync()
    {
        return await _db.KycRecords
            .CountAsync(x => x.Status == KycStatus.Pending);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }

    public async Task<List<KycRecord>> GetFilteredAsync(AdminKycQuery query)
    {
        var dbQuery = _db.KycRecords.AsQueryable();

        if (query.Status.HasValue)
        {
            dbQuery = dbQuery.Where(x => x.Status == (KycStatus)query.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            dbQuery = dbQuery.Where(x => x.FullName.Contains(query.Search));
        }

        return await dbQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();
    }

    public async Task<int> GetFilteredCountAsync(AdminKycQuery query)
    {
        var dbQuery = _db.KycRecords.AsQueryable();

        if (query.Status.HasValue)
        {
            dbQuery = dbQuery.Where(x => x.Status == (KycStatus)query.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            dbQuery = dbQuery.Where(x => x.FullName.Contains(query.Search));
        }

        return await dbQuery.CountAsync();
    }
}
