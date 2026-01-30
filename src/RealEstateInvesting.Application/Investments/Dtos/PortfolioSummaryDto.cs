namespace RealEstateInvesting.Application.Investments.Dtos;

public class PortfolioSummaryDto
{
    public decimal TotalInvested { get; set; }
    public int PropertiesCount { get; set; }
    public int TotalSharesOwned { get; set; }
}
