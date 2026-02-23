using Org.BouncyCastle.Math.EC.Rfc7748;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Transactions.Dtos;
using RealEstateInvesting.Domain.Enums;
namespace RealEstateInvesting.Application.Transactions;

public class TransactionQueryService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IPropertyRepository _propertyRepository;

    public TransactionQueryService(ITransactionRepository transactionRepository, IPropertyRepository propertyRepository)
    {
        _transactionRepository = transactionRepository;
        _propertyRepository = propertyRepository;
    }
  public async Task<object> GetMyTransactionsAsync(
    Guid userId,
    int page,
    int pageSize,
    TransactionType? type)
{
    var (transactions, totalCount) =
        await _transactionRepository
            .GetByUserIdPagedAsync(userId, page, pageSize, type);

    if (!transactions.Any())
    {
        return new
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = 0,
            HasMore = false,
            Items = new List<MyTransactionDto>()
        };
    }

    var propertyIds = transactions
        .Where(t => t.PropertyId.HasValue)
        .Select(t => t.PropertyId!.Value)
        .Distinct()
        .ToList();

    var properties =
        await _propertyRepository.GetByIdsAsync(propertyIds);

    var propertyDict =
        properties.ToDictionary(p => p.Id, p => p.Name);

    var items = transactions.Select(t => new MyTransactionDto
    {
        TransactionId = t.Id,
        PropertyId = t.PropertyId,
        PropertyName = t.PropertyId.HasValue &&
                       propertyDict.ContainsKey(t.PropertyId.Value)
            ? propertyDict[t.PropertyId.Value]
            : null,

        Type = t.Type,
        AmountUsd = t.AmountUsd,
        Currency = t.Currency,
        EthAmountAtExecution = t.EthAmountAtExecution,
        EthUsdRateAtExecution = t.EthUsdRateAtExecution,
        AmountEth = t.EthAmountAtExecution,
        Status = t.IsSuccessful ? "Completed" : "Pending",
        CreatedAt = t.CreatedAt
    });

    return new
    {
        Page = page,
        PageSize = pageSize,
        TotalCount = totalCount,
        HasMore = page * pageSize < totalCount,
        Items = items
    };
}


}
