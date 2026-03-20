using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

public class TokenPurchaseRepository : ITokenPurchaseRepository
{
    private readonly AppDbContext _context;

    public TokenPurchaseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TokenPurchase entity, CancellationToken ct = default)
    {
        await _context.TokenPurchases.AddAsync(entity, ct);
    }

    public async Task<List<TokenPurchase>> GetByWalletAsync(
    string walletAddress,
    int page,
    int pageSize,
    CancellationToken ct = default)
    {
        return await _context.TokenPurchases
            .Where(x => x.BuyerAddress == walletAddress
                     || x.SellerAddress == walletAddress)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }
}