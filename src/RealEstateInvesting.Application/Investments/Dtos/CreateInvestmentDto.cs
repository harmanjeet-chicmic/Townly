namespace RealEstateInvesting.Application.Investments.Dtos;

public class CreateInvestmentDto
{
    public Guid PropertyId { get; set; }
    public int Shares { get; set; }
    public decimal PlatformFeeUsd { get; set; }

}
