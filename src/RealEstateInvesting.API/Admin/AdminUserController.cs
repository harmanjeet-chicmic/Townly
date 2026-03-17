using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.API.Contracts;
using RealEstateInvesting.Application.Admin.Users.DTOs;
using RealEstateInvesting.Application.Admin.Users.Interfaces;
using RealEstateInvesting.Application.Common.DTOs;

namespace RealEstateInvesting.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUserController : ControllerBase
{
    private readonly IAdminUserService _service;

    public AdminUserController(IAdminUserService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all users with portfolio data (Admin)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] AdminUserQuery query)
    {
        query ??= new AdminUserQuery();

        var result = await _service.GetAllAsync(query);

        return Ok(
            ApiResponse<PaginatedResponse<AdminUserPortfolioDto>>
                .Success(result, "Users fetched successfully")
        );
    }
}