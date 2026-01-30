using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Analytics;
using System.Security.Claims;

namespace RealEstateInvesting.Api.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly AnalyticsQueryService _service;

    public AnalyticsController(AnalyticsQueryService service)
    {
        _service = service;
    }

    // ----------------------------------------
    // Property analytics (public)
    // ----------------------------------------
    [HttpGet("properties/{propertyId}/trend")]
    public async Task<IActionResult> GetPropertyTrend(
        Guid propertyId,
        [FromQuery] int hours = 7)
    {
        var result = await _service.GetPropertyTrendAsync(propertyId, hours);
        return Ok(result);
    }

    // ----------------------------------------
    // Portfolio analytics (authenticated user)
    // ----------------------------------------
    [HttpGet("portfolio/me/trend")]
    [Authorize]
    public async Task<IActionResult> GetMyPortfolioTrend(
        [FromQuery] int hours = 7)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _service.GetPortfolioTrendAsync(userId, hours);
        return Ok(result);
    }
    [HttpGet("portfolio/me/allocation")]
    [Authorize]
    public async Task<IActionResult> GetMyPortfolioAllocation()
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result =
            await _service.GetPortfolioAllocationAsync(userId);

        return Ok(result);
    }
    [HttpGet("portfolio/me/line")]
    [Authorize]
    public async Task<IActionResult> GetMyPortfolioLine(
    [FromQuery] int hours = 7)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result =
            await _service.GetPortfolioLineAsync(userId, hours);

        return Ok(result);
    }


}
