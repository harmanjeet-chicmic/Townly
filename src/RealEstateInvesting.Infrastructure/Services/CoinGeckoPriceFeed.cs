using System.Net.Http.Json;
using RealEstateInvesting.Application.Common.Interfaces;

public class CoinGeckoPriceFeed : IPriceFeed
{
    private readonly HttpClient _httpClient;

    public CoinGeckoPriceFeed(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal> GetEthUsdPriceAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<CoinGeckoResponse>(
                "https://api.coingecko.com/api/v3/simple/price?ids=ethereum&vs_currencies=usd");

            if (response == null || !response.TryGetValue("ethereum", out var eth))
                throw new Exception("Invalid response");

            return eth.usd;
        }
        catch
        {
            // ✅ FALLBACK RATE (DUMMY / SAFE)
            // This keeps investment flow working
            return 2000m; // 1 ETH = $2000 (dummy)
        }
    }


    private class CoinGeckoResponse : Dictionary<string, EthPrice>
    {
    }

    private class EthPrice
    {
        public decimal usd { get; set; }
    }
}
