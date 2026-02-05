using RealEstateInvesting.Domain.Common;

namespace RealEstateInvesting.Domain.Entities;

public class UserTokenBalance : BaseEntity
{
    public Guid UserId { get; private set; }
    public decimal TotalGranted { get; private set; }
    public decimal TotalUsed { get; private set; }

    public decimal Available => TotalGranted - TotalUsed;

    private UserTokenBalance() { }

    public static UserTokenBalance Create(Guid userId)
    {
        return new UserTokenBalance
        {
            UserId = userId,
            TotalGranted = 0,
            TotalUsed = 0
        };
    }

    public void Grant(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Grant amount must be positive.");

        TotalGranted += amount;
    }

    public void Deduct(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Deduction amount must be positive.");

        if (Available < amount)
            throw new InvalidOperationException("Insufficient token balance.");

        TotalUsed += amount;
    }
}
