using RealEstateInvesting.Application.Admin.Users.DTOs;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Admin.Users.Interfaces;
public interface IAdminUserRepository
{
    Task<(List<User>, int)> GetAllAsync(AdminUserQuery query);
}