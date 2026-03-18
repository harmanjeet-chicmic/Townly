using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.API.Contracts;
using RealEstateInvesting.Application.Admin.Kyc.DTOs;
using RealEstateInvesting.Application.Admin.Kyc.Interfaces;
using RealEstateInvesting.Application.Common.Interfaces;
using System.Security.Claims;

namespace RealEstateInvesting.Api.Controllers.Admin;

/// <summary>
/// Admin KYC: in-app pending/approve/reject and on-chain flows (Flow 1–3).
/// On-chain: check verified, update identity (AGENT_ROLE), set country (AGENT_ROLE); all writes stored in OnChainKycActions.
/// </summary>
[ApiController]
[Route("api/admin/kyc")]
[Authorize(Roles = "Admin")]
public class AdminKycController : ControllerBase
{
    private readonly IAdminKycService _service;
    private readonly IOnChainKycService _onChainKycService;

    public AdminKycController(IAdminKycService service, IOnChainKycService onChainKycService)
    {
        _service = service;
        _onChainKycService = onChainKycService;
    }

    /// <summary>Lists in-app KYC submissions that are pending review.</summary>
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var result = await _service.GetPendingAsync();
        return Ok(result);
    }

    /// <summary>Approves a KYC submission in-app (updates DB only; use on-chain/identity to register on chain).</summary>
    [HttpPost("{kycId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid kycId)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _service.ApproveAsync(kycId, adminId);
        return Ok();
    }

    /// <summary>Rejects a KYC submission in-app with a reason.</summary>
    [HttpPost("{kycId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid kycId, [FromBody] RejectKycRequest request)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _service.RejectAsync(kycId, adminId, request.Reason);
        return Ok();
    }

    /// <summary>
    /// Check if an address is KYC-verified on chain (T-REX Identity Registry).
    /// </summary>
    [HttpGet("on-chain/verified/{address}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOnChainVerified(string address, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(address))
            return BadRequest(new { message = "Address is required." });
        var isVerified = await _onChainKycService.IsVerifiedAsync(address, cancellationToken);
        return Ok(new { address, isVerified });
    }

    /// <summary>
    /// Register/update identity on chain for a user (requires AGENT_ROLE wallet in TRex:PrivateKey).
    /// </summary>
    [HttpPost("on-chain/identity")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateIdentity([FromBody] UpdateIdentityRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserAddress) || string.IsNullOrWhiteSpace(request.IdentityContractAddress))
            return BadRequest(new { message = "UserAddress and IdentityContractAddress are required." });

        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var txHash = await _onChainKycService.UpdateIdentityOnChainAsync(request.UserAddress, request.IdentityContractAddress, adminId, cancellationToken);
        return Ok(new { transactionHash = txHash });
    }

    /// <summary>
    /// Set country code on chain for a user (optional geofencing). ISO 3166-1 numeric, e.g. 250 = France.
    /// </summary>
    [HttpPost("on-chain/country")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCountry([FromBody] UpdateCountryRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserAddress))
            return BadRequest(new { message = "UserAddress is required." });

        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var txHash = await _onChainKycService.UpdateCountryOnChainAsync(request.UserAddress, request.CountryCode, adminId, cancellationToken);
        return Ok(new { transactionHash = txHash });
    }

    /// <summary>
    /// Buy Property Step 1 (optional): register identity and country in one on-chain call when the Identity Registry supports registerIdentity(user, identity, country).
    /// </summary>
    [HttpPost("on-chain/register-identity")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterIdentity([FromBody] RegisterIdentityRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.UserAddress) || string.IsNullOrWhiteSpace(request.IdentityContractAddress))
            return BadRequest(new { message = "UserAddress and IdentityContractAddress are required." });

        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var txHash = await _onChainKycService.RegisterIdentityOnChainAsync(request.UserAddress, request.IdentityContractAddress, request.CountryCode, adminId, cancellationToken);
        return Ok(new { transactionHash = txHash });
    }
}
