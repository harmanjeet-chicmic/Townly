using RealEstateInvesting.Application.Admin.Kyc.DTOs;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Admin.Kyc.Interfaces;

public interface IAdminKycRepository
{
    Task<List<KycRecord>> GetPendingAsync();
    Task<KycRecord?> GetByIdAsync(Guid id);
    Task<int> GetPendingKycCountAsync();
    Task SaveChangesAsync();
    Task<List<KycRecord>> GetFilteredAsync(AdminKycQuery query);
    Task<int> GetFilteredCountAsync(AdminKycQuery query);
}
