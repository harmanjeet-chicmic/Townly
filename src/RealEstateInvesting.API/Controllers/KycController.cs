using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Api.DTOs;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Kyc;
using RealEstateInvesting.Infrastructure.Kyc;
using RealEstateInvesting.Application.Kyc.Handlers;
using RealEstateInvesting.Application.Kyc.Queries;
namespace RealEstateInvesting.Api.Controllers;

[ApiController]
[Route("api/kyc")]
[Authorize] // 🔒 Wallet-authenticated users only
public class KycController : ControllerBase
{
    private readonly SubmitKycHandler _submitKycHandler;
    private readonly IKycFileStorageService _fileStorageService;
    private readonly ICurrentUser _currentUser;
    private readonly GetMyKycStatusHandler _getMyKycStatusHandler;
   
    public KycController(
        SubmitKycHandler submitKycHandler,
        IKycFileStorageService fileStorageService,
        ICurrentUser currentUser,
        GetMyKycStatusHandler getMyKycStatusHandler)
    {
        _submitKycHandler = submitKycHandler;
        _fileStorageService = fileStorageService;
        _currentUser = currentUser;
        _getMyKycStatusHandler = getMyKycStatusHandler;
    }

    [HttpPost("submit")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> SubmitKyc(
        [FromForm] SubmitKycHttpRequest request,
        CancellationToken cancellationToken)
    {
        // Extra safety (handler also checks this)
        if (_currentUser.IsBlocked)
            return Forbid("Blocked users cannot submit KYC.");

        // 1️⃣ Save files via Infrastructure service
        var (documentUrl, selfieUrl) =
            await _fileStorageService.SaveKycFilesAsync(
                _currentUser.UserId,
                request.DocumentFile,
                request.SelfieFile,
                cancellationToken);

        // 2️⃣ Build application command (pure data)
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

}
