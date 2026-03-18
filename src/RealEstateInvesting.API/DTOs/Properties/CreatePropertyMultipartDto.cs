// using Microsoft.AspNetCore.Http;
// using RealEstateInvesting.API.Dtos.Properties;
// namespace RealEstateInvesting.API.Dtos.Properties;

// public class CreatePropertyMultipartDto
// {
//     // Property info
//     public string Name { get; set; } = null!;
//     public string Description { get; set; } = null!;
//     public string Location { get; set; } = null!;
//     public string PropertyType { get; set; } = null!;

//     public decimal InitialValuation { get; set; }
//     public int TotalUnits { get; set; }
//     public decimal AnnualYieldPercent { get; set; }
//     public decimal RentalIncome { get; set; }

//     // Files
//     public IFormFile? Image { get; set; }

//     // Multiple documents
//       public List<PropertyDocumentUploadDto> Documents { get; set; } = new();
// }


using Microsoft.AspNetCore.Http;

namespace RealEstateInvesting.API.Dtos.Properties;

public class CreatePropertyMultipartDto
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string PropertyType { get; set; } = default!;

    public decimal TotalPropertyValueUsd { get; set; }
    public decimal SquareFeet { get; set; }
  

    public decimal SellingPercentage { get; set; }
    public decimal SharePerSquareFeet { get; set; }

    public List<IFormFile>? Images { get; set; }

    public List<PropertyDocumentUploadDto> Documents { get; set; } = new();
}

public class PropertyDocumentUploadDto
{
    public string Title { get; set; } = default!;
    public IFormFile File { get; set; } = default!;
}