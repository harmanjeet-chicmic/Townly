using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Transactions.Dtos;

namespace RealEstateInvesting.Application.Transactions;

public class TransactionQueryService
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionQueryService(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<IEnumerable<MyTransactionDto>> GetMyTransactionsAsync(Guid userId)
    {
        var transactions = await _transactionRepository.GetByUserIdAsync(userId);

        return transactions.Select(t => new MyTransactionDto
        {
            TransactionId = t.Id,
            PropertyId = t.PropertyId,
            Type = t.Type,

            AmountUsd = t.Amount,
            Currency = t.Currency,

            // ETH snapshot intentionally not populated here
            EthAmountAtExecution = null,
            EthUsdRateAtExecution = null,

            CreatedAt = t.CreatedAt
        });

    }
}
