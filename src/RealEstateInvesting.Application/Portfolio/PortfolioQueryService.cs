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
        IEthPriceService ethPriceService,
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
        // 🔥 Get latest snapshot
        var snapshot =
        (await _snapshotRepo.GetUserPortfolioSnapshotsAsync(
        userId,
        DateTime.UtcNow.AddHours(-24)))
        .OrderByDescending(s => s.SnapshotAt)
        .FirstOrDefault();


        // 🔥 Get investments (used everywhere)
        var investments =
            await _investmentRepository.GetByUserIdAsync(userId);

        if (!investments.Any())
            return new PortfolioOverviewDto();

        var ethUsd = await _ethPriceService.GetEthUsdPriceAsync();

        // 🔥 REAL ETH invested (historical truth)
        var totalInvestedEth =
            investments.Sum(i => i.EthAmountAtExecution);

        // ✅ Declare once (fix for CS0136)
        decimal currentValueEth = 0;
        decimal monthlyIncomeEth = 0;

        // =========================================
        // 🔴 FALLBACK (no snapshot yet)
        // =========================================
        if (snapshot == null)
        {
            var propertyIds =
                investments.Select(i => i.PropertyId).Distinct();

            var properties =
                await _propertyRepository.GetByIdsAsync(propertyIds);

            var propertyMap =
                properties.ToDictionary(p => p.Id);

            foreach (var inv in investments)
            {
                if (!propertyMap.TryGetValue(inv.PropertyId, out var property))
                    continue;

                var pricePerShareUsd =
                    property.ApprovedValuation / property.TotalUnits;

                var pricePerShareEth =
                    ethUsd == 0 ? 0 : pricePerShareUsd / ethUsd;

                var valueEth =
                    inv.SharesPurchased * pricePerShareEth;

                currentValueEth += valueEth;

                monthlyIncomeEth +=
                    valueEth * property.AnnualYieldPercent / 12m;
            }
        }
        // =========================================
        // 🟢 NORMAL FLOW (snapshot exists)
        // =========================================
        else
        {
            currentValueEth =
                ethUsd == 0 ? 0 : snapshot.PortfolioValue / ethUsd;

            monthlyIncomeEth =
                ethUsd == 0 ? 0 : snapshot.MonthlyIncome / ethUsd;
        }

        // 🔥 Common calculations
        var totalReturnEth =
            currentValueEth - totalInvestedEth;

        var totalReturnPercent =
            totalInvestedEth == 0
                ? 0
                : (totalReturnEth / totalInvestedEth) * 100;

        return new PortfolioOverviewDto
        {
            TotalInvestedEth = Math.Round(totalInvestedEth, 8),
            CurrentValueEth = Math.Round(currentValueEth, 6),
            TotalReturnEth = Math.Round(totalReturnEth, 6),
            TotalReturnPercent = Math.Round(totalReturnPercent, 2),
            MonthlyIncomeEth = Math.Round(monthlyIncomeEth, 6)
        };


    }


    public async Task<IEnumerable<PortfolioPropertyDto>> GetMyPortfolioPropertiesAsync(Guid userId)
    {
        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();

        var investments = await _investmentRepository.GetByUserIdAsync(userId);
        if (!investments.Any())
            return Enumerable.Empty<PortfolioPropertyDto>();

        var grouped = investments.GroupBy(i => i.PropertyId);
        var propertyIds = grouped.Select(g => g.Key).ToList();

        var properties = await _propertyRepository.GetByIdsAsync(propertyIds);
        var propertyMap = properties.ToDictionary(p => p.Id);

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

            var investedEth =
                group.Sum(i => i.EthAmountAtExecution);

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

        return result
            .OrderByDescending(p => p.CurrentValueEth)
            .ToList();
    }


}
