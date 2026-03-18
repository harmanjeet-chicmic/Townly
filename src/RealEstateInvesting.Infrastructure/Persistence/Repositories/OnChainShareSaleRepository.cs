using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Infrastructure.Persistence;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

/// <summary>Persists Flow 6 share sale records.</summary>
public class OnChainShareSaleRepository : IOnChainShareSaleRepository
{
    private readonly AppDbContext _context;

    public OnChainShareSaleRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task AddAsync(OnChainShareSale sale, CancellationToken cancellationToken = default)
    {
        await _context.OnChainShareSales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
