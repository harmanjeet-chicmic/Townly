// using RealEstateInvesting.Domain.Entities;

// namespace RealEstateInvesting.Application.Admin.Properties.Interfaces;

// public interface IAdminPropertyRepository
// {
//     Task<List<Property>> GetPendingAsync();
//     Task<Property?> GetByIdAsync(Guid id);
//     Task SaveChangesAsync();
// }

using RealEstateInvesting.Application.Admin.Properties.DTOs;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Admin.Properties.Interfaces;

public interface IAdminPropertyRepository
{
    Task<(List<Property> Properties, int TotalCount)> GetPendingAsync(AdminPropertyQuery query);

    Task<Property?> GetByIdAsync(Guid id);

    Task SaveChangesAsync();
    Task<(List<Property>, int)> GetAllAsync(AdminPropertyQuery query);
}