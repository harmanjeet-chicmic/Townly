using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Analytics.Dtos;

namespace RealEstateInvesting.Application.Analytics;

public class AnalyticsQueryService
{
    private readonly IAnalyticsSnapshotRepository _snapshotRepository;
    private readonly IInvestmentRepository _investmentRepository;
    private readonly IPropertyRepository _propertyRepository;
    public AnalyticsQueryService(IAnalyticsSnapshotRepository snapshotRepository,
    IInvestmentRepository investmentRepository,
    IPropertyRepository propertyRepository)
    {
        _snapshotRepository = snapshotRepository;
        _investmentRepository = investmentRepository;
        _propertyRepository = propertyRepository;
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
    GetPortfolioLineAsync(Guid userId, int hours)
    {
        var fromUtc = DateTime.UtcNow.AddHours(-hours);

        var snapshots =
            await _snapshotRepository
                .GetUserPortfolioSnapshotsAsync(userId, fromUtc);

        // Group by hour bucket
        var grouped = snapshots
            .GroupBy(s => new DateTime(
                s.SnapshotAt.Year,
                s.SnapshotAt.Month,
                s.SnapshotAt.Day,
                s.SnapshotAt.Hour,
                0,
                0,
                DateTimeKind.Utc))
            .OrderBy(g => g.Key);

        var result = new List<PortfolioLineChartDto>();

        foreach (var group in grouped)
        {
            // Take latest snapshot of that hour
            var latest = group
                .OrderByDescending(s => s.SnapshotAt)
                .First();

            result.Add(new PortfolioLineChartDto
            {
                Label = group.Key.ToString("HH:mm"),
                Value = latest.PortfolioValue
            });
        }

        return result;
    }



}
