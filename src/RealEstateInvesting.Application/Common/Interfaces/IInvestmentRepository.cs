using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface IInvestmentRepository
{
    Task AddAsync(Investment investment);

    Task<int> GetTotalSharesInvestedAsync(Guid propertyId);

    Task<IEnumerable<Investment>> GetByUserIdAsync(Guid userId);
    Task<int> GetUserTokensOwnedAsync(Guid userId, Guid propertyId);
    Task<decimal> GetTotalAmountInvestedAsync(Guid propertyId);
    Task<IEnumerable<Investment>> GetAllUserInvestmentsAsync();
    Task<int> GetSharesInvestedInLastDaysAsync(
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
    Task<Dictionary<Guid, int>>
GetSoldUnitsForPropertiesAsync(List<Guid> propertyIds);

    Task<DateTime?> GetLastInvestmentAtAsync(Guid propertyId);
    Task<int> GetSharesInvestedInLastHoursAsync(
     Guid propertyId,
     int hours);
    Task<decimal?> GetUserInvestmentAmountAsync(Guid userId, Guid propertyId);



}
