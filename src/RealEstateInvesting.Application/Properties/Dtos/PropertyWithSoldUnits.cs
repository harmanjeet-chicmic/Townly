namespace RealEstateInvesting.Application.Properties.Dtos;

public class PropertyWithSoldUnits
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string PropertyType { get; set; } = default!;
    public string? ImageUrl { get; set; }

    public decimal ApprovedValuation { get; set; }
    public int TotalUnits { get; set; }
    public decimal AnnualYieldPercent { get; set; }

    public int SoldUnits { get; set; }
}
