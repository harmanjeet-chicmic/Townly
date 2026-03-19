using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Properties.Dtos;

public class MyPropertyDto
{   
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string PropertyType { get; set; } = default!;
    public List<string> Images { get; set; } = new();
    public PropertyStatus Status { get; set; }

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

} 
