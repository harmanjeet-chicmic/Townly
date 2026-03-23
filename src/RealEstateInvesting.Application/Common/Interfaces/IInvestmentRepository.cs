using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface IInvestmentRepository
{
    Task AddAsync(Investment investment);

    Task<long> GetTotalSharesInvestedAsync(Guid propertyId);

    Task<IEnumerable<Investment>> GetByUserIdAsync(Guid userId);
    Task<long> GetUserTokensOwnedAsync(Guid userId, Guid propertyId);
    Task<decimal> GetTotalAmountInvestedAsync(Guid propertyId);
    Task<IEnumerable<Investment>> GetAllUserInvestmentsAsync();
    Task<long> GetSharesInvestedInLastDaysAsync(
   Guid propertyId,
   int days);
    Task<(IEnumerable<Investment> Items, int TotalCount)>
GetByUserIdPagedAsync(
    Guid userId,
    int page,
    int pageSize,
    string? search,
    string? propertyType);


    Task<int> GetUniqueInvestorCountAsync(Guid propertyId);
    Task<int> GetTotalInvestorsCountAsync();
    Task<long> GetTotalTokensIssuedAsync();
    Task<Dictionary<Guid, long>>
GetSoldUnitsForPropertiesAsync(List<Guid> propertyIds);

    Task<DateTime?> GetLastInvestmentAtAsync(Guid propertyId);
    Task<long> GetSharesInvestedInLastHoursAsync(
     Guid propertyId,
     int hours);
    Task<decimal?> GetUserInvestmentAmountAsync(Guid userId, Guid propertyId);



}
