using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface ITokenPurchaseRepository
{
    Task<List<TokenPurchase>> GetByWalletAsync(
        string walletAddress,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<List<TokenPurchase>> GetAllByWalletAsync(
        string walletAddress,
        CancellationToken ct = default);
}