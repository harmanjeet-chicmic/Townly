using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Api.DTOs;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Kyc;
using RealEstateInvesting.Application.Kyc.Handlers;
using RealEstateInvesting.Application.Kyc.Queries;
using RealEstateInvesting.Infrastructure.Kyc;

namespace RealEstateInvesting.Api.Controllers;

/// <summary>
/// User KYC: submit documents (in-app), get in-app status, and Flow 1: check if current user wallet is verified on chain.
/// </summary>
[ApiController]
[Route("api/kyc")]
[Authorize]
public class KycController : ControllerBase
{
    private readonly SubmitKycHandler _submitKycHandler;
    private readonly IKycFileStorageService _fileStorageService;
    private readonly ICurrentUser _currentUser;
    private readonly GetMyKycStatusHandler _getMyKycStatusHandler;
    private readonly IIdentityRegistryContractService _identityRegistry;

    public KycController(SubmitKycHandler submitKycHandler, IKycFileStorageService fileStorageService,
        ICurrentUser currentUser, GetMyKycStatusHandler getMyKycStatusHandler, IIdentityRegistryContractService identityRegistry)
    {
        _submitKycHandler = submitKycHandler;
        _fileStorageService = fileStorageService;
        _currentUser = currentUser;
        _getMyKycStatusHandler = getMyKycStatusHandler;
        _identityRegistry = identityRegistry;
    }

    [HttpPost("submit")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> SubmitKyc([FromForm] SubmitKycHttpRequest request, CancellationToken cancellationToken)
    {
        // Extra safety (handler also checks this)
        if (_currentUser.IsBlocked)
            return Forbid("Blocked users cannot submit KYC.");

        // Save files via Infrastructure service
        var (documentUrl, selfieUrl) =
            await _fileStorageService.SaveKycFilesAsync(_currentUser.UserId, request.DocumentFile,
                request.SelfieFile, cancellationToken);

        // Build application command (pure data)
        var command = new SubmitKycCommand
        {
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth,
            FullAddress = request.FullAddress,
            DocumentType = request.DocumentType,
            DocumentUrl = documentUrl,
            SelfieUrl = selfieUrl
        };

        // 3️⃣ Execute business logic
        await _submitKycHandler.HandleAsync(command, cancellationToken);

        return Ok(new
        {
            message = "KYC submitted successfully and is under review."
        });
    }

    [HttpGet("me/status")]
    public async Task<IActionResult> GetMyKycStatus(
    CancellationToken cancellationToken)
    {
        var result = await _getMyKycStatusHandler.HandleAsync(
            new GetMyKycStatusQuery(_currentUser.UserId),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>Flow 1: Check if the current user's wallet is KYC-verified on chain (T-REX Identity Registry).</summary>
    [HttpGet("on-chain/verified")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOnChainVerified(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_currentUser.WalletAddress))
            return BadRequest(new { message = "Wallet address not found for current user." });

        var isVerified = await _identityRegistry.IsVerified(_currentUser.WalletAddress, cancellationToken);
        return Ok(new { _currentUser.WalletAddress, isVerified });
    }
}
