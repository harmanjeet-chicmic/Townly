using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Transactions.Dtos;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Transactions;

public class TokenPurchaseQueryService
{
    private readonly ITokenPurchaseRepository _repo;

    public TokenPurchaseQueryService(ITokenPurchaseRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TokenPurchaseDto>> GetMyTransactions(
    string walletAddress,
    int page,
    int pageSize,
    CancellationToken ct)
    {
        var data = await _repo.GetByWalletAsync(walletAddress, page, pageSize, ct);

        return data.Select(x => new TokenPurchaseDto
        {
            Id = x.Id,
            Status = x.Status,
            PropertyId = x.PropertyId,
            TransactionHash = x.TransactionHash,
            BuyerAddress = x.BuyerAddress,
            SellerAddress = x.SellerAddress,
            Shares = x.Shares,
            PricePerShare = x.PricePerShare,
            ErrorMessage = x.ErrorMessage,
            CreatedAt = x.CreatedAt
        }).ToList();
    }
}