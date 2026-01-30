using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Properties;
using RealEstateInvesting.Application.Properties.Dtos;
using System.Security.Claims;

namespace RealEstateInvesting.Api.Controllers;

[ApiController]
[Route("api/properties")]
public class PropertyUpdateController : ControllerBase
{
    private readonly PropertyUpdateService _service;

    public PropertyUpdateController(PropertyUpdateService service)
    {
        _service = service;
    }

    [HttpPost("{propertyId}/update-request")]
    [Authorize]
    public async Task<IActionResult> RequestUpdate(
        Guid propertyId,
        [FromBody] RequestPropertyUpdateDto dto)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var requestId = await _service.RequestUpdateAsync(
            userId,
            propertyId,
            dto);

        return Ok(new
        {
            UpdateRequestId = requestId,
            Message = "Update request submitted for admin approval."
        });
    }
}
