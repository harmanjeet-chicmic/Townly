namespace RealEstateInvesting.Application.Tokens.Balance;

public class UserTokenBalanceDto
{
    public Guid UserId { get; set; }
    public decimal TotalGranted { get; set; }
    public decimal TotalUsed { get; set; }
    public decimal Available { get; set; }
}
