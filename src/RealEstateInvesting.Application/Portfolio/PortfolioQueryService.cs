using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Portfolio.Dtos;

namespace RealEstateInvesting.Application.Portfolio;

public class PortfolioQueryService
{   
     private readonly IInvestmentRepository _investmentRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IAnalyticsSnapshotRepository _snapshotRepo;
     private readonly IAnalyticsSnapshotRepository _analyticsSnapshotRepository;
    private readonly IEthPriceService _ethPriceService;

    public PortfolioQueryService(
        IAnalyticsSnapshotRepository snapshotRepo,
        IEthPriceService ethPriceService ,
         IInvestmentRepository investmentRepository,
        IPropertyRepository propertyRepository,
        IAnalyticsSnapshotRepository analyticsSnapshotRepository)
    {    
        _investmentRepository = investmentRepository;
        _propertyRepository = propertyRepository;
        _snapshotRepo = snapshotRepo;
        _ethPriceService = ethPriceService;
        _analyticsSnapshotRepository = analyticsSnapshotRepository;

    }

    public async Task<PortfolioOverviewDto> GetOverviewAsync(Guid userId)
    {
        // 🔥 Latest snapshot = current portfolio state
        var snapshot =
            (await _snapshotRepo.GetUserPortfolioSnapshotsAsync(
                userId,
                DateTime.UtcNow.AddHours(-24)))
            .OrderByDescending(s => s.SnapshotAt)
            .FirstOrDefault();

        if (snapshot == null)
            return new PortfolioOverviewDto();

        // 🔥 ETH price (cached / resilient)
        var ethUsd = await _ethPriceService.GetEthUsdPriceAsync();

        var totalInvestedEth =
            ethUsd == 0 ? 0 : snapshot.TotalInvested / ethUsd;

        var currentValueEth =
            ethUsd == 0 ? 0 : snapshot.PortfolioValue / ethUsd;

        var totalReturnEth =
            currentValueEth - totalInvestedEth;

        var totalReturnPercent =
            snapshot.TotalInvested == 0
                ? 0
                : (snapshot.PortfolioValue - snapshot.TotalInvested)
                    / snapshot.TotalInvested * 100;

        var monthlyIncomeEth =
            ethUsd == 0 ? 0 : snapshot.MonthlyIncome / ethUsd;

        return new PortfolioOverviewDto
        {
            TotalInvestedEth = decimal.Round(totalInvestedEth, 6),
            CurrentValueEth = decimal.Round(currentValueEth, 6),
            TotalReturnEth = decimal.Round(totalReturnEth, 6),
            TotalReturnPercent = decimal.Round(totalReturnPercent, 2),
            MonthlyIncomeEth = decimal.Round(monthlyIncomeEth, 6)
        };
    }
    public async Task<IEnumerable<PortfolioPropertyDto>> GetMyPortfolioPropertiesAsync(Guid userId)
    {
        // 🔥 ETH price (cached internally)
        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();

        // 1️⃣ User investments
        var investments = await _investmentRepository.GetByUserIdAsync(userId);
        if (!investments.Any())
            return Enumerable.Empty<PortfolioPropertyDto>();

        // 2️⃣ Group by property
        var grouped = investments.GroupBy(i => i.PropertyId);
        var propertyIds = grouped.Select(g => g.Key).ToList();

        // 3️⃣ Fetch properties
        var properties = await _propertyRepository.GetByIdsAsync(propertyIds);
        var propertyMap = properties.ToDictionary(p => p.Id);

        // 4️⃣ Fetch latest analytics snapshots (SAFE bulk)
        var snapshots =
            await _analyticsSnapshotRepository
                .GetLatestPropertySnapshotsAsync(propertyIds);

        var snapshotMap = snapshots.ToDictionary(s => s.PropertyId);

        var result = new List<PortfolioPropertyDto>();

        foreach (var group in grouped)
        {
            if (!propertyMap.TryGetValue(group.Key, out var property))
                continue;

            snapshotMap.TryGetValue(group.Key, out var snapshot);

            var tokensOwned = group.Sum(i => i.SharesPurchased);

            // ETH invested (historical truth)
            var investedEth =
                group.Sum(i => i.EthAmountAtExecution);

            // Current value (USD → ETH)
            decimal currentValueEth = 0;
            decimal monthlyIncomeEth = 0;

            if (snapshot != null)
            {
                var pricePerShareEth =
                    snapshot.PricePerShare / ethUsdRate;

                currentValueEth =
                    tokensOwned * pricePerShareEth;

                monthlyIncomeEth =
                    tokensOwned *
                    pricePerShareEth *
                    property.AnnualYieldPercent / 12m;
            }

            var unrealizedPnLEth =
                currentValueEth - investedEth;

            var unrealizedPnLPercent =
                investedEth == 0
                    ? 0
                    : Math.Round(
                        unrealizedPnLEth / investedEth * 100m,
                        2);

            result.Add(new PortfolioPropertyDto
            {
                PropertyId = property.Id,
                PropertyName = property.Name,
                ImageUrl = property.ImageUrl,
                Location = property.Location,

                TokensOwned = tokensOwned,

                InvestedEth = Math.Round(investedEth, 6),
                CurrentValueEth = Math.Round(currentValueEth, 6),

                UnrealizedPnLEth = Math.Round(unrealizedPnLEth, 6),
                UnrealizedPnLPercent = unrealizedPnLPercent,

                MonthlyIncomeEth = Math.Round(monthlyIncomeEth, 6),

                RiskScore = snapshot?.RiskScore
            });
        }

        // 🔥 Industry default: sort by current value
        return result
            .OrderByDescending(p => p.CurrentValueEth)
            .ToList();
    }
}
