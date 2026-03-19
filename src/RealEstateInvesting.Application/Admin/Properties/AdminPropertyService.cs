// using RealEstateInvesting.Application.Admin.Properties.DTOs;
// using RealEstateInvesting.Application.Admin.Properties.Interfaces;
// using RealEstateInvesting.Application.Common.Interfaces;
// using RealEstateInvesting.Domain.Enums;
// using RealEstateInvesting.Application.Notifications.Interfaces;
// using RealEstateInvesting.Domain.Entities;

// namespace RealEstateInvesting.Application.Admin.Properties;

// public class AdminPropertyService : IAdminPropertyService
// {
//     private readonly IAdminPropertyRepository _propertyRepo;
//     private readonly INotificationService _notificationService;
//     private readonly IPropertyUpdateRequestRepository _updateRepo;
//     private readonly IPropertyRepository _propertyRepository;

//     public AdminPropertyService(
//         IAdminPropertyRepository propertyRepo,
//         INotificationService notificationService,
//         IPropertyUpdateRequestRepository updateRepo,
//         IPropertyRepository propertyRepository)
//     {
//         _propertyRepo = propertyRepo;
//         _notificationService = notificationService;
//         _updateRepo = updateRepo;
//         _propertyRepository = propertyRepository;
//     }

//     // ================================
//     // PROPERTY APPROVAL FLOW
//     // ================================

//     public async Task<List<AdminPropertyListDto>> GetPendingAsync()
//     {
//         var properties = await _propertyRepo.GetPendingAsync();

//         return properties.Select(p => new AdminPropertyListDto
//         {
//             PropertyId = p.Id,
//             Name = p.Name,
//             Location = p.Location,
//             Status = p.Status,
//             CreatedAt = p.CreatedAt
//         }).ToList();
//     }

//     public async Task ApproveAsync(Guid propertyId, Guid adminId)
//     {
//         var property = await _propertyRepo.GetByIdAsync(propertyId)
//             ?? throw new InvalidOperationException("Property not found.");

//         property.Activate();

//         await _propertyRepo.SaveChangesAsync();

//         await _notificationService.CreateAsync(
//             property.OwnerUserId,
//             NotificationType.PropertyApproved,
//             "Property Approved",
//             $"Your property \"{property.Name}\" has been approved and is now live.",
//             property.Id
//         );
//     }

//     public async Task RejectAsync(Guid propertyId, Guid adminId, string reason)
//     {
//         var property = await _propertyRepo.GetByIdAsync(propertyId)
//             ?? throw new InvalidOperationException("Property not found.");

//         property.Reject(adminId, reason);

//         await _propertyRepo.SaveChangesAsync();

//         await _notificationService.CreateAsync(
//             property.OwnerUserId,
//             NotificationType.PropertyRejected,
//             "Property Rejected",
//             $"Your property \"{property.Name}\" was rejected. Reason: {reason}",
//             property.Id
//         );
//     }

//     public async Task ModifyRequest(Guid propertyId, Guid adminId, string reason)
//     {
//         var property = await _propertyRepo.GetByIdAsync(propertyId)
//             ?? throw new InvalidOperationException("Property not found.");

//         property.ModifyRequest(adminId, reason);

//         await _propertyRepo.SaveChangesAsync();

//         await _notificationService.CreateAsync(
//             property.OwnerUserId,
//             NotificationType.PropertyRejected,
//             "Property Modification Required",
//             $"Your property \"{property.Name}\" requires modification. Reason: {reason}",
//             property.Id
//         );
//     }

//     // ================================
//     // UPDATE REQUEST FLOW (ACTIVE ONLY)
//     // ================================

//     public async Task<IEnumerable<PendingPropertyUpdateDto>>
//     GetPendingUpdateRequestsAsync()
// {
//     var pendingRequests = await _updateRepo.GetAllPendingAsync();

//     if (!pendingRequests.Any())
//         return Enumerable.Empty<PendingPropertyUpdateDto>();

//     return pendingRequests.Select(r => new PendingPropertyUpdateDto
//     {
//         UpdateRequestId = r.Id,
//         PropertyId = r.PropertyId,
//         RequestedByUserId = r.RequestedByUserId,
//         Description = r.Description,
//         ImageUrl = r.ImageUrl,
//         RequestedAt = r.RequestedAt
//     });
// }

//     public async Task ApproveUpdateRequestAsync(
//         Guid updateRequestId,
//         Guid adminId)
//     {
//         var request = await _updateRepo.GetByIdAsync(updateRequestId)
//             ?? throw new InvalidOperationException("Update request not found.");

