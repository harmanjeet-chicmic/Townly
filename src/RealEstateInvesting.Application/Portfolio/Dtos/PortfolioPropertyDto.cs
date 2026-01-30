namespace RealEstateInvesting.Application.Portfolio.Dtos;

public class PortfolioPropertyDto
{
    public Guid PropertyId { get; set; }
    public string PropertyName { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public string Location { get; set; } = default!;

    // Ownership
    public int TokensOwned { get; set; }

    // ETH-based values
    public decimal InvestedEth { get; set; }
    public decimal CurrentValueEth { get; set; }

    public decimal UnrealizedPnLEth { get; set; }
    public decimal UnrealizedPnLPercent { get; set; }

    public decimal MonthlyIncomeEth { get; set; }

    // Risk
    public decimal? RiskScore { get; set; }
}
