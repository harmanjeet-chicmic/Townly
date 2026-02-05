using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface ITokenTransactionRepository
{
    Task AddAsync(TokenTransaction transaction);
    Task SaveChangesAsync();
}
