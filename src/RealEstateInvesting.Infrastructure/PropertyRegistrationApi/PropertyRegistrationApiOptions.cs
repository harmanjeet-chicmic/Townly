namespace RealEstateInvesting.Infrastructure.PropertyRegistrationApi;

/// <summary>
/// Configuration for the external T-REX property registration API.
/// Bind from appsettings "PropertyRegistrationApi" section.
/// </summary>
public class PropertyRegistrationApiOptions
{
    public const string SectionName = "PropertyRegistrationApi";

    /// <summary>
    /// Base URL of the property-register API (e.g. http://192.180.3.86:3008).
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Path for the property-register endpoint (e.g. v1/property-register). Appended to BaseUrl.
    /// </summary>
    public string PropertyRegisterPath { get; set; } = "v1/property-register";

    /// <summary>
    /// Request timeout in seconds. Default 60.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 60;
}
