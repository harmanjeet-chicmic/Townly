using Microsoft.Extensions.Caching.Memory;
using RealEstateInvesting.Application.Common.Interfaces;

namespace RealEstateInvesting.Infrastructure.Pricing;

public class CachedEthPriceService : IEthPriceService
{
    private readonly IEthPriceService _inner;
    private readonly IMemoryCache _cache;

    private const string CacheKey = "ETH_USD_PRICE";

    public CachedEthPriceService(
        IEthPriceService inner,
        IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<decimal> GetEthUsdPriceAsync(
        CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKey, out decimal cached))
            return cached;

        try
        {
            var price = await _inner.GetEthUsdPriceAsync(cancellationToken);

            _cache.Set(CacheKey, price, TimeSpan.FromSeconds(60));
            return price;
        }
        catch
        {
            if (_cache.TryGetValue(CacheKey, out cached))
                return cached;

            return 3000m; // absolute fallback
        }
    }
}
