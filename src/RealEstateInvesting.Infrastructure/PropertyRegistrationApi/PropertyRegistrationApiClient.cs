using Microsoft.Extensions.Options;
using RealEstateInvesting.Application.Properties.PropertyRegistrationApi;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace RealEstateInvesting.Infrastructure.PropertyRegistrationApi;

/// <summary>
/// HTTP client for the external T-REX property registration API (POST /v1/property-register).
/// </summary>
public class PropertyRegistrationApiClient : IPropertyRegistrationApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _path;

    public PropertyRegistrationApiClient(HttpClient httpClient, IOptions<PropertyRegistrationApiOptions> options)
    {
        _httpClient = httpClient;
        var opts = options.Value;
        if (!string.IsNullOrWhiteSpace(opts.BaseUrl))
            _httpClient.BaseAddress = new Uri(opts.BaseUrl.TrimEnd('/'));
        _path = !string.IsNullOrWhiteSpace(opts.PropertyRegisterPath)
            ? opts.PropertyRegisterPath.TrimStart('/')
            : "v1/property-register";

        _httpClient.Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds > 0 ? opts.TimeoutSeconds : 60);
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <inheritdoc />
    public async Task<PropertyRegisterResponseDto> RegisterPropertyAsync(PropertyRegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(_path ?? "https://1a86-112-196-113-3.ngrok-free.app/v1/property-register", request, cancellationToken);

        // Try to deserialize the response body even for non-2xx responses.
        PropertyRegisterResponseDto? result = null;
        try
        {
            result = await response.Content.ReadFromJsonAsync<PropertyRegisterResponseDto>(cancellationToken);
        }
        catch
        {
            // Ignore deserialization issues and fallback to a synthetic error response below.
        }

        if (!response.IsSuccessStatusCode)
        {
            var statusCode = (int)response.StatusCode;
            var reason = string.IsNullOrWhiteSpace(response.ReasonPhrase)
                ? "Property registration API request failed."
                : response.ReasonPhrase;

            if (result is null)
            {
                return new PropertyRegisterResponseDto
                {
                    StatusCode = statusCode,
                    Status = false,
                    Message = reason,
                    Type = "FAILED",
                    Data = null
                };
            }

            // Preserve any error details from the body, but force Status=false for non-2xx.
            result.StatusCode = statusCode;
            result.Status = false;
            if (string.IsNullOrWhiteSpace(result.Message))
                result.Message = reason;

            return result;
        }

        // Successful HTTP response; body must exist.
        return result
               ?? throw new InvalidOperationException("Property registration API returned empty response.");
    }

    /// <inheritdoc />
    public async Task<PropertyRegisterJobStatusResponseDto?> GetJobStatusAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var statusPath = _path?.TrimEnd('/') + "/status";
        var url = string.IsNullOrEmpty(statusPath) ? $"v1/property-register/status?jobId={jobId}" : $"{statusPath}?jobId={jobId}";
        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;
            var result = await response.Content.ReadFromJsonAsync<PropertyRegisterJobStatusResponseDto>(cancellationToken);
            return result;
        }
        catch
        {
            return null;
        }
    }
}
