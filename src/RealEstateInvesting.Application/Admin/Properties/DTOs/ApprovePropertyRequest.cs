using RealEstateInvesting.Application.Properties.Dtos;

namespace RealEstateInvesting.Application.Admin.Properties.DTOs;

public class ApprovePropertyRequest
{
    public List<PropertyDocumentDto>? Documents { get; set; }
}
