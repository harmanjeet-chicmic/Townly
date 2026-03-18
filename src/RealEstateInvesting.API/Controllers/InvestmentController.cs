using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.API.Contracts;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Investments;
using RealEstateInvesting.Application.Investments.Dtos;
using System.Security.Claims;

namespace RealEstateInvesting.Api.Controllers;

/// <summary>Investments: in-app invest, Flow 5 on-chain buy shares, Flow 6 on-chain sell shares.</summary>
[ApiController]
[Route("api/investments")]
[Authorize]
public class InvestmentController : ControllerBase
{
    private readonly InvestmentService _investmentService;
    private readonly IBuySharesOnChainService _buySharesOnChainService;
    private readonly ISellSharesOnChainService _sellSharesOnChainService;
    private readonly ICurrentUser _currentUser;

    public InvestmentController(
        InvestmentService investmentService,
        IBuySharesOnChainService buySharesOnChainService,
        ISellSharesOnChainService sellSharesOnChainService,
        ICurrentUser currentUser)
    {
        _investmentService = investmentService;
        _buySharesOnChainService = buySharesOnChainService;
        _sellSharesOnChainService = sellSharesOnChainService;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<IActionResult> Invest([FromBody] CreateInvestmentDto dto)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var investmentId =
            await _investmentService.InvestAsync(userId, dto);

        return Ok(new
        {
            InvestmentId = investmentId,
            Message = "Investment successful."
        });
    }

    /// <summary>
    /// Flow 5: Buy shares on chain. Verifies current user wallet is KYC'd, approves stablecoin, calls marketplace.buyShares, records in DB.
    /// Requires TRex:PrivateKey wallet to hold stablecoin (executor).
    /// </summary>
    [HttpPost("on-chain/buy")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> BuySharesOnChain([FromBody] BuySharesOnChainRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.PropertyTokenAddress) || string.IsNullOrWhiteSpace(request.AmountOfSharesRaw))
            return BadRequest(new { message = "PropertyTokenAddress and AmountOfSharesRaw are required." });

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userWallet = _currentUser.WalletAddress;
        if (string.IsNullOrWhiteSpace(userWallet))
            return BadRequest(new { message = "Wallet address not found for current user." });

        var result = await _buySharesOnChainService.BuySharesOnChainAsync(
            userWallet,
            userId,
            request.PropertyTokenAddress,
            request.AmountOfSharesRaw,
            request.AmountStablecoinToApproveRaw ?? "0",
            cancellationToken);

        return Ok(new
        {
            approveTxHash = result.ApproveTxHash,
            buyTxHash = result.BuyTxHash,
            message = "Shares purchased on chain."
        });
    }

    /// <summary>
    /// Flow 6: Sell shares on chain. Approves property token for marketplace, calls sellShares, records in DB.
    /// Requires TRex:PrivateKey wallet to hold the property token (executor).
    /// </summary>
    [HttpPost("on-chain/sell")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SellSharesOnChain([FromBody] SellSharesOnChainRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.PropertyTokenAddress) || string.IsNullOrWhiteSpace(request.AmountOfSharesRaw))
            return BadRequest(new { message = "PropertyTokenAddress and AmountOfSharesRaw are required." });

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userWallet = _currentUser.WalletAddress ?? string.Empty;

        var result = await _sellSharesOnChainService.SellSharesOnChainAsync(
            userWallet,
            userId,
            request.PropertyTokenAddress,
            request.AmountOfSharesRaw,
            cancellationToken);

        return Ok(new
        {
            approveTxHash = result.ApproveTxHash,
            sellTxHash = result.SellTxHash,
            message = "Shares sold on chain."
        });
    }
}
