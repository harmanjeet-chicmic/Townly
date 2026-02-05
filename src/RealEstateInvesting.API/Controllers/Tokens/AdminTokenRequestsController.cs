using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Tokens.Requests;
using RealEstateInvesting.Application.Common.Interfaces;

namespace RealEstateInvesting.API.Controllers.Tokens;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/tokens/requests")]
public class AdminTokenRequestsController : ControllerBase
{
    private readonly ReviewTokenRequestHandler _handler;
    private readonly ITokenRequestRepository _requestRepository;

    public AdminTokenRequestsController(ReviewTokenRequestHandler handler , 
        ITokenRequestRepository requestRepository)
    {
        _handler = handler;
        _requestRepository  = requestRepository;
    }
    [HttpGet]
    public async Task<IActionResult> GetPending()
    {
        var requests = await _requestRepository.GetPendingAsync();

        var result = requests.Select(x => new AdminTokenRequestListDto
        {
            RequestId = x.Id,
            UserId = x.UserId,
            RequestedAmount = x.RequestedAmount,
            Status = x.Status,
            CreatedAt = x.CreatedAt
        });

        return Ok(result);
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
