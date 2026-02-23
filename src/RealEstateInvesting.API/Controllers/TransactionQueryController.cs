using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Transactions;
using System.Security.Claims;
using RealEstateInvesting.Domain.Enums;
namespace RealEstateInvesting.Api.Controllers;

[ApiController]
[Route("api/transactions")]
[Authorize]
public class TransactionQueryController : ControllerBase
{
    private readonly TransactionQueryService _service;
   private readonly ILogger<TransactionQueryController> _logger;
    public TransactionQueryController(TransactionQueryService service , ILogger<TransactionQueryController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("me")]
public async Task<IActionResult> GetMyTransactions(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5,
    [FromQuery] TransactionType? type = null)
{   

    Console.WriteLine("==========================================  transaction api ===========================");
    var userId = Guid.Parse(
        User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // 🔥 LOG USER REQUEST
    _logger.LogInformation(
        "User {UserId} requested transactions. Page: {Page}, PageSize: {PageSize}, Type: {Type}",
        userId,
        page,
        pageSize,
        type?.ToString() ?? "All");

    var result = await _service.GetMyTransactionsAsync(
        userId, page, pageSize, type);
    Console.WriteLine("==========================transaction api ======================================");

    return Ok(result);
}
}
