public interface IPriceFeed
{
    Task<decimal> GetEthUsdPriceAsync();
}
