namespace RealEstateInvesting.Application.Tokens.Requests;

public record CreateTokenRequestCommand(Guid UserId, decimal Amount);
