using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface IUserTokenBalanceRepository
{
    Task<UserTokenBalance?> GetByUserIdAsync(Guid userId);
    Task AddAsync(UserTokenBalance balance);
    Task SaveChangesAsync();
}
