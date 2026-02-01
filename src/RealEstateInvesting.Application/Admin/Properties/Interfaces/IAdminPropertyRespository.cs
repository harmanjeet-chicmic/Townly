using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Admin.Properties.Interfaces;

public interface IAdminPropertyRepository
{
    Task<List<Property>> GetPendingAsync();
    Task<Property?> GetByIdAsync(Guid id);
    Task SaveChangesAsync();
}
