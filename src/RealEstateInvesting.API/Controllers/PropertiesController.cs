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
[Route("api/properties")]
public class PropertiesController : ControllerBase
{
    private readonly PropertyService _propertyService;
    private readonly IWebHostEnvironment _env;
    private readonly IFileStorage _fileStorage;

    public PropertiesController(
        PropertyService propertyService,
        IWebHostEnvironment env,
         IFileStorage fileStorage)
    {
        _propertyService = propertyService;
        _env = env;
        _fileStorage = fileStorage;
    }

    // ----------------------------------------
    // Create Property (multipart/form-data)
    // ----------------------------------------
    [HttpPost]
    [Authorize]
    [EnableRateLimiting("PropertyCreationPolicy")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateProperty(
        [FromForm] CreatePropertyMultipartDto request)
    {   
        var userId = GetUserId();
         var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
         Console.WriteLine("=================PROPERTY CONTROLLER HITTED");
        Console.WriteLine($"================================Authorization Header: {authHeader}");

        // 1️⃣ Save image
        // string? imageUrl = null;
        // if (request.Image != null)
        // {
        //     imageUrl = await SaveFileAsync(
        //         request.Image,
        //         "uploads/properties/images");
        // }
        string? imageUrl = null;
        if (request.Image != null)
        {
            imageUrl = await _fileStorage.SaveAsync(
                request.Image.OpenReadStream(),
                request.Image.ContentType,
                request.Image.FileName,
                "properties/images",
                HttpContext.RequestAborted);
        }


        // 2️⃣ Save documents
        var documentDtos = new List<PropertyDocumentDto>();

        // foreach (var doc in request.Documents)
        // {
        //     var docUrl = await SaveFileAsync(
        //         doc,
        //         "uploads/properties/documents");

        //     documentDtos.Add(new PropertyDocumentDto
        //     {
        //         DocumentName = doc.FileName,
        //         DocumentUrl = docUrl
        //     });
        // }
        foreach (var doc in request.Documents)
        {
            var docUrl = await _fileStorage.SaveAsync(
                doc.OpenReadStream(),
                doc.ContentType,
                doc.FileName,
                "properties/documents",
                HttpContext.RequestAborted);

            documentDtos.Add(new PropertyDocumentDto
            {
                DocumentName = doc.FileName,
                DocumentUrl = docUrl
            });
        }


        // 3️⃣ Map API DTO → Application DTO
        var command = new CreatePropertyCommand
        {
            Name = request.Name,
            Description = request.Description,
            Location = request.Location,
            PropertyType = request.PropertyType,
            InitialValuation = request.InitialValuation,
            TotalUnits = request.TotalUnits,
            AnnualYieldPercent = request.AnnualYieldPercent,
            ImageUrl = imageUrl,
            Documents = documentDtos
        };

        var propertyId = await _propertyService.CreatePropertyAsync(
            userId,
            command);

        return Ok(new
        {
            PropertyId = propertyId,
            Message = "Property submitted for approval."
        });
    }

    // ----------------------------------------
    // Helpers
    // ----------------------------------------
    // private async Task<string> SaveFileAsync(
    //     IFormFile file,
    //     string relativeFolder)
    // {
    //     var rootPath = Path.Combine(_env.WebRootPath, relativeFolder);
    //     Directory.CreateDirectory(rootPath);

    //     var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
    //     var fullPath = Path.Combine(rootPath, fileName);

    //     using var stream = new FileStream(fullPath, FileMode.Create);
    //     await file.CopyToAsync(stream);

    //     return $"/{relativeFolder}/{fileName}";
    // }

    private Guid GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(claim))
            throw new UnauthorizedAccessException("Invalid token.");

        return Guid.Parse(claim);
    }
}
