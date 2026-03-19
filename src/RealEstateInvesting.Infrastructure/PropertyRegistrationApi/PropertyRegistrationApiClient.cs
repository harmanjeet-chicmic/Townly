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
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PropertyRegisterResponseDto>(cancellationToken)
            ?? throw new InvalidOperationException("Property registration API returned empty response.");

        return result;
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
