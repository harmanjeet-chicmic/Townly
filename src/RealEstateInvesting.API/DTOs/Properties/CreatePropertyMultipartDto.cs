using Microsoft.AspNetCore.Http;

namespace RealEstateInvesting.API.Dtos.Properties;

public class CreatePropertyMultipartDto
{
    // Property info
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
    public string PropertyType { get; set; } = null!;

    public decimal InitialValuation { get; set; }
    public int TotalUnits { get; set; }
    public decimal AnnualYieldPercent { get; set; }

    // Files
    public IFormFile? Image { get; set; }

    // Multiple documents
    public List<IFormFile> Documents { get; set; } = new();
}
