using RealEstateInvesting.Domain.Common;

namespace RealEstateInvesting.Domain.Entities;

public class TokenPurchase 
{
    public Guid Id { get; private set; }

    public int Status { get; private set; }
   // public Guid UserId { get; private set; }

    // From FE
    public Guid PropertyId { get; private set; }
    public string TransactionHash { get; private set; } = null!;

    // From blockchain event
    public string? BuyerAddress { get; private set; }
    public string? SellerAddress { get; private set; }
    public decimal? Shares { get; private set; }
    public decimal? Amount { get; private set; }

    public string? ErrorMessage { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    // Factory method
    public static TokenPurchase Create(Guid propertyId, string transactionHash)
    {
        return new TokenPurchase
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            TransactionHash = transactionHash,
            Status = 1, // PENDING
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void MarkCompleted(string buyer, string seller, decimal shares, decimal amount)
    {
        BuyerAddress = buyer;
        SellerAddress = seller;
        Shares = shares;
        Amount = amount;
        Status = 2; // SUCCESS
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string error)
    {
        ErrorMessage = error;
        Status = 3; // FAILED
        UpdatedAt = DateTime.UtcNow;
    }
}