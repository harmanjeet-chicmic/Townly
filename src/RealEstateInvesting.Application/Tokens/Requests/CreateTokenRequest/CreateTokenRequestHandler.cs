using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Tokens.Requests;

public class CreateTokenRequestHandler
{
    private readonly ITokenRequestRepository _repository;

    public CreateTokenRequestHandler(ITokenRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateTokenRequestCommand command)
    {
        var request = TokenRequest.Create(command.UserId, command.Amount);
        await _repository.AddAsync(request);
        await _repository.SaveChangesAsync();
        return request.Id;
    }
}
