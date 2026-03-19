using RealEstateInvesting.Domain.Common;

namespace RealEstateInvesting.Domain.Entities;

public class PropertyImage : BaseEntity
{
    public Guid PropertyId { get; private set; }
    public string FileName { get; private set; } = default!;
    public string ImageUrl { get; private set; } = default!;
    public DateTime UploadedAt { get; private set; }

    private PropertyImage() { }

    public static PropertyImage Create(
        Guid propertyId,
        string fileName,
        string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new InvalidOperationException("File name is required.");

        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new InvalidOperationException("Image URL is required.");

        return new PropertyImage
        {
            PropertyId = propertyId,
            FileName = fileName,
            ImageUrl = imageUrl,
            UploadedAt = DateTime.UtcNow
        };
    }
}
