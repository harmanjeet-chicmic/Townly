using RealEstateInvesting.Application.Common.Exceptions;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Portfolio.Dtos;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Properties.InvestmentInfo;

public class PropertyInvestmentInfoService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IEthPriceService _ethPriceService;
    private readonly IPropertyRegistrationJobRepository _propertyRegistrationJobRepository;

    public PropertyInvestmentInfoService(
        IPropertyRepository propertyRepository,
        IEthPriceService ethPriceService,
        IPropertyRegistrationJobRepository propertyRegistrationJobRepository)
    {
        _propertyRepository = propertyRepository;
        _ethPriceService = ethPriceService;
        _propertyRegistrationJobRepository = propertyRegistrationJobRepository;
    }

    public async Task<PropertyInvestmentInfoDto> GetAsync(Guid propertyId)
    {
        var property =
            await _propertyRepository.GetByIdAsync(propertyId)
            ?? throw new NotFoundException("Property not found.");

        if (property.TotalUnits <= 0)
            throw new InvalidOperationException("Invalid property units.");

        var registrationJobsMap = await _propertyRegistrationJobRepository.GetLatestByPropertyIdsAsync(new[] { propertyId });
        registrationJobsMap.TryGetValue(propertyId, out var registrationJob);

        return new PropertyInvestmentInfoDto
        {
            // Platform rules
            MinimumInvestmentShares = InvestmentTerms.MinimumInvestmentShares,
            DividendFrequency = InvestmentTerms.DividendFrequency,
            InvestmentType = InvestmentTerms.InvestmentType,
            Security = InvestmentTerms.Security,

            // Property-specific
            PropertyOwnerUserId = property.OwnerUserId,
            ExpectedAnnualReturnPercent = property.AnnualYieldPercent,
            PricePerShare = registrationJob?.PricePerShare ?? (property.TotalUnits == 0 ? 0 : property.ApprovedValuation / property.TotalUnits)
        };
    }
}
