using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Admin.Kyc.Interfaces;

public interface IAdminKycRepository
{
    Task<List<KycRecord>> GetPendingAsync();
    Task<KycRecord?> GetByIdAsync(Guid id);
    Task SaveChangesAsync();
}
