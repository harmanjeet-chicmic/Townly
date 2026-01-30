using RealEstateInvesting.Application.Common.Interfaces;
using System.Net.Http.Json;

namespace RealEstateInvesting.Infrastructure.Pricing;

public class CoinGeckoEthPriceService : IEthPriceService
{
    private readonly HttpClient _httpClient;

    public CoinGeckoEthPriceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal> GetEthUsdPriceAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://api.coingecko.com/api/v3/simple/price?ids=ethereum&vs_currencies=usd");

        request.Headers.UserAgent.ParseAdd("Townly/1.0");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json =
            await response.Content.ReadFromJsonAsync<
                Dictionary<string, Dictionary<string, decimal>>>(cancellationToken);

        return json!["ethereum"]["usd"];
    }
}
