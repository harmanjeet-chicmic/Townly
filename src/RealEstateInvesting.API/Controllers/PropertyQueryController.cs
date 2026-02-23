using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Properties;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Domain.Enums;

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
        // var userId = Guid.Parse(
        //     User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        Guid? currentUserId = null;

        if (User.Identity?.IsAuthenticated == true)
        {
            currentUserId = Guid.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!); // your extension method
        }
        var result = await _service.GetMarketplaceAsync(
           currentUserId, page, pageSize, search, propertyType);

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
        Guid? currentUserId = null;

        if (User.Identity?.IsAuthenticated == true)
        {
            currentUserId = Guid.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!); // your extension method
        }
        var result =    await _service.GetDetailsAsync( currentUserId , propertyId);
        return Ok(result);
    }
    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured()
    {   
         Guid? currentUserId = null;

        if (User.Identity?.IsAuthenticated == true)
        {
            currentUserId = Guid.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!); // your extension method
        }
        var result = await _service.GetFeaturedAsync(currentUserId);
        return Ok(result);
    }


    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMyProperties(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 6,
    [FromQuery] PropertyStatus? status = null,
     [FromQuery] string? search = null)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _service.GetMyPropertiesAsync(
            userId, page, pageSize, status , search);

        return Ok(result);
    }

    [HttpGet("me/{propertyId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetMyPropertyDetails(Guid propertyId)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _service.GetMyPropertyDetailsAsync(userId, propertyId);

        return Ok(result);
    }

    [HttpGet("{id:guid}/related")]
    public async Task<IActionResult> GetRelatedProperties(Guid id)
    {
        var result = await _service.GetRelatedPropertiesAsync(id);
        return Ok(result);
    }
    [HttpDelete("{propertyId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteProperty(Guid propertyId)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _service.DeletePropertyAsync(userId, propertyId);

        return NoContent();
    }




}
