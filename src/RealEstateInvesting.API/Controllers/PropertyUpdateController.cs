using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Properties;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.API.Dtos.Properties;
using System.Security.Claims;

namespace RealEstateInvesting.Api.Controllers;

[ApiController]
[Route("api/properties")]
public class PropertyUpdateController : ControllerBase
{
    private readonly PropertyUpdateService _service;
    private readonly IFileStorage _fileStorage;

    public PropertyUpdateController(PropertyUpdateService service , IFileStorage  fileStorage)
    {
        _service = service;
        _fileStorage = fileStorage;
    }

    [HttpPost("{propertyId}/update-request")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> RequestUpdate(
     Guid propertyId,
     [FromForm] RequestPropertyUpdateDto dto)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        string? imageUrl = null;

        if (dto.Image != null)
        {
            imageUrl = await _fileStorage.SaveAsync(
                dto.Image.OpenReadStream(),
                dto.Image.ContentType,
                dto.Image.FileName,
                "properties/images",
                HttpContext.RequestAborted);
        }

        var requestId = await _service.RequestUpdateAsync(
            userId,
            propertyId,
            dto.Description,
            imageUrl);

        return Ok(new
        {
            UpdateRequestId = requestId,
            Message = "Update request submitted for admin approval."
        });
    }
}
