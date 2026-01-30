using RealEstateInvesting.Domain.Common;

namespace RealEstateInvesting.Domain.Entities;

public class Investment : BaseEntity
{
    // Ownership
    public Guid UserId { get; private set; }
    public Guid PropertyId { get; private set; }

    // Investment snapshot (USD-based)
    public int SharesPurchased { get; private set; }
    public decimal PricePerShareAtPurchase { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal EthUsdRateAtExecution { get; private set; }
    public decimal EthAmountAtExecution { get; private set; }

    private Investment() { }

    public static Investment Create(
    Guid userId,
    Guid propertyId,
    int sharesPurchased,
    decimal pricePerShareUsd,
    decimal ethUsdRate)
    {
        if (sharesPurchased <= 0)
            throw new InvalidOperationException("Shares purchased must be greater than zero.");

        if (pricePerShareUsd <= 0)
            throw new InvalidOperationException("Price per share must be greater than zero.");

        if (ethUsdRate <= 0)
            throw new InvalidOperationException("Invalid ETH rate.");

        var totalUsd = decimal.Round(sharesPurchased * pricePerShareUsd, 2);
        var ethAmount = decimal.Round(totalUsd / ethUsdRate, 8);

        return new Investment
        {
            UserId = userId,
            PropertyId = propertyId,
            SharesPurchased = sharesPurchased,
            PricePerShareAtPurchase = pricePerShareUsd,
            TotalAmount = totalUsd,
            EthUsdRateAtExecution = ethUsdRate,
            EthAmountAtExecution = ethAmount
        };
    }

}
