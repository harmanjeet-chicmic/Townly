using RealEstateInvesting.Application.Admin.Kyc.DTOs;
using RealEstateInvesting.Application.Common.DTOs;

namespace RealEstateInvesting.Application.Admin.Kyc.Interfaces;

public interface IAdminKycService
{
    Task<PaginatedResponse<AdminKycListDto>> GetFilteredAsync(AdminKycQuery query);
    Task ApproveAsync(Guid kycId, Guid adminUserId);
    Task RejectAsync(Guid kycId, Guid adminUserId, RejectKycRequest request);
}
