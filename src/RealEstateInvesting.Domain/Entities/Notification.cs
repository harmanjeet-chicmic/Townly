using RealEstateInvesting.Domain.Common;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Domain.Entities;

public class Notification : BaseEntity
{
    // Recipient
    public Guid UserId { get; private set; }

    // Type of notification
    public NotificationType Type { get; private set; }

    // Content snapshot
    public string Title { get; private set; } = default!;
    public string Message { get; private set; } = default!;

    // Read state
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    // Optional reference (property, investment, etc.)
    public Guid? ReferenceId { get; private set; }

    // EF
    private Notification() { }

    // ---------------------------
    // Factory: Create Notification
    // ---------------------------
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

    // ---------------------------
    // Domain behavior
    // ---------------------------
    public void MarkAsRead()
    {
        if (IsRead)
            return;

        IsRead = true;
        ReadAt = DateTime.UtcNow;

        MarkUpdated();
    }
}
