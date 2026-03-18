namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// T-REX Real Estate Registry contract (on-chain property registration).
/// Amoy: 0xB79a38247D66369fD9A5dF844Eac0fe94a88abd1.
/// Flow 4: Register property (requires REGISTER_ROLE).
/// </summary>
public interface IRealEstateRegistryContractService
{
    /// <summary>
    /// Registers a property on chain. Requires REGISTER_ROLE wallet in TRex:PrivateKey.
    /// </summary>
    /// <param name="to">Property owner/operator address.</param>
    /// <param name="uri">Property metadata URI (e.g. ipfs://Qm...).</param>
    /// <param name="tokenAddress">Property ERC-20 token address.</param>
    /// <param name="vaultAddress">Property vault address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Transaction hash. On-chain property ID can be obtained from transaction receipt/events.</returns>
    Task<string> RegisterPropertyAsync(string to, string uri, string tokenAddress, string vaultAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the property token address for a given on-chain property ID (view call).
    /// </summary>
    Task<string> GetPropertyTokenAsync(long onChainPropertyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the property vault address for a given on-chain property ID (view call).
    /// </summary>
    Task<string> GetPropertyVaultAsync(long onChainPropertyId, CancellationToken cancellationToken = default);
}
