using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;

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

    public async Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}
