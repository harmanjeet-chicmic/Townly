using RealEstateInvesting.Application.Admin.Properties.DTOs;
using RealEstateInvesting.Application.Admin.Properties.Interfaces;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Application.Notifications.Interfaces;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Admin.Properties;

public class AdminPropertyService : IAdminPropertyService
{
    private readonly IAdminPropertyRepository _propertyRepo;
    private readonly INotificationService _notificationService;
    private readonly IPropertyUpdateRequestRepository _updateRepo;

    private readonly IPropertyRepository _propertyRepository;

    public AdminPropertyService(IAdminPropertyRepository propertyRepo,
              INotificationService notificationService,
              IPropertyUpdateRequestRepository updateRepo,
              IPropertyRepository propertyRepository)
    {
        _propertyRepo = propertyRepo;
        _notificationService = notificationService;
        _updateRepo = updateRepo;
        _propertyRepository = propertyRepository;
    }

    public async Task<List<AdminPropertyListDto>> GetPendingAsync()
    {
        var properties = await _propertyRepo.GetPendingAsync();

        return properties.Select(p => new AdminPropertyListDto
        {
            PropertyId = p.Id,
            Name = p.Name,
            Location = p.Location,
            Status = p.Status,
            CreatedAt = p.CreatedAt
        }).ToList();
    }
    public async Task<IEnumerable<PendingPropertyUpdateDto>>
    GetPendingUpdateRequestsAsync()
    {
        var pendingRequests =
            await _updateRepo.GetAllPendingAsync();

        if (!pendingRequests.Any())
            return Enumerable.Empty<PendingPropertyUpdateDto>();

        return pendingRequests.Select(r => new PendingPropertyUpdateDto
        {
            UpdateRequestId = r.Id,
            PropertyId = r.PropertyId,
            RequestedByUserId = r.RequestedByUserId,

            Name = r.Name,
            Location = r.Location,
            PropertyType = r.PropertyType,
            ImageUrl = r.ImageUrl,

            RequestedAt = r.RequestedAt
        });
    }
    public async Task ApproveUpdateRequestAsync(
    Guid updateRequestId,
    Guid adminId)
    {
        var request =
            await _updateRepo.GetByIdAsync(updateRequestId)
            ?? throw new InvalidOperationException("Update request not found.");

        if (request.Status != PropertyUpdateStatus.Pending)
            throw new InvalidOperationException("Request already reviewed.");

        var property =
            await _propertyRepository.GetByIdAsync(request.PropertyId)
            ?? throw new InvalidOperationException("Property not found.");

        // 🔥 Apply metadata safely through domain method
        property.ApplyApprovedUpdate(
            request.Name,
            request.Description,
            request.Location,
            request.PropertyType,
            request.ImageUrl
        );

        request.Approve();

        await _propertyRepository.UpdateAsync(property);
        await _notificationService.CreateAsync(
            property.OwnerUserId,
            NotificationType.PropertyApproved,
            "Property Update Approved",
            $"Your property \"{property.Name}\" has been approved and is now live.",
            property.Id
        );
        await _updateRepo.UpdateAsync(request);
    }



    public async Task ApproveAsync(Guid propertyId, Guid adminId)
    {
        var property = await _propertyRepo.GetByIdAsync(propertyId)
            ?? throw new InvalidOperationException("Property not found");

        property.Activate();

        await _propertyRepo.SaveChangesAsync();

        // 🔔 Notification to property owner
        await _notificationService.CreateAsync(
            property.OwnerUserId,
            NotificationType.PropertyApproved,
            "Property Approved",
            $"Your property \"{property.Name}\" has been approved and is now live.",
            property.Id
        );

    }

    public async Task RejectAsync(Guid propertyId, Guid adminId, string reason)
    {
        var property = await _propertyRepo.GetByIdAsync(propertyId)
            ?? throw new InvalidOperationException("Property not found");

        property.Reject(adminId, reason);
        await _notificationService.CreateAsync(
            property.OwnerUserId,
            NotificationType.PropertyRejected,
            "Property Rejected",
            $"Your property \"{property.Name}\" was rejected. Reason: {reason}",
            property.Id
        );

        await _propertyRepo.SaveChangesAsync();
    }

    public async Task RejectUpdateRequestAsync(
    Guid updateRequestId,
    Guid adminId,
    string reason)
    {
        var request =
            await _updateRepo.GetByIdAsync(updateRequestId)
            ?? throw new InvalidOperationException("Update request not found.");

        if (request.Status != PropertyUpdateStatus.Pending)
            throw new InvalidOperationException("Request already reviewed.");

        var property =
            await _propertyRepository.GetByIdAsync(request.PropertyId)
            ?? throw new InvalidOperationException("Property not found.");

        // 🔥 Mark request rejected
        request.Reject();

        await _updateRepo.UpdateAsync(request);

        // 🔔 Notify property owner
        await _notificationService.CreateAsync(
            property.OwnerUserId,
           NotificationType.PropertyUpdateRejected,

            "Property Update Rejected",
            $"Your update request for property \"{property.Name}\" was rejected. Reason: {reason}",
            property.Id
        );
    }

}
