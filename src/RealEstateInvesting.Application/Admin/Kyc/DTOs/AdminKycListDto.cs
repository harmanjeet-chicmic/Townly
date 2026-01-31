using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Admin.Kyc.DTOs;

public class AdminKycListDto
{
    public Guid KycId { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = default!;
    public KycStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
