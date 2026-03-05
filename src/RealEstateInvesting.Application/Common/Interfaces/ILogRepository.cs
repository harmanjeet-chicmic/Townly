using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface ILogRepository
{
    Task<int> CountAsync();

    Task AddAsync(Log log);

    Task<Log?> GetOldestAsync();

    Task DeleteAsync(Log log);

}
