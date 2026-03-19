using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// Repository for property activation (T-REX registration job) records.
/// </summary>
public interface IPropertyActivationRecordRepository
{
    /// <summary>
    /// Gets the latest job ID per property. For each property ID that has at least one activation record,
    /// returns the JobId of the most recent record (by CreatedAt).
    /// </summary>
    Task<IReadOnlyDictionary<Guid, Guid>> GetLatestJobIdByPropertyIdsAsync(
        IEnumerable<Guid> propertyIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the activation record by job ID, or null if not found.
    /// </summary>
    Task<PropertyActivationRecord?> GetByJobIdAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists changes to an existing activation record.
    /// </summary>
    Task UpdateAsync(PropertyActivationRecord record, CancellationToken cancellationToken = default);
}
