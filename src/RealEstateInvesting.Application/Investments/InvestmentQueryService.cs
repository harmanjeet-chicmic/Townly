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

    public async Task<object> GetMyInvestmentsAsync(
        Guid userId,
        int page,
        int pageSize)
    {
        var (investments, totalCount) =
            await _investmentRepository
                .GetByUserIdPagedAsync(userId, page, pageSize);

        if (!investments.Any())
        {
            return new
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = 0,
                HasMore = false,
                Items = new List<MyInvestmentDto>()
            };
        }

        var propertyIds = investments
            .Select(i => i.PropertyId)
            .Distinct()
            .ToList();

        var properties =
            await _propertyRepository.GetByIdsAsync(propertyIds);

        var propertyLookup = properties.ToDictionary(p => p.Id);

        var items = investments.Select(i =>
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

                PricePerShareUsd = i.PricePerShareAtPurchase,
                TotalAmountUsd = i.TotalAmount,

                EthAmountAtExecution = i.EthAmountAtExecution,
                EthUsdRateAtExecution = i.EthUsdRateAtExecution,

                InvestedAt = i.CreatedAt
            };
        });

        return new
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            HasMore = page * pageSize < totalCount,
            Items = items
        };
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
