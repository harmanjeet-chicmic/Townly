using RealEstateInvesting.Application.Admin.Kyc.DTOs;

namespace RealEstateInvesting.Application.Admin.Kyc.Interfaces;

public interface IAdminKycService
{
    Task<List<AdminKycListDto>> GetPendingAsync();
    Task ApproveAsync(Guid kycId, Guid adminUserId);
    Task RejectAsync(Guid kycId, Guid adminUserId, string reason);
}
