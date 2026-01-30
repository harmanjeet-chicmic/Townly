namespace RealEstateInvesting.Application.Analytics.Dtos;

public class PropertyAnalyticsTrendDto
{
    public DateTime SnapshotAt { get; set; }
    public decimal DemandScore { get; set; }
    public decimal PricePerShare { get; set; }
    public decimal Valuation { get; set; }
}
