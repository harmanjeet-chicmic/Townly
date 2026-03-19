namespace RealEstateInvesting.Application.Admin.Properties.DTOs;

public class AdminPropertyStatsDto
{
    public decimal TotalAssetValue { get; set; }
    public int TotalInvestors { get; set; }
    public int TokensIssued { get; set; }
    public int PendingKyc { get; set; }
    public decimal PlatformRevenue { get; set; }
    public int PendingPropertyApprovals { get; set; }
}
