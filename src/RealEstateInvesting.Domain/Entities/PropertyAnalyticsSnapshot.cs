using RealEstateInvesting.Domain.Common;

namespace RealEstateInvesting.Domain.Entities;

public class PropertyAnalyticsSnapshot : BaseEntity
{
    public Guid PropertyId { get; private set; }
    public DateTime SnapshotAt { get; private set; }
    public long SharesSold { get; private set; }
    public decimal TotalInvested { get; private set; }

    public decimal DemandScore { get; private set; }
    public decimal RiskScore { get; private set; }   
    public decimal PricePerShare { get; private set; }
    public decimal Valuation { get; private set; }

    private PropertyAnalyticsSnapshot() { }

    public static PropertyAnalyticsSnapshot Create(
        Guid propertyId,
        DateTime snapshotAt,
        long sharesSold,
        decimal totalInvested,
        decimal demandScore,
        decimal riskScore,
        decimal pricePerShare,
        decimal valuation)
    {
        return new PropertyAnalyticsSnapshot
        {
            PropertyId = propertyId,
            SnapshotAt = snapshotAt,
            SharesSold = sharesSold,
            TotalInvested = totalInvested,
            DemandScore = demandScore,
            RiskScore = riskScore,
            PricePerShare = pricePerShare,
            Valuation = valuation
        };
    }
}
