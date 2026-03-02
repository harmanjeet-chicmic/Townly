using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface IPropertyDocumentRepository
{
    Task AddRangeAsync(IEnumerable<PropertyDocument> documents);
    Task<List<PropertyDocument>> GetByPropertyIdAsync(Guid propertyId);
    Task SoftDeleteByPropertyIdAsync(Guid propertyId, Guid? deletedBy = null);
}
