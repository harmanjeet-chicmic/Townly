using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Infrastructure.Persistence;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

public class TokenRequestRepository : ITokenRequestRepository
{
    private readonly AppDbContext _context;

    public TokenRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TokenRequest request)
    {
        await _context.Set<TokenRequest>().AddAsync(request);
    }

    public async Task<TokenRequest?> GetByIdAsync(Guid id)
    {
        return await _context.Set<TokenRequest>().FindAsync(id);
    }

    public async Task<List<TokenRequest>> GetPendingAsync()
    {
        return await _context.Set<TokenRequest>()
            .Where(x => x.Status == TokenRequestStatus.Pending)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<TokenRequest>> GetByUserAsync(Guid userId)
    {
        return await _context.Set<TokenRequest>()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
