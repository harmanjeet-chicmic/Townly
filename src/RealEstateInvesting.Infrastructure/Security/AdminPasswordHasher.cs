using Microsoft.AspNetCore.Identity;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.Security;

public class AdminPasswordHasher : IAdminPasswordHasher
{
    private readonly PasswordHasher<AdminUser> _hasher = new();

    public bool Verify(string hashedPassword, string providedPassword)
    {
        var result = _hasher.VerifyHashedPassword(
            null!,
            hashedPassword,
            providedPassword);

        return result != PasswordVerificationResult.Failed;
    }
}
