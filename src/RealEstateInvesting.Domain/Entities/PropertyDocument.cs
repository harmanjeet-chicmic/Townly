using RealEstateInvesting.Domain.Common;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Domain.Entities;

public class PropertyDocument : BaseEntity
{
    public Guid PropertyId { get; private set; }

    // 🔥 Business display name (Registry, NOC, etc.)
    public string Title { get; private set; } = default!;

    // 🔥 Actual stored file name
    public string FileName { get; private set; } = default!;

    public string DocumentUrl { get; private set; } = default!;
    public PropertyDocumentType Type { get; private set; }
    public DateTime UploadedAt { get; private set; }

    private PropertyDocument() { }

    public static PropertyDocument Create(
        Guid propertyId,
        string title,
        string fileName,
        string documentUrl,
        PropertyDocumentType type = PropertyDocumentType.Property)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new InvalidOperationException("Document title is required.");

        if (string.IsNullOrWhiteSpace(fileName))
            throw new InvalidOperationException("File name is required.");

        if (string.IsNullOrWhiteSpace(documentUrl))
            throw new InvalidOperationException("Document URL is required.");

        return new PropertyDocument
        {
            PropertyId = propertyId,
            Title = title,
            FileName = fileName,
            DocumentUrl = documentUrl,
            Type = type,
            UploadedAt = DateTime.UtcNow
        };
    }
}