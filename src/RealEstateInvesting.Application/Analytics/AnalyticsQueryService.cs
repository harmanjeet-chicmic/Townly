using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Analytics.Dtos;
using Amazon.Runtime.Internal.Util;

namespace RealEstateInvesting.Application.Analytics;

public class AnalyticsQueryService
{
    private readonly IAnalyticsSnapshotRepository _snapshotRepository;
    private readonly IInvestmentRepository _investmentRepository;
    private readonly IPropertyRepository _propertyRepository;

    private readonly IEthPriceService _ethPriceService;
    public AnalyticsQueryService(IAnalyticsSnapshotRepository snapshotRepository,
    IInvestmentRepository investmentRepository,
    IPropertyRepository propertyRepository,
    IEthPriceService ethPriceService)
    {
        _snapshotRepository = snapshotRepository;
        _investmentRepository = investmentRepository;
        _propertyRepository = propertyRepository;
        _ethPriceService = ethPriceService;
    }

    public async Task<IEnumerable<PropertyAnalyticsTrendDto>>
        GetPropertyTrendAsync(Guid propertyId, int hours)
    {
        var fromUtc = DateTime.UtcNow.AddHours(-hours);

        var snapshots =
            await _snapshotRepository.GetPropertySnapshotsAsync(propertyId, fromUtc);


        return snapshots.Select(s => new PropertyAnalyticsTrendDto
        {
            SnapshotAt = s.SnapshotAt,
            DemandScore = s.DemandScore,
            PricePerShare = s.PricePerShare,
            Valuation = s.Valuation
        });
    }
    public async Task<IEnumerable<PortfolioTrendDto>>
    GetPortfolioTrendAsync(Guid userId, int hours)
    {
        var fromUtc = DateTime.UtcNow.AddHours(-hours);

        var snapshots =
            await _snapshotRepository.GetUserPortfolioSnapshotsAsync(userId, fromUtc);

        return snapshots.Select(s => new PortfolioTrendDto
        {
            SnapshotAt = s.SnapshotAt,
            PortfolioValue = s.PortfolioValue,
            UnrealizedPnL = s.UnrealizedPnL
        });
    }
    public async Task<IEnumerable<PortfolioAllocationDto>>
    GetPortfolioAllocationAsync(Guid userId)
    {
        var investments =
            await _investmentRepository.GetByUserIdAsync(userId);

        if (!investments.Any())
            return Enumerable.Empty<PortfolioAllocationDto>();

        var propertyIds =
            investments.Select(i => i.PropertyId).Distinct();

        var properties =
            await _propertyRepository.GetByIdsAsync(propertyIds);

        var propertyMap =
            properties.ToDictionary(p => p.Id);

        decimal totalValue = 0;
        var valueByType = new Dictionary<string, decimal>();

        foreach (var inv in investments)
        {
            var snapshot =
                await _snapshotRepository
                    .GetLatestPropertySnapshotAsync(inv.PropertyId);

            if (snapshot == null)
                continue;

            var value =
                inv.SharesPurchased * snapshot.PricePerShare;

            totalValue += value;

            var type = propertyMap[inv.PropertyId].PropertyType;

            valueByType[type] =
                valueByType.GetValueOrDefault(type) + value;
        }

        return valueByType.Select(x => new PortfolioAllocationDto
        {
            Category = x.Key,
            Percentage =
                totalValue == 0
                    ? 0
                    : Math.Round((x.Value / totalValue) * 100, 1)
        });
    }

    public async Task<IEnumerable<PortfolioLineChartDto>>
  GetPortfolioLineAsync(Guid userId, int days)
    {
        var now = DateTime.UtcNow;
        var fromUtc = now.AddDays(-days);

        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();

        var snapshots =
            await _snapshotRepository
                .GetUserPortfolioSnapshotsAsync(userId, fromUtc);

        // 🔹 GROUP BY DAY (not hour)
        var snapshotMap = snapshots
            .GroupBy(s => s.SnapshotAt.Date)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(x => x.SnapshotAt).First()
            );

        var result = new List<PortfolioLineChartDto>();

        // 🔹 get last snapshot BEFORE range
        var previous =
            await _snapshotRepository.GetLastUserSnapshotBeforeAsync(userId, fromUtc);

        decimal lastValueUsd = previous?.PortfolioValue ?? 0;

        // 🔹 LOOP OVER DAYS
        for (int i = days; i >= 0; i--)
        {
            var bucket = now.Date.AddDays(-i);

            if (snapshotMap.TryGetValue(bucket, out var snapshot))
            {
                lastValueUsd = snapshot.PortfolioValue;
            }

            var valueEth =
                ethUsdRate == 0 ? 0 :
                decimal.Round(lastValueUsd / ethUsdRate, 8);

            result.Add(new PortfolioLineChartDto
            {
                Label = bucket.ToString("dd MMM"), // 🔥 e.g. "18 Mar"
                Value = valueEth
            });
        }

        return result;
    }

}
