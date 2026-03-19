using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Admin.Properties.Interfaces;
public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(Guid id);
    Task<List<Organization>> GetAllAsync(CancellationToken ct = default);
}