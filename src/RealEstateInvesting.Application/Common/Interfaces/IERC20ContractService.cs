namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// ERC20 token contract (stablecoin or property token). Used for Flow 5 (approve + buy) and Flow 6 (approve + sell).
/// </summary>
public interface IERC20ContractService
{
    /// <summary>
    /// Approve spender to transfer up to amount from the signer's balance. Uses TRex:PrivateKey as signer.
    /// </summary>
    /// <param name="tokenAddress">ERC20 contract address (e.g. stablecoin or property token).</param>
    /// <param name="spender">Address to approve (e.g. marketplace).</param>
    /// <param name="amountRaw">Amount in token smallest units as string (e.g. "1000000" for 1 USDC with 6 decimals).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Transaction hash.</returns>
    Task<string> ApproveAsync(string tokenAddress, string spender, string amountRaw, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get token balance of an account (view call, no signer required). Returns balance in smallest units.
    /// </summary>
    Task<string> BalanceOfAsync(string tokenAddress, string accountAddress, CancellationToken cancellationToken = default);
}
