using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Infrastructure.Persistence;

namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

/// <summary>
/// Persists on-chain property registration records (Flow 4) for audit and future lookup.
/// </summary>
public class OnChainPropertyRegistrationRepository : IOnChainPropertyRegistrationRepository
{
    private readonly AppDbContext _context;

    public OnChainPropertyRegistrationRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task AddAsync(OnChainPropertyRegistration registration, CancellationToken cancellationToken = default)
    {
        await _context.OnChainPropertyRegistrations.AddAsync(registration, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task RecordRegistrationAsync(
        string toAddress,
        string uri,
        string tokenAddress,
        string vaultAddress,
        string transactionHash,
        Guid? propertyId,
        long? onChainPropertyId,
        Guid? performedByAdminId,
        CancellationToken cancellationToken = default)
    {
        var registration = OnChainPropertyRegistration.Create(
            toAddress, uri, tokenAddress, vaultAddress,
            transactionHash, propertyId, onChainPropertyId, performedByAdminId);
        await _context.OnChainPropertyRegistrations.AddAsync(registration, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
