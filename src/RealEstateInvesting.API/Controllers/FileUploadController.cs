using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.API.Dtos.Properties;
using RealEstateInvesting.Application.Properties;
using RealEstateInvesting.Application.Properties.Dtos;
using System.Security.Claims;
using RealEstateInvesting.Application.Common.Interfaces;
using Microsoft.AspNetCore.RateLimiting;
namespace RealEstateInvesting.Api.Controllers;

[ApiController]
public class FileUploadController : ControllerBase
{
   private readonly IFileStorage _fileStorage;
    public FileUploadController(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
    }
    [HttpPost("upload")]
    [Authorize]
    public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] string type = "document")
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var folder = type.ToLower() == "document" ? "properties/documents" : "properties/images";

        var url = await _fileStorage.SaveAsync(
            file.OpenReadStream(),
            file.ContentType,
            file.FileName,
            folder,
            HttpContext.RequestAborted);

        return Ok(new { Url = url, FileName = file.FileName });
    }

    [HttpPost("batch-upload")]
    [Authorize]
    public async Task<IActionResult> BatchUploadFiles(IEnumerable<IFormFile> files, [FromQuery] string type = "document")
    {
        if (files == null || !files.Any())
            return BadRequest("No files uploaded.");

        var folder = type.ToLower() == "document" ? "properties/documents" : "properties/images";
        var results = new List<object>();

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var url = await _fileStorage.SaveAsync(
                    file.OpenReadStream(),
                    file.ContentType,
                    file.FileName,
                    folder,
                    HttpContext.RequestAborted);

                results.Add(new { Url = url, FileName = file.FileName });
            }
        }

        return Ok(results);
    }
}