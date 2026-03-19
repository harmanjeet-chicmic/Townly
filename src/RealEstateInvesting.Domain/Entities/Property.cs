// using RealEstateInvesting.Domain.Common;
// using RealEstateInvesting.Domain.Enums;

// namespace RealEstateInvesting.Domain.Entities;

// public class Property : BaseEntity
// {
//     // Ownership
//     public Guid OwnerUserId { get; private set; }

//     // Basic details
//     public string Name { get; private set; } = default!;
//     public string Description { get; private set; } = default!;
//     public string Location { get; private set; } = default!;
//     public string PropertyType { get; private set; } = default!;
//     public string? ImageUrl { get; private set; }

//     // Valuation & Yield
//     public decimal InitialValuation { get; private set; }
//     public decimal ApprovedValuation { get; private set; }
//     public int TotalUnits { get; private set; }
//     public decimal AnnualYieldPercent { get; private set; }
//     public decimal RentalIncomeHistory { get; private set; }

//     // Lifecycle
//     public PropertyStatus Status { get; private set; } = PropertyStatus.Draft;

//     // Approval
//     public DateTime? ApprovedAt { get; private set; }
//     public Guid? ReviewedBy { get; private set; }
//     public DateTime? ReviewedAt { get; private set; }
//     public string? RejectionReason { get; private set; }

//     private Property() { }

//     // ---------------------------
//     // Factory
//     // ---------------------------
//     public static Property CreateDraft(
//         Guid ownerUserId,
//         string name,
//         string description,
//         string location,
//         string propertyType,
//         string? imageUrl,
//         decimal initialValuation,
//         int totalUnits,
//         decimal annualYieldPercent,
//         decimal rentalIncomeHistory)
//     {
//         if (totalUnits <= 0)
//             throw new InvalidOperationException("Total units must be greater than zero.");
//         if (string.IsNullOrWhiteSpace(name))
//             throw new InvalidOperationException("Name is required.");

//         if (initialValuation <= 0)
//             throw new InvalidOperationException("Valuation must be positive.");

//         if (totalUnits <= 0)
//             throw new InvalidOperationException("Total units must be greater than zero.");

//         if (annualYieldPercent <= 0 || annualYieldPercent > 50)
//             throw new InvalidOperationException("Yield percent must be between 0 and 50.");

//         return new Property
//         {
//             OwnerUserId = ownerUserId,
//             Name = name,
//             Description = description,
//             Location = location,
//             PropertyType = propertyType,
//             ImageUrl = imageUrl,
//             InitialValuation = initialValuation,
//             ApprovedValuation = initialValuation,
//             TotalUnits = totalUnits,
//             RentalIncomeHistory = rentalIncomeHistory,
//             AnnualYieldPercent = annualYieldPercent,
//             Status = PropertyStatus.Draft
//         };
//     }

//     // ---------------------------
//     // Lifecycle
//     // ---------------------------
//     public void Submit()
//     {
//         if (Status != PropertyStatus.Draft)
//             throw new InvalidOperationException("Only draft properties can be submitted.");

//         Status = PropertyStatus.PendingApproval;
//         MarkUpdated();
//     }

//     public void Activate()
//     {
//         if (Status != PropertyStatus.PendingApproval)
//             throw new InvalidOperationException("Property must be pending approval.");

//         Status = PropertyStatus.Active;
//         ApprovedAt = DateTime.UtcNow;

//         RejectionReason = null;
//         ReviewedBy = null;
//         ReviewedAt = null;

//         MarkUpdated();
//     }
//     public void MarkSoldOut()
//     {
//         if (Status != PropertyStatus.Active)
//             throw new InvalidOperationException("Only active properties can be sold out.");

//         Status = PropertyStatus.SoldOut;
//         MarkUpdated();
//     }
//     public void Reject(Guid adminUserId, string reason)
//     {
//         if (Status != PropertyStatus.PendingApproval)
//             throw new InvalidOperationException("Only pending properties can be rejected.");

//         if (string.IsNullOrWhiteSpace(reason))
//             throw new InvalidOperationException("Rejection reason is required.");

//         Status = PropertyStatus.Rejected;
//         ReviewedBy = adminUserId;
//         ReviewedAt = DateTime.UtcNow;
//         RejectionReason = reason;

//         MarkUpdated();
//     }
//     public void ModifyRequest(Guid adminUserId, string reason)
//     {
//         if (Status != PropertyStatus.PendingApproval)
//             throw new InvalidOperationException(
//                 "Only pending approval properties can be marked as modification required.");

//         if (string.IsNullOrWhiteSpace(reason))
//             throw new InvalidOperationException("Modification reason is required.");

