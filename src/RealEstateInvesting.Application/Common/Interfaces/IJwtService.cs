using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
     string GenerateAdminToken(AdminUser admin);
}
