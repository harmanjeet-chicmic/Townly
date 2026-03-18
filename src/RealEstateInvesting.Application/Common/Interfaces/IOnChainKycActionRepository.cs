using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// Persists on-chain KYC actions (Flow 2 &amp; 3) for audit: identity updates and country updates.
/// </summary>
public interface IOnChainKycActionRepository
{
    /// <summary>Adds a single on-chain KYC action entity.</summary>
    Task AddAsync(OnChainKycAction action, CancellationToken cancellationToken = default);

    /// <summary>Resolves UserId by wallet, creates and saves an identity-update record.</summary>
    Task RecordIdentityUpdateAsync(string walletAddress, string identityContractAddress, string transactionHash, Guid? performedByAdminId, CancellationToken cancellationToken = default);

    /// <summary>Resolves UserId by wallet, creates and saves a country-update record.</summary>
    Task RecordCountryUpdateAsync(string walletAddress, ushort countryCode, string transactionHash, Guid? performedByAdminId, CancellationToken cancellationToken = default);

    /// <summary>Resolves UserId by wallet, creates and saves a registerIdentity (identity + country) record.</summary>
    Task RecordRegisterIdentityAsync(string walletAddress, string identityContractAddress, ushort countryCode, string transactionHash, Guid? performedByAdminId, CancellationToken cancellationToken = default);
}