//         Status = PropertyStatus.ModificationRequired;
//         ReviewedBy = adminUserId;
//         ReviewedAt = DateTime.UtcNow;
//         RejectionReason = reason;

//         MarkUpdated();
//     }
//     // ---------------------------
//     // Metadata Update (Admin Approved)
//     // ---------------------------
//     public void ApplyApprovedUpdate(
//         string name,
//         string description,
//         string location,
//         string propertyType,
//         string? imageUrl)
//     {
//         if (Status != PropertyStatus.Active &&
//             Status != PropertyStatus.SoldOut)
//             throw new InvalidOperationException(
//                 "Only active or sold properties can be updated.");

//         Name = name;
//         Description = description;
//         Location = location;
//         PropertyType = propertyType;
//         ImageUrl = imageUrl;

//         MarkUpdated();
//     }
//     // public void MarkUpdatePending()
//     // {
//     //     if (Status != PropertyStatus.Active &&
//     //         Status != PropertyStatus.SoldOut)
//     //         throw new InvalidOperationException(
//     //             "Only active or sold properties can request update.");

//     //     Status = PropertyStatus.Updatepending;

//     //     ReviewedBy = null;
//     //     ReviewedAt = null;
//     //     RejectionReason = null;

//     //     MarkUpdated();
//     // }



//     public void UpdateBeforeActivation(
//       string name,
//       string description,
//       string location,
//       string propertyType,
//       string? imageUrl,
//       decimal initialValuation,
//       int totalUnits,
//       decimal annualYieldPercent,
//       decimal rentalIncomeHistory)
//     {
//         if (Status == PropertyStatus.Active ||
//             Status == PropertyStatus.SoldOut ||
//             Status == PropertyStatus.Rejected)
//             throw new InvalidOperationException(
//                 "This property can no longer be fully edited.");

//         Name = name;
//         Description = description;
//         Location = location;
//         PropertyType = propertyType;
//         ImageUrl = imageUrl;
//         InitialValuation = initialValuation;
//         ApprovedValuation = initialValuation;
//         TotalUnits = totalUnits;
//         AnnualYieldPercent = annualYieldPercent;
//         RentalIncomeHistory = rentalIncomeHistory;

//         MarkUpdated();
//     }

//     public void Resubmit()
//     {
//         if (Status != PropertyStatus.PendingApproval &&
//             Status != PropertyStatus.ModificationRequired)
//             throw new InvalidOperationException(
//                 "Only pending or modification required properties can be resubmitted.");

//         Status = PropertyStatus.PendingApproval;

//         ReviewedBy = null;
//         ReviewedAt = null;
//         RejectionReason = null;

//         MarkUpdated();
//     }
//     public void ApplyApprovedMetadataUpdate(
//     string description,
//     string? imageUrl)
//     {
//         if (Status != PropertyStatus.Active)
//             throw new InvalidOperationException(
//                 "Only active properties can apply metadata updates.");

//         if (string.IsNullOrWhiteSpace(description))
//             throw new InvalidOperationException("Description is required.");

//         Description = description;
//         ImageUrl = imageUrl;

//         MarkUpdated();
//     }
//     public bool IsHiddenFromOwner { get; private set; }

//     public void HideFromOwner()
//     {
//         if (Status != PropertyStatus.SoldOut)
//             throw new InvalidOperationException(
//                 "Only sold out properties can be hidden.");

//         IsHiddenFromOwner = true;
//         MarkUpdated();
//     }

// }

using RealEstateInvesting.Domain.Common;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Domain.Entities;

public class Property : BaseEntity
{

    public Guid OwnerUserId { get; private set; }

    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string Location { get; private set; } = default!;
    public string PropertyType { get; private set; } = default!;
    public string? ImageUrl { get; private set; }


    public decimal InitialValuation { get; private set; }
    public decimal ApprovedValuation { get; private set; }
    public int TotalUnits { get; private set; }
    public decimal AnnualYieldPercent { get; private set; }
    public decimal RentalIncomeHistory { get; private set; }
    public Guid? OrganizationId { get; private set; }


    public decimal SquareFeet { get; private set; }
    public decimal SellingPercentage { get; private set; }
    public decimal SharePerSquareFeet { get; private set; }

    public PropertyStatus Status { get; private set; } = PropertyStatus.Draft;

