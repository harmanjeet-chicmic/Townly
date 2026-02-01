using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Admin.Properties.DTOs;

public class AdminPropertyListDto
{
    public Guid PropertyId { get; set; }
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public PropertyStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
