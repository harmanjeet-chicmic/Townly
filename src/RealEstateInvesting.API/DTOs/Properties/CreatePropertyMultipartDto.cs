using Microsoft.AspNetCore.Http;
using RealEstateInvesting.API.Dtos.Properties;
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
    public decimal RentalIncome { get; set; }

    // Files
    public IFormFile? Image { get; set; }

    // Multiple documents
      public List<PropertyDocumentUploadDto> Documents { get; set; } = new();
}
