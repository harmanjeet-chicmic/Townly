using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Properties;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace RealEstateInvesting.Api.Controllers;

[ApiController]
[Route("api/properties")]
public class PropertyQueryController : ControllerBase
{
    private readonly PropertyQueryService _service;

    public PropertyQueryController(PropertyQueryService service)
    {
        _service = service;
    }

    [HttpGet("marketplace")]
    public async Task<IActionResult> GetMarketplace(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 9,
        [FromQuery] string? search = null,
        [FromQuery] string? propertyType = null)
    {
        var result = await _service.GetMarketplaceAsync(
            page, pageSize, search, propertyType);

        return Ok(result);
    }
    [HttpGet("marketplace/cursor")]
    public async Task<IActionResult> GetMarketplaceCursor(
    [FromQuery] int limit = 9,
    [FromQuery] string? cursor = null,
    [FromQuery] string? search = null,
    [FromQuery] string? propertyType = null)
    {
        var result = await _service.GetMarketplaceCursorAsync(
            limit, cursor, search, propertyType);

        return Ok(result);
    }

    [HttpGet("{propertyId}")]
    public async Task<IActionResult> GetPropertyDetails(Guid propertyId)
    {
        var result = await _service.GetDetailsAsync(propertyId);
        return Ok(result);
    }
    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured()
    {
        var result = await _service.GetFeaturedAsync();
        return Ok(result);
    }


    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMyProperties()
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _service.GetMyPropertiesAsync(userId);
        return Ok(result);
    }
    [HttpGet("{id:guid}/related")]
    public async Task<IActionResult> GetRelatedProperties(Guid id)
    {
        var result = await _service.GetRelatedPropertiesAsync(id);
        return Ok(result);
    }




}
