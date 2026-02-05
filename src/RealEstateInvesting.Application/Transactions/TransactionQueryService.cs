using Org.BouncyCastle.Math.EC.Rfc7748;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Transactions.Dtos;

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

    public async Task<IEnumerable<MyTransactionDto>> GetMyTransactionsAsync(Guid userId)
    {
        var transactions = await _transactionRepository.GetByUserIdAsync(userId);
        var pId =  transactions.Where(t=>t.UserId == userId ).Select(t=>t.PropertyId);

        //var propertyName = await _propertyRepository.GetByIdAsync();

        return transactions.Select(t => new MyTransactionDto
        {
            TransactionId = t.Id,
            PropertyId = t.PropertyId,
            Type = t.Type,

            AmountUsd = t.AmountUsd,
            Currency = t.Currency,

            
            EthAmountAtExecution = t.EthAmountAtExecution,
            EthUsdRateAtExecution = t.EthUsdRateAtExecution,
            Status = t.IsSuccessful?"Completed":"pending",

            CreatedAt = t.CreatedAt
        });

    }
}
