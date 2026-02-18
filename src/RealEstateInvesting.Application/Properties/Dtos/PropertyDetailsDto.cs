namespace RealEstateInvesting.Application.Properties.Dtos;

public class PropertyDetailsDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string PropertyType { get; set; } = default!;
    public string? ImageUrl { get; set; }

    public decimal TotalValue { get; set; }
    public int TotalUnits { get; set; }
    public decimal PricePerUnit { get; set; }
    public decimal AnnualYieldPercent { get; set; }
    public int AvailableUnits { get; set; }
    public decimal? RiskScore { get; set; }
    public decimal? DemandScore { get; set; }
    public decimal PricePerUnitEth { get; set; }
    public decimal? UserInvestmentAmount { get; set; }
    public decimal? UserInvestedAmountEth {get;set;}

}
