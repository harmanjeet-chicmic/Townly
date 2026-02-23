using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;
namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;

    public TransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(IEnumerable<Transaction> transactions)
    {
        _context.Transactions.AddRange(transactions);
        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<Transaction> Items, int TotalCount)>
GetByUserIdPagedAsync(
    Guid userId,
    int page,
    int pageSize,
    TransactionType? type)
    {
        var query = _context.Transactions
            .Where(t => t.UserId == userId);

        if (type.HasValue)
        {
            query = query.Where(t => t.Type == type.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

}
