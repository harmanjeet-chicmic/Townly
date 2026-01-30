using RealEstateInvesting.Domain.Common;

namespace RealEstateInvesting.Domain.Entities;

public class PropertyDocument : BaseEntity
{
    public Guid PropertyId { get; private set; }

    public string DocumentName { get; private set; } = default!;
    public string DocumentUrl { get; private set; } = default!;

    public DateTime UploadedAt { get; private set; }

    private PropertyDocument() { }

    public static PropertyDocument Create(
        Guid propertyId,
        string documentName,
        string documentUrl)
    {
        if (string.IsNullOrWhiteSpace(documentName))
            throw new InvalidOperationException("Document name is required.");

        if (string.IsNullOrWhiteSpace(documentUrl))
            throw new InvalidOperationException("Document URL is required.");

        return new PropertyDocument
        {
            PropertyId = propertyId,
            DocumentName = documentName,
            DocumentUrl = documentUrl,
            UploadedAt = DateTime.UtcNow
        };
    }
}
