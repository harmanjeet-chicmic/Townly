namespace RealEstateInvesting.Application.Investments.Dtos;

public class MyInvestmentDto
{
    public Guid PropertyId { get; set; }
    public string PropertyName { get; set; } = default!;
    public List<string> PropertyImageUrls { get; set; } = new();
    public string Location { get; set; } = default!;
    public string PropertyType { get; set; } = default!;

    public long SharesPurchased { get; set; }

    // USD
    public decimal TotalAmountUsd { get; set; }

    // Historical ETH (true invested)
    public decimal TotalInvestedEth { get; set; }

    // Live valuation
    public decimal CurrentValueEth { get; set; }
    public decimal TotalReturnEth { get; set; }

    public decimal MonthlyIncomeEth { get; set; }

    public decimal? AnnualYieldPercent { get; set; }
    public decimal? RiskScore { get; set; }

    public DateTime InvestedAt { get; set; }
}