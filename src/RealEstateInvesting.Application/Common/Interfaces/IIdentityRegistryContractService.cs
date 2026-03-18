namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// T-REX Identity Registry contract (on-chain KYC).
/// Amoy network: 0x192738Fb0FF12Aa5564FAF18dEa315ccdF5A98ec
/// </summary>
public interface IIdentityRegistryContractService
{
    /// <summary>
    /// Check if a user address is KYC-verified on chain.
    /// </summary>
    Task<bool> IsVerified(string userAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Register or update identity for a user (requires AGENT_ROLE on contract).
    /// </summary>
    Task<string> UpdateIdentity(string userAddress, string identityContractAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set country code for a user (optional geofencing). ISO 3166-1 numeric, e.g. 250 = France.
    /// </summary>
    Task<string> UpdateCountry(string userAddress, ushort countryCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Register identity and country in one call (Buy Property Step 1 — when contract supports registerIdentity).
    /// Equivalent to updateIdentity + updateCountry in a single transaction.
    /// </summary>
    Task<string> RegisterIdentity(string userAddress, string identityContractAddress, ushort countryCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Same as RegisterIdentity but targets a specific Identity Registry contract (e.g. the token's IR after deployTREXSuite).
    /// </summary>
    Task<string> RegisterIdentityOnRegistryAsync(string identityRegistryAddress, string userAddress, string identityContractAddress, ushort countryCode, CancellationToken cancellationToken = default);
}
