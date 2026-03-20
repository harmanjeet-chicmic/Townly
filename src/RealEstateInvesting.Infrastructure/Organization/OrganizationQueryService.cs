using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Properties.Dtos;
using RealEstateInvesting.Application.Properties.PropertyRegistrationApi;
using RealEstateInvesting.Infrastructure.Persistence;
using RealEstateInvesting.Application.Common.Dtos;
using RealEstateInvesting.Admin.Application.Organizations;
using RealEstateInvesting.Application.Organizations.Dtos;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Infrastructure.Organizations;

public class OrganizationQueryService
{
    private readonly AppDbContext _context;
    private readonly IPropertyRegistrationApiClient _propertyRegistrationApi;

    public OrganizationQueryService(
        AppDbContext context,
        IPropertyRegistrationApiClient propertyRegistrationApi)
    {
        _context = context;
        _propertyRegistrationApi = propertyRegistrationApi;
    }

    public async Task<PagedResult<OrganizationRowDto>> GetAllAsync(
    OrganizationQuery query,
    CancellationToken ct = default)
    {
        // ✅ safety defaults
        query.Page = query.Page <= 0 ? 1 : query.Page;
        query.PageSize = query.PageSize <= 0 ? 10 : query.PageSize;

        var baseQuery = _context.Organizations
            .Where(o => !o.IsDeleted);

        var totalCount = await baseQuery.CountAsync(ct);

        // 🔥 pagination
        var organizations = await baseQuery
            .OrderByDescending(o => o.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        // 🔥 property count (optimized)
        var orgIds = organizations.Select(o => o.Id).ToList();

        var propertyCounts = await _context.Properties
            .Where(p => p.OrganizationId != null && orgIds.Contains(p.OrganizationId.Value))
            .GroupBy(p => p.OrganizationId)
            .Select(g => new
            {
                OrganizationId = g.Key,
                Count = g.Count()
            })
            .ToDictionaryAsync(x => x.OrganizationId, x => x.Count, ct);

        var items = organizations.Select(o => new OrganizationRowDto
        {
            Id = o.Id,
            Name = o.Name,
            EntityType = o.EntityType,
            RegistrationNumber = o.RegistrationNumber,
            Jurisdiction = o.Jurisdiction,
            IncorporationDate = o.IncorporationDate,
            PropertyHolds = propertyCounts.TryGetValue(o.Id, out var count)
    ? count
    : 0
        }).ToList();

        return new PagedResult<OrganizationRowDto>
        {
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            HasMore = (query.Page * query.PageSize) < totalCount,
            Items = items
        };
    }
    public async Task<PagedResult<OrganizationPropertyDto>> GetPropertiesByOrganizationAsync( Guid organizationId, OrganizationQuery query,
    CancellationToken ct = default)
    {
        // safety defaults
        query.Page = query.Page <= 0 ? 1 : query.Page;
        query.PageSize = query.PageSize <= 0 ? 10 : query.PageSize;

        var baseQuery = _context.Properties
            .Include(p => p.PropertyImages)
            .Where(p => p.OrganizationId == organizationId);

        var totalCount = await baseQuery.CountAsync(ct);

        var properties = await baseQuery
            .OrderByDescending(p => p.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        var ownerIds = properties.Select(p => p.OwnerUserId).Distinct().ToList();
        var owners = await _context.Users
            .Where(u => ownerIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.WalletAddress, ct);

        var items = properties.Select(property => new OrganizationPropertyDto
        {
            Id = property.Id,
            Name = property.Name,
            Location = property.Location,
            PropertyType = property.PropertyType,

            TotalValue = property.ApprovedValuation,
            TotalUnits = property.TotalUnits,

            Status = (int)property.Status,
            Image = property.ImageUrl ?? property.PropertyImages.FirstOrDefault()?.ImageUrl,
            OwnerWalletAddress = owners.TryGetValue(property.OwnerUserId, out var wallet) ? wallet : null
        }).ToList();

        return new PagedResult<OrganizationPropertyDto>
        {
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            HasMore = (query.Page * query.PageSize) < totalCount,
            Items = items
        };
    }
    /// <summary>
    /// Activates a property: calls external T-REX property-register API, then finalizes tokenization, activates, and creates analytics snapshot.
    /// </summary>
    public async Task<PropertyRegisterResponseDto> ActivatePropertyAsync(Guid organizationId, Guid propertyId,
                                                                         ActivatePropertyDto dto, Guid adminUserId,
                                                                         CancellationToken ct = default)
    {
        var property = await _context.Properties.FirstOrDefaultAsync(p => p.Id == propertyId, ct);

        if (property == null)
            throw new Exception("Property not found");

        if (property.OrganizationId != organizationId)
            throw new Exception("Property does not belong to this organization");

        if (string.IsNullOrWhiteSpace(dto.OwnerAddress))
            throw new ArgumentException("OwnerAddress is required for on-chain property registration.", nameof(dto));

        // If STO sends a revised approved valuation, persist it on the property first.
        if (dto.ApprovedValuation.HasValue)
            property.SetApprovedValuation(dto.ApprovedValuation.Value);
        await _context.SaveChangesAsync(ct);

        var totalUnits = dto.TotalUnits;
        var pricePerShare = totalUnits > 0 ? (property.ApprovedValuation / totalUnits) * 1000000 : 0m;

        var registerRequest = new PropertyRegisterRequestDto
        {
            PropertyId = propertyId.ToString(),
            OwnerAddress = dto.OwnerAddress.Trim(),
            PricePerShare = ((long)pricePerShare).ToString(),
            MintAmount = totalUnits.ToString(),
            IpfsUri = !string.IsNullOrWhiteSpace(dto.IpfsUri) ? dto.IpfsUri : (property.ImageUrl ?? "string"),
            PropertyName = property.Name ?? "string",
            Description = property.Description ?? "string",
            Location = property.Location ?? "string",
            PropertyType = property.PropertyType ?? "string"
        };

        PropertyRegisterResponseDto apiResponse;
        try
        {
            apiResponse = await _propertyRegistrationApi.RegisterPropertyAsync(registerRequest, ct);
        }
        catch (Exception ex)
        {
            // If the external API call itself fails, mark the property as FAILED.
            property.SetRegistrationJobStatus(PropertyStatus.FAILED);
            await _context.SaveChangesAsync(ct);

            return new PropertyRegisterResponseDto
            {
                StatusCode = 500,
                Status = false,
                Message = $"Property registration API call failed: {ex.Message}",
                Type = "FAILED",
                Data = null
            };
        }

        // If the external API returned an invalid or failed job, mark the property as FAILED.
        if (apiResponse?.Data == null)
        {
            property.SetRegistrationJobStatus(PropertyStatus.FAILED);
            await _context.SaveChangesAsync(ct);

            return apiResponse ?? new PropertyRegisterResponseDto
            {
                StatusCode = 500,
                Status = false,
                Message = "Property registration API returned an empty response.",
                Type = "FAILED",
                Data = null
            };
        }

        var mappedStatus = MapTrexStatus(apiResponse.Data.Status);

        // Persist activation record when we have job identifiers.
        var job = PropertyActivationRecord.Create(
            jobId: apiResponse.Data.JobId,
            propertyId: apiResponse.Data.PropertyId,
            status: apiResponse.Data.Status,
            trexDeployTxHash: apiResponse.Data.TrexDeployTxHash,
            createdBy: adminUserId);
        _context.PropertyActivationRecords.Add(job);

        // If the API response indicates failure (HTTP may be success, but job failed), don't finalize tokenization.
        if (apiResponse.Status != true || mappedStatus == PropertyStatus.FAILED)
        {
            property.SetRegistrationJobStatus(PropertyStatus.FAILED);
            await _context.SaveChangesAsync(ct);
            return apiResponse;
        }

        property.FinalizeTokenization(dto.TotalUnits, dto.RentalIncome, dto.AnnualYieldPercent);
        property.SetRegistrationJobStatus(mappedStatus);

        var snapshot = PropertyAnalyticsSnapshot.Create(
            propertyId: property.Id,
            snapshotAt: DateTime.UtcNow,
            sharesSold: 0,
            totalInvested: 0m,
            demandScore: 0m,
            riskScore: dto.RiskScore,
            pricePerShare: pricePerShare,
            valuation: property.ApprovedValuation);
        _context.PropertyAnalyticsSnapshots.Add(snapshot);
        await _context.SaveChangesAsync(ct);
        return apiResponse;
    }

    public static PropertyStatus MapTrexStatus(int trexStatusCode) => TrexStatusMapper.MapToPropertyStatus(trexStatusCode);
}