using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Properties.Dtos;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Properties;

public class PropertyService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPropertyDocumentRepository _propertyDocumentRepository;

    public PropertyService(
        IPropertyRepository propertyRepository,
        IUserRepository userRepository,
        IPropertyDocumentRepository propertyDocumentRepository)
    {
        _propertyRepository = propertyRepository;
        _userRepository = userRepository;
        _propertyDocumentRepository = propertyDocumentRepository;
    }

    public async Task<Guid> CreatePropertyAsync(
        Guid userId,
        CreatePropertyCommand command)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new InvalidOperationException("User not found.");
        Console.WriteLine("=========================== kyc statuds========"+user.KycStatus);
        Console.WriteLine("=================User ID==================="+user.Id);
        if (user.KycStatus != KycStatus.Approved)
            throw new InvalidOperationException("KYC approval required.");

        if (user.IsBlocked)
            throw new InvalidOperationException("User is blocked.");

        var property = Property.CreateDraft(
            ownerUserId: userId,
            name: command.Name,
            description: command.Description,
            location: command.Location,
            propertyType: command.PropertyType,
            imageUrl: command.ImageUrl,
            initialValuation: command.InitialValuation,
            totalUnits: command.TotalUnits,
            annualYieldPercent: command.AnnualYieldPercent
        );

        property.Submit();

        await _propertyRepository.AddAsync(property);

        if (command.Documents.Any())
        {
            var docs = command.Documents.Select(d =>
                PropertyDocument.Create(
                    property.Id,
                    d.DocumentName,
                    d.DocumentUrl));

            await _propertyDocumentRepository.AddRangeAsync(docs);
        }

        return property.Id;
    }
}
