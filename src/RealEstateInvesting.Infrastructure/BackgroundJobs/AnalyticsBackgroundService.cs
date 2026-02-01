using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Infrastructure.BackgroundJobs;

public class AnalyticsBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public AnalyticsBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await RunAnalyticsAsync();
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task RunAnalyticsAsync()
    {
        using var scope = _scopeFactory.CreateScope();

        var propertyRepo = scope.ServiceProvider.GetRequiredService<IPropertyRepository>();
        var investmentRepo = scope.ServiceProvider.GetRequiredService<IInvestmentRepository>();
        var snapshotRepo = scope.ServiceProvider.GetRequiredService<IAnalyticsSnapshotRepository>();

        var snapshotTime = DateTime.UtcNow;

        // ------------------------------------
        // 1️⃣ PROPERTY ANALYTICS (HOURLY v2)
        // ------------------------------------
        var properties = await propertyRepo.GetByStatusAsync(PropertyStatus.Active);

        int maxMomentum = 0;
        int maxUniqueInvestors = 0;

        var momentumMap = new Dictionary<Guid, int>();
        var investorsMap = new Dictionary<Guid, int>();
        var lastInvestmentMap = new Dictionary<Guid, DateTime?>();

        // -------- Normalization pass --------
        foreach (var property in properties)
        {
            var s1h  = await investmentRepo.GetSharesInvestedInLastHoursAsync(property.Id, 1);
            var s6h  = await investmentRepo.GetSharesInvestedInLastHoursAsync(property.Id, 6);
            var s24h = await investmentRepo.GetSharesInvestedInLastHoursAsync(property.Id, 24);

            var momentumRaw = (s1h * 3) + (s6h * 2) + (s24h * 1);
            momentumMap[property.Id] = momentumRaw;

            if (momentumRaw > maxMomentum)
                maxMomentum = momentumRaw;

            var investors = await investmentRepo.GetUniqueInvestorCountAsync(property.Id);
            investorsMap[property.Id] = investors;

            if (investors > maxUniqueInvestors)
                maxUniqueInvestors = investors;

            lastInvestmentMap[property.Id] =
                await investmentRepo.GetLastInvestmentAtAsync(property.Id);
        }

        // -------- Per property calculation --------
        foreach (var property in properties)
        {
            var sharesSold =
                await investmentRepo.GetTotalSharesInvestedAsync(property.Id);

            var totalInvested =
                await investmentRepo.GetTotalAmountInvestedAsync(property.Id);

            // ---------- Funding Score ----------
            var fundingScore =
                property.TotalUnits == 0
                    ? 0
                    : (int)Math.Round(
                        (decimal)sharesSold / property.TotalUnits * 100
                      );

            // ---------- Momentum Score ----------
            var momentumRaw = momentumMap[property.Id];

            var momentumScore =
                maxMomentum == 0
                    ? 0
                    : (int)Math.Round(
                        (decimal)momentumRaw / maxMomentum * 100
                      );

            // ---------- Diversity Score ----------
            var uniqueInvestors = investorsMap[property.Id];

            var diversityScore =
                maxUniqueInvestors == 0
                    ? 0
                    : (int)Math.Round(
                        (decimal)uniqueInvestors / maxUniqueInvestors * 100
                      );

            // ---------- Recency + Decay ----------
            var lastInvestmentAt = lastInvestmentMap[property.Id];

            int recencyScore = 0;
            decimal decayFactor = 0.15m;

            if (lastInvestmentAt.HasValue)
            {
                var hoursAgo =
                    (snapshotTime - lastInvestmentAt.Value).TotalHours;

                recencyScore =
                    hoursAgo <= 1  ? 100 :
                    hoursAgo <= 3  ? 85  :
                    hoursAgo <= 6  ? 70  :
                    hoursAgo <= 12 ? 50  :
                    hoursAgo <= 24 ? 30  : 10;

                decayFactor =
                    hoursAgo <= 1  ? 1.00m :
                    hoursAgo <= 3  ? 0.90m :
                    hoursAgo <= 6  ? 0.75m :
                    hoursAgo <= 12 ? 0.55m :
                    hoursAgo <= 24 ? 0.35m : 0.15m;
            }

            var baseDemand =
                (momentumScore  * 0.40m) +
                (fundingScore   * 0.25m) +
                (diversityScore * 0.20m) +
                (recencyScore   * 0.15m);

            var demandScore =
                Math.Round(baseDemand * decayFactor, 2);

            // -------------------------------
            // 🔥 Risk Factor v1 (UNCHANGED)
            // -------------------------------
            var fundingRatio =
                property.TotalUnits == 0
                    ? 0
                    : (decimal)sharesSold / property.TotalUnits;

            var fundingRisk =
                fundingRatio < 0.20m ? 8 :
                fundingRatio < 0.40m ? 6 :
                fundingRatio < 0.60m ? 4 :
                fundingRatio < 0.80m ? 2 : 1;

            var investorRisk =
                uniqueInvestors <= 2  ? 8 :
                uniqueInvestors <= 5  ? 6 :
                uniqueInvestors <= 10 ? 4 :
                uniqueInvestors <= 20 ? 2 : 1;

            var ageDays =
                property.ApprovedAt.HasValue
                    ? (snapshotTime - property.ApprovedAt.Value).TotalDays
                    : 0;

            var ageRisk =
                ageDays < 30  ? 7 :
                ageDays < 90  ? 5 :
                ageDays < 180 ? 3 : 1;

            var yieldRisk =
                property.AnnualYieldPercent > 0.15m ? 7 :
                property.AnnualYieldPercent > 0.12m ? 5 :
                property.AnnualYieldPercent > 0.08m ? 3 : 1;

            var riskScore =
                Math.Round(
                    (fundingRisk + investorRisk + ageRisk + yieldRisk) / 4m,
                    1);

            // -------------------------------
            // Pricing
            // -------------------------------
            var basePricePerShare =
                property.InitialValuation / property.TotalUnits;

            var pricePerShare =
                basePricePerShare * (1 + (demandScore / 100m) * 0.05m);

            var valuation =
                pricePerShare * property.TotalUnits;

            var snapshot = PropertyAnalyticsSnapshot.Create(
                property.Id,
                snapshotTime,
                sharesSold,
                totalInvested,
                demandScore,
                riskScore,
                pricePerShare,
                valuation
            );

            await snapshotRepo.AddPropertySnapshotAsync(snapshot);
        }

        // ------------------------------------
        // 2️⃣ USER PORTFOLIO ANALYTICS (UNCHANGED)
        // ------------------------------------
        var userInvestments =
            await investmentRepo.GetAllUserInvestmentsAsync();

        var groupedByUser = userInvestments.GroupBy(i => i.UserId);

        foreach (var userGroup in groupedByUser)
        {
            var userId = userGroup.Key;

            var totalInvested =
                userGroup.Sum(i => i.TotalAmount);

            decimal portfolioValue = 0;
            decimal monthlyIncome = 0;

            var propertyIds =
                userGroup.Select(i => i.PropertyId).Distinct();

            var propertiesMap =
                (await propertyRepo.GetByIdsAsync(propertyIds))
                .ToDictionary(p => p.Id);

            foreach (var inv in userGroup)
            {
                var latestSnapshot =
                    await snapshotRepo.GetLatestPropertySnapshotAsync(inv.PropertyId);

                if (latestSnapshot == null)
                    continue;

                portfolioValue +=
                    inv.SharesPurchased * latestSnapshot.PricePerShare;

                if (propertiesMap.TryGetValue(inv.PropertyId, out var property))
                {
                    monthlyIncome +=
                        inv.SharesPurchased *
                        latestSnapshot.PricePerShare *
                        property.AnnualYieldPercent / 12m;
                }
            }

            var userSnapshot = UserPortfolioSnapshot.Create(
                userId,
                snapshotTime,
                totalInvested,
                portfolioValue,
                monthlyIncome
            );

            await snapshotRepo.AddUserPortfolioSnapshotAsync(userSnapshot);
        }
    }
}
