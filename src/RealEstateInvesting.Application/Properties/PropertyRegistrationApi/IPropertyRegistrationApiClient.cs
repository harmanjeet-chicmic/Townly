namespace RealEstateInvesting.Application.Properties.PropertyRegistrationApi;

/// <summary>
/// Client for the external T-REX property registration API (POST /v1/property-register).
/// </summary>
public interface IPropertyRegistrationApiClient
{
    /// <summary>
    /// Initiates on-chain property registration. Remaining steps run asynchronously after T-REX deployment TX is confirmed.
    /// </summary>
    Task<PropertyRegisterResponseDto> RegisterPropertyAsync(
        PropertyRegisterRequestDto request,
        CancellationToken cancellationToken = default);
}
