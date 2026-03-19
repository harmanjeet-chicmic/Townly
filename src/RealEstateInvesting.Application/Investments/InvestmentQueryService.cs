using RealEstateInvesting.Application.Common.Dtos;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Investments.Dtos;

namespace RealEstateInvesting.Application.Investments;

public class InvestmentQueryService
{
    private readonly IInvestmentRepository _investmentRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IPropertyImageRepository _propertyImageRepository;
    private readonly IAnalyticsSnapshotRepository _snapshotRepo;
    private readonly IEthPriceService _ethPriceService;

    public InvestmentQueryService(
        IInvestmentRepository investmentRepository,
        IPropertyRepository propertyRepository,
        IPropertyImageRepository propertyImageRepository,
        IAnalyticsSnapshotRepository snapshotRepo,
        IEthPriceService ethPriceService)
    {
        _investmentRepository = investmentRepository;
        _propertyRepository = propertyRepository;
        _propertyImageRepository = propertyImageRepository;
        _snapshotRepo = snapshotRepo;
        _ethPriceService = ethPriceService;
    }

    public async Task<PagedResult<MyInvestmentDto>> GetMyInvestmentsAsync(
        Guid userId,
        int page,
        int pageSize,
        string? search,
        string? propertyType)
    {
        var (investments, totalCount) =
            await _investmentRepository
                .GetByUserIdPagedAsync(userId, page, pageSize, search, propertyType);

        if (!investments.Any())
        {
            return new PagedResult<MyInvestmentDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = 0,
                HasMore = false
            };
        }

        var propertyIds = investments
            .Select(i => i.PropertyId)
            .Distinct()
            .ToList();

        var properties =
            await _propertyRepository.GetByIdsAsync(propertyIds);

        var propertyLookup = properties.ToDictionary(p => p.Id);

        var snapshots =
            await _snapshotRepo.GetLatestPropertySnapshotsAsync(propertyIds);

        var snapshotMap = snapshots.ToDictionary(s => s.PropertyId);

        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();

        // 🔥 Bulk fetch images from PropertyImages table
        var images = await _propertyImageRepository.GetByPropertyIdsAsync(propertyIds);
        var imageMap = images.GroupBy(i => i.PropertyId)
            .ToDictionary(g => g.Key, g => g.Select(i => i.ImageUrl).ToList());

        // 🔥 Bulk fetch images that are stored in PropertyDocuments table (legacy/fallback)
        var propertyDocs = await _propertyRepository.GetDocumentsByPropertyIdsAsync(propertyIds);
        var docImageMap = propertyDocs
            .Where(d => d.Title.Equals("Image", StringComparison.OrdinalIgnoreCase))
            .GroupBy(d => d.PropertyId)
            .ToDictionary(g => g.Key, g => g.Select(d => d.DocumentUrl).ToList());

        var grouped = investments.GroupBy(i => i.PropertyId);

        var items = new List<MyInvestmentDto>();

        foreach (var group in grouped)
        {
            if (!propertyLookup.TryGetValue(group.Key, out var property))
                continue;

            snapshotMap.TryGetValue(group.Key, out var snapshot);
            imageMap.TryGetValue(group.Key, out var propertyImages);
            docImageMap.TryGetValue(group.Key, out var docImages);

            // Merge both image sources
            var allImages = (propertyImages ?? new List<string>())
                .Concat(docImages ?? new List<string>())
                .Distinct()
                .ToList();

            var totalShares = group.Sum(i => i.SharesPurchased);
            var totalAmountUsd = group.Sum(i => i.TotalAmount);
            var investedEth = group.Sum(i => i.EthAmountAtExecution);

            decimal currentValueEth = 0;
            decimal monthlyIncomeEth = 0;

            if (snapshot != null && ethUsdRate > 0)
            {
                var pricePerShareEth =
                    snapshot.PricePerShare / ethUsdRate;

                currentValueEth =
                    totalShares * pricePerShareEth;

                monthlyIncomeEth =
                    totalShares *
                    pricePerShareEth *
                    property.AnnualYieldPercent / 12m;
            }

            var totalReturnEth = currentValueEth - investedEth;

            items.Add(new MyInvestmentDto
            {
                PropertyId = property.Id,
                PropertyName = property.Name,
                PropertyImageUrls = allImages,
                Location = property.Location,
                PropertyType = property.PropertyType,

                SharesPurchased = totalShares,

                TotalAmountUsd = totalAmountUsd,
                TotalInvestedEth = Math.Round(investedEth, 6),

                CurrentValueEth = Math.Round(currentValueEth, 6),
                TotalReturnEth = Math.Round(totalReturnEth, 6),

                MonthlyIncomeEth = Math.Round(monthlyIncomeEth, 6),

                AnnualYieldPercent = property.AnnualYieldPercent,
                RiskScore = snapshot?.RiskScore,

                InvestedAt = group.Max(x => x.CreatedAt)
            });
        }

        return new PagedResult<MyInvestmentDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            HasMore = page * pageSize < totalCount,
            Items = items
                .OrderByDescending(x => x.CurrentValueEth)
                .ToList()
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