namespace RealEstateInvesting.Application.Organizations.Dtos;

public class OrganizationPropertyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string PropertyType { get; set; } = default!;

    public decimal TotalValue { get; set; }
    public int TotalUnits { get; set; }
   

    public int Status { get; set; }

    public string? Image { get; set; }
    public string? OwnerWalletAddress { get; set; }
}