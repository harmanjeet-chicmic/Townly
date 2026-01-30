using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Properties.Dtos;

public class PropertyResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string Location { get; set; } = null!;
    public string PropertyType { get; set; } = null!;

    public decimal ApprovedValuation { get; set; }
    public int TotalUnits { get; set; }
    public decimal AnnualYieldPercent { get; set; }

    public PropertyStatus Status { get; set; }
}
