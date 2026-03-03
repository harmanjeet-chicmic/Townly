using Microsoft.AspNetCore.Http;

namespace RealEstateInvesting.API.Dtos.Properties;


public class RequestPropertyUpdateDto
{
    public string Description { get; set; } = default!;
    public IFormFile? Image { get; set; }
}