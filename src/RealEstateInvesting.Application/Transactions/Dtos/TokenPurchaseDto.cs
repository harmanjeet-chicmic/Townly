namespace RealEstateInvesting.Application.Transactions.Dtos;

public class TokenPurchaseDto
{
    public Guid Id { get; set; }
    public int Status { get; set; }
    public Guid PropertyId { get; set; }
    public string TransactionHash { get; set; } = null!;
    public string? BuyerAddress { get; set; }
    public string? SellerAddress { get; set; }
    public decimal? Shares { get; set; }
    public decimal? Amount { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}