using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// Repository for on-chain vault supply audit records (Add Property Step 4).
/// </summary>
public interface IOnChainVaultSupplyRepository
{
    /// <summary>
    /// Saves a new vault supply record after identity and/or mint operations.
    /// </summary>
    Task AddAsync(OnChainVaultSupply supply, CancellationToken cancellationToken = default);
}
