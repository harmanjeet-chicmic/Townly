using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Properties.InvestmentInfo;

public class PropertyInvestmentInfoService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IEthPriceService _ethPriceService;

    public PropertyInvestmentInfoService(
        IPropertyRepository propertyRepository,
        IEthPriceService ethPriceService)
    {
        _propertyRepository = propertyRepository;
        _ethPriceService = ethPriceService;
    }

    public async Task<PropertyInvestmentInfoDto> GetAsync(Guid propertyId)
    {
        var property =
            await _propertyRepository.GetByIdAsync(propertyId)
            ?? throw new InvalidOperationException("Property not found.");

        if (property.TotalUnits <= 0)
            throw new InvalidOperationException("Invalid property units.");

        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();
        if (ethUsdRate <= 0)
            throw new InvalidOperationException("Invalid ETH price.");

        var pricePerShareUsd =
            decimal.Round(property.ApprovedValuation / property.TotalUnits, 2);

        var pricePerShareEth =
            decimal.Round(pricePerShareUsd / ethUsdRate, 8);

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
            PricePerShareUsd = pricePerShareUsd,
            PricePerShareEth = pricePerShareEth
        };
    }
}
