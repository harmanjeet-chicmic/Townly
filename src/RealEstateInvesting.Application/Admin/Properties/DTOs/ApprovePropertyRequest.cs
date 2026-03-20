using RealEstateInvesting.Application.Properties.Dtos;

namespace RealEstateInvesting.Application.Admin.Properties.DTOs;

public class ApprovePropertyRequest
{
    public string? Reason { get; set; }
    public List<PropertyDocumentDto>? Documents { get; set; }
}
