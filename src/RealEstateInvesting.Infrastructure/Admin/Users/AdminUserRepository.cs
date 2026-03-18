using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Admin.Users.DTOs;
using RealEstateInvesting.Application.Admin.Users.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Infrastructure.Persistence;

namespace RealEstateInvesting.Infrastructure.Admin.Users;

public class AdminUserRepository : IAdminUserRepository
{
    private readonly AppDbContext _context;

    public AdminUserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<User>, int)> GetAllAsync(AdminUserQuery query)
    {
        query ??= new AdminUserQuery();

        var dbQuery = _context.Users.AsQueryable();

        // 🔥 Filter: Blocked / Active
        if (query.IsBlocked.HasValue)
        {
            dbQuery = dbQuery.Where(u => u.IsBlocked == query.IsBlocked.Value);
        }

        // 🔥 Filter: KYC Status (ENUM ✅)
        if (query.KycStatus.HasValue)
        {
            dbQuery = dbQuery.Where(u => u.KycStatus == query.KycStatus.Value);
        }

        // 🔥 Filter: Role (ENUM ✅)
        if (query.Role.HasValue)
        {
            dbQuery = dbQuery.Where(u => u.Role == query.Role.Value);
        }

        // 🔥 Search (Wallet only, since no email/name)
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            dbQuery = dbQuery.Where(u =>
                u.WalletAddress.Contains(query.Search));
        }

        var totalCount = await dbQuery.CountAsync();

        var users = await dbQuery
            .OrderByDescending(u => u.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (users, totalCount);
    }
}