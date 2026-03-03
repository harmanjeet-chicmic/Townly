using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Townly.AI.DTOs;
using Townly.AI.Services.Interfaces;

namespace Townly.AI.Controllers;

[ApiController]
[Route("api/ai/portfolio-chat")]
[Authorize]
public class PortfolioChatController : ControllerBase
{
    private readonly IPortfolioAiService _aiService;

    public PortfolioChatController(IPortfolioAiService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost]
    public async Task<IActionResult> Ask([FromBody] AiPortfolioRequestDto request)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _aiService.HandleAsync(userId, request.Question);

        return Ok(result);
    }
}