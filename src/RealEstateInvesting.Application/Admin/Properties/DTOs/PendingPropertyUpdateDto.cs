namespace RealEstateInvesting.Application.Admin.Properties.DTOs;

public class PendingPropertyUpdateDto
{
    public Guid UpdateRequestId { get; set; }
    public Guid PropertyId { get; set; }
    public Guid RequestedByUserId { get; set; }

    public string Description { get; set; } = default!;
    public string? ImageUrl { get; set; }

    public DateTime RequestedAt { get; set; }
}