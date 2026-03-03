using System.Net.Http.Headers;
using System.Text.Json;

namespace Townly.AI.Infrastructure.HttpClients;

public class TownlyApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TownlyApiClient(HttpClient httpClient,
                           IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    private void AttachUserToken()
    {
        var token = _httpContextAccessor.HttpContext?
            .Request.Headers["Authorization"]
            .ToString();

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                AuthenticationHeaderValue.Parse(token);
        }
    }

    public async Task<string> GetPortfolioSummaryAsync()
    {
        AttachUserToken();

        var response = await _httpClient.GetAsync("/api/portfolio/me/overview");

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetInvestmentsAsync()
    {
        AttachUserToken();

        var response = await _httpClient.GetAsync("/api/investments/me");

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}