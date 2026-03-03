
namespace RealEstateInvesting.API.Dtos.Properties;

public class PropertyDocumentUploadDto
{
    public string Title { get; set; } = default!;
    public IFormFile File { get; set; } = default!;
}