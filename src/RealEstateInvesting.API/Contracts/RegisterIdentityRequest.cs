namespace RealEstateInvesting.API.Contracts;

/// <summary>
/// Request for Buy Property Step 1: registerIdentity(user, identity, country) in one on-chain call (when contract supports it).
/// </summary>
public class RegisterIdentityRequest
{
    public string UserAddress { get; set; } = string.Empty;
    public string IdentityContractAddress { get; set; } = string.Empty;
    /// <summary>ISO 3166-1 numeric country code, e.g. 250 = France.</summary>
    public ushort CountryCode { get; set; }
}
