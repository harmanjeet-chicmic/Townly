namespace RealEstateInvesting.Domain.Enums;

public enum PropertyStatus
{
    Draft = 0,                 // keep (even if unused)
    PendingApproval = 1,
    Active = 2,
    SoldOut = 3,
    Rejected = 4,
    ModificationRequired = 5   // newly added
}