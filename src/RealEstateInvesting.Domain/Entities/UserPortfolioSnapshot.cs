using RealEstateInvesting.Domain.Common;

namespace RealEstateInvesting.Domain.Entities;

public class UserPortfolioSnapshot : BaseEntity
{
    public Guid UserId { get; private set; }
    public DateTime SnapshotAt { get; private set; }

    public decimal TotalInvested { get; private set; }
    public decimal PortfolioValue { get; private set; }
    public decimal UnrealizedPnL { get; private set; }

    // 🔥 NEW FIELD
    public decimal MonthlyIncome { get; private set; }

    private UserPortfolioSnapshot() { }

    public static UserPortfolioSnapshot Create(
        Guid userId,
        DateTime snapshotAt,
        decimal totalInvested,
        decimal portfolioValue,
        decimal monthlyIncome)
    {
        return new UserPortfolioSnapshot
        {
            UserId = userId,
            SnapshotAt = snapshotAt,
            TotalInvested = totalInvested,
            PortfolioValue = portfolioValue,
            UnrealizedPnL = portfolioValue - totalInvested,
            MonthlyIncome = monthlyIncome
        };
    }
}
