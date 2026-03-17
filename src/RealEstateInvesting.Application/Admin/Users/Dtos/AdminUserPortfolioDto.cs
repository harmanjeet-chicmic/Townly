namespace RealEstateInvesting.Application.Admin.Users.DTOs;

public class AdminUserPortfolioDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string WalletAddress { get; set; } = default!;

    public int Properties { get; set; }

    public decimal TotalInvestment { get; set; }

    public decimal PortfolioValue { get; set; }

    public string KycStatus { get; set; } = default!;
}