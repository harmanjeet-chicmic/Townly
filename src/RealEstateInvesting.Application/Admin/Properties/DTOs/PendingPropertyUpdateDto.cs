namespace RealEstateInvesting.Application.Admin.Properties.DTOs;

public class PendingPropertyUpdateDto
{
    public Guid UpdateRequestId { get; set; }
    public Guid PropertyId { get; set; }
    public Guid RequestedByUserId { get; set; }

    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string PropertyType { get; set; } = default!;
    public string? ImageUrl { get; set; }

    public DateTime RequestedAt { get; set; }
}
