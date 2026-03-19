using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Properties.Dtos;
using RealEstateInvesting.Infrastructure.Persistence;
using RealEstateInvesting.Application.Common.Dtos;
using RealEstateInvesting.Admin.Application.Organizations;
using RealEstateInvesting.Application.Organizations.Dtos;
using RealEstateInvesting.Domain.Entities;
namespace RealEstateInvesting.Infrastructure.Organizations;

public class OrganizationQueryService
{
    private readonly AppDbContext _context;

    public OrganizationQueryService(AppDbContext context)
    {
        _context = context;
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
    public async Task<PagedResult<OrganizationPropertyDto>> GetPropertiesByOrganizationAsync(
    Guid organizationId,
    OrganizationQuery query,
    CancellationToken ct = default)
    {
        // ✅ safety defaults
        query.Page = query.Page <= 0 ? 1 : query.Page;
        query.PageSize = query.PageSize <= 0 ? 10 : query.PageSize;

        var baseQuery = _context.Properties
            .Where(p => p.OrganizationId == organizationId);

        var totalCount = await baseQuery.CountAsync(ct);

        var properties = await baseQuery
            .OrderByDescending(p => p.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        var items = properties.Select(property => new OrganizationPropertyDto
        {
            Id = property.Id,
            Name = property.Name,
            Location = property.Location,
            PropertyType = property.PropertyType,

            TotalValue = property.ApprovedValuation,
            TotalUnits = property.TotalUnits,

            Status = (int)property.Status
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
    public async Task ActivatePropertyAsync(
    Guid organizationId,
    Guid propertyId,
    ActivatePropertyDto dto,
    Guid adminUserId,
    CancellationToken ct = default)
    {
        var property = await _context.Properties
            .FirstOrDefaultAsync(p => p.Id == propertyId, ct);

        if (property == null)
            throw new Exception("Property not found");

        // 🔥 CRITICAL CHECK
        if (property.OrganizationId != organizationId)
            throw new Exception("Property does not belong to this organization");

        // ✅ finalize tokenization
        property.FinalizeTokenization(
            dto.TotalUnits,
            dto.RentalIncome,
            dto.AnnualYieldPercent
          
        );

        // ✅ activate
        property.Activate();

        // ✅ create initial analytics snapshot with the risk score
        var snapshot = PropertyAnalyticsSnapshot.Create(
            propertyId: property.Id,
            snapshotAt: DateTime.UtcNow,
            sharesSold: 0,
            totalInvested: 0m,
            demandScore: 0m,
            riskScore: dto.RiskScore,
            pricePerShare: property.ApprovedValuation / dto.TotalUnits,
            valuation: property.ApprovedValuation
        );
        _context.PropertyAnalyticsSnapshots.Add(snapshot);

        await _context.SaveChangesAsync(ct);
    }
}