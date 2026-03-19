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

    /// <summary>
    /// Gets the current status of a property registration job. GET /v1/property-register/status?jobId={jobId}.
    /// </summary>
    /// <param name="jobId">The job ID returned when the registration was initiated.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The job status, or null if the job was not found or the API returned an error.</returns>
    Task<PropertyRegisterJobStatusResponseDto?> GetJobStatusAsync(
        Guid jobId,
        CancellationToken cancellationToken = default);
}
