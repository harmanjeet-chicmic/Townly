using RealEstateInvesting.Application.Admin.Users.DTOs;
using RealEstateInvesting.Application.Common.DTOs;

namespace RealEstateInvesting.Application.Admin.Users.Interfaces;

public interface IAdminUserService
{
    Task<PaginatedResponse<AdminUserPortfolioDto>> GetAllAsync(AdminUserQuery query);
}