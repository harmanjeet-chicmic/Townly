using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Properties.Dtos;
using RealEstateInvesting.Application.Properties.PropertyRegistrationApi;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Application.VectorSearch;
using RealEstateInvesting.Application.Common.Exceptions;

namespace RealEstateInvesting.Application.Properties;

public class PropertyQueryService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IInvestmentRepository _investmentRepository;
    private readonly IAnalyticsSnapshotRepository _analyticsSnapshotRepository;
    private readonly IEthPriceService _ethPriceService;
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorStore _vectorStore;
    private readonly IPropertyDocumentRepository _propertyDocumentRepository;
    private readonly IPropertyUpdateRequestRepository _updateRepository;
    private readonly IPropertyImageRepository _propertyImageRepository;
    private readonly IPropertyRegistrationApiClient _propertyRegistrationApi;
    private readonly IPropertyActivationRecordRepository _activationRecordRepository;
    private readonly IPropertyRegistrationJobRepository _propertyRegistrationJobRepository;

    public PropertyQueryService(IPropertyRepository propertyRepository,
                IInvestmentRepository investmentRepository,
                 IAnalyticsSnapshotRepository analyticsSnapshotRepository,
                  IEthPriceService ethPriceService,
                   IEmbeddingService embeddingService,
                IVectorStore vectorStore,
                IPropertyDocumentRepository propertyDocumentRepository,
                 IPropertyUpdateRequestRepository updateRepository,
                 IPropertyImageRepository propertyImageRepository,
                 IPropertyRegistrationApiClient propertyRegistrationApi,
                 IPropertyActivationRecordRepository activationRecordRepository,
                 IPropertyRegistrationJobRepository propertyRegistrationJobRepository)
    {
        _propertyRepository = propertyRepository;
        _investmentRepository = investmentRepository;
        _analyticsSnapshotRepository = analyticsSnapshotRepository;
        _ethPriceService = ethPriceService;
        _embeddingService = embeddingService;
        _propertyImageRepository = propertyImageRepository;
        _vectorStore = vectorStore;
        _propertyDocumentRepository = propertyDocumentRepository;
        _updateRepository = updateRepository;
        _propertyRegistrationApi = propertyRegistrationApi;
        _activationRecordRepository = activationRecordRepository;
        _propertyRegistrationJobRepository = propertyRegistrationJobRepository;
    }

    public async Task<object> GetMarketplaceAsync(Guid? currentUserId, int page,int pageSize,string? search,
                                                 string? propertyType,List<PropertyStatus>? status)
    {
        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();

        var (items, totalCount) =
            await _propertyRepository.GetMarketplaceAsync(
                currentUserId, page, pageSize, search, propertyType, status);

        var propertyIds = items.Select(p => p.Id).ToList();
        var registrationJobsMap = await _propertyRegistrationJobRepository.GetLatestByPropertyIdsAsync(propertyIds);
        var snapshots =
            await _analyticsSnapshotRepository
                .GetLatestPropertySnapshotsAsync(propertyIds);
        var snapshotMap = snapshots.ToDictionary(s => s.PropertyId);

        // 🔥 Bulk fetch images
        var images = await _propertyImageRepository.GetByPropertyIdsAsync(propertyIds);
        var imageMap = images.GroupBy(i => i.PropertyId)
            .ToDictionary(g => g.Key, g => g.Select(i => i.ImageUrl).ToList());

        var data = items.Select(p =>
        {
            snapshotMap.TryGetValue(p.Id, out var snapshot);
            imageMap.TryGetValue(p.Id, out var propertyImages);
            registrationJobsMap.TryGetValue(p.Id, out var registrationJob);

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
                ImageUrls = propertyImages ?? new List<string>(),
                Status = p.Status,
                RegistrationJobId = registrationJob?.Id,
                TokenAddress = registrationJob?.TokenAddress,
                ApprovedValuation = p.ApprovedValuation,
                AnnualYieldPercent = p.AnnualYieldPercent,
                TotalUnits = p.TotalUnits,
                AvailableUnits = registrationJob != null && registrationJob.MintAmount.HasValue 
                    ? (long)registrationJob.MintAmount.Value - p.SoldUnits 
                    : p.TotalUnits - p.SoldUnits,

                PricePerUnitEth = pricePerUnitEth,
                RiskScore = snapshot?.RiskScore ?? 5,

                PriceInUSD = registrationJob?.PricePerShare,
                TotalUnitMint = registrationJob?.MintAmount
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
    string? propertyType,
    List<PropertyStatus>? status)
    {
        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();

        var items = await _propertyRepository.GetMarketplaceCursorAsync(
            limit, cursor, search, propertyType, status);

        var propertyIds = items.Select(p => p.Id).ToList();
        var registrationJobsMap = await _propertyRegistrationJobRepository.GetLatestByPropertyIdsAsync(propertyIds);
        var snapshots =
            await _analyticsSnapshotRepository
                .GetLatestPropertySnapshotsAsync(propertyIds);

        var snapshotMap = snapshots.ToDictionary(s => s.PropertyId);

        // 🔥 Bulk fetch images
        var images = await _propertyImageRepository.GetByPropertyIdsAsync(propertyIds);
        var imageMap = images.GroupBy(i => i.PropertyId)
            .ToDictionary(g => g.Key, g => g.Select(i => i.ImageUrl).ToList());

        var data = items.Select(p =>
        {
            snapshotMap.TryGetValue(p.Id, out var snapshot);
            imageMap.TryGetValue(p.Id, out var propertyImages);
            registrationJobsMap.TryGetValue(p.Id, out var registrationJob);

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
                ImageUrls = propertyImages ?? new List<string>(),

                ApprovedValuation = p.ApprovedValuation,
                AnnualYieldPercent = p.AnnualYieldPercent,
                TotalUnits = p.TotalUnits,
                AvailableUnits = registrationJob != null && registrationJob.MintAmount.HasValue 
                    ? (long)registrationJob.MintAmount.Value - p.SoldUnits 
                    : p.TotalUnits - p.SoldUnits,

                Status = p.Status,
                PricePerUnitEth = pricePerUnitEth,
                RiskScore = snapshot?.RiskScore ?? 5
            };
        }).ToList();
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

    public async Task<PropertyDetailsDto> GetDetailsAsync(Guid? userId, Guid propertyId)
    {
        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();

        var property =
            await _propertyRepository.GetDetailsWithSoldUnitsAsync(propertyId)
            ?? throw new NotFoundException("Property not found.");


        var snapshot =
            await _analyticsSnapshotRepository
                .GetLatestPropertySnapshotAsync(propertyId);

        var registrationJobsMap = await _propertyRegistrationJobRepository.GetLatestByPropertyIdsAsync(new[] { propertyId });
        registrationJobsMap.TryGetValue(propertyId, out var registrationJob);
        
        

        var pricePerUnitUsd =
            property.TotalUnits == 0 ? 0 : property.ApprovedValuation / property.TotalUnits;

        var pricePerUnitEth =
            ethUsdRate == 0 ? 0 : decimal.Round(pricePerUnitUsd / ethUsdRate, 8);
        decimal? userInvestmentAmount = null;
        long? tokensOwned = null;
        if (userId.HasValue)
        {
            tokensOwned =
        await _investmentRepository
            .GetUserTokensOwnedAsync(
                userId.Value,
                propertyId);
            userInvestmentAmount =
                await _investmentRepository
                    .GetUserInvestmentAmountAsync(
                        userId.Value,
                        propertyId);
        }
        decimal? userInvestmentAmountEth = null;


        if (userInvestmentAmount.HasValue && ethUsdRate != 0)
        {
            userInvestmentAmountEth =
                decimal.Round(userInvestmentAmount.Value / ethUsdRate, 8);
        }


        var images = await _propertyImageRepository.GetByPropertyIdAsync(propertyId);

        return new PropertyDetailsDto
        {
            Id = property.Id,
            Name = property.Name,
            Description = property.Description,
            Location = property.Location,
            PropertyType = property.PropertyType,
            ImageUrls = images.Select(x => x.ImageUrl).ToList(),
            Status = property.Status,
            PropertySize = property.SquareFeet,
            ListedPercentage = property.SellingPercentage,
            ApprovedReason = property.ApprovedReason,

            TotalValue = property.ApprovedValuation,
            TotalUnits = property.TotalUnits,
            PricePerUnit = pricePerUnitUsd,
            PricePerUnitEth = pricePerUnitEth,
            TokensOwned = tokensOwned,

            AnnualYieldPercent = property.AnnualYieldPercent,
            //AvailableUnits = property.TotalUnits - property.SoldUnits,
             AvailableUnits = registrationJob != null && registrationJob.MintAmount.HasValue 
                    ? (long)registrationJob.MintAmount.Value - property.SoldUnits 
                    : property.TotalUnits - property.SoldUnits,

            RiskScore = snapshot?.RiskScore ?? 5,
            DemandScore = snapshot?.DemandScore,
            UserInvestmentAmount = userInvestmentAmount,
            UserInvestedAmountEth = userInvestmentAmountEth,

            RegistrationJobId = registrationJob?.Id,
            TokenAddress = registrationJob?.TokenAddress,
            PriceInUSD = registrationJob?.PricePerShare,
            TotalUnitMint = registrationJob?.MintAmount
        };
    }



    public async Task<IEnumerable<MarketplacePropertyDto>> GetFeaturedAsync(Guid? currentUserId)
    {
        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();

        var properties = await _propertyRepository.GetFeaturedAsync(6, currentUserId);

        if (!properties.Any())
            return Enumerable.Empty<MarketplacePropertyDto>();

        var propertyIds = properties.Select(p => p.Id).ToList();
        var soldUnitsMap =
            await _investmentRepository
                .GetSoldUnitsForPropertiesAsync(propertyIds);

        var registrationJobsMap = await _propertyRegistrationJobRepository.GetLatestByPropertyIdsAsync(propertyIds);

        var snapshots =
            await _analyticsSnapshotRepository
                .GetLatestPropertySnapshotsAsync(propertyIds);

        var snapshotMap = snapshots.ToDictionary(s => s.PropertyId);

        // 🔥 Bulk fetch images
        var propertyImages = await _propertyImageRepository.GetByPropertyIdsAsync(propertyIds);
        var imageMap = propertyImages.GroupBy(i => i.PropertyId)
            .ToDictionary(g => g.Key, g => g.Select(i => i.ImageUrl).ToList());

        return properties.Select(p =>
        {
            snapshotMap.TryGetValue(p.Id, out var snapshot);
            soldUnitsMap.TryGetValue(p.Id, out var soldUnits);
            imageMap.TryGetValue(p.Id, out var images);
            registrationJobsMap.TryGetValue(p.Id, out var registrationJob);

            var availableUnits = registrationJob != null && registrationJob.MintAmount.HasValue 
                ? (long)registrationJob.MintAmount.Value - soldUnits 
                : p.TotalUnits - soldUnits;

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
                Status = p.Status,
                ImageUrls = images ?? new List<string>(),
                ApprovedValuation = p.ApprovedValuation,
                AnnualYieldPercent = p.AnnualYieldPercent,
                TotalUnits = p.TotalUnits,
                AvailableUnits = availableUnits,

                PricePerUnitEth = pricePerUnitEth,
                RiskScore = snapshot?.RiskScore ?? 5
            };
        });
    }

    public async Task<object> GetMyPropertiesAsync(Guid userId, int page, int pageSize, PropertyStatus? status, string? search)
    {
        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();
        var (properties, totalCount) = await _propertyRepository.GetByOwnerIdPagedAsync(userId, page, pageSize, status, search);
        var propertyList = properties.ToList();

        if (!propertyList.Any())
        {
            return new
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = 0,
                HasMore = false,
                Items = new List<MyPropertyDto>()
            };
        }

        var propertyIds = propertyList.Select(p => p.Id).ToList();

        // Sync status from property-register/status API for non-Active properties (skip if already Active)
        var nonActiveIds = propertyList.Where(p => p.Status != PropertyStatus.Active).Select(p => p.Id).ToList();
        if (nonActiveIds.Count > 0)
        {
            var jobIdsByPropertyId = await _activationRecordRepository.GetLatestJobIdByPropertyIdsAsync(nonActiveIds);
            foreach (var prop in propertyList.Where(p => p.Status != PropertyStatus.Active))
            {
                if (!jobIdsByPropertyId.TryGetValue(prop.Id, out var jobId))
                    continue;
                var jobStatus = await _propertyRegistrationApi.GetJobStatusAsync(jobId);
                if (jobStatus?.Data == null)
                    continue;
                var mappedStatus = TrexStatusMapper.MapToPropertyStatus(jobStatus.Data.Status);
                prop.SetRegistrationJobStatus(mappedStatus);
                await _propertyRepository.UpdateAsync(prop);

                // Update PropertyActivationRecords table with latest status from API
                var activationRecord = await _activationRecordRepository.GetByJobIdAsync(jobId);
                if (activationRecord != null)
                {
                    activationRecord.UpdateStatus(jobStatus.Data.Status, jobStatus.Data.TrexDeployTxHash);
                    await _activationRecordRepository.UpdateAsync(activationRecord);
                }
            }
        }

        var documents = await _propertyDocumentRepository.GetByPropertyIdsAsync(propertyIds);
        var documentMap = documents.GroupBy(d => d.PropertyId).ToDictionary(g => g.Key, g => g.ToList());
        var pendingUpdatePropertyIds = await _updateRepository.GetPendingPropertyIdsAsync(propertyIds);
        var pendingUpdateSet = pendingUpdatePropertyIds.ToHashSet();
        var soldUnitsMap = await _investmentRepository.GetSoldUnitsForPropertiesAsync(propertyIds);
        var registrationJobsMap = await _propertyRegistrationJobRepository.GetLatestByPropertyIdsAsync(propertyIds);
        var snapshots = await _analyticsSnapshotRepository.GetLatestPropertySnapshotsAsync(propertyIds);
        var snapshotMap = snapshots.ToDictionary(s => s.PropertyId);
        
        // 🔥 Bulk fetch images
        var propertyImages = await _propertyImageRepository.GetByPropertyIdsAsync(propertyIds);
        var imageMap = propertyImages.GroupBy(i => i.PropertyId).ToDictionary(g => g.Key, g => g.Select(i => i.ImageUrl).ToList());

        var items = properties.Select(p =>
        {
            snapshotMap.TryGetValue(p.Id, out var snapshot);
            soldUnitsMap.TryGetValue(p.Id, out var soldUnits);
            imageMap.TryGetValue(p.Id, out var images);
            registrationJobsMap.TryGetValue(p.Id, out var registrationJob);

            // 🔥 Documents extraction
            documentMap.TryGetValue(p.Id, out var docs);

            var docImages = docs?
                .Where(d => d.Title == "Image")
                .Select(d => d.DocumentUrl)
                .ToList() ?? new List<string>();

            var availableUnits = registrationJob != null && registrationJob.MintAmount.HasValue 
                ? (long)registrationJob.MintAmount.Value - soldUnits 
                : p.TotalUnits - soldUnits;
            var progressPercent =
                p.TotalUnits == 0 ? 0 :
                Math.Round((decimal)soldUnits / p.TotalUnits * 100, 2);
            var pricePerUnitUsd =
                p.TotalUnits == 0 ? 0 :
                p.ApprovedValuation / p.TotalUnits;
            var pricePerUnitEth =
                ethUsdRate == 0 ? 0 :
                decimal.Round(pricePerUnitUsd / ethUsdRate, 8);

      return new MyPropertyDto
      {
          Id = p.Id,
          Name = p.Name,
          Location = p.Location,
          PropertyType = p.PropertyType,
          ImageUrls = images ?? new List<string>(),

                 Status = p.Status,
                 RejectionReason = p.RejectionReason,
                 ApprovedReason = p.ApprovedReason,
                ApprovedValuation = p.ApprovedValuation,
                TotalUnits = p.TotalUnits,
                AnnualYieldPercent = p.AnnualYieldPercent,
                 SoldUnits = soldUnits,
                 AvailableUnits = availableUnits,
                 InvestmentProgressPercent = progressPercent,
                 PricePerUnitEth = pricePerUnitEth,
                 RiskScore = snapshot?.RiskScore ?? 5,
                 HasPendingUpdateRequest = pendingUpdateSet.Contains(p.Id),
                 AdminDocuments = docs?
                    .Where(d => d.Type == PropertyDocumentType.Approved || d.Type == PropertyDocumentType.Rejected)
                    .Select(d => new PropertyDocumentDto
                    {
                        Title = d.Title,
                        FileName = d.FileName,
                        DocumentUrl = d.DocumentUrl,
                        Type = d.Type
                    }).ToList() ?? new List<PropertyDocumentDto>()
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
    public async Task<MyPropertyDetailsDto> GetMyPropertyDetailsAsync(
         Guid userId,
         Guid propertyId)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId)
            ?? throw new NotFoundException("Property not found.");

        // 🔒 Ownership check
        if (property.OwnerUserId != userId)
            throw new UnauthorizedAccessException(
                "This property does not belong to you.");

        var registrationJobsMap = await _propertyRegistrationJobRepository.GetLatestByPropertyIdsAsync(new[] { propertyId });
        registrationJobsMap.TryGetValue(propertyId, out var registrationJob);

        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();

        // 🔥 Sold units
        var soldUnits =
            await _investmentRepository.GetTotalSharesInvestedAsync(propertyId);

        // 🔥 Analytics snapshot
        var snapshot =
            await _analyticsSnapshotRepository
                .GetLatestPropertySnapshotAsync(propertyId);

        // 🔥 Check pending update request
        var pendingUpdate =
            await _updateRepository
                .GetPendingByPropertyIdAsync(propertyId);

        var hasPendingUpdateRequest = pendingUpdate != null;

        // 🔥 Price calculations
        var pricePerUnitUsd =
            property.TotalUnits == 0 ? 0 :
            property.ApprovedValuation / property.TotalUnits;

        var pricePerUnitEth =
            ethUsdRate == 0 ? 0 :
            decimal.Round(pricePerUnitUsd / ethUsdRate, 8);

        // 🔥 Documents
        var documents = await _propertyDocumentRepository
            .GetByPropertyIdAsync(propertyId);

        var images = await _propertyImageRepository.GetByPropertyIdAsync(propertyId);
        var imageUrls = images.Select(x => x.ImageUrl).ToList();

        var documentDtos = documents.Select(d => new PropertyDocumentDto
        {
            Title = d.Title,
            FileName = d.FileName,
            DocumentUrl = d.DocumentUrl,
            Type = d.Type
        }).ToList();

        // ==============================
        // 🔐 Capability Flags
        // ==============================

        bool canEditFullProperty =
            property.Status == PropertyStatus.Draft ||
            property.Status == PropertyStatus.PendingApproval ||
            property.Status == PropertyStatus.ModificationRequired;

        bool canResubmit =
            property.Status == PropertyStatus.ModificationRequired || property.Status == PropertyStatus.PendingApproval;

        bool canRequestUpdate =
            property.Status == PropertyStatus.Active &&
            !hasPendingUpdateRequest;

        bool canDelete =
            property.Status != PropertyStatus.Active &&
            soldUnits == 0;

        return new MyPropertyDetailsDto
        {
            Id = property.Id,
            Name = property.Name,
            Description = property.Description,
            Location = property.Location,
            PropertyType = property.PropertyType,
            ImageUrls = imageUrls,
            Status = property.Status,
            PropertySize = property.SquareFeet,
            ListedPercentage = property.SellingPercentage,
            // RejectionReason =
            //     property.Status == PropertyStatus.Rejected
            //         ? property.RejectionReason
            //         : null,
            RejectionReason = property.RejectionReason,
            ApprovedReason = property.ApprovedReason,
            RentalIncomeHistory = property.RentalIncomeHistory,

            TotalValue = property.ApprovedValuation,
            TotalUnits = property.TotalUnits,
            PricePerUnit = pricePerUnitUsd,
            PricePerUnitEth = pricePerUnitEth,
            AnnualYieldPercent = property.AnnualYieldPercent,
            AvailableUnits = registrationJob != null && registrationJob.MintAmount.HasValue 
                   ? (long)registrationJob.MintAmount.Value - soldUnits 
                   : property.TotalUnits - soldUnits,

            RiskScore = snapshot?.RiskScore ?? 5,
            DemandScore = snapshot?.DemandScore,

            Documents = documentDtos,
            AdminDocuments = documentDtos
                .Where(d => d.Type == PropertyDocumentType.Approved || d.Type == PropertyDocumentType.Rejected)
                .ToList(),

            // 🔥 Update Visibility
            HasPendingUpdateRequest = hasPendingUpdateRequest,

            // 🔐 Capability Flags
            CanEditFullProperty = canEditFullProperty,
            CanResubmit = canResubmit,
            CanRequestUpdate = canRequestUpdate,
            CanDelete = canDelete
        };
    }

    public async Task<IEnumerable<MarketplacePropertyDto>> GetRelatedPropertiesAsync(Guid propertyId)
    {
        // 1️⃣ Get base property
        var baseProperty = await _propertyRepository.GetByIdAsync(propertyId)
            ?? throw new NotFoundException("Property not found.");

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
        var propertyIds = relatedProperties.Select(p => p.Id).ToList();

        // 🔥 NEW: Bulk fetch snapshots
        var snapshots =
            await _analyticsSnapshotRepository
                .GetLatestPropertySnapshotsAsync(propertyIds);

        var snapshotMap = snapshots.ToDictionary(s => s.PropertyId);

        // 6️⃣ ETH price
        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();


        // 🔥 Bulk fetch images
        var propertyImages = await _propertyImageRepository.GetByPropertyIdsAsync(propertyIds);
        var imageMap = propertyImages.GroupBy(i => i.PropertyId)
            .ToDictionary(g => g.Key, g => g.Select(i => i.ImageUrl).ToList());

        return relatedProperties
            .Where(p => p.Status == PropertyStatus.Active)
            .Select(p =>
            {
                snapshotMap.TryGetValue(p.Id, out var snapshot);
                imageMap.TryGetValue(p.Id, out var images);

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
                    ImageUrls = images ?? new List<string>(),
                    Status = p.Status,

                    ApprovedValuation = p.ApprovedValuation,
                    AnnualYieldPercent = p.AnnualYieldPercent,
                    TotalUnits = p.TotalUnits,
                    AvailableUnits = p.TotalUnits, // sold units optional here

                    PricePerUnitEth = pricePerUnitEth,
                    RiskScore = snapshot?.RiskScore ?? 5

                };
            });
    }
    // public async Task DeletePropertyAsync(Guid userId, Guid propertyId)
    // {
    //     var property = await _propertyRepository.GetByIdAsync(propertyId)
    //         ?? throw new InvalidOperationException("Property not found.");

    //     // 🔒 Ownership check
    //     if (property.OwnerUserId != userId)
    //         throw new UnauthorizedAccessException("You cannot delete this property.");

    //     // 🔥 NEW RULE: Only certain statuses allowed
    //     if (property.Status == PropertyStatus.Active)
    //         throw new InvalidOperationException(
    //             "Active properties cannot be deleted.");

    //     // 🔥 Check if any shares sold (extra safety)
    //     var soldShares =
    //         await _investmentRepository.GetTotalSharesInvestedAsync(propertyId);

    //     if (soldShares > 0)
    //         throw new InvalidOperationException(
    //             "Cannot delete property because shares have already been sold.");

    //     // ✅ Safe to delete
    //     await _propertyRepository.DeleteAsync(property);
    // }
    public async Task DeletePropertyAsync(Guid userId, Guid propertyId)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId)
            ?? throw new NotFoundException("Property not found.");

        if (property.OwnerUserId != userId)
            throw new UnauthorizedAccessException(
                "You cannot delete this property.");

        var soldUnits =
            await _investmentRepository
                .GetTotalSharesInvestedAsync(propertyId);

        // 🔵 SoldOut → Hide only
        if (property.Status == PropertyStatus.SoldOut)
        {
            property.HideFromOwner();
            await _propertyRepository.UpdateAsync(property);
            return;
        }

        // 🔴 Active → Block
        if (property.Status == PropertyStatus.Active)
            throw new InvalidOperationException(
                "Active properties cannot be deleted.");

        // 🟢 Pending / ModificationRequired / Rejected
        if (property.Status == PropertyStatus.PendingApproval ||
            property.Status == PropertyStatus.ModificationRequired ||
            property.Status == PropertyStatus.Rejected)
        {
            property.SoftDelete(userId);
            await _propertyRepository.UpdateAsync(property);
            return;
        }

        throw new InvalidOperationException("Invalid delete operation.");
    }


}
