using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;
namespace RealEstateInvesting.Application.Common.Interfaces;

public interface ITransactionRepository
{
    Task AddRangeAsync(IEnumerable<Transaction> transactions);

    Task<(IEnumerable<Transaction> Items, int TotalCount)>
GetByUserIdPagedAsync(
    Guid userId,
    int page,
    int pageSize,
    TransactionType? type);

}
