namespace RealEstateInvesting.Application.Properties.InvestmentInfo;

public class PropertyInvestmentInfoDto
{
    // Platform rules
    public int MinimumInvestmentShares { get; set; }
    public string DividendFrequency { get; set; } = default!;
    public string InvestmentType { get; set; } = default!;
    public string Security { get; set; } = default!;

    // Property-specific
    public Guid PropertyOwnerUserId { get; set; }
    public decimal ExpectedAnnualReturnPercent { get; set; }
    public decimal PricePerShareUsd { get; set; }
    public decimal PricePerShareEth { get; set; }
}
