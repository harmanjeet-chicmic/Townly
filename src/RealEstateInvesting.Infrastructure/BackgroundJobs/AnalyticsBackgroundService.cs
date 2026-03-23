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
            await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken);
        }
    }

    private async Task RunAnalyticsAsync()
    {
        using var scope = _scopeFactory.CreateScope();

        var propertyRepo = scope.ServiceProvider.GetRequiredService<IPropertyRepository>();
        var investmentRepo = scope.ServiceProvider.GetRequiredService<IInvestmentRepository>();
        var snapshotRepo = scope.ServiceProvider.GetRequiredService<IAnalyticsSnapshotRepository>();
        var tokenPurchaseRepo = scope.ServiceProvider.GetRequiredService<ITokenPurchaseRepository>();
        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var snapshotTime = DateTime.UtcNow;

        // ------------------------------------
        // 1️⃣ PROPERTY ANALYTICS (HOURLY v2)
        // ------------------------------------
        var properties = await propertyRepo.GetByStatusAsync(PropertyStatus.Active);

        long maxMomentum = 0;
        int maxUniqueInvestors = 0;

        var momentumMap = new Dictionary<Guid, long>();
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

            lastInvestmentAt_Map_Update(property.Id, await investmentRepo.GetLastInvestmentAtAsync(property.Id));
        }

        void lastInvestmentAt_Map_Update(Guid id, DateTime? val) => lastInvestmentMap[id] = val;

        // -------- Per property calculation --------
        foreach (var property in properties)
        {
            var sharesSold = await investmentRepo.GetTotalSharesInvestedAsync(property.Id);
            var totalInvested = await investmentRepo.GetTotalAmountInvestedAsync(property.Id);

            // ---------- Funding Score ----------
            var fundingScore = property.TotalUnits == 0 ? 0 : (int)Math.Round((decimal)sharesSold / property.TotalUnits * 100);

            // ---------- Momentum Score ----------
            var momentumRaw = momentumMap[property.Id];
            var momentumScore = maxMomentum == 0 ? 0 : (int)Math.Round((decimal)momentumRaw / maxMomentum * 100);

            // ---------- Diversity Score ----------
            var uniqueInvestors = investorsMap[property.Id];
            var diversityScore = maxUniqueInvestors == 0 ? 0 : (int)Math.Round((decimal)uniqueInvestors / maxUniqueInvestors * 100);

            // ---------- Recency + Decay ----------
            var lastInvestmentAt = lastInvestmentMap[property.Id];
            int recencyScore = 0;
            decimal decayFactor = 0.15m;

            if (lastInvestmentAt.HasValue)
            {
                var hoursAgo = (snapshotTime - lastInvestmentAt.Value).TotalHours;
                recencyScore = hoursAgo <= 1  ? 100 : hoursAgo <= 3  ? 85  : hoursAgo <= 6  ? 70  : hoursAgo <= 12 ? 50  : hoursAgo <= 24 ? 30  : 10;
                decayFactor = hoursAgo <= 1  ? 1.00m : hoursAgo <= 3  ? 0.90m : hoursAgo <= 6  ? 0.75m : hoursAgo <= 12 ? 0.55m : hoursAgo <= 24 ? 0.35m : 0.15m;
            }

            var baseDemand = (momentumScore * 0.40m) + (fundingScore * 0.25m) + (diversityScore * 0.20m) + (recencyScore * 0.15m);
            var demandScore = Math.Round(baseDemand * decayFactor, 2);

            // -------------------------------
            // Risk Factor
            // -------------------------------
            var latestSnapshot = await snapshotRepo.GetLatestPropertySnapshotAsync(property.Id);
            decimal riskScore = latestSnapshot?.RiskScore ?? 5; // Default or preserve

            // -------------------------------
            // Pricing
            // -------------------------------
            var basePricePerShare = property.TotalUnits == 0 ? 0 : property.InitialValuation / property.TotalUnits; 
            var pricePerShare = basePricePerShare * (1 + (demandScore / 100m) * 0.05m);
            var valuation = pricePerShare * property.TotalUnits;

            var snapshot = PropertyAnalyticsSnapshot.Create(
                property.Id, snapshotTime, sharesSold, totalInvested, demandScore, riskScore, pricePerShare, valuation
            );
            await snapshotRepo.AddPropertySnapshotAsync(snapshot);
        }

        // ------------------------------------
        // 2️⃣ USER PORTFOLIO ANALYTICS (Refactored)
        // ------------------------------------
        var allInvestments = await investmentRepo.GetAllUserInvestmentsAsync();
        var allTokenPurchases = await tokenPurchaseRepo.GetAllByStatusAsync(1); // 1 = Success
        var users = await userRepo.GetAllWithWalletsAsync();
        var walletToUser = users.Where(u => u.WalletAddress != null).ToDictionary(u => u.WalletAddress!.ToLower(), u => u.Id);

        var userToPropertyShares = new Dictionary<Guid, Dictionary<Guid, decimal>>();
        var userToTotalInvested = new Dictionary<Guid, decimal>();

        foreach (var inv in allInvestments)
        {
            if (!userToPropertyShares.ContainsKey(inv.UserId)) userToPropertyShares[inv.UserId] = new Dictionary<Guid, decimal>();
            var propertyShares = userToPropertyShares[inv.UserId];
            propertyShares[inv.PropertyId] = propertyShares.GetValueOrDefault(inv.PropertyId) + inv.SharesPurchased;
            userToTotalInvested[inv.UserId] = userToTotalInvested.GetValueOrDefault(inv.UserId) + inv.TotalAmount;
        }

        foreach (var tp in allTokenPurchases)
        {
            if (tp.BuyerAddress == null || !walletToUser.TryGetValue(tp.BuyerAddress.ToLower(), out var userId)) continue;
            if (!userToPropertyShares.ContainsKey(userId)) userToPropertyShares[userId] = new Dictionary<Guid, decimal>();
            var propertyShares = userToPropertyShares[userId];
            propertyShares[tp.PropertyId] = propertyShares.GetValueOrDefault(tp.PropertyId) + (tp.Shares ?? 0);
            userToTotalInvested[userId] = userToTotalInvested.GetValueOrDefault(userId) + ((tp.Shares ?? 0) * (tp.PricePerShare ?? 0));
        }

        foreach (var userId in userToPropertyShares.Keys)
        {
            var propertyShares = userToPropertyShares[userId];
            var totalInvested = userToTotalInvested.GetValueOrDefault(userId);
            decimal portfolioValue = 0;
            decimal monthlyIncome = 0;

            var propertyIds = propertyShares.Keys.ToList();
            var propertiesMap = (await propertyRepo.GetByIdsAsync(propertyIds)).ToDictionary(p => p.Id);

            foreach (var propEntry in propertyShares)
            {
                var propId = propEntry.Key;
                var shares = propEntry.Value;
                var latestSnapshot = await snapshotRepo.GetLatestPropertySnapshotAsync(propId);
                if (latestSnapshot == null) continue;

                portfolioValue += shares * latestSnapshot.PricePerShare;
                if (propertiesMap.TryGetValue(propId, out var property))
                {
                    monthlyIncome += shares * latestSnapshot.PricePerShare * property.AnnualYieldPercent / 12m;
                }
            }

            var userSnapshot = UserPortfolioSnapshot.Create(userId, snapshotTime, totalInvested, portfolioValue, monthlyIncome);
            await snapshotRepo.AddUserPortfolioSnapshotAsync(userSnapshot);
        }
    }
}
