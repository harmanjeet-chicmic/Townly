namespace RealEstateInvesting.Application.Common.Errors;

public static class ErrorMessages
{
    public const string UserNotFound = "User not found.";
    public const string KycRequired = "KYC approval is required to invest.";
    public const string UserBlocked = "Your account is blocked.";
    public const string PropertyNotFound = "Property not found.";
    public const string OwnPropertyInvestment = "You cannot invest in your own property.";
    public const string PropertyNotActive = "This property is not open for investment.";
    public const string InsufficientShares = "Not enough shares are available.";
    public const string InsufficientTokens = "You do not have enough tokens.";
    public const string InvalidEthPrice = "Unable to fetch ETH price. Try again later.";

    public const string ExcessShares = "Cannot buy more than 10000 shares";
    public const string DobAgeLessThan18 = "DOB Must be greater than 18 for kyc";
    public const string DobGreaterThanCurrentDate = "DOB Should not be current or futurer date";
}
