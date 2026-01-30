namespace RealEstateInvesting.Application.Properties.Dtos;

public class CreatePropertyRequestDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
    public string PropertyType { get; set; } = null!;
    public string? ImageUrl { get; set; }

    public decimal InitialValuation { get; set; }
    public int TotalUnits { get; set; }
    public decimal AnnualYieldPercent { get; set; }

    public List<PropertyDocumentDto> Documents { get; set; } = new();
}
