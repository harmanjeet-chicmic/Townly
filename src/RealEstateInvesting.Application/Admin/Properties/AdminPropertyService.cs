using RealEstateInvesting.Application.Admin.Properties.DTOs;
using RealEstateInvesting.Application.Admin.Properties.Interfaces;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Admin.Properties;

public class AdminPropertyService : IAdminPropertyService
{
    private readonly IAdminPropertyRepository _propertyRepo;

    public AdminPropertyService(IAdminPropertyRepository propertyRepo)
    {
        _propertyRepo = propertyRepo;
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
    }

    public async Task RejectAsync(Guid propertyId, Guid adminId, string reason)
    {
        var property = await _propertyRepo.GetByIdAsync(propertyId)
            ?? throw new InvalidOperationException("Property not found");

        property.Reject(adminId, reason);

        await _propertyRepo.SaveChangesAsync();
    }
}
