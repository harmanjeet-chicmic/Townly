namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// Orchestrates on-chain KYC: Flow 1 (read verified), Flow 2 (update identity), Flow 3 (set country).
/// Uses Identity Registry contract and persists write operations to OnChainKycActions table.
/// </summary>
public interface IOnChainKycService
{
    /// <summary>Flow 1: Check if an address is KYC-verified on chain (view call, no DB).</summary>
    Task<bool> IsVerifiedAsync(string address, CancellationToken cancellationToken = default);

    /// <summary>Calls Identity Registry updateIdentity then records the action in DB. Returns transaction hash.</summary>
    Task<string> UpdateIdentityOnChainAsync(string userAddress, string identityContractAddress, Guid performedByAdminId, CancellationToken cancellationToken = default);

    /// <summary>Calls Identity Registry updateCountry then records the action in DB. Returns transaction hash.</summary>
    Task<string> UpdateCountryOnChainAsync(string userAddress, ushort countryCode, Guid performedByAdminId, CancellationToken cancellationToken = default);

    /// <summary>Buy Property Step 1 (optional): registerIdentity(user, identity, country) in one tx when contract supports it; records in DB.</summary>
    Task<string> RegisterIdentityOnChainAsync(string userAddress, string identityContractAddress, ushort countryCode, Guid performedByAdminId, CancellationToken cancellationToken = default);
}
