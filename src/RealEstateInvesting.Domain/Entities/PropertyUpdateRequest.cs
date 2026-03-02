using RealEstateInvesting.Domain.Common;

namespace RealEstateInvesting.Domain.Entities;

public enum PropertyUpdateStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}

public class PropertyUpdateRequest : BaseEntity
{
    public Guid PropertyId { get; private set; }
    public Guid RequestedByUserId { get; private set; }

    public string Description { get; private set; } = default!;
    public string? ImageUrl { get; private set; }

    public PropertyUpdateStatus Status { get; private set; } = PropertyUpdateStatus.Pending;
    public DateTime RequestedAt { get; private set; }
    public DateTime? ReviewedAt { get; private set; }

    private PropertyUpdateRequest() { }

    public static PropertyUpdateRequest Create(
        Guid propertyId,
        Guid userId,
        string description,
        string? imageUrl)
    {
        return new PropertyUpdateRequest
        {
            PropertyId = propertyId,
            RequestedByUserId = userId,
            Description = description,
            ImageUrl = imageUrl,
            RequestedAt = DateTime.UtcNow,
            Status = PropertyUpdateStatus.Pending
        };
    }

    public void Approve()
    {
        Status = PropertyUpdateStatus.Approved;
        ReviewedAt = DateTime.UtcNow;
        MarkUpdated();
    }

    public void Reject()
    {
        Status = PropertyUpdateStatus.Rejected;
        ReviewedAt = DateTime.UtcNow;
        MarkUpdated();
    }
}