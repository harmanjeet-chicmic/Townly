using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Application.Properties.Dtos;
namespace RealEstateInvesting.Infrastructure.Persistence.Repositories;

public class PropertyRepository : IPropertyRepository
{
    private readonly AppDbContext _context;

    public PropertyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Property property)
    {
        _context.Properties.Add(property);
        await _context.SaveChangesAsync();
    }

    public async Task<Property?> GetByIdAsync(Guid propertyId)
    {
        return await _context.Properties
            .FirstOrDefaultAsync(p => p.Id == propertyId);
    }

    public async Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerUserId)
    {
        return await _context.Properties
            .Where(p => p.OwnerUserId == ownerUserId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetByStatusAsync(PropertyStatus status)
    {
        return await _context.Properties
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
    public async Task<(IEnumerable<MarketplacePropertyReadModel> Items, int TotalCount)>
    GetMarketplaceAsync(
         Guid? currentUserId,
         int page,
         int pageSize,
         string? search,
         string? propertyType)
    {
        var query = _context.Properties
            .Where(p => p.Status == PropertyStatus.Active && p.OwnerUserId != currentUserId);
        Console.WriteLine("=============================seach=======================" + search + "-------------------------");
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            Console.WriteLine("=============================seach=======================" + search);
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    p.Location.Contains(search));
            }
        }


        if (!string.IsNullOrWhiteSpace(propertyType))
        {
            query = query.Where(p => p.PropertyType == propertyType);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new MarketplacePropertyReadModel
            {
                Id = p.Id,
                Name = p.Name,
                Location = p.Location,
                PropertyType = p.PropertyType,
                ImageUrl = p.ImageUrl,
                ApprovedValuation = p.ApprovedValuation,
                AnnualYieldPercent = p.AnnualYieldPercent,
                TotalUnits = p.TotalUnits,

                SoldUnits = _context.Investments
                    .Where(i => i.PropertyId == p.Id)
                    .Sum(i => (int?)i.SharesPurchased) ?? 0
            })
            .ToListAsync();

        return (items, totalCount);
    }
    public async Task<(IEnumerable<Property> Items, int TotalCount)>
GetByOwnerIdPagedAsync(
    Guid ownerUserId,
    int page,
    int pageSize,
    PropertyStatus? status,
    string? search)
    {
        var query = _context.Properties
            .Where(p => p.OwnerUserId == ownerUserId);
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            Console.WriteLine("=============================seach=======================" + search);
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    p.Location.Contains(search));
            }
        }
        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<MarketplacePropertyReadModel>> GetMarketplaceCursorAsync(
    int limit,
    string? cursor,
    string? search,
    string? propertyType)
    {
        var query = _context.Properties
            .Where(p => p.Status == PropertyStatus.Active);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Name.Contains(search) ||
                p.Location.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(propertyType))
        {
            query = query.Where(p => p.PropertyType == propertyType);
        }

        // 🔑 Cursor logic
        if (!string.IsNullOrWhiteSpace(cursor))
        {
            var parts = cursor.Split('|');
            var createdAt = DateTime.Parse(parts[0]);
            var id = Guid.Parse(parts[1]);

            query = query.Where(p =>
                p.CreatedAt < createdAt ||
                (p.CreatedAt == createdAt && p.Id.CompareTo(id) < 0));
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.Id)
            .Take(limit)
            .Select(p => new MarketplacePropertyReadModel
            {
                Id = p.Id,
                CreatedAt = p.CreatedAt,

                Name = p.Name,
                Location = p.Location,
                PropertyType = p.PropertyType,
                ImageUrl = p.ImageUrl,
                ApprovedValuation = p.ApprovedValuation,
                AnnualYieldPercent = p.AnnualYieldPercent,
                TotalUnits = p.TotalUnits,

                SoldUnits = _context.Investments
                    .Where(i => i.PropertyId == p.Id)
                    .Sum(i => (int?)i.SharesPurchased) ?? 0
            })
            .ToListAsync();
    }


    public async Task<IEnumerable<Property>> GetFeaturedAsync(int limit , Guid? CurrentUserId)
    {
        return await _context.Properties
            .Where(p => p.Status == PropertyStatus.Active && p.OwnerUserId!=CurrentUserId)
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task UpdateAsync(Property property)
    {
        _context.Properties.Update(property);
        await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<Property>> GetByIdsAsync(IEnumerable<Guid> propertyIds)
    {
        return await _context.Properties
            .Where(p => propertyIds.Contains(p.Id))
            .ToListAsync();
    }
    public async Task<PropertyWithSoldUnits?> GetDetailsWithSoldUnitsAsync(Guid propertyId)
    {
        return await _context.Properties
            .Where(p => p.Id == propertyId &&( p.Status == PropertyStatus.Active || p.Status == PropertyStatus.SoldOut))
            .Select(p => new PropertyWithSoldUnits
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Location = p.Location,
                PropertyType = p.PropertyType,
                ImageUrl = p.ImageUrl,

                ApprovedValuation = p.ApprovedValuation,
                TotalUnits = p.TotalUnits,
                AnnualYieldPercent = p.AnnualYieldPercent,

                SoldUnits = _context.Investments
                    .Where(i => i.PropertyId == p.Id)
                    .Sum(i => (int?)i.SharesPurchased) ?? 0
            })
            .FirstOrDefaultAsync();
    }

    public async Task DeleteAsync(Property property)
    {
        _context.Properties.Remove(property);
        await _context.SaveChangesAsync();
    }


}

