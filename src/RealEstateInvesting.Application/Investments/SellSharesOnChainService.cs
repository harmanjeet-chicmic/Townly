using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Investments;

/// <summary>Flow 6: Approve property token for marketplace → sellShares → record in OnChainShareSales.</summary>
public sealed class SellSharesOnChainService : ISellSharesOnChainService
{
    private readonly IBlockchainSettings _blockchainSettings;
    private readonly IERC20ContractService _erc20;
    private readonly IRealEstateMarketplaceContractService _marketplace;
    private readonly IOnChainShareSaleRepository _saleRepository;

    public SellSharesOnChainService(
        IBlockchainSettings blockchainSettings,
        IERC20ContractService erc20,
        IRealEstateMarketplaceContractService marketplace,
        IOnChainShareSaleRepository saleRepository)
    {
        _blockchainSettings = blockchainSettings;
        _erc20 = erc20;
        _marketplace = marketplace;
        _saleRepository = saleRepository;
    }

    /// <inheritdoc />
    public async Task<SellSharesOnChainResult> SellSharesOnChainAsync(
        string userWalletAddress,
        Guid? userId,
        string propertyTokenAddress,
        string amountOfSharesRaw,
        CancellationToken cancellationToken = default)
    {
        string? approveTxHash = null;
        if (!string.IsNullOrWhiteSpace(amountOfSharesRaw) && amountOfSharesRaw.Trim() != "0")
        {
            approveTxHash = await _erc20.ApproveAsync(
                propertyTokenAddress,
                _blockchainSettings.RealEstateMarketplaceAddress,
                amountOfSharesRaw.Trim(),
                cancellationToken);
        }

        var sellTxHash = await _marketplace.SellSharesAsync(propertyTokenAddress, amountOfSharesRaw, cancellationToken);

        var sale = OnChainShareSale.Create(
            propertyTokenAddress,
            amountOfSharesRaw,
            approveTxHash,
            sellTxHash,
            userId,
            userWalletAddress);
        await _saleRepository.AddAsync(sale, cancellationToken);

        return new SellSharesOnChainResult(approveTxHash, sellTxHash);
    }
}
