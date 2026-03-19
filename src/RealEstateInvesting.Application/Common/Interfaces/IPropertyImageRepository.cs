using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface IPropertyImageRepository
{
    Task AddRangeAsync(IEnumerable<PropertyImage> images);
    Task<List<PropertyImage>> GetByPropertyIdAsync(Guid propertyId);
    Task<List<PropertyImage>> GetByPropertyIdsAsync(IEnumerable<Guid> propertyIds);
    Task SoftDeleteByPropertyIdAsync(Guid propertyId, Guid? deletedBy = null);
}
