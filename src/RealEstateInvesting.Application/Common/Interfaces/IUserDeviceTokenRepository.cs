using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface IUserDeviceTokenRepository
{
    Task<UserDeviceToken?> GetByTokenAsync(string deviceToken);
    Task<List<UserDeviceToken>> GetActiveByUserIdAsync(Guid userId);
    Task AddAsync(UserDeviceToken token);
    Task SaveChangesAsync();
}
