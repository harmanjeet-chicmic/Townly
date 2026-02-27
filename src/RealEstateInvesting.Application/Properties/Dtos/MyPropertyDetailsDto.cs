using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Application.Properties.Dtos;
public class MyPropertyDetailsDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
    public string PropertyType { get; set; } = null!;
    public string? ImageUrl { get; set; }

    public PropertyStatus Status { get; set; }
    public string? RejectionReason { get; set; }

    public decimal TotalValue { get; set; }
    public int TotalUnits { get; set; }
    public int AvailableUnits { get; set; }

    public decimal PricePerUnit { get; set; }
    public decimal PricePerUnitEth { get; set; }

    public decimal AnnualYieldPercent { get; set; }

    public decimal? RiskScore { get; set; }
    public decimal? DemandScore { get; set; }

    // 🔥 NEW
    public List<PropertyDocumentDto> Documents { get; set; } = new();
}