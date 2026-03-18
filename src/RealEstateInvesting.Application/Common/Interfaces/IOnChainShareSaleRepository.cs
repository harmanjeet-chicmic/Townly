using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>Repository for Flow 6 on-chain share sale audit records.</summary>
public interface IOnChainShareSaleRepository
{
    /// <summary>Persists a share sale record after successful approve + sellShares.</summary>
    Task AddAsync(OnChainShareSale sale, CancellationToken cancellationToken = default);
}
