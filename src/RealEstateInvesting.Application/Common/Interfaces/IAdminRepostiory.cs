using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface IAdminRepository
{
    Task<AdminUser?> GetByEmailAsync(string email);
}
