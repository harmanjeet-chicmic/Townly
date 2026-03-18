namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// T-REX Real Estate Marketplace contract. Flow 5: buyShares. Flow 6: sellShares.
/// Amoy: 0x4f087f31e47F6EC53e3eAaE7Eb7234B7A459DfBA.
/// </summary>
public interface IRealEstateMarketplaceContractService
{
    /// <summary>
    /// Buy shares of a property token. Caller (signer) must have approved stablecoin for the marketplace.
    /// Uses TRex:PrivateKey as signer.
    /// </summary>
    /// <param name="propertyTokenAddress">Property ERC-20 token address.</param>
    /// <param name="amountOfSharesRaw">Amount in token smallest units as string (typically 18 decimals).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Transaction hash.</returns>
    Task<string> BuySharesAsync(string propertyTokenAddress, string amountOfSharesRaw, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sell shares of a property token. Caller must have approved the property token for the marketplace.
    /// Uses TRex:PrivateKey as signer.
    /// </summary>
    Task<string> SellSharesAsync(string propertyTokenAddress, string amountOfSharesRaw, CancellationToken cancellationToken = default);
}
