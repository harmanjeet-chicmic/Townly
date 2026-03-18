namespace RealEstateInvesting.Domain.Enums;

public enum OnChainKycActionType
{
    IdentityUpdate = 0,
    CountryUpdate = 1,
    /// <summary>Identity + country in one on-chain call (registerIdentity).</summary>
    RegisterIdentity = 2
}
