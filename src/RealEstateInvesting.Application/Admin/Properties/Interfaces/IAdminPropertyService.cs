using RealEstateInvesting.Application.Admin.Properties.DTOs;
using RealEstateInvesting.Application.Common.DTOs;
using RealEstateInvesting.Application.Properties.Dtos;

namespace RealEstateInvesting.Application.Admin.Properties.Interfaces;

public interface IAdminPropertyService
{
    Task<PaginatedResponse<MyPropertyDetailsDto>> GetPendingAsync(AdminPropertyQuery query);

    Task ApproveAsync(Guid propertyId, Guid adminId);

    Task RejectAsync(Guid propertyId, Guid adminId, string reason);

    Task ModifyRequest(Guid propertyId, Guid adminId, string reason);

    Task<IEnumerable<PendingPropertyUpdateDto>> GetPendingUpdateRequestsAsync();

    Task ApproveUpdateRequestAsync(Guid updateRequestId, Guid adminId);

    Task RejectUpdateRequestAsync(
        Guid updateRequestId,
        Guid adminId,
        string reason);
    Task<PaginatedResponse<MyPropertyDetailsDto>> GetAllAsync(AdminPropertyQuery query);
    Task AssignToOrganizationAsync(Guid propertyId, Guid organizationId);
}