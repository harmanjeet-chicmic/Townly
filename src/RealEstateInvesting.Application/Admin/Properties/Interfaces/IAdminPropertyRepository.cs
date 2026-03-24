using RealEstateInvesting.Application.Admin.Properties.DTOs;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Admin.Properties.Interfaces;

public interface IAdminPropertyRepository
{
    Task<(List<Property>, int)> GetPendingAsync(AdminPropertyQuery query);
    Task<Property?> GetByIdAsync(Guid id);
    Task SaveChangesAsync();
    Task<(List<Property>, int)> GetAllAsync(AdminPropertyQuery query);
    Task<decimal> GetTotalAssetValueAsync();
    Task<int> GetPendingApprovalsCountAsync();
}
