namespace RealEstateInvesting.Application.Properties.Dtos;

public class MarketplacePropertyReadModel
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string PropertyType { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public PropertyStatus Status { get; set; }

    public decimal ApprovedValuation { get; set; }
    public decimal AnnualYieldPercent { get; set; }

    public long TotalUnits { get; set; }
    public long SoldUnits { get; set; }
}
