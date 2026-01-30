using Microsoft.AspNetCore.Http;

namespace RealEstateInvesting.Api.DTOs;

public class SubmitKycHttpRequest
{
    public string FullName { get; set; } = default!;
    public DateTime DateOfBirth { get; set; }
    public string FullAddress { get; set; } = default!;
    public string DocumentType { get; set; } = default!;

    public IFormFile DocumentFile { get; set; } = default!;
    public IFormFile SelfieFile { get; set; } = default!;
}
