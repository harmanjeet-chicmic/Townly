using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface ITransactionRepository
{
    Task AddRangeAsync(IEnumerable<Transaction> transactions);

    Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId);
}
