using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetByWalletAddressAsync(string walletAddress, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<List<User>> GetByIdsAsync(IEnumerable<Guid> userIds);
    Task<List<User>> GetAllWithWalletsAsync(CancellationToken ct = default);
}
