using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Properties.Dtos;

public class MarketplaceSearchRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 9;
    public string? Search { get; set; }
    public string? PropertyType { get; set; }
    public string? Cursor { get; set; }
    public List<PropertyStatus>? Status { get; set; }
}
