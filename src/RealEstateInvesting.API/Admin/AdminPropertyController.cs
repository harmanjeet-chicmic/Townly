using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Admin.Properties.DTOs;
using RealEstateInvesting.Application.Admin.Properties.Interfaces;
using System.Security.Claims;
using RealEstateInvesting.Application.Admin.Properties.DTOs;

namespace RealEstateInvesting.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/properties")]
[Authorize(Roles = "Admin")]
public class AdminPropertyController : ControllerBase
{
    private readonly IAdminPropertyService _service;

    public AdminPropertyController(IAdminPropertyService service)
    {
        _service = service;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var result = await _service.GetPendingAsync();
        return Ok(result);
    }

    [HttpPost("{propertyId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid propertyId)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _service.ApproveAsync(propertyId, adminId);
        return Ok();
    }

    [HttpPost("{propertyId:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid propertyId,
        [FromBody] RejectPropertyRequest request)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _service.RejectAsync(propertyId, adminId, request.Reason);

        return Ok();
    }
    [HttpGet("update-requests/pending")]
    public async Task<IActionResult> GetPendingUpdateRequests()
    {
        var result = await _service.GetPendingUpdateRequestsAsync();
        return Ok(result);
    }
    [HttpPost("update-requests/{updateRequestId:guid}/approve")]
    public async Task<IActionResult> ApproveUpdateRequest(Guid updateRequestId)
    {
        var adminId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _service.ApproveUpdateRequestAsync(updateRequestId, adminId);

        return Ok();
    }

    [HttpPost("update-requests/{updateRequestId:guid}/reject")]
    public async Task<IActionResult> RejectUpdateRequest(
     Guid updateRequestId,
     [FromBody] RejectUpdateRequestDto request)
    {
        var adminId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _service.RejectUpdateRequestAsync(
            updateRequestId,
            adminId,
            request.Reason);

        return Ok();
    }


}
