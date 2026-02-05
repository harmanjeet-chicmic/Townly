using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Infrastructure.Persistence;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

public class TokenTransactionRepository : ITokenTransactionRepository
{
    private readonly AppDbContext _context;

    public TokenTransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TokenTransaction transaction)
    {
        await _context.Set<TokenTransaction>().AddAsync(transaction);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
