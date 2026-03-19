using System.Text.Json.Serialization;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Properties.Dtos;

public class PropertyDocumentDto
{
    public string Title { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public string DocumentUrl { get; set; } = default!;
    [JsonIgnore]
    public PropertyDocumentType Type { get; set; }
}