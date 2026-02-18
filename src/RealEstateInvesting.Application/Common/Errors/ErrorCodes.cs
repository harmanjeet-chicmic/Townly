namespace RealEstateInvesting.Application.Common.Errors;

public static class ErrorCodes
{
    public const string UserNotFound = "USER_NOT_FOUND";
    public const string KycRequired = "KYC_REQUIRED";
    public const string UserBlocked = "USER_BLOCKED";
    public const string PropertyNotFound = "PROPERTY_NOT_FOUND";
    public const string OwnPropertyInvestment = "OWN_PROPERTY_INVESTMENT";
    public const string PropertyNotActive = "PROPERTY_NOT_ACTIVE";
    public const string InsufficientShares = "INSUFFICIENT_SHARES";
    public const string InsufficientTokens = "INSUFFICIENT_TOKENS";
    public const string InvalidEthPrice = "INVALID_ETH_PRICE";
    public const string ExcessShares = "EXCESS_SHARES";
    public const string DobAgeLessThan18 = "DOB_LESS_THAN_18";
    public const string DobGreaterThanCurrentDate = "DOB_GREATER_THAN_CURRENT_DATE";
}
