namespace RealEstateInvesting.Application.Admin.Kyc.DTOs;

public enum AdminKycFilterStatus
{
    NotStarted = 0,
    Pending = 1,
    Approved = 2,
    Rejected = 3
}

public class AdminKycQuery
{
    public string? Search { get; set; }
    public AdminKycFilterStatus? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
