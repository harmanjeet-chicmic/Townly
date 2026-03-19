// namespace RealEstateInvesting.Domain.Enums;

// public enum PropertyStatus
// {
//     Draft = 0,                 
//     PendingApproval = 1,
//     Active = 2,
//     SoldOut = 3,
//     Rejected = 4,
//     ModificationRequired = 5   
// }

public enum PropertyStatus
{   
    Draft = 0,  
    PendingApproval = 1,
    AdminApproved = 2,
    OrganizationAssigned = 3,
    Active = 4,
    SoldOut = 5,
    Rejected = 6,
    ModificationRequired = 7,
    PENDING_TREX= 8, // Job created, about to submit deployTREXSuite TX
    TREX_DEPLOYING= 9, // deployTREXSuite TX submitted, waiting for TREXSuiteDeployed event
    VAULT_DEPLOYING= 10, // T-REX deployed, deploying vault
    REGISTERING= 11, // Vault deployed, calling registerProperty
    KYC_VERIFYING= 12, // Registered, setting up identities in token IR
    MINTING= 13, // KYC done, minting tokens to vault + binding compliance
    FAILED= 14 // Any step failed; see error_message column
}

//Real Data from node blockchain service for T-REX deployment flow;
//kept separate from main PropertyStatus enum to avoid confusion in other parts
//of the system that don't interact with T-REX directly
public enum PropertyStatusTREX
{
    PENDING_TREX = 1, // Job created, about to submit deployTREXSuite TX
    TREX_DEPLOYING = 2, // deployTREXSuite TX submitted, waiting for TREXSuiteDeployed event
    VAULT_DEPLOYING = 3, // T-REX deployed, deploying vault
    REGISTERING = 4, // Vault deployed, calling registerProperty
    KYC_VERIFYING = 5, // Registered, setting up identities in token IR
    MINTING = 6, // KYC done, minting tokens to vault + binding compliance
    COMPLETED= 7, // All steps done, property is live
    FAILED = 8 // Any step failed; see error_message column
}