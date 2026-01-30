using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface IAnalyticsSnapshotRepository
{
    // Property analytics
    Task AddPropertySnapshotAsync(PropertyAnalyticsSnapshot snapshot);
    Task<IEnumerable<PropertyAnalyticsSnapshot>> GetPropertySnapshotsAsync(
        Guid propertyId,
        DateTime fromUtc);

    // User portfolio analytics
    Task AddUserPortfolioSnapshotAsync(UserPortfolioSnapshot snapshot);
    Task<IEnumerable<UserPortfolioSnapshot>> GetUserPortfolioSnapshotsAsync(
        Guid userId,
        DateTime fromUtc);
    Task<PropertyAnalyticsSnapshot?> GetLatestPropertySnapshotAsync(Guid propertyId);
    Task<IEnumerable<PropertyAnalyticsSnapshot>>
    GetLatestPropertySnapshotsAsync(IEnumerable<Guid> propertyIds);


}
