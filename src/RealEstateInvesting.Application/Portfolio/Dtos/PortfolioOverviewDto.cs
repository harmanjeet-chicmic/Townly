namespace RealEstateInvesting.Application.Portfolio.Dtos;

public class PortfolioOverviewDto
{
    public decimal TotalInvestedEth { get; set; }
    public decimal CurrentValueEth { get; set; }

    public decimal TotalReturnEth { get; set; }
    public decimal TotalReturnPercent { get; set; }

    public decimal MonthlyIncomeEth { get; set; }
}
