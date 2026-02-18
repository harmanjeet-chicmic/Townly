using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface IPropertyUpdateRequestRepository
{
    Task AddAsync(PropertyUpdateRequest request);
    Task<PropertyUpdateRequest?> GetPendingByPropertyIdAsync(Guid propertyId);
    Task<List<PropertyUpdateRequest>> GetAllPendingAsync();

    Task<PropertyUpdateRequest?> GetByIdAsync(Guid id);
    Task UpdateAsync(PropertyUpdateRequest request);
}
