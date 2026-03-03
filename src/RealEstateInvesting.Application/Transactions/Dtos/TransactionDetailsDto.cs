
using RealEstateInvesting.Domain.Enums;
namespace RealEstateInvesting.Application.Transactions.Dtos;

public class TransactionDetailsDto
{
    public Guid TransactionId { get; set; }
    public Guid? PropertyId { get; set; }
    public string? PropertyName { get; set; }

    public TransactionType Type { get; set; }

    public decimal AmountUsd { get; set; }
    public string Currency { get; set; }

     public decimal? AmountEth { get; set; }

    public decimal? EthAmountAtExecution { get; set; }
    public decimal? EthUsdRateAtExecution { get; set; }

    public string Status { get; set; }

    public DateTime CreatedAt { get; set; }
}