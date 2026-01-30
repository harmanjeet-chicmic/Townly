namespace RealEstateInvesting.Application.Kyc;

public class SubmitKycCommand
{
    public string FullName { get; init; } = default!;
    public DateTime DateOfBirth { get; init; }
    public string FullAddress { get; init; } = default!;
    public string DocumentType { get; init; } = default!;
    public string DocumentUrl { get; init; } = default!;
    public string SelfieUrl { get; init; } = default!;
}
