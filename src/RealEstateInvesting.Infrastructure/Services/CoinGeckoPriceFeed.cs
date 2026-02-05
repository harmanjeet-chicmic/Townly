using System.Net.Http.Json;
using RealEstateInvesting.Application.Common.Interfaces;

public class CoinGeckoPriceFeed : IPriceFeed
{
    private readonly HttpClient _httpClient;

    public CoinGeckoPriceFeed(HttpClient httpClient)
    {
        _httpClient = httpClient;

        // CoinGecko recommends setting a User-Agent
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("RealEstateInvestingApp/1.0");
    }

    public async Task<decimal> GetEthUsdPriceAsync()
    {
        Console.WriteLine("================== API HIT FOR ETH PRICE ======");

        try
        {
            var response = await _httpClient.GetFromJsonAsync<CoinGeckoResponse>(
                "https://api.coingecko.com/api/v3/simple/price?ids=ethereum&vs_currencies=usd");

            if (response == null)
                throw new Exception("Response is null");

            if (!response.TryGetValue("ethereum", out var eth))
                throw new Exception("Ethereum key not found in response");

            Console.WriteLine($"=========== CURRENT ETH PRICE (USD): {eth.usd} =======");

            return eth.usd;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR fetching ETH price: " + ex.Message);

            // ✅ FALLBACK RATE (SAFE DEFAULT)
            return 2000m; // 1 ETH = $2000
        }
    }

    // Matches:
    // {
    //   "ethereum": { "usd": 3125.45 }
    // }
    private class CoinGeckoResponse : Dictionary<string, EthPrice>
    {
    }

    private class EthPrice
    {
        public decimal usd { get; set; }
    }
}
