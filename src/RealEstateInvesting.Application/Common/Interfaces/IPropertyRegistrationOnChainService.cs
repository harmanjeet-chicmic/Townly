namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// Orchestrates Flow 4: Register property on chain (T-REX Real Estate Registry) and persist audit record in DB.
/// Maps to "Add Property" step 3 in the T-REX Real Estate Tokenization Developer Guide: registerProperty(issuer, metadataUri, tokenAddress, vaultAddress) mints the property NFT linking token and vault. Steps 1–2 (deploy suite, deploy vault) and step 4 (supply vault) are out of scope.
/// </summary>
public interface IPropertyRegistrationOnChainService
{
    /// <summary>
    /// Calls Real Estate Registry registerProperty then records the registration in DB.
    /// </summary>
    /// <param name="toAddress">Property owner/operator wallet.</param>
    /// <param name="uri">Property metadata URI (e.g. ipfs://Qm...).</param>
    /// <param name="tokenAddress">Property ERC-20 token address.</param>
    /// <param name="vaultAddress">Property vault address.</param>
    /// <param name="propertyId">Optional our internal Property ID to link.</param>
    /// <param name="performedByAdminId">Admin who triggered the registration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Transaction hash. On-chain property ID can be read from receipt/events if needed.</returns>
    Task<string> RegisterPropertyOnChainAsync(
        string toAddress,
        string uri,
        string tokenAddress,
        string vaultAddress,
        Guid? propertyId,
        Guid performedByAdminId,
        CancellationToken cancellationToken = default);
}
