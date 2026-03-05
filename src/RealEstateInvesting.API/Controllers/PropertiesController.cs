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

        Console.WriteLine("============== PROPERTY CREATION REQUEST RECEIVED ==============");

        Console.WriteLine($"UserId: {userId}");
        Console.WriteLine($"Name: {request.Name}");
        Console.WriteLine($"Description: {request.Description}");
        Console.WriteLine($"Location: {request.Location}");
        Console.WriteLine($"PropertyType: {request.PropertyType}");
        Console.WriteLine($"InitialValuation: {request.InitialValuation}");
        Console.WriteLine($"TotalUnits: {request.TotalUnits}");
        Console.WriteLine($"AnnualYieldPercent: {request.AnnualYieldPercent}");

        Console.WriteLine($"Image Present: {request.Image != null}");
        Console.WriteLine($"Documents Count: {request.Documents?.Count}");

        // Log raw form keys (debugging frontend issues)
        foreach (var key in Request.Form.Keys)
        {
            Console.WriteLine($"FORM KEY RECEIVED: {key}");
        }

        Console.WriteLine("===============================================================");

        // ----------------------------------------
        // VALIDATION SECTION
        // ----------------------------------------

        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Property name is required." });

        if (string.IsNullOrWhiteSpace(request.Description))
            return BadRequest(new { message = "Property description is required." });

        if (string.IsNullOrWhiteSpace(request.Location))
            return BadRequest(new { message = "Property location is required." });

        if (string.IsNullOrWhiteSpace(request.PropertyType))
            return BadRequest(new { message = "Property type is required." });

        if (request.InitialValuation <= 0)
            return BadRequest(new { message = "Initial valuation must be greater than zero." });

        if (request.TotalUnits <= 0)
            return BadRequest(new { message = "Total units must be greater than zero." });

        if (request.AnnualYieldPercent <= 0)
            return BadRequest(new { message = "Annual yield percent must be greater than zero." });

        if (request.Image == null)
            return BadRequest(new { message = "Property image is required." });

        if (request.Documents == null || !request.Documents.Any())
            return BadRequest(new { message = "At least one property document is required." });

        foreach (var doc in request.Documents)
        {
            if (string.IsNullOrWhiteSpace(doc.Title))
                return BadRequest(new { message = "Document title is required." });

            if (doc.File == null || doc.File.Length == 0)
                return BadRequest(new { message = "Document file is required." });
        }

        // ----------------------------------------
        // SAVE IMAGE
        // ----------------------------------------

        string? imageUrl = await _fileStorage.SaveAsync(
            request.Image.OpenReadStream(),
            request.Image.ContentType,
            request.Image.FileName,
            "properties/images",
            HttpContext.RequestAborted);


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
                doc.File.OpenReadStream(),
                doc.File.ContentType,
                doc.File.FileName,
                "properties/documents",
                HttpContext.RequestAborted);

            documentDtos.Add(new PropertyDocumentDto
            {
                Title = doc.Title,
                FileName = doc.File.FileName,
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
            RentalIncomeHistory = request.RentalIncome,
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
    // [HttpPost("{propertyId:guid}/resubmit")]
    // [Authorize]
    // public async Task<IActionResult> Resubmit(Guid propertyId)
    // {
    //     var userId = Guid.Parse(
    //         User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    //     await _propertyService.ResubmitAsync(userId, propertyId);

    //     return Ok(new
    //     {
    //         Message = "Property resubmitted for approval."
    //     });
    // }
    [HttpPost("{propertyId:guid}/resubmit")]
    [Authorize]

    public async Task<IActionResult> Resubmit(
     Guid propertyId,
     [FromForm] CreatePropertyMultipartDto request)
    {
        var userId = GetUserId();

        // 🔹 Save image (if provided)
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

        // 🔹 Save documents
        var documentDtos = new List<PropertyDocumentDto>();

        foreach (var doc in request.Documents)
        {
            var docUrl = await _fileStorage.SaveAsync(
                doc.File.OpenReadStream(),
                doc.File.ContentType,
                doc.File.FileName,
                "properties/documents",
                HttpContext.RequestAborted);

            documentDtos.Add(new PropertyDocumentDto
            {
                Title = doc.Title,
                FileName = doc.File.FileName,
                DocumentUrl = docUrl
            });
        }

        // 🔹 Map to CreatePropertyCommand
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

        await _propertyService.ResubmitAsync(userId, propertyId, command);

        return Ok(new
        {
            Message = "Property updated and resubmitted for approval."
        });
    }
}