    public DateTime? ApprovedAt { get; private set; }
    public Guid? ReviewedBy { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    public bool IsHiddenFromOwner { get; private set; }

    private Property() { }

    public static Property CreateDraft(
        Guid ownerUserId,
        string name,
        string description,
        string location,
        string propertyType,
        string? imageUrl,

        decimal initialValuation,
        decimal squareFeet,

        decimal sellingPercentage,
        decimal sharePerSquareFeet
    )
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException("Name is required.");

        if (initialValuation <= 0)
            throw new InvalidOperationException("Property value must be positive.");

        if (squareFeet <= 0)
            throw new InvalidOperationException("Square feet must be positive.");

        if (sellingPercentage < 0 || sellingPercentage > 100)
            throw new InvalidOperationException("Selling % must be between 0–100.");

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
            TotalUnits = 1,
            AnnualYieldPercent = 0,
            RentalIncomeHistory = 0,

            SquareFeet = squareFeet,

            SellingPercentage = sellingPercentage,
            SharePerSquareFeet = sharePerSquareFeet,

            Status = PropertyStatus.Draft
        };
    }
    public void Submit()
    {
        if (Status != PropertyStatus.Draft)
            throw new InvalidOperationException("Only draft properties can be submitted.");

        Status = PropertyStatus.PendingApproval;
        MarkUpdated();
    }
    public void MarkAdminApproved(Guid adminUserId)
    {
        if (Status != PropertyStatus.PendingApproval)
            throw new InvalidOperationException("Only pending properties can be approved.");

        Status = PropertyStatus.AdminApproved;
        ReviewedBy = adminUserId;
        ReviewedAt = DateTime.UtcNow;

        MarkUpdated();
    }
    public void AssignToOrganization(Guid organizationId)
    {
        if (Status != PropertyStatus.AdminApproved)
            throw new InvalidOperationException("Only admin approved properties can be assigned.");

        OrganizationId = organizationId; 
        Status = PropertyStatus.OrganizationAssigned;

        MarkUpdated();
    }

    public void Activate()
    {
        if (Status != PropertyStatus.OrganizationAssigned)
            throw new InvalidOperationException("Property must be assigned to organization.");

        Status = PropertyStatus.Active;
        ApprovedAt = DateTime.UtcNow;

        MarkUpdated();
    }


    public void Reject(Guid adminUserId, string reason)
    {
        if (Status != PropertyStatus.PendingApproval)
            throw new InvalidOperationException("Only pending properties can be rejected.");

        Status = PropertyStatus.Rejected;
        ReviewedBy = adminUserId;
        ReviewedAt = DateTime.UtcNow;
        RejectionReason = reason;

        MarkUpdated();
    }

    public void ModifyRequest(Guid adminUserId, string reason)
    {
        if (Status != PropertyStatus.PendingApproval)
            throw new InvalidOperationException("Only pending approval properties allowed.");

        Status = PropertyStatus.ModificationRequired;
        ReviewedBy = adminUserId;
        ReviewedAt = DateTime.UtcNow;
        RejectionReason = reason;

        MarkUpdated();
    }
    public void MarkSoldOut()
    {
        if (Status != PropertyStatus.Active)
            throw new InvalidOperationException("Only active properties can be sold out.");

        Status = PropertyStatus.SoldOut;
        MarkUpdated();
    }
    public void ApplyApprovedMetadataUpdate(
        string description,
        string? imageUrl)
    {
        if (Status != PropertyStatus.Active)
            throw new InvalidOperationException(
                "Only active properties can apply metadata updates.");

        if (string.IsNullOrWhiteSpace(description))
            throw new InvalidOperationException("Description is required.");

        Description = description;
        ImageUrl = imageUrl;

        MarkUpdated();
    }
    public void Resubmit()
    {
        if (Status != PropertyStatus.PendingApproval &&
            Status != PropertyStatus.ModificationRequired)
            throw new InvalidOperationException("Invalid state for resubmit.");

        Status = PropertyStatus.PendingApproval;
        ReviewedBy = null;
        ReviewedAt = null;
        RejectionReason = null;

        MarkUpdated();
    }
    public void FinalizeTokenization(
    int totalUnits,
    decimal rentalIncome,
    decimal annualYieldPercent)
    {
        if (Status != PropertyStatus.OrganizationAssigned)
            throw new InvalidOperationException("Property must be assigned to organization.");

        if (totalUnits <= 0)
            throw new InvalidOperationException("Total units must be greater than zero.");

        if (annualYieldPercent < 0 || annualYieldPercent > 50)
            throw new InvalidOperationException("Invalid yield percent.");

        TotalUnits = totalUnits;
        RentalIncomeHistory = rentalIncome;
        AnnualYieldPercent = annualYieldPercent;
       

        MarkUpdated();
    }

    public void HideFromOwner()
    {
        if (Status != PropertyStatus.SoldOut)
            throw new InvalidOperationException("Only sold out properties can be hidden.");

        IsHiddenFromOwner = true;
        MarkUpdated();
    }
}
