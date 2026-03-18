using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Investments;

/// <summary>
/// Flow 5: KYC check → approve stablecoin → buyShares → record in OnChainSharePurchases.
/// Maps to "Buy Property" steps 3–4 in the T-REX Developer Guide: approve marketplace to spend stablecoin, then marketplace.buyShares(tokenAddress, amount). The marketplace resolves the vault, gets pricePerShare, pulls USDC from the user, and calls vault.buyFor(user, amount).
/// </summary>
public sealed class BuySharesOnChainService : IBuySharesOnChainService
{
    private readonly IOnChainKycService _kycService;
    private readonly IBlockchainSettings _blockchainSettings;
    private readonly IERC20ContractService _erc20;
    private readonly IRealEstateMarketplaceContractService _marketplace;
    private readonly IOnChainSharePurchaseRepository _purchaseRepository;

    public BuySharesOnChainService(
        IOnChainKycService kycService,
        IBlockchainSettings blockchainSettings,
        IERC20ContractService erc20,
        IRealEstateMarketplaceContractService marketplace,
        IOnChainSharePurchaseRepository purchaseRepository)
    {
        _kycService = kycService;
        _blockchainSettings = blockchainSettings;
        _erc20 = erc20;
        _marketplace = marketplace;
        _purchaseRepository = purchaseRepository;
    }

    /// <inheritdoc />
    public async Task<BuySharesOnChainResult> BuySharesOnChainAsync(
        string userWalletAddress,
        Guid? userId,
        string propertyTokenAddress,
        string amountOfSharesRaw,
        string amountStablecoinToApproveRaw,
        CancellationToken cancellationToken = default)
    {
        var isVerified = await _kycService.IsVerifiedAsync(userWalletAddress, cancellationToken);
        if (!isVerified)
            throw new InvalidOperationException("User wallet is not KYC verified. Complete KYC before buying shares.");

        var stablecoinAmount = string.IsNullOrWhiteSpace(amountStablecoinToApproveRaw) ? "0" : amountStablecoinToApproveRaw.Trim();
        string? approveTxHash = null;
        if (stablecoinAmount != "0")
        {
            approveTxHash = await _erc20.ApproveAsync(
                _blockchainSettings.StablecoinAddress,
                _blockchainSettings.RealEstateMarketplaceAddress,
                stablecoinAmount,
                cancellationToken);
        }

        var buyTxHash = await _marketplace.BuySharesAsync(propertyTokenAddress, amountOfSharesRaw, cancellationToken);

        var purchase = OnChainSharePurchase.Create(
            propertyTokenAddress,
            amountOfSharesRaw,
            stablecoinAmount,
            approveTxHash,
            buyTxHash,
            userId,
            userWalletAddress);
        await _purchaseRepository.AddAsync(purchase, cancellationToken);

        return new BuySharesOnChainResult(approveTxHash, buyTxHash);
    }
}
