using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Properties;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Application.Properties.Dtos;

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

    [HttpPost("marketplace")]
    public async Task<IActionResult> GetMarketplace([FromBody] MarketplaceSearchRequest request)
    {
        Guid? currentUserId = null;

        if (User.Identity?.IsAuthenticated == true)
        {
            currentUserId = Guid.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
        var result = await _service.GetMarketplaceAsync(
           currentUserId, request.Page, request.PageSize, request.Search, request.PropertyType, request.Status);

        return Ok(result);
    }
    // [HttpPost("marketplace/cursor")]
    // public async Task<IActionResult> GetMarketplaceCursor([FromBody] MarketplaceSearchRequest request)
    // {
    //     var result = await _service.GetMarketplaceCursorAsync(
    //         request.PageSize, request.Cursor, request.Search, request.PropertyType,request.Status);

    //     return Ok(result);
    // }

    [HttpGet("{propertyId}")]
    public async Task<IActionResult> GetPropertyDetails(Guid propertyId)
    {
        Guid? currentUserId = null;

        if (User.Identity?.IsAuthenticated == true)
        {
            currentUserId = Guid.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
        var result = await _service.GetDetailsAsync(currentUserId, propertyId);
        return Ok(result);
    }
    // [HttpGet("featured")]
    // public async Task<IActionResult> GetFeatured()
    // {   
    //      Guid? currentUserId = null;

    //     if (User.Identity?.IsAuthenticated == true)
    //     {
    //         currentUserId = Guid.Parse(
    //             User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    //     }
    //     var result = await _service.GetFeaturedAsync(currentUserId);
    //     return Ok(result);
    // }


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
            userId, page, pageSize, status, search);

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
