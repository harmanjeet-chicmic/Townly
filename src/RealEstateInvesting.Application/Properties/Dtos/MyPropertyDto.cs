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
    public long TotalUnits { get; set; }
    public decimal AnnualYieldPercent { get; set; }

    public long SoldUnits { get; set; }
    public long AvailableUnits { get; set; }
    public decimal InvestmentProgressPercent { get; set; }
    public decimal TotalAmountInvestedUsd { get; set; }
    public decimal? RiskScore { get; set; }
    public decimal? PricePerShare { get; set; }
    public bool HasPendingUpdateRequest { get; set; }
    public List<PropertyDocumentDto> AdminDocuments { get; set; } = new();
}
