using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Tokens.Requests;

namespace RealEstateInvesting.API.Controllers.Tokens;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/tokens/requests")]
public class AdminTokenRequestsController : ControllerBase
{
    private readonly ReviewTokenRequestHandler _handler;

    public AdminTokenRequestsController(ReviewTokenRequestHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("{requestId:guid}/review")]
    public async Task<IActionResult> Review(
        Guid requestId,
        [FromBody] ReviewTokenRequestCommand command)
    {
        await _handler.Handle(
            command with { RequestId = requestId });

        return Ok();
    }
}
