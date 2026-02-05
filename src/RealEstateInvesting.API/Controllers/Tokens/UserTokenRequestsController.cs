using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Tokens.Requests;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Tokens.Requests.Dtos.RequestTokensDto;
namespace RealEstateInvesting.API.Controllers.Tokens;

[ApiController]
[Authorize]
[Route("api/tokens/requests")]
public class UserTokenRequestsController : ControllerBase
{
    private readonly CreateTokenRequestHandler _handler;
    private readonly ICurrentUserService _currentUser;
    

    public UserTokenRequestsController(CreateTokenRequestHandler handler, ICurrentUserService currentUser)
    {
        _handler = handler;
        _currentUser = currentUser;
    }
    


    [HttpPost]
    public async Task<IActionResult> RequestTokens([FromBody] RequestTokensDto request)
    {
        var userId = _currentUser.UserId;

        var requestId = await _handler.Handle(
            new CreateTokenRequestCommand(userId, request.Amount));

        return Ok(new { RequestId = requestId });
    }
}
