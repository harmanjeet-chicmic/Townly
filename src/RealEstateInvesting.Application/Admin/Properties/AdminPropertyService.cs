using RealEstateInvesting.Application.Admin.Properties.DTOs;
using RealEstateInvesting.Application.Admin.Properties.Interfaces;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Application.Notifications.Interfaces;

namespace RealEstateInvesting.Application.Admin.Properties;

public class AdminPropertyService : IAdminPropertyService
{
    private readonly IAdminPropertyRepository _propertyRepo;
    private readonly INotificationService _notificationService;
    public AdminPropertyService(IAdminPropertyRepository propertyRepo,
              INotificationService notificationService)
    {
        _propertyRepo = propertyRepo;
        _notificationService = notificationService;
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
}
