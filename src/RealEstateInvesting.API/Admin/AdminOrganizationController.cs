using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Infrastructure.Organizations;
using RealEstateInvesting.Application.Common.Dtos;
using RealEstateInvesting.Admin.Application.Organizations;
using System.Security.Claims;

using RealEstateInvesting.Application.Properties.Dtos;
namespace RealEstateInvesting.Api.Controllers;

[ApiController]
[Route("api/admin/organizations")]
[Authorize(Roles = "Admin")]
public class AdminOrganizationController : ControllerBase
{
    private readonly OrganizationQueryService _service;

    public AdminOrganizationController(OrganizationQueryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] OrganizationQuery query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(result);
    }

    [HttpGet("{organizationId}/properties")]
    public async Task<IActionResult> GetPropertiesByOrganization(
     Guid organizationId,
     [FromQuery] OrganizationQuery query)
    {
        var result = await _service.GetPropertiesByOrganizationAsync(organizationId, query);
        return Ok(result);
    }


    [HttpPost("{organizationId}/properties/{propertyId}/activate")]
    public async Task<IActionResult> ActivateProperty(Guid organizationId, Guid propertyId, [FromBody] ActivatePropertyDto dto)
    {
        var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("Admin ID not found"));
        var result = await _service.ActivatePropertyAsync(organizationId, propertyId, dto, adminId);
        return Ok(new
        {
            message = result.Message,
            statusCode = result.StatusCode,
            status = result.Status,
            type = result.Type,
            data = result.Data is null ? null : new
            {
                result.Data.JobId,
                result.Data.PropertyId,
                result.Data.Status
            }
        });
    }
}