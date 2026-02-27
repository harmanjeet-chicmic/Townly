using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Tokens.Requests;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Tokens.Requests.Dtos.RequestTokensDto;
using System.Security.Claims;

namespace RealEstateInvesting.API.Controllers.Tokens;

[ApiController]
[Authorize]
[Route("api/tokens/requests")]
public class UserTokenRequestsController : ControllerBase
{
    private readonly CreateTokenRequestHandler _handler;
    private readonly ICurrentUserService _currentUser;
    private readonly ITokenRequestRepository _tokenRequestRepository;
    

    public UserTokenRequestsController(CreateTokenRequestHandler handler, ICurrentUserService currentUser , ITokenRequestRepository tokenRequestRepository)
    {
        _handler = handler;
        _currentUser = currentUser;
        _tokenRequestRepository = tokenRequestRepository;
    }
    


    [HttpPost]
    public async Task<IActionResult> RequestTokens([FromBody] RequestTokensDto request)
    {
        var userId = _currentUser.UserId;

        var requestId = await _handler.Handle(
            new CreateTokenRequestCommand(userId, request.Amount));

        return Ok(new { RequestId = requestId });
    }
    [HttpGet("me")]
    public async Task<IActionResult> GetRequestsByUser()
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _tokenRequestRepository.GetByUserAsync( userId);
        return Ok(result);
    }

    
}
