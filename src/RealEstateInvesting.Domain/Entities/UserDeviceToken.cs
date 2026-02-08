using RealEstateInvesting.Domain.Common;

namespace RealEstateInvesting.Domain.Entities;

public class UserDeviceToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string DeviceToken { get; private set; } = default!;
    public string Platform { get; private set; } = default!; // Android / iOS
    public bool IsActive { get; private set; }
    public DateTime LastUsedAt { get; private set; }

    private UserDeviceToken() { }

    public static UserDeviceToken Create(
        Guid userId,
        string deviceToken,
        string platform)
    {
        if (string.IsNullOrWhiteSpace(deviceToken))
            throw new InvalidOperationException("Device token is required.");

        if (string.IsNullOrWhiteSpace(platform))
            throw new InvalidOperationException("Platform is required.");

        return new UserDeviceToken
        {
            UserId = userId,
            DeviceToken = deviceToken,
            Platform = platform,
            IsActive = true,
            LastUsedAt = DateTime.UtcNow
        };
    }

    public void Refresh()
    {
        IsActive = true;
        LastUsedAt = DateTime.UtcNow;
        MarkUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }
}
