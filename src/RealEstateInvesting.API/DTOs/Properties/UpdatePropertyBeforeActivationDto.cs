
namespace RealEstateInvesting.Api.DTOs.properties;

public class UpdatePropertyBeforeActivationDto
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string PropertyType { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public decimal InitialValuation { get; set; }
    public int TotalUnits { get; set; }
    public decimal AnnualYieldPercent { get; set; }
}