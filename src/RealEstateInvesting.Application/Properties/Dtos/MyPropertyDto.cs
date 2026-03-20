using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Properties.Dtos;

public class MyPropertyDto
{   
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string PropertyType { get; set; } = default!;
    public List<string> ImageUrls { get; set; } = new();
    

    public PropertyStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public string? ApprovedReason { get; set; }

    public decimal ApprovedValuation { get; set; }
    public int TotalUnits { get; set; }
    public decimal AnnualYieldPercent { get; set; }

    public int SoldUnits { get; set; }
    public int AvailableUnits { get; set; }
    public decimal InvestmentProgressPercent { get; set; }
    public decimal TotalAmountInvestedUsd { get; set; }
    public decimal? RiskScore { get; set; }
    public decimal PricePerUnitEth { get; set; }
    public bool HasPendingUpdateRequest { get; set; }
    public List<PropertyDocumentDto> AdminDocuments { get; set; } = new();
}
