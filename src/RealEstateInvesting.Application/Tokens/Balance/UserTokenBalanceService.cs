using RealEstateInvesting.Application.Common.Interfaces;

namespace RealEstateInvesting.Application.Tokens.Balance;

public class UserTokenBalanceService
{
    private readonly IUserTokenBalanceRepository _repository;

    public UserTokenBalanceService(IUserTokenBalanceRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserTokenBalanceDto> GetAsync(Guid userId)
    {
        var balance = await _repository.GetByUserIdAsync(userId);

        return new UserTokenBalanceDto
        {
            UserId = userId,
            TotalGranted = balance?.TotalGranted ?? 0,
            TotalUsed = balance?.TotalUsed ?? 0,
            Available = balance?.Available ?? 0
        };
    }
}
