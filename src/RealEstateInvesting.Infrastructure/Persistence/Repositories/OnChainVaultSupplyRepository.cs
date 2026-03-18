using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Infrastructure.Persistence;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

/// <summary>
/// Persists on-chain vault supply records (Add Property Step 4) for audit.
/// </summary>
public class OnChainVaultSupplyRepository : IOnChainVaultSupplyRepository
{
    private readonly AppDbContext _context;

    public OnChainVaultSupplyRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task AddAsync(OnChainVaultSupply supply, CancellationToken cancellationToken = default)
    {
        await _context.OnChainVaultSupplies.AddAsync(supply, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
