using RealEstateInvesting.Application.Admin.Properties.DTOs;
using RealEstateInvesting.Application.Admin.Properties.Interfaces;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Common.DTOs;
using RealEstateInvesting.Application.Admin.Properties.DTOs;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Properties.Dtos;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Application.Notifications.Interfaces;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Admin.Properties;

public class AdminPropertyService : IAdminPropertyService
{
    private readonly IAdminPropertyRepository _propertyRepo;
    private readonly INotificationService _notificationService;
    private readonly IPropertyUpdateRequestRepository _updateRepo;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IPropertyRegistrationJobRepository _registrationJobRepository;
    private readonly IPropertyDocumentRepository _documentRepository;

    public AdminPropertyService(
        IAdminPropertyRepository propertyRepo,
        INotificationService notificationService,
        IPropertyUpdateRequestRepository updateRepo,
        IPropertyRepository propertyRepository,
        IPropertyRegistrationJobRepository registrationJobRepository,
        IPropertyDocumentRepository documentRepository)
    {
        _propertyRepo = propertyRepo;
        _notificationService = notificationService;
        _updateRepo = updateRepo;
        _propertyRepository = propertyRepository;
        _registrationJobRepository = registrationJobRepository;
        _documentRepository = documentRepository;
    }

    public async Task<PaginatedResponse<MyPropertyDetailsDto>> GetPendingAsync(AdminPropertyQuery query)
    {
        var (properties, totalCount) = await _propertyRepo.GetPendingAsync(query);
        var propertyIds = properties.Select(p => p.Id).ToList();
        var jobsMap = await _registrationJobRepository.GetLatestByPropertyIdsAsync(propertyIds);
        
        var items = properties.Select(p => {
            jobsMap.TryGetValue(p.Id, out var job);
            return new MyPropertyDetailsDto
            {
                Id = p.Id,
                Name = p.Name,
                Location = p.Location,
                Status = p.Status,
                PropertyType = p.PropertyType,
                ApprovedValuation = p.ApprovedValuation,
                PricePerShare = job?.PricePerShare ?? (p.TotalUnits == 0 ? 0 : p.ApprovedValuation / p.TotalUnits),
                TotalUnits = p.TotalUnits,
                AnnualYieldPercent = p.AnnualYieldPercent
            };
        }).ToList();

        return new PaginatedResponse<MyPropertyDetailsDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task ApproveAsync(Guid propertyId, Guid adminId, ApprovePropertyRequest request)
    {
        var property = await _propertyRepo.GetByIdAsync(propertyId)
            ?? throw new InvalidOperationException("Property not found.");

        property.MarkAdminApproved(adminId, request?.Reason);

        if (request?.Documents != null && request.Documents.Any())
        {
            foreach (var docDto in request.Documents)
            {
                var doc = PropertyDocument.Create(
                    propertyId,
                    docDto.Title,
                    docDto.FileName,
                    docDto.DocumentUrl,
                    PropertyDocumentType.Approved);
                await _documentRepository.AddAsync(doc);
            }
        }

        await _propertyRepo.SaveChangesAsync();

        await _notificationService.CreateAsync(
            property.OwnerUserId,
            NotificationType.PropertyApproved,
            "Property Approved",
            $"Your property \"{property.Name}\" has been approved and is now live.",
            property.Id
        );
    }

    public async Task RejectAsync(Guid propertyId, Guid adminId, RejectPropertyRequest request)
    {
        var property = await _propertyRepo.GetByIdAsync(propertyId)
            ?? throw new InvalidOperationException("Property not found.");

        property.Reject(adminId, request.Reason);

        if (request.Documents != null && request.Documents.Any())
        {
            foreach (var docDto in request.Documents)
            {
                var doc = PropertyDocument.Create(
                    propertyId,
                    docDto.Title,
                    docDto.FileName,
                    docDto.DocumentUrl,
                    PropertyDocumentType.Rejected);
                await _documentRepository.AddAsync(doc);
            }
        }

        await _propertyRepo.SaveChangesAsync();

        await _notificationService.CreateAsync(
            property.OwnerUserId,
            NotificationType.PropertyRejected,
            "Property Rejected",
            $"Your property \"{property.Name}\" was rejected. Reason: {request.Reason}",
            property.Id
        );
    }

    public async Task ModifyRequest(Guid propertyId, Guid adminId, string reason)
    {
        var property = await _propertyRepo.GetByIdAsync(propertyId)
            ?? throw new InvalidOperationException("Property not found.");

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
        // Placeholder implementation as I don't have the full repo method yet
        return Enumerable.Empty<PendingPropertyUpdateDto>();
    }

    public async Task ApproveUpdateRequestAsync(Guid updateRequestId, Guid adminId)
    {
        // Placeholder
    }

    public async Task RejectUpdateRequestAsync(Guid updateRequestId, Guid adminId, string reason)
    {
        // Placeholder
    }

    public async Task<PaginatedResponse<MyPropertyDetailsDto>> GetAllAsync(AdminPropertyQuery query)
    {
        var (properties, totalCount) = await _propertyRepo.GetAllAsync(query);
        var propertyIds = properties.Select(p => p.Id).ToList();
        var jobsMap = await _registrationJobRepository.GetLatestByPropertyIdsAsync(propertyIds);

        var items = properties.Select(p => {
            jobsMap.TryGetValue(p.Id, out var job);
            return new MyPropertyDetailsDto
            {
                Id = p.Id,
                Name = p.Name,
                Location = p.Location,
                Status = p.Status,
                PropertyType = p.PropertyType,
                ApprovedValuation = p.ApprovedValuation,
                PricePerShare = job?.PricePerShare ?? (p.TotalUnits == 0 ? 0 : p.ApprovedValuation / p.TotalUnits),
                TotalUnits = p.TotalUnits,
                AnnualYieldPercent = p.AnnualYieldPercent
            };
        }).ToList();

        return new PaginatedResponse<MyPropertyDetailsDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task AssignToOrganizationAsync(Guid propertyId, Guid organizationId)
    {
        // Placeholder
    }

    public async Task<AdminPropertyStatsDto> GetStatsAsync()
    {
        var (properties, totalCount) = await _propertyRepo.GetAllAsync(new AdminPropertyQuery { PageSize = 10000 });

        return new AdminPropertyStatsDto
        {
            TotalProperties = totalCount,
            ActiveProperties = properties.Count(p => p.Status == PropertyStatus.Active),
            PendingProperties = properties.Count(p => p.Status == PropertyStatus.PendingApproval),
            RejectedProperties = properties.Count(p => p.Status == PropertyStatus.Rejected),
            PendingPropertyApprovals = properties.Count(p => p.Status == PropertyStatus.PendingApproval)
        };
    }
}