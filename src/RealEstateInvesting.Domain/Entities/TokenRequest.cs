using RealEstateInvesting.Domain.Common;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Domain.Entities;

public class TokenRequest : BaseEntity
{
    public Guid UserId { get; private set; }
    public decimal RequestedAmount { get; private set; }
    public TokenRequestStatus Status { get; private set; }
    public Guid? ReviewedBy { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    private TokenRequest() { }

    public static TokenRequest Create(Guid userId, decimal amount)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Requested amount must be greater than zero.");

        return new TokenRequest
        {
            UserId = userId,
            RequestedAmount = amount,
            Status = TokenRequestStatus.Pending
        };
    }

    public void Approve(Guid adminId)
    {
        if (Status != TokenRequestStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be approved.");

        Status = TokenRequestStatus.Approved;
        ReviewedBy = adminId;
        ReviewedAt = DateTime.UtcNow;
    }

    public void Reject(Guid adminId, string reason)
    {
        if (Status != TokenRequestStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be rejected.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException("Rejection reason is required.");

        Status = TokenRequestStatus.Rejected;
        ReviewedBy = adminId;
        ReviewedAt = DateTime.UtcNow;
        RejectionReason = reason;
    }
}
