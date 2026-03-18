using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Admin.Properties.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Infrastructure.Persistence;
using RealEstateInvesting.Application.Admin.Properties.DTOs;
namespace RealEstateInvesting.Infrastructure.Admin.Properties;

public class AdminPropertyRepository : IAdminPropertyRepository
{
    private readonly AppDbContext _db;
    

    public AdminPropertyRepository(AppDbContext db)
    {
        _db = db;
    }


    // public async Task<List<Property>> GetPendingAsync()
    // {
    //     return await _db.Properties
    //         .Where(p => p.Status == PropertyStatus.PendingApproval)
    //         .OrderBy(p => p.CreatedAt)
    //         .ToListAsync();
    // }
    public async Task<(List<Property>, int)> GetPendingAsync(AdminPropertyQuery query)
    {
        var dbQuery = _db.Properties
            .Where(p => p.Status == PropertyStatus.PendingApproval)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            dbQuery = dbQuery.Where(p =>
                p.Name.Contains(query.Search) ||
                p.Location.Contains(query.Search));
        }

        var totalCount = await dbQuery.CountAsync();

        var properties = await dbQuery
            .OrderByDescending(p => p.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (properties, totalCount);
    }


    public async Task<Property?> GetByIdAsync(Guid id)
    {
        return await _db.Properties.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
    public async Task<(List<Property>, int)> GetAllAsync(AdminPropertyQuery query)
{
    query ??= new AdminPropertyQuery();

    var dbQuery = _db.Properties.AsQueryable();

    if (query.Status.HasValue)
    {
        dbQuery = dbQuery.Where(p => p.Status == query.Status.Value);
    }

    if (!string.IsNullOrWhiteSpace(query.Search))
    {
        dbQuery = dbQuery.Where(p =>
            p.Name.Contains(query.Search) ||
            p.Location.Contains(query.Search));
    }

    var totalCount = await dbQuery.CountAsync();

    var properties = await dbQuery
        .OrderByDescending(p => p.CreatedAt)
        .Skip((query.Page - 1) * query.PageSize)
        .Take(query.PageSize)
        .ToListAsync();

    return (properties, totalCount);
}

    public async Task<decimal> GetTotalAssetValueAsync()
    {
        return await _db.Properties
            .Where(p => p.Status == PropertyStatus.Active || p.Status == PropertyStatus.SoldOut)
            .SumAsync(p => p.ApprovedValuation);
    }

    public async Task<int> GetPendingApprovalsCountAsync()
    {
        return await _db.Properties
            .CountAsync(p => p.Status == PropertyStatus.PendingApproval);
    }
}
