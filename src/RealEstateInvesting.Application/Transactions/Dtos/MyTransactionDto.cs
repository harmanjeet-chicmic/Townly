using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Transactions.Dtos;

public class MyTransactionDto
{
    public Guid TransactionId { get; set; }
    public Guid? PropertyId { get; set; }

    public TransactionType Type { get; set; }

    // USD (ledger truth)
    public decimal AmountUsd { get; set; }
    public string Currency { get; set; } = "USD";

    // ETH snapshot (execution-time)
    public decimal? EthAmountAtExecution { get; set; }
    public decimal? EthUsdRateAtExecution { get; set; }

    public DateTime CreatedAt { get; set; }
}
