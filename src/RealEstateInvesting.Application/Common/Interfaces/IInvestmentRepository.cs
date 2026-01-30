using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface IInvestmentRepository
{
    Task AddAsync(Investment investment);

    Task<int> GetTotalSharesInvestedAsync(Guid propertyId);

    Task<IEnumerable<Investment>> GetByUserIdAsync(Guid userId);
    Task<decimal> GetTotalAmountInvestedAsync(Guid propertyId);
    Task<IEnumerable<Investment>> GetAllUserInvestmentsAsync();
    Task<int> GetSharesInvestedInLastDaysAsync(
   Guid propertyId,
   int days);

    Task<int> GetUniqueInvestorCountAsync(Guid propertyId);

    Task<DateTime?> GetLastInvestmentAtAsync(Guid propertyId);
   Task<int> GetSharesInvestedInLastHoursAsync(
    Guid propertyId,
    int hours);


}
