using RealEstateInvesting.Domain.Common;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Domain.Entities;

public class KycRecord : BaseEntity
{
    
    public Guid UserId { get; private set; }

    public string FullName { get; private set; } = default!;
    public DateTime DateOfBirth { get; private set; }
    public string FullAddress { get; private set; } = default!;

    public string DocumentType { get; private set; } = default!;
    public string DocumentUrl { get; private set; } = default!;
    public string SelfieUrl { get; private set; } = default!;

    public KycStatus Status { get; private set; }
    public Guid? ReviewedBy { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    private KycRecord() { }

    public static KycRecord Submit(
        Guid userId,
        string fullName,
        DateTime dateOfBirth,
        string fullAddress,
        string documentType,
        string documentUrl,
        string selfieUrl)
    {
        return new KycRecord
        {
            UserId = userId,
            FullName = fullName,
            DateOfBirth = dateOfBirth,
            FullAddress = fullAddress,
            DocumentType = documentType,
            DocumentUrl = documentUrl,
            SelfieUrl = selfieUrl,
            Status = KycStatus.Pending
        };
    }


    public void Approve(Guid adminUserId)
    {
        if (Status != KycStatus.Pending && Status != KycStatus.NotStarted)
            throw new InvalidOperationException("Only pending or not started KYC records can be approved.");

        Status = KycStatus.Approved;
        ReviewedBy = adminUserId;
        ReviewedAt = DateTime.UtcNow;
        RejectionReason = null;

        MarkUpdated();
    }

    public void Reject(Guid adminUserId, string reason)
    {
        if (Status != KycStatus.Pending && Status != KycStatus.NotStarted)
            throw new InvalidOperationException("Only pending or not started KYC records can be rejected.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException("Rejection reason is required.");

        Status = KycStatus.Rejected;
        ReviewedBy = adminUserId;
        ReviewedAt = DateTime.UtcNow;
        RejectionReason = reason;

        MarkUpdated();
    }
}
