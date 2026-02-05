using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Infrastructure.Persistence;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

public class UserTokenBalanceRepository : IUserTokenBalanceRepository
{
    private readonly AppDbContext _context;

    public UserTokenBalanceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserTokenBalance?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Set<UserTokenBalance>()
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task AddAsync(UserTokenBalance balance)
    {
        await _context.Set<UserTokenBalance>().AddAsync(balance);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