//         if (request.Status != PropertyUpdateStatus.Pending)
//             throw new InvalidOperationException("Request already reviewed.");

//         var property = await _propertyRepository.GetByIdAsync(request.PropertyId)
//             ?? throw new InvalidOperationException("Property not found.");

//         // Ensure property is still Active
//         if (property.Status != PropertyStatus.Active)
//             throw new InvalidOperationException(
//                 "Update can only be approved for active properties.");

//         // Apply metadata update
//         property.ApplyApprovedMetadataUpdate(
//             request.Description,
//             request.ImageUrl
//         );

//         request.Approve();

//         await _propertyRepository.UpdateAsync(property);
//         await _updateRepo.UpdateAsync(request);

//         await _notificationService.CreateAsync(
//             property.OwnerUserId,
//             NotificationType.PropertyApproved,
//             "Property Update Approved",
//             $"Your update request for property \"{property.Name}\" has been approved.",
//             property.Id
//         );
//     }

//     public async Task RejectUpdateRequestAsync(
//         Guid updateRequestId,
//         Guid adminId,
//         string reason)
//     {
//         var request = await _updateRepo.GetByIdAsync(updateRequestId)
//             ?? throw new InvalidOperationException("Update request not found.");

//         if (request.Status != PropertyUpdateStatus.Pending)
//             throw new InvalidOperationException("Request already reviewed.");

//         var property = await _propertyRepository.GetByIdAsync(request.PropertyId)
//             ?? throw new InvalidOperationException("Property not found.");

//         request.Reject();

//         await _updateRepo.UpdateAsync(request);

//         await _notificationService.CreateAsync(
//             property.OwnerUserId,
//             NotificationType.PropertyUpdateRejected,
//             "Property Update Rejected",
//             $"Your update request for property \"{property.Name}\" was rejected. Reason: {reason}",
//             property.Id
//         );
//     }
// }

using RealEstateInvesting.Application.Admin.Properties.DTOs;
using RealEstateInvesting.Application.Admin.Properties.Interfaces;
using RealEstateInvesting.Application.Common.DTOs;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Notifications.Interfaces;
using RealEstateInvesting.Application.Properties.Dtos;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Application.Admin.Kyc.Interfaces;
using RealEstateInvesting.Application.Common.Exceptions;

namespace RealEstateInvesting.Application.Admin.Properties;

public class AdminPropertyService : IAdminPropertyService
{
    private readonly IAdminPropertyRepository _propertyRepo;
    private readonly INotificationService _notificationService;
    private readonly IPropertyUpdateRequestRepository _updateRepo;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IInvestmentRepository _investmentRepository;
    private readonly IEthPriceService _ethPriceService;
    private readonly IAnalyticsSnapshotRepository _analyticsSnapshotRepository;
    private readonly IPropertyDocumentRepository _propertyDocumentRepository;
    private readonly IAdminKycRepository _adminKycRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IPropertyImageRepository _propertyImageRepository;

