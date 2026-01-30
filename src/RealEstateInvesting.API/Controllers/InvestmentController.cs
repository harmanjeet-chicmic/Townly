using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Investments;
using RealEstateInvesting.Application.Investments.Dtos;
using System.Security.Claims;

namespace RealEstateInvesting.Api.Controllers;

[ApiController]
[Route("api/investments")]
[Authorize]
public class InvestmentController : ControllerBase
{
    private readonly InvestmentService _investmentService;

    public InvestmentController(InvestmentService investmentService)
    {
        _investmentService = investmentService;
    }

    [HttpPost]
    public async Task<IActionResult> Invest([FromBody] CreateInvestmentDto dto)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var investmentId =
            await _investmentService.InvestAsync(userId, dto);

        return Ok(new
        {
            InvestmentId = investmentId,
            Message = "Investment successful."
        });
    }
}
