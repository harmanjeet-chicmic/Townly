namespace RealEstateInvesting.API.Contracts;

public class UpdateCountryRequest
{
    public string UserAddress { get; set; } = string.Empty;
    /// <summary>ISO 3166-1 numeric country code, e.g. 250 = France.</summary>
    public ushort CountryCode { get; set; }
}
