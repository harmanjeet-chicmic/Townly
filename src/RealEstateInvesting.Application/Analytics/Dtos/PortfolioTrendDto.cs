namespace RealEstateInvesting.Application.Analytics.Dtos;

public class PortfolioTrendDto
{
    public DateTime SnapshotAt { get; set; }
    public decimal PortfolioValue { get; set; }
    public decimal UnrealizedPnL { get; set; }
}
