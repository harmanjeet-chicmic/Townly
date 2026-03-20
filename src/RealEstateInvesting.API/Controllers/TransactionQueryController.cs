// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using RealEstateInvesting.Application.Transactions;
// using System.Security.Claims;
// using RealEstateInvesting.Domain.Enums;
// using Org.BouncyCastle.Asn1.Iana;
// namespace RealEstateInvesting.Api.Controllers;

// [ApiController]
// [Route("api/transactions")]
// [Authorize]
// public class TransactionQueryController : ControllerBase
// {
//     private readonly TransactionQueryService _service;
//     private readonly ILogger<TransactionQueryController> _logger;
//     public TransactionQueryController(TransactionQueryService service, ILogger<TransactionQueryController> logger)
//     {
//         _service = service;
//         _logger = logger;
//     }

//     [HttpGet("me")]
//     public async Task<IActionResult> GetMyTransactions(
//     [FromQuery] int page = 1,
//     [FromQuery] int pageSize = 5,
//     [FromQuery] TransactionType? type = null)
//     {

//         Console.WriteLine("==========================================  transaction api ===========================");
//         var userId = Guid.Parse(
//             User.FindFirstValue(ClaimTypes.NameIdentifier)!);

//         // 🔥 LOG USER REQUEST
//         _logger.LogInformation(
//             "User {UserId} requested transactions. Page: {Page}, PageSize: {PageSize}, Type: {Type}",
//             userId,
//             page,
//             pageSize,
//             type?.ToString() ?? "All");

//         var result = await _service.GetMyTransactionsAsync(
//             userId, page, pageSize, type);
//         Console.WriteLine("==========================transaction api ======================================");

//         return Ok(result);
//     }
//     [HttpGet("me/{transactionId:guid}")]
//     public async Task<IActionResult> GetMyTransactionDetails(
//     Guid transactionId)
//     {
//         var userId = Guid.Parse(
//             User.FindFirstValue(ClaimTypes.NameIdentifier)!);

//         var result = await _service
//             .GetMyTransactionDetailsAsync(userId, transactionId);

//         if (result == null)
//             return NotFound("Transaction not found.");

//         return Ok(result);
//     }
// }


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Transactions;
using RealEstateInvesting.API.Contracts;
using System.Security.Claims;

namespace RealEstateInvesting.Api.Controllers;

[ApiController]
[Route("api/transactions")]
[Authorize]
public class TokenPurchaseController : ControllerBase
{
    private readonly TokenPurchaseQueryService _service;
    private readonly ILogger<TokenPurchaseController> _logger;

    public TokenPurchaseController(
        TokenPurchaseQueryService service,
        ILogger<TokenPurchaseController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyTransactions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        CancellationToken ct = default)
    {
        var walletAddress = User.FindFirst("walletAddress")?.Value;

        if (string.IsNullOrEmpty(walletAddress))
        {
            return Unauthorized(ApiResponse<string>.Failure(
                "Wallet not found in token",
                401));
        }

        _logger.LogInformation(
            "Wallet {Wallet} requested token purchases. Page: {Page}, PageSize: {PageSize}",
            walletAddress,
            page,
            pageSize);

        var result = await _service.GetMyTransactions(
            walletAddress, page, pageSize, ct);

        return Ok(ApiResponse<object>.Success(
            result,
            "Transactions fetched successfully"));
    }
}