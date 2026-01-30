using RealEstateInvesting.Domain.Common;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Domain.Entities;

public class Transaction : BaseEntity
{
    // Ownership
    public Guid UserId { get; private set; }

    // Context (nullable for platform-wide tx)
    public Guid? PropertyId { get; private set; }

    // Ledger data
    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "USD";

    // Reference to source entity
    public Guid ReferenceId { get; private set; }

    // Status (simple for now)
    public bool IsSuccessful { get; private set; }

    // EF
    private Transaction() { }

    // ---------------------------
    // Factory: Create Transaction
    // ---------------------------
    public static Transaction Create(
        Guid userId,
        TransactionType type,
        decimal amount,
        Guid referenceId,
        Guid? propertyId = null,
        string currency = "USD")
    {
        if (amount <= 0)
            throw new InvalidOperationException("Transaction amount must be greater than zero.");

        return new Transaction
        {
            UserId = userId,
            PropertyId = propertyId,
            Type = type,
            Amount = amount,
            Currency = currency,
            ReferenceId = referenceId,
            IsSuccessful = true
        };
    }
}
