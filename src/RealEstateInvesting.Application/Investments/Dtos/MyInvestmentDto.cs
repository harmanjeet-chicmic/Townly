namespace RealEstateInvesting.Application.Investments.Dtos;

public class MyInvestmentDto
{
    public Guid InvestmentId { get; set; }
    public Guid PropertyId { get; set; }

    public string PropertyName { get; set; } = default!;
    public string? PropertyImageUrl { get; set; }
    public string Location { get; set; } = default!;

    // Ownership
    public int SharesPurchased { get; set; }

    // USD (source of truth)
    public decimal PricePerShareUsd { get; set; }
    public decimal TotalAmountUsd { get; set; }

    // ETH snapshot (execution-time)
    public decimal EthAmountAtExecution { get; set; }
    public decimal EthUsdRateAtExecution { get; set; }

    public DateTime InvestedAt { get; set; }
}
