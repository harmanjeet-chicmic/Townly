using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.API.Contracts;
using RealEstateInvesting.Application.Admin.Properties.DTOs;
using RealEstateInvesting.Application.Admin.Properties.Interfaces;
using RealEstateInvesting.Application.Common.Interfaces;
using System.Security.Claims;

namespace RealEstateInvesting.Api.Controllers.Admin;

/// <summary>
/// Admin endpoints for property lifecycle (approve, reject, etc.). For full on-chain property creation use api/property-suite/create.
/// </summary>
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

    // [HttpGet("pending")]
    // public async Task<IActionResult> GetPending()
    // {
    //     var result = await _service.GetPendingAsync();
    //     return Ok(result);
    // }
    // [HttpGet("pending")]
    // public async Task<IActionResult> GetPending([FromQuery] AdminPropertyQuery query)
    // {
    //     var result = await _service.GetPendingAsync(query);
    //     return Ok(result);
    // }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] AdminPropertyQuery query)
    {
        var result = await _service.GetAllAsync(query);
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

    [HttpPost("{propertyId:guid}/modify")]
    public async Task<IActionResult> modify(Guid propertyId, [FromBody] RejectPropertyRequest request)
    {
        Console.WriteLine("=============MODIFY API HITTED============");
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _service.ModifyRequest(propertyId, adminId, request.Reason);
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
    [HttpPost("{propertyId:guid}/assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignProperty(
    Guid propertyId,
    [FromBody] AssignPropertyDto request)
    {
        await _service.AssignToOrganizationAsync(propertyId, request.OrganizationId);

        return Ok(new
        {
            Message = "Property assigned to organization successfully"
        });
    }

    [HttpGet("Details")]
    public async Task<IActionResult> GetDetails()
    {
        var result = await _service.GetStatsAsync();
        return Ok(result);
    }
}
