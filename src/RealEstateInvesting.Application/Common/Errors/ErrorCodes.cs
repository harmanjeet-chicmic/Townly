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
}
