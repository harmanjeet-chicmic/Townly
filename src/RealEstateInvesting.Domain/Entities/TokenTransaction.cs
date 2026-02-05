using RealEstateInvesting.Domain.Common;

namespace RealEstateInvesting.Domain.Entities;

public class TokenTransaction : BaseEntity
{
    public Guid UserId { get; private set; }
    public decimal Amount { get; private set; }
    public string Type { get; private set; } = default!;
    public string Reference { get; private set; } = default!;

    private TokenTransaction() { }

    public static TokenTransaction Create(
        Guid userId,
        decimal amount,
        string type,
        string reference)
    {
        return new TokenTransaction
        {
            UserId = userId,
            Amount = amount,
            Type = type,
            Reference = reference
        };
    }
}
