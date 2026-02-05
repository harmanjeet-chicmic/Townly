using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Tokens.Balance;

namespace RealEstateInvesting.API.Controllers.Wallet;

[ApiController]
[Authorize]
[Route("api/tokens/balance")]
public class UserTokenBalanceController : ControllerBase
{   
    private readonly ICurrentUserService _currentUser;
    private readonly UserTokenBalanceService _service;

    public UserTokenBalanceController(UserTokenBalanceService service ,
                              ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetBalance()
    {
        var userId = _currentUser.UserId;
        return Ok(await _service.GetAsync(userId));
    }
}
