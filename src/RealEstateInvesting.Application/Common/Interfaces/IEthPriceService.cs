namespace RealEstateInvesting.Application.Common.Interfaces;

public interface IEthPriceService
{
    Task<decimal> GetEthUsdPriceAsync(
        CancellationToken cancellationToken = default);
}
