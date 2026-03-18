using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// Repository for on-chain property registration audit records (Flow 4).
/// </summary>
public interface IOnChainPropertyRegistrationRepository
{
    /// <summary>
    /// Saves a new property registration record after a successful on-chain registerProperty call.
    /// </summary>
    Task AddAsync(OnChainPropertyRegistration registration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Records a registration with the given parameters (resolves nothing; caller provides all data).
    /// </summary>
    Task RecordRegistrationAsync(
        string toAddress,
        string uri,
        string tokenAddress,
        string vaultAddress,
        string transactionHash,
        Guid? propertyId,
        long? onChainPropertyId,
        Guid? performedByAdminId,
        CancellationToken cancellationToken = default);
}
