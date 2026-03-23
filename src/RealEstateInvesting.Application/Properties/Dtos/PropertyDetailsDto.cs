using RealEstateInvesting.Domain.Enums;
namespace RealEstateInvesting.Application.Properties.Dtos;

public class PropertyDetailsDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string PropertyType { get; set; } = default!;
    public List<string> ImageUrls { get; set; } = new();
    public PropertyStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public string? ApprovedReason { get; set; }

    public decimal PropertySize { get; set; }
    public decimal ListedPercentage { get; set; }

    public long? TokensOwned { get; set; }
    public decimal? InvestedEth { get; set; }

   public string? RegistrationJobId { get; set; }
    public string? TokenAddress { get; set; }
    public decimal TotalValue { get; set; }
    public long TotalUnits { get; set; }
    public decimal PricePerUnit { get; set; }
    public decimal AnnualYieldPercent { get; set; }
    public long AvailableUnits { get; set; }
    public decimal? RiskScore { get; set; }
    public decimal? DemandScore { get; set; }
    public decimal? PricePerShare { get; set; }
    public decimal? UserInvestmentAmount { get; set; }
    public decimal? UserInvestedAmountEth { get; set; }

    /// <summary>Total units minted from PropertyRegistrationJob.MintAmount.</summary>
    public decimal? TotalUnitMint { get; set; }

}
