using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface IKycRecordRepository
{
    Task<bool> HasPendingKycAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(KycRecord kycRecord, CancellationToken cancellationToken = default);
}
