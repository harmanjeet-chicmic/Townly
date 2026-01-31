using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Admin.Kyc.DTOs;
using RealEstateInvesting.Application.Admin.Kyc.Interfaces;
using System.Security.Claims;

namespace RealEstateInvesting.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/kyc")]
[Authorize(Roles = "Admin")]
public class AdminKycController : ControllerBase
{
    private readonly IAdminKycService _service;

    public AdminKycController(IAdminKycService service)
    {
        _service = service;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var result = await _service.GetPendingAsync();
        return Ok(result);
    }

    [HttpPost("{kycId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid kycId)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _service.ApproveAsync(kycId, adminId);
        return Ok();
    }

    [HttpPost("{kycId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid kycId, [FromBody] RejectKycRequest request)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _service.RejectAsync(kycId, adminId, request.Reason);
        return Ok();
    }
}
