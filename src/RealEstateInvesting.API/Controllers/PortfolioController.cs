using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Portfolio;
using System.Security.Claims;

namespace RealEstateInvesting.Api.Controllers;

[ApiController]
[Route("api/portfolio")]
[Authorize]
public class PortfolioController : ControllerBase
{
    private readonly PortfolioQueryService _service;

    public PortfolioController(PortfolioQueryService service)
    {
        _service = service;
    }

    [HttpGet("me/overview")]
    public async Task<IActionResult> GetOverview()
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _service.GetOverviewAsync(userId);
        return Ok(result);
    }
     [HttpGet("me/properties")]
    public async Task<IActionResult> GetMyPortfolioProperties()
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result =
            await _service.GetMyPortfolioPropertiesAsync(userId);

        return Ok(result);
    }
}
