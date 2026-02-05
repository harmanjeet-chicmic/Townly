
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

public class InvestmentRepository : IInvestmentRepository
{
    private readonly AppDbContext _context;

    public InvestmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Investment investment)
    {
        await _context.Investments.AddAsync(investment);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetTotalSharesInvestedAsync(Guid propertyId)
    {
        return await _context.Investments
            .Where(i => i.PropertyId == propertyId)
            .SumAsync(i => i.SharesPurchased);
    }

    public async Task<decimal> GetTotalAmountInvestedAsync(Guid propertyId)
    {
        return await _context.Investments
            .Where(i => i.PropertyId == propertyId)
            .SumAsync(i => i.TotalAmount);
    }

    public async Task<IEnumerable<Investment>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Investments
            .Where(i => i.UserId == userId).OrderByDescending(x=>x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Investment>> GetAllUserInvestmentsAsync()
    {
        return await _context.Investments.ToListAsync();
    }

    // -----------------------------
    // 🔥 Demand Score v1 methods
    // -----------------------------

    public async Task<int> GetSharesInvestedInLastDaysAsync(
        Guid propertyId,
        int days)
    {
        var fromDate = DateTime.UtcNow.AddDays(-days);

        return await _context.Investments
            .Where(i =>
                i.PropertyId == propertyId &&
                i.CreatedAt >= fromDate)
            .SumAsync(i => i.SharesPurchased);
    }

    public async Task<int> GetUniqueInvestorCountAsync(Guid propertyId)
    {
        return await _context.Investments
            .Where(i => i.PropertyId == propertyId)
            .Select(i => i.UserId)
            .Distinct()
            .CountAsync();
    }

    public async Task<DateTime?> GetLastInvestmentAtAsync(Guid propertyId)
    {
        return await _context.Investments
            .Where(i => i.PropertyId == propertyId)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => (DateTime?)i.CreatedAt)
            .FirstOrDefaultAsync();
    }
    public async Task<int> GetSharesInvestedInLastHoursAsync(
    Guid propertyId,
    int hours)
    {
        var fromTime = DateTime.UtcNow.AddHours(-hours);

        return await _context.Investments
            .Where(i =>
                !i.IsDeleted &&
                i.PropertyId == propertyId &&
                i.CreatedAt >= fromTime)
            .SumAsync(i => i.SharesPurchased);
    }

}

