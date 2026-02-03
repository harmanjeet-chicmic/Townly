using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Kyc.Queries;

public sealed class GetMyKycStatusResult
{
    public KycStatus Status { get; init; }
    public int StatusCode => (int)Status;

    public DateTime? SubmittedAt { get; init; }
    public string? RejectionReason { get; init; }
}
