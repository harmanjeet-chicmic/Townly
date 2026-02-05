
using RealEstateInvesting.Domain.Enums;
namespace RealEstateInvesting.Application.Tokens.Requests;

public class AdminTokenRequestListDto
{
    public Guid RequestId { get; set; }
    public Guid UserId { get; set; }
    public decimal RequestedAmount { get; set; }
    public TokenRequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
