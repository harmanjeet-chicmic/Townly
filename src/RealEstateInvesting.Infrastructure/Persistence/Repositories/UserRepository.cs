using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Infrastructure.Persistence;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<User?> GetByWalletAddressAsync(string walletAddress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(walletAddress)) return null;
        var normalized = walletAddress.Trim().ToLowerInvariant();
        if (!normalized.StartsWith("0x")) normalized = "0x" + normalized;
        return await _context.Users
            .FirstOrDefaultAsync(u => u.WalletAddress == normalized, cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task<List<User>> GetByIdsAsync(IEnumerable<Guid> userIds)
    {
        return await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync();
    }
}
