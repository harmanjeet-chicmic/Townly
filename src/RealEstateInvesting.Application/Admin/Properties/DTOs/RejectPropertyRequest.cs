using RealEstateInvesting.Application.Properties.Dtos;

namespace RealEstateInvesting.Application.Admin.Properties.DTOs;

public class RejectPropertyRequest
{
    public string Reason { get; set; } = default!;
    public List<PropertyDocumentDto>? Documents { get; set; }
}
