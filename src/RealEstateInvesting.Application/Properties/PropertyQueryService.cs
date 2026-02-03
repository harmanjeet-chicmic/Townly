using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Properties.Dtos;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.VectorSearch;

namespace RealEstateInvesting.Application.Properties;

public class PropertyQueryService
{


    private readonly IPropertyRepository _propertyRepository;
    private readonly IInvestmentRepository _investmentRepository;
    private readonly IAnalyticsSnapshotRepository _analyticsSnapshotRepository;
    private readonly IEthPriceService _ethPriceService;
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorStore _vectorStore;

    public PropertyQueryService(IPropertyRepository propertyRepository,
                IInvestmentRepository investmentRepository,
                 IAnalyticsSnapshotRepository analyticsSnapshotRepository,
                  IEthPriceService ethPriceService,
                   IEmbeddingService embeddingService,
                IVectorStore vectorStore)
    {
        _propertyRepository = propertyRepository;
        _investmentRepository = investmentRepository;
        _analyticsSnapshotRepository = analyticsSnapshotRepository;
        _ethPriceService = ethPriceService;
        _embeddingService = embeddingService;
        _vectorStore = vectorStore;
    }

    public async Task<object> GetMarketplaceAsync(
    int page,
    int pageSize,
    string? search,
    string? propertyType)
    {
        // 🔥 1. Get ETH price ONCE
        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();

        var (items, totalCount) =
            await _propertyRepository.GetMarketplaceAsync(
                page, pageSize, search, propertyType);

        var propertyIds = items.Select(p => p.Id).ToList();

        // 🔥 2. Bulk analytics fetch (SAFE)
        var snapshots =
            await _analyticsSnapshotRepository
                .GetLatestPropertySnapshotsAsync(propertyIds);

        var snapshotMap = snapshots.ToDictionary(s => s.PropertyId);

        var data = items.Select(p =>
        {
            snapshotMap.TryGetValue(p.Id, out var snapshot);

            var pricePerUnitUsd =
                p.TotalUnits == 0 ? 0 : p.ApprovedValuation / p.TotalUnits;

            var pricePerUnitEth =
                ethUsdRate == 0 ? 0 : decimal.Round(pricePerUnitUsd / ethUsdRate, 8);

            return new MarketplacePropertyDto
            {
                Id = p.Id,
                Name = p.Name,
                Location = p.Location,
                PropertyType = p.PropertyType,
                ImageUrl = p.ImageUrl,

                ApprovedValuation = p.ApprovedValuation,
                AnnualYieldPercent = p.AnnualYieldPercent,
                TotalUnits = p.TotalUnits,
                AvailableUnits = p.TotalUnits - p.SoldUnits,

                // 🔥 ETH display
                PricePerUnitEth = pricePerUnitEth,

                // 🔥 Analytics
                RiskScore = snapshot?.RiskScore
            };
        });

        return new
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            HasMore = page * pageSize < totalCount,
            Items = data
        };
    }


    public async Task<object> GetMarketplaceCursorAsync(
    int limit,
    string? cursor,
    string? search,
    string? propertyType)
    {
        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();

        var items = await _propertyRepository.GetMarketplaceCursorAsync(
            limit, cursor, search, propertyType);

        var propertyIds = items.Select(p => p.Id).ToList();

        var snapshots =
            await _analyticsSnapshotRepository
                .GetLatestPropertySnapshotsAsync(propertyIds);

        var snapshotMap = snapshots.ToDictionary(s => s.PropertyId);

        var data = items.Select(p =>
        {
            snapshotMap.TryGetValue(p.Id, out var snapshot);

            var pricePerUnitUsd =
                p.TotalUnits == 0 ? 0 : p.ApprovedValuation / p.TotalUnits;

            var pricePerUnitEth =
                ethUsdRate == 0 ? 0 : decimal.Round(pricePerUnitUsd / ethUsdRate, 8);

            return new MarketplacePropertyDto
            {
                Id = p.Id,
                Name = p.Name,
                Location = p.Location,
                PropertyType = p.PropertyType,
                ImageUrl = p.ImageUrl,

                ApprovedValuation = p.ApprovedValuation,
                AnnualYieldPercent = p.AnnualYieldPercent,
                TotalUnits = p.TotalUnits,
                AvailableUnits = p.TotalUnits - p.SoldUnits,

                PricePerUnitEth = pricePerUnitEth,
                RiskScore = snapshot?.RiskScore
            };
        }).ToList();

        // 🔑 Generate next cursor
        var last = items.LastOrDefault();
        var nextCursor = last == null
            ? null
            : $"{last.CreatedAt:o}|{last.Id}";

        return new
        {
            Items = data,
            NextCursor = nextCursor
        };
    }

    public async Task<PropertyDetailsDto> GetDetailsAsync(Guid propertyId)
    {
        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();

        var property =
            await _propertyRepository.GetDetailsWithSoldUnitsAsync(propertyId)
            ?? throw new InvalidOperationException("Property not found.");

        var snapshot =
            await _analyticsSnapshotRepository
                .GetLatestPropertySnapshotAsync(propertyId);

        var pricePerUnitUsd =
            property.TotalUnits == 0 ? 0 : property.ApprovedValuation / property.TotalUnits;

        var pricePerUnitEth =
            ethUsdRate == 0 ? 0 : decimal.Round(pricePerUnitUsd / ethUsdRate, 8);

        return new PropertyDetailsDto
        {
            Id = property.Id,
            Name = property.Name,
            Description = property.Description,
            Location = property.Location,
            PropertyType = property.PropertyType,
            ImageUrl = property.ImageUrl,

            TotalValue = property.ApprovedValuation,
            TotalUnits = property.TotalUnits,
            PricePerUnit = pricePerUnitUsd,
            PricePerUnitEth = pricePerUnitEth,

            AnnualYieldPercent = property.AnnualYieldPercent,
            AvailableUnits = property.TotalUnits - property.SoldUnits,

            RiskScore = snapshot?.RiskScore,
            DemandScore = snapshot?.DemandScore
        };
    }



    public async Task<IEnumerable<MarketplacePropertyDto>> GetFeaturedAsync()
    {
        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();

        var properties = await _propertyRepository.GetFeaturedAsync(6);

        if (!properties.Any())
            return Enumerable.Empty<MarketplacePropertyDto>();

        var propertyIds = properties.Select(p => p.Id).ToList();

        var snapshots =
            await _analyticsSnapshotRepository
                .GetLatestPropertySnapshotsAsync(propertyIds);

        var snapshotMap = snapshots.ToDictionary(s => s.PropertyId);

        return properties.Select(p =>
        {
            snapshotMap.TryGetValue(p.Id, out var snapshot);

            var pricePerUnitUsd =
                p.TotalUnits == 0 ? 0 : p.ApprovedValuation / p.TotalUnits;

            var pricePerUnitEth =
                ethUsdRate == 0 ? 0 : decimal.Round(pricePerUnitUsd / ethUsdRate, 8);

            return new MarketplacePropertyDto
            {
                Id = p.Id,
                Name = p.Name,
                Location = p.Location,
                PropertyType = p.PropertyType,
                ImageUrl = p.ImageUrl,

                ApprovedValuation = p.ApprovedValuation,
                AnnualYieldPercent = p.AnnualYieldPercent,
                TotalUnits = p.TotalUnits,
                AvailableUnits = p.TotalUnits,

                PricePerUnitEth = pricePerUnitEth,
                RiskScore = snapshot?.RiskScore
            };
        });
    }


    public async Task<IEnumerable<MyPropertyDto>> GetMyPropertiesAsync(Guid userId)
    {
        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();

        var properties = await _propertyRepository.GetByOwnerIdAsync(userId);

        if (!properties.Any())
            return Enumerable.Empty<MyPropertyDto>();

        var investments = await _investmentRepository.GetAllUserInvestmentsAsync();

        var propertyIds = properties.Select(p => p.Id).ToList();

        var snapshots =
            await _analyticsSnapshotRepository
                .GetLatestPropertySnapshotsAsync(propertyIds);

        var snapshotMap = snapshots.ToDictionary(s => s.PropertyId);

        return properties.Select(p =>
        {
            var propertyInvestments =
                investments.Where(i => i.PropertyId == p.Id).ToList();

            var soldUnits = propertyInvestments.Sum(i => i.SharesPurchased);
            var availableUnits = p.TotalUnits - soldUnits;

            var totalAmountInvestedUsd =
                propertyInvestments.Sum(i => i.TotalAmount);

            var progressPercent =
                p.TotalUnits == 0 ? 0 :
                Math.Round((decimal)soldUnits / p.TotalUnits * 100, 2);

            var pricePerUnitUsd =
                p.TotalUnits == 0 ? 0 : p.ApprovedValuation / p.TotalUnits;

            var pricePerUnitEth =
                ethUsdRate == 0 ? 0 : decimal.Round(pricePerUnitUsd / ethUsdRate, 8);

            snapshotMap.TryGetValue(p.Id, out var snapshot);

            return new MyPropertyDto
            {
                Id = p.Id,
                Name = p.Name,
                Location = p.Location,
                PropertyType = p.PropertyType,
                ImageUrl = p.ImageUrl,

                Status = p.Status,
                ApprovedValuation = p.ApprovedValuation,
                TotalUnits = p.TotalUnits,
                AnnualYieldPercent = p.AnnualYieldPercent,

                SoldUnits = soldUnits,
                AvailableUnits = availableUnits,
                InvestmentProgressPercent = progressPercent,
                TotalAmountInvestedUsd = totalAmountInvestedUsd,

                PricePerUnitEth = pricePerUnitEth,
                RiskScore = snapshot?.RiskScore
            };
        });
    }
    public async Task<IEnumerable<MarketplacePropertyDto>> GetRelatedPropertiesAsync(Guid propertyId)
    {
        // 1️⃣ Get base property
        var baseProperty = await _propertyRepository.GetByIdAsync(propertyId)
            ?? throw new InvalidOperationException("Property not found.");

        // 2️⃣ Build embedding text (stable + meaningful)
        var embeddingText = $"""
    Property name: {baseProperty.Name}
    Property type: {baseProperty.PropertyType}
    Location: {baseProperty.Location}

    {baseProperty.Description}

    Approved valuation: {baseProperty.ApprovedValuation}
    Total units: {baseProperty.TotalUnits}
    Annual yield: {baseProperty.AnnualYieldPercent}
    """;

        // 3️⃣ Generate vector
        List<Guid> relatedIds;

        //   try{

        var vector = await _embeddingService.GenerateEmbeddingAsync(embeddingText);
        relatedIds =
            await _vectorStore.SearchSimilarAsync(propertyId, vector, limit: 3);
        // }
        // catch (Exception)

        // {   

        //     Console.WriteLine("=====================Fallback one =====================");
        //     // 🔁 Fallback: rule-based related properties
        //     relatedIds = (await _propertyRepository.GetFeaturedAsync(6))
        //         .Where(p => p.Id != propertyId)
        //         .Select(p => p.Id)
        //         .ToList();
        // }

        if (!relatedIds.Any())
            return Enumerable.Empty<MarketplacePropertyDto>();

        // 5️⃣ Fetch properties
        var relatedProperties =
            await _propertyRepository.GetByIdsAsync(relatedIds);

        // 6️⃣ ETH price
        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();

        return relatedProperties
            .Where(p => p.Status == PropertyStatus.Active)
            .Select(p =>
            {
                var pricePerUnitUsd =
                    p.TotalUnits == 0 ? 0 : p.ApprovedValuation / p.TotalUnits;

                var pricePerUnitEth =
                    ethUsdRate == 0 ? 0 : decimal.Round(pricePerUnitUsd / ethUsdRate, 8);

                return new MarketplacePropertyDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Location = p.Location,
                    PropertyType = p.PropertyType,
                    ImageUrl = p.ImageUrl,

                    ApprovedValuation = p.ApprovedValuation,
                    AnnualYieldPercent = p.AnnualYieldPercent,
                    TotalUnits = p.TotalUnits,
                    AvailableUnits = p.TotalUnits, // sold units optional here

                    PricePerUnitEth = pricePerUnitEth
                };
            });
    }



}
