using RealEstateInvesting.Domain.Common;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Domain.Entities;

public class Property : BaseEntity
{
    // Ownership
    public Guid OwnerUserId { get; private set; }

    // Basic details
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string Location { get; private set; } = default!;
    public string PropertyType { get; private set; } = default!;
    public string? ImageUrl { get; private set; }

    // Valuation & Yield
    public decimal InitialValuation { get; private set; }
    public decimal ApprovedValuation { get; private set; }
    public int TotalUnits { get; private set; }
    public decimal AnnualYieldPercent { get; private set; }

    // Lifecycle
    public PropertyStatus Status { get; private set; } = PropertyStatus.Draft;

    // Approval
    public DateTime? ApprovedAt { get; private set; }
    public Guid? ReviewedBy { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    private Property() { }

    // ---------------------------
    // Factory
    // ---------------------------
    public static Property CreateDraft(
        Guid ownerUserId,
        string name,
        string description,
        string location,
        string propertyType,
        string? imageUrl,
        decimal initialValuation,
        int totalUnits,
        decimal annualYieldPercent)
    {
        if (totalUnits <= 0)
            throw new InvalidOperationException("Total units must be greater than zero.");

        return new Property
        {
            OwnerUserId = ownerUserId,
            Name = name,
            Description = description,
            Location = location,
            PropertyType = propertyType,
            ImageUrl = imageUrl,
            InitialValuation = initialValuation,
            ApprovedValuation = initialValuation,
            TotalUnits = totalUnits,
            AnnualYieldPercent = annualYieldPercent,
            Status = PropertyStatus.Draft
        };
    }

    // ---------------------------
    // Lifecycle
    // ---------------------------
    public void Submit()
    {
        if (Status != PropertyStatus.Draft)
            throw new InvalidOperationException("Only draft properties can be submitted.");

        Status = PropertyStatus.PendingApproval;
        MarkUpdated();
    }

    public void Activate()
    {
        if (Status != PropertyStatus.PendingApproval)
            throw new InvalidOperationException("Property must be pending approval.");

        Status = PropertyStatus.Active;
        ApprovedAt = DateTime.UtcNow;
        MarkUpdated();
    }
    public void MarkSoldOut()
    {
        if (Status != PropertyStatus.Active)
            throw new InvalidOperationException("Only active properties can be sold out.");

        Status = PropertyStatus.SoldOut;
        MarkUpdated();
    }
    public void Reject(Guid adminUserId, string reason)
    {
        if (Status != PropertyStatus.PendingApproval)
            throw new InvalidOperationException("Only pending properties can be rejected.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException("Rejection reason is required.");

        Status = PropertyStatus.Rejected;
        ReviewedBy = adminUserId;
        ReviewedAt = DateTime.UtcNow;
        RejectionReason = reason;

        MarkUpdated();
    }
    // ---------------------------
    // Metadata Update (Admin Approved)
    // ---------------------------
    public void ApplyApprovedUpdate(
        string name,
        string description,
        string location,
        string propertyType,
        string? imageUrl)
    {
        if (Status != PropertyStatus.Active &&
            Status != PropertyStatus.SoldOut)
            throw new InvalidOperationException(
                "Only active or sold properties can be updated.");

        Name = name;
        Description = description;
        Location = location;
        PropertyType = propertyType;
        ImageUrl = imageUrl;

        MarkUpdated();
    }


}
