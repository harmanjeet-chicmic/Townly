using RealEstateInvesting.Domain.Common;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Domain.Entities;

public class Notification : BaseEntity
{
    
    public Guid UserId { get; private set; }
    public NotificationType Type { get; private set; }

    public string Title { get; private set; } = default!;
    public string Message { get; private set; } = default!;

    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    public Guid? ReferenceId { get; private set; }

    
    private Notification() { }

    public static Notification Create(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        Guid? referenceId = null)
    {
        return new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            ReferenceId = referenceId,
            IsRead = false
        };
    }

    public void MarkAsRead()
    {
        if (IsRead)
            return;

        IsRead = true;
        ReadAt = DateTime.UtcNow;

        MarkUpdated();
    }
}
