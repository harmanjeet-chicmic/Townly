using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>Repository for Flow 5 on-chain share purchase audit records.</summary>
public interface IOnChainSharePurchaseRepository
{
    /// <summary>Persists a share purchase record after successful approve + buyShares.</summary>
    Task AddAsync(OnChainSharePurchase purchase, CancellationToken cancellationToken = default);
}
