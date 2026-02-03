using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Health.Handlers;
using Microsoft.AspNetCore.Authorization;

namespace RealEstateInvesting.Api.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly GetHealthStatusHandler _handler;

    public HealthController(GetHealthStatusHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _handler.HandleAsync(cancellationToken);

        if (result.Status != "Healthy")
            return StatusCode(StatusCodes.Status503ServiceUnavailable, result);

        return Ok(result);
    }
}
