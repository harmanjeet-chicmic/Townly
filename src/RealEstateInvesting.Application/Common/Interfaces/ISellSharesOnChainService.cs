namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// Flow 6: Sell shares on chain. Approves property token for marketplace, calls sellShares, records in DB.
/// </summary>
public interface ISellSharesOnChainService
{
    /// <summary>
    /// 1) Approves property token for marketplace (if amount &gt; 0). 2) Calls marketplace.sellShares. 3) Records sale in DB.
    /// Uses TRex:PrivateKey wallet as the executor (that wallet must hold the property token).
    /// </summary>
    /// <param name="userWalletAddress">Wallet for audit (e.g. current user's wallet).</param>
    /// <param name="userId">Our user ID for audit.</param>
    /// <param name="propertyTokenAddress">Property ERC-20 token address.</param>
    /// <param name="amountOfSharesRaw">Amount of shares in smallest units (e.g. 18 decimals).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Approve tx hash (if any) and sell tx hash.</returns>
    Task<SellSharesOnChainResult> SellSharesOnChainAsync(
        string userWalletAddress,
        Guid? userId,
        string propertyTokenAddress,
        string amountOfSharesRaw,
        CancellationToken cancellationToken = default);
}

/// <summary>Result of a successful on-chain share sale.</summary>
public record SellSharesOnChainResult(string? ApproveTxHash, string SellTxHash);
