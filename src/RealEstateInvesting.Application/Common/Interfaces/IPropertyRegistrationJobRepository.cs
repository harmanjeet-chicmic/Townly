using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface IPropertyRegistrationJobRepository
{
    Task<IReadOnlyDictionary<Guid, PropertyRegistrationJob>> GetLatestByPropertyIdsAsync(
        IEnumerable<Guid> propertyIds,
        CancellationToken cancellationToken = default);
}
