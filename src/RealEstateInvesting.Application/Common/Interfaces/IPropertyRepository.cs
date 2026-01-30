using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Application.Properties.Dtos;
public interface IPropertyRepository
{
    Task AddAsync(Property property);

    Task<Property?> GetByIdAsync(Guid propertyId);

    Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerUserId);

    Task<IEnumerable<Property>> GetByStatusAsync(PropertyStatus status);
    Task<(IEnumerable<MarketplacePropertyReadModel> Items, int TotalCount)>
GetMarketplaceAsync(
    int page,
    int pageSize,
    string? search,
    string? propertyType);


    Task<IEnumerable<Property>> GetFeaturedAsync(int limit);
    Task UpdateAsync(Property property);
    Task<IEnumerable<Property>> GetByIdsAsync(IEnumerable<Guid> propertyIds);

    Task<PropertyWithSoldUnits?> GetDetailsWithSoldUnitsAsync(Guid propertyId);



}
