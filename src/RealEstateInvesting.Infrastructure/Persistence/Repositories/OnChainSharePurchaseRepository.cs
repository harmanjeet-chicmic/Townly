using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Infrastructure.Persistence;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

/// <summary>Persists Flow 5 share purchase records.</summary>
public class OnChainSharePurchaseRepository : IOnChainSharePurchaseRepository
{
    private readonly AppDbContext _context;

    public OnChainSharePurchaseRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task AddAsync(OnChainSharePurchase purchase, CancellationToken cancellationToken = default)
    {
        await _context.OnChainSharePurchases.AddAsync(purchase, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
