// using RealEstateInvesting.Domain.Common;
// using RealEstateInvesting.Domain.Enums;

// namespace RealEstateInvesting.Domain.Entities;

// public class Transaction : BaseEntity
// {
//     // Ownership
//     public Guid UserId { get; private set; }

//     // Context (nullable for platform-wide tx)
//     public Guid? PropertyId { get; private set; }

//     // Ledger data
//     public TransactionType Type { get; private set; }
//     public decimal Amount { get; private set; }
//     public string Currency { get; private set; } = "USD";

//     // Reference to source entity
//     public Guid ReferenceId { get; private set; }

//     // Status (simple for now)
//     public bool IsSuccessful { get; private set; }

//     // EF
//     private Transaction() { }

//     // ---------------------------
//     // Factory: Create Transaction
//     // ---------------------------
//     public static Transaction Create(
//         Guid userId,
//         TransactionType type,
//         decimal amount,
//         Guid referenceId,
//         Guid? propertyId = null,
//         string currency = "USD")
//     {
//         if (amount <= 0)
//             throw new InvalidOperationException("Transaction amount must be greater than zero.");

//         return new Transaction
//         {
//             UserId = userId,
//             PropertyId = propertyId,
//             Type = type,
//             Amount = amount,
//             Currency = currency,
//             ReferenceId = referenceId,
//             IsSuccessful = true
//         };
//     }
// }
using RealEstateInvesting.Domain.Common;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Domain.Entities;

public class Transaction : BaseEntity
{
    // Ownership
    public Guid UserId { get; private set; }

    // Context
    public Guid? PropertyId { get; private set; }

    // Ledger data
    public TransactionType Type { get; private set; }

    // USD (source of truth)
    public decimal AmountUsd { get; private set; }
    public string Currency { get; private set; } = "USD";

    // 🔥 ETH snapshot (at execution time)
    public decimal? EthAmountAtExecution { get; private set; }
    public decimal? EthUsdRateAtExecution { get; private set; }

    // Reference to source entity
    public Guid ReferenceId { get; private set; }

    // Status
    public bool IsSuccessful { get; private set; }

    private Transaction() { }

    // ---------------------------
    // Factory: Investment Transaction
    // ---------------------------
    public static Transaction CreateInvestment(
        Guid userId,
        Guid propertyId,
        decimal amountUsd,
        decimal ethAmount,
        decimal ethUsdRate,
        Guid referenceId)
    {
        if (amountUsd <= 0)
            throw new InvalidOperationException("Transaction amount must be greater than zero.");

        if (ethAmount <= 0 || ethUsdRate <= 0)
            throw new InvalidOperationException("Invalid ETH snapshot.");

        return new Transaction
        {
            UserId = userId,
            PropertyId = propertyId,
            Type = TransactionType.Investment,
            AmountUsd = amountUsd,
            Currency = "USD",
            EthAmountAtExecution = ethAmount,
            EthUsdRateAtExecution = ethUsdRate,
            ReferenceId = referenceId,
            IsSuccessful = true
        };
    }

    // ---------------------------
    // Factory: Non-investment Transaction
    // ---------------------------
    public static Transaction CreateSimple(
        Guid userId,
        TransactionType type,
        decimal amountUsd,
        Guid referenceId,
        Guid? propertyId = null)
    {
        if (amountUsd <= 0)
            throw new InvalidOperationException("Transaction amount must be greater than zero.");

        return new Transaction
        {
            UserId = userId,
            PropertyId = propertyId,
            Type = type,
            AmountUsd = amountUsd,
            Currency = "USD",
            ReferenceId = referenceId,
            IsSuccessful = true
        };
    }
}
