using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Investments.Dtos;

namespace RealEstateInvesting.Application.Investments;

public class InvestmentQueryService
{
    private readonly IInvestmentRepository _investmentRepository;
    private readonly IPropertyRepository _propertyRepository;


    public InvestmentQueryService(
        IInvestmentRepository investmentRepository,
        IPropertyRepository propertyRepository)
    {
        _investmentRepository = investmentRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task<IEnumerable<MyInvestmentDto>> GetMyInvestmentsAsync(Guid userId)
    {
        var investments = await _investmentRepository.GetByUserIdAsync(userId);

        if (!investments.Any())
            return Enumerable.Empty<MyInvestmentDto>();

        var propertyIds = investments
            .Select(i => i.PropertyId)
            .Distinct()
            .ToList();

        var properties = await _propertyRepository.GetByIdsAsync(propertyIds);

        var propertyLookup = properties.ToDictionary(p => p.Id);

        return investments.Select(i =>
        {
            var property = propertyLookup[i.PropertyId];

            return new MyInvestmentDto
            {
                InvestmentId = i.Id,
                PropertyId = property.Id,
                PropertyName = property.Name,
                PropertyImageUrl = property.ImageUrl,
                Location = property.Location,

                SharesPurchased = i.SharesPurchased,

                // USD truth
                PricePerShareUsd = i.PricePerShareAtPurchase,
                TotalAmountUsd = i.TotalAmount,

                // ETH snapshot
                EthAmountAtExecution = i.EthAmountAtExecution,
                EthUsdRateAtExecution = i.EthUsdRateAtExecution,

                InvestedAt = i.CreatedAt
            };

        });
    }

    public async Task<PortfolioSummaryDto> GetPortfolioSummaryAsync(Guid userId)
    {
        var investments = await _investmentRepository.GetByUserIdAsync(userId);

        return new PortfolioSummaryDto
        {
            TotalInvested = investments.Sum(i => i.TotalAmount),
            PropertiesCount = investments.Select(i => i.PropertyId).Distinct().Count(),
            TotalSharesOwned = investments.Sum(i => i.SharesPurchased)
        };
    }

}
