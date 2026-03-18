namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// Flow 5: Buy shares on chain. Verifies KYC, approves stablecoin for marketplace, calls buyShares, records in DB.
/// </summary>
public interface IBuySharesOnChainService
{
    /// <summary>
    /// 1) Verifies userWallet is KYC'd. 2) Approves stablecoin for marketplace. 3) Calls marketplace.buyShares. 4) Records purchase in DB.
    /// Uses TRex:PrivateKey wallet as the executor (that wallet must hold stablecoin).
    /// </summary>
    /// <param name="userWalletAddress">Wallet to check for KYC (e.g. current user's wallet).</param>
    /// <param name="userId">Our user ID for audit.</param>
    /// <param name="propertyTokenAddress">Property ERC-20 token address.</param>
    /// <param name="amountOfSharesRaw">Amount of shares in smallest units (e.g. 18 decimals).</param>
    /// <param name="amountStablecoinToApproveRaw">Stablecoin amount to approve in smallest units (e.g. 6 decimals).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Approve tx hash (if any) and buy tx hash.</returns>
    Task<BuySharesOnChainResult> BuySharesOnChainAsync(
        string userWalletAddress,
        Guid? userId,
        string propertyTokenAddress,
        string amountOfSharesRaw,
        string amountStablecoinToApproveRaw,
        CancellationToken cancellationToken = default);
}

/// <summary>Result of a successful on-chain share purchase.</summary>
public record BuySharesOnChainResult(string? ApproveTxHash, string BuyTxHash);
