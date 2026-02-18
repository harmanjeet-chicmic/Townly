using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Investments;
using System.Security.Claims;

namespace RealEstateInvesting.Api.Controllers;

[ApiController]
[Route("api/investments")]
[Authorize]
public class InvestmentQueryController : ControllerBase
{
    private readonly InvestmentQueryService _service;

    public InvestmentQueryController(InvestmentQueryService service)
    {
        _service = service;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyInvestments(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _service.GetMyInvestmentsAsync(userId, page, pageSize);

        return Ok(result);
    }

    [HttpGet("me/summary")]
    public async Task<IActionResult> GetPortfolioSummary()
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _service.GetPortfolioSummaryAsync(userId);
        return Ok(result);
    }

}