    public AdminPropertyService(
        IAdminPropertyRepository propertyRepo,
        INotificationService notificationService,
        IPropertyUpdateRequestRepository updateRepo,
        IPropertyRepository propertyRepository,
        IInvestmentRepository investmentRepository,
        IEthPriceService ethPriceService,
        IAnalyticsSnapshotRepository analyticsSnapshotRepository,
        IPropertyDocumentRepository propertyDocumentRepository,
        IAdminKycRepository adminKycRepository,
        ITransactionRepository transactionRepository,
        IPropertyImageRepository propertyImageRepository)
    {
        _propertyRepo = propertyRepo;
        _notificationService = notificationService;
        _updateRepo = updateRepo;
        _propertyRepository = propertyRepository;
        _investmentRepository = investmentRepository;
        _ethPriceService = ethPriceService;
        _analyticsSnapshotRepository = analyticsSnapshotRepository;
        _propertyDocumentRepository = propertyDocumentRepository;
        _adminKycRepository = adminKycRepository;
        _propertyImageRepository = propertyImageRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<PaginatedResponse<MyPropertyDetailsDto>> GetPendingAsync(AdminPropertyQuery query)
    {
        var (properties, totalCount) = await _propertyRepo.GetPendingAsync(query);

        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();

        var results = new List<MyPropertyDetailsDto>();

        foreach (var property in properties)
        {
            var soldUnits =
                await _investmentRepository.GetTotalSharesInvestedAsync(property.Id);

            var snapshot =
                await _analyticsSnapshotRepository
                .GetLatestPropertySnapshotAsync(property.Id);

            var pendingUpdate =
                await _updateRepo.GetPendingByPropertyIdAsync(property.Id);

            var documents =
                await _propertyDocumentRepository.GetByPropertyIdAsync(property.Id);

            var documentDtos = documents.Select(d => new PropertyDocumentDto
            {
                Title = d.Title,
                FileName = d.FileName,
                DocumentUrl = d.DocumentUrl
            }).ToList();

            var pricePerUnitUsd =
                property.TotalUnits == 0 ? 0 :
                property.ApprovedValuation / property.TotalUnits;

            var pricePerUnitEth =
                ethUsdRate == 0 ? 0 :
                decimal.Round(pricePerUnitUsd / ethUsdRate, 8);

            results.Add(new MyPropertyDetailsDto
            {
                Id = property.Id,
                Name = property.Name,
                Description = property.Description,
                Location = property.Location,
                PropertyType = property.PropertyType,
                ImageUrls = _propertyImageRepository.GetByPropertyIdAsync(property.Id).Result.Select(x => x.ImageUrl).ToList(),
                Status = property.Status,
                RejectionReason = property.RejectionReason,
                RentalIncomeHistory = property.RentalIncomeHistory,

                TotalValue = property.ApprovedValuation,
                TotalUnits = property.TotalUnits,
                AvailableUnits = property.TotalUnits - soldUnits,

                PricePerUnit = pricePerUnitUsd,
                PricePerUnitEth = pricePerUnitEth,
                AnnualYieldPercent = property.AnnualYieldPercent,

                RiskScore = snapshot?.RiskScore ?? 5,
                DemandScore = snapshot?.DemandScore,

                Documents = documentDtos,

                HasPendingUpdateRequest = pendingUpdate != null,

                CanEditFullProperty = true,
                CanResubmit = true,
                CanRequestUpdate = false,
                CanDelete = false
            });
        }

        return new PaginatedResponse<MyPropertyDetailsDto>
        {
            Items = results,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
        };
    }

    public async Task ApproveAsync(Guid propertyId, Guid adminId)
    {
        var property = await _propertyRepo.GetByIdAsync(propertyId)
            ?? throw new NotFoundException("Property not found.");

        property.Activate();

        await _propertyRepo.SaveChangesAsync();

        await _notificationService.CreateAsync(
            property.OwnerUserId,
            NotificationType.PropertyApproved,
            "Property Approved",
            $"Your property \"{property.Name}\" has been approved and is now live.",
            property.Id
        );
    }

    public async Task RejectAsync(Guid propertyId, Guid adminId, string reason)
    {
        var property = await _propertyRepo.GetByIdAsync(propertyId)
            ?? throw new NotFoundException("Property not found.");

        property.Reject(adminId, reason);

        await _propertyRepo.SaveChangesAsync();

        await _notificationService.CreateAsync(
            property.OwnerUserId,
            NotificationType.PropertyRejected,
            "Property Rejected",
            $"Your property \"{property.Name}\" was rejected. Reason: {reason}",
            property.Id
        );
    }

    public async Task ModifyRequest(Guid propertyId, Guid adminId, string reason)
    {
        var property = await _propertyRepo.GetByIdAsync(propertyId)
            ?? throw new NotFoundException("Property not found.");

        property.ModifyRequest(adminId, reason);

        await _propertyRepo.SaveChangesAsync();

        await _notificationService.CreateAsync(
            property.OwnerUserId,
            NotificationType.PropertyRejected,
            "Property Modification Required",
            $"Your property \"{property.Name}\" requires modification. Reason: {reason}",
            property.Id
        );
    }

    public async Task<IEnumerable<PendingPropertyUpdateDto>> GetPendingUpdateRequestsAsync()
    {
        var pendingRequests = await _updateRepo.GetAllPendingAsync();

        return pendingRequests.Select(r => new PendingPropertyUpdateDto
        {
            UpdateRequestId = r.Id,
            PropertyId = r.PropertyId,
            RequestedByUserId = r.RequestedByUserId,
            Description = r.Description,
            ImageUrl = r.ImageUrl,
            RequestedAt = r.RequestedAt
        });
    }

    public async Task ApproveUpdateRequestAsync(Guid updateRequestId, Guid adminId)
    {
        var request = await _updateRepo.GetByIdAsync(updateRequestId)
            ?? throw new NotFoundException("Update request not found.");

        var property = await _propertyRepository.GetByIdAsync(request.PropertyId)
            ?? throw new NotFoundException("Property not found.");

        property.ApplyApprovedMetadataUpdate(
            request.Description,
            request.ImageUrl
        );

        request.Approve();

        await _propertyRepository.UpdateAsync(property);
        await _updateRepo.UpdateAsync(request);

        await _notificationService.CreateAsync(
            property.OwnerUserId,
            NotificationType.PropertyApproved,
            "Property Update Approved",
            $"Your update request for property \"{property.Name}\" has been approved.",
            property.Id
        );
    }

    public async Task RejectUpdateRequestAsync(Guid updateRequestId, Guid adminId, string reason)
    {
        var request = await _updateRepo.GetByIdAsync(updateRequestId)
            ?? throw new NotFoundException("Update request not found.");

        var property = await _propertyRepository.GetByIdAsync(request.PropertyId)
            ?? throw new NotFoundException("Property not found.");

        request.Reject();

        await _updateRepo.UpdateAsync(request);

        await _notificationService.CreateAsync(
            property.OwnerUserId,
            NotificationType.PropertyUpdateRejected,
            "Property Update Rejected",
            $"Your update request for property \"{property.Name}\" was rejected. Reason: {reason}",
            property.Id
        );
    }
    public async Task<PaginatedResponse<MyPropertyDetailsDto>> GetAllAsync(AdminPropertyQuery query)
    {
        var (properties, totalCount) = await _propertyRepo.GetAllAsync(query);

        var ethUsdRate = await _ethPriceService.GetEthUsdPriceAsync();

        var results = new List<MyPropertyDetailsDto>();

        foreach (var property in properties)
        {
            var soldUnits =
                await _investmentRepository.GetTotalSharesInvestedAsync(property.Id);

            var snapshot =
                await _analyticsSnapshotRepository
                    .GetLatestPropertySnapshotAsync(property.Id);

            var pendingUpdate =
                await _updateRepo.GetPendingByPropertyIdAsync(property.Id);

            var documents =
                await _propertyDocumentRepository.GetByPropertyIdAsync(property.Id);

            var documentDtos = documents.Select(d => new PropertyDocumentDto
            {
                Title = d.Title,
                FileName = d.FileName,
                DocumentUrl = d.DocumentUrl
            }).ToList();

            var pricePerUnitUsd =
                property.TotalUnits == 0 ? 0 :
                property.ApprovedValuation / property.TotalUnits;

            var pricePerUnitEth =
                ethUsdRate == 0 ? 0 :
                decimal.Round(pricePerUnitUsd / ethUsdRate, 8);

            results.Add(new MyPropertyDetailsDto
            {
                Id = property.Id,
                Name = property.Name,
                Description = property.Description,
                Location = property.Location,
                PropertyType = property.PropertyType,
                ImageUrls = _propertyImageRepository.GetByPropertyIdAsync(property.Id).Result.Select(x => x.ImageUrl).ToList(),
                Status = property.Status,
                RejectionReason = property.RejectionReason,
                RentalIncomeHistory = property.RentalIncomeHistory,

                TotalValue = property.ApprovedValuation,
                TotalUnits = property.TotalUnits,
                AvailableUnits = property.TotalUnits - soldUnits,

                PricePerUnit = pricePerUnitUsd,
                PricePerUnitEth = pricePerUnitEth,
                AnnualYieldPercent = property.AnnualYieldPercent,

                RiskScore = snapshot?.RiskScore ?? 5,
                DemandScore = snapshot?.DemandScore,

                Documents = documentDtos,

                HasPendingUpdateRequest = pendingUpdate != null,

                CanEditFullProperty = false,
                CanResubmit = false,
                CanRequestUpdate = false,
                CanDelete = false
            });
        }

        return new PaginatedResponse<MyPropertyDetailsDto>
        {
            Items = results,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
        };
    }

    public async Task<AdminPropertyStatsDto> GetStatsAsync()
    {
        var totalAssetValue = await _propertyRepo.GetTotalAssetValueAsync();
        var totalInvestors = await _investmentRepository.GetTotalInvestorsCountAsync();
        var tokensIssued = await _investmentRepository.GetTotalTokensIssuedAsync();
        var pendingKyc = await _adminKycRepository.GetPendingKycCountAsync();
        var platformRevenue = await _transactionRepository.GetPlatformRevenueAsync();
        var pendingPropertyApprovals = await _propertyRepo.GetPendingApprovalsCountAsync();

        return new AdminPropertyStatsDto
        {
            TotalAssetValue = totalAssetValue,
            TotalInvestors = totalInvestors,
            TokensIssued = tokensIssued,
            PendingKyc = pendingKyc,
            PlatformRevenue = platformRevenue,
            PendingPropertyApprovals = pendingPropertyApprovals
        };
    }
}