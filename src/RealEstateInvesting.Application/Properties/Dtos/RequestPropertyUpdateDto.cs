namespace RealEstateInvesting.Application.Properties.Dtos;

public class RequestPropertyUpdateDto
{
    public string Description { get; set; } = default!;
    public string? ImageUrl { get; set; }
}
