using RealEstateInvesting.Domain.Common;

namespace RealEstateInvesting.Domain.Entities;

public class AdminUser : BaseEntity
{
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private AdminUser() {}

    public static AdminUser Create(string email, string passwordHash)
    {
        return new AdminUser
        {
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            IsActive = true
        };
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }
}
