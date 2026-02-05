namespace RealEstateInvesting.Application.Tokens.Requests;

public record ReviewTokenRequestCommand(
    Guid RequestId,
    Guid AdminId,
    bool Approve,
    string? RejectionReason);
