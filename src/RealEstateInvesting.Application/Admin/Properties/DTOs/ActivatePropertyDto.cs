namespace RealEstateInvesting.Application.Properties.Dtos;

public class ActivatePropertyDto
{
    public int TotalUnits { get; set; }
    public decimal RentalIncome { get; set; }
    public decimal AnnualYieldPercent { get; set; }
    public decimal RiskScore { get; set; }

    /// <summary>
    /// Ethereum wallet address of the property owner/issuer (required for on-chain property registration).
    /// </summary>
    public string OwnerAddress { get; set; } = default!;

    /// <summary>
    /// Optional IPFS or image URI for the property. If not set, the property's ImageUrl is used.
    /// </summary>
    public string? IpfsUri { get; set; }
}