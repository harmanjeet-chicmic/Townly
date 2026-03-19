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
        var response = await _httpClient.PostAsJsonAsync(_path ?? "https://rwa-blockchain-service.projectlabs.in/v1/property-register", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PropertyRegisterResponseDto>(cancellationToken)
            ?? throw new InvalidOperationException("Property registration API returned empty response.");

        return result;
    }
}
