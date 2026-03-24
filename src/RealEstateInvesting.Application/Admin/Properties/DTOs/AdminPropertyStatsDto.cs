namespace RealEstateInvesting.Application.Admin.Properties.DTOs;

public class AdminPropertyStatsDto
{
    public decimal TotalAssetValue { get; set; }
    public int TotalInvestors { get; set; }
    public long TokensIssued { get; set; }
    public int PendingKyc { get; set; }
    public decimal PlatformRevenue { get; set; }
    public int PendingPropertyApprovals { get; set; }
    public int TotalProperties { get; set; }
    public int ActiveProperties { get; set; }
    public int PendingProperties { get; set; }
    public int RejectedProperties { get; set; }
}
