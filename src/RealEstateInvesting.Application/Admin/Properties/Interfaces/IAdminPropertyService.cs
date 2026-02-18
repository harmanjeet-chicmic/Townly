using RealEstateInvesting.Application.Admin.Properties.DTOs;

namespace RealEstateInvesting.Application.Admin.Properties.Interfaces;

public interface IAdminPropertyService
{
    Task<List<AdminPropertyListDto>> GetPendingAsync();
    Task ApproveAsync(Guid propertyId, Guid adminId);
    Task RejectAsync(Guid propertyId, Guid adminId, string reason);
    Task<IEnumerable<PendingPropertyUpdateDto>> 
    GetPendingUpdateRequestsAsync();
    Task ApproveUpdateRequestAsync(Guid updateRequestId, Guid adminId);
    
Task RejectUpdateRequestAsync(
    Guid updateRequestId,
    Guid adminId,
    string reason);


}
