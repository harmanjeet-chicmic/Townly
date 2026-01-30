using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Properties.Dtos;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Properties;

public class PropertyUpdateService
{
    private readonly IUserRepository _userRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IPropertyUpdateRequestRepository _updateRepository;

    public PropertyUpdateService(
        IUserRepository userRepository,
        IPropertyRepository propertyRepository,
        IPropertyUpdateRequestRepository updateRepository)
    {
        _userRepository = userRepository;
        _propertyRepository = propertyRepository;
        _updateRepository = updateRepository;
    }

    public async Task<Guid> RequestUpdateAsync(
        Guid userId,
        Guid propertyId,
        RequestPropertyUpdateDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new InvalidOperationException("User not found.");

        if (user.KycStatus != KycStatus.Approved)
            throw new InvalidOperationException("KYC approval required.");

        if (user.IsBlocked)
            throw new InvalidOperationException("User is blocked.");

        var property = await _propertyRepository.GetByIdAsync(propertyId)
            ?? throw new InvalidOperationException("Property not found.");

        if (property.OwnerUserId != userId)
            throw new UnauthorizedAccessException("Not property owner.");

        if (property.Status != PropertyStatus.Active)
            throw new InvalidOperationException("Only active properties can be updated.");

        var existingPending =
            await _updateRepository.GetPendingByPropertyIdAsync(propertyId);

        if (existingPending != null)
            throw new InvalidOperationException("Pending update already exists.");

        var request = PropertyUpdateRequest.Create(
            propertyId,
            userId,
            dto.Name,
            dto.Description,
            dto.Location,
            dto.PropertyType,
            dto.ImageUrl
        );

        await _updateRepository.AddAsync(request);

        return request.Id;
    }
}
