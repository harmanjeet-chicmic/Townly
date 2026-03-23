namespace RealEstateInvesting.Application.Properties.Dtos;

public class MyPropertyDetailsDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
    public string PropertyType { get; set; } = null!;
    public List<string> ImageUrls { get; set; } = new();

    public PropertyStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public string? ApprovedReason { get; set; }

    public decimal PropertySize { get; set; }
    public decimal ListedPercentage { get; set; }

    public decimal ApprovedValuation { get; set; }
    public decimal TotalValue { get; set; }
    public long TotalUnits { get; set; }
    public long AvailableUnits { get; set; }

    public decimal PricePerUnit { get; set; }
    public decimal? PricePerShare { get; set; }

    public decimal AnnualYieldPercent { get; set; }

    public decimal? RiskScore { get; set; }
    public decimal? DemandScore { get; set; }
     public decimal RentalIncomeHistory { get; set; }

    // 🔥 NEW
    public List<PropertyDocumentDto> Documents { get; set; } = new();
    public List<PropertyDocumentDto> AdminDocuments { get; set; } = new();
    public bool HasPendingUpdateRequest { get; set; }
    public bool CanEditFullProperty { get; set; }
    public bool CanResubmit { get; set; }
    public bool CanRequestUpdate { get; set; }
    public bool CanDelete { get; set; }
}