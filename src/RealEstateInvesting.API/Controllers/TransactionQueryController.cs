using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Transactions;
using System.Security.Claims;

namespace RealEstateInvesting.Api.Controllers;

[ApiController]
[Route("api/transactions")]
[Authorize]
public class TransactionQueryController : ControllerBase
{
    private readonly TransactionQueryService _service;

    public TransactionQueryController(TransactionQueryService service)
    {
        _service = service;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyTransactions()
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _service.GetMyTransactionsAsync(userId);
        return Ok(result);
    }
}
