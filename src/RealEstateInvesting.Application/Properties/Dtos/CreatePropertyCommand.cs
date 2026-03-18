// namespace RealEstateInvesting.Application.Properties.Dtos;

// public class CreatePropertyCommand
// {
//     public string Name { get; set; } = null!;
//     public string Description { get; set; } = null!;
//     public string Location { get; set; } = null!;
//     public string PropertyType { get; set; } = null!;


//     public decimal InitialValuation { get; set; }
//     public int TotalUnits { get; set; }
//     public decimal AnnualYieldPercent { get; set; }
//     public decimal RentalIncomeHistory { get; set; }

//     public string? ImageUrl { get; set; }
//     public List<PropertyDocumentDto> Documents { get; set; } = new();
// }


namespace RealEstateInvesting.Application.Properties.Dtos;

public class CreatePropertyCommand
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string PropertyType { get; set; } = default!;

    public decimal TotalPropertyValueUsd { get; set; }
    public decimal SquareFeet { get; set; }

    public decimal SellingPercentage { get; set; }
    public decimal SharePerSquareFeet { get; set; }

    public List<PropertyDocumentDto> Documents { get; set; } = new();
}