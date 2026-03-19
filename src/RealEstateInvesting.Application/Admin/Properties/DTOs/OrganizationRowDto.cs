namespace RealEstateInvesting.Application.Properties.Dtos;

public class OrganizationRowDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string EntityType { get; set; } = default!;
    public string RegistrationNumber { get; set; } = default!;
    public string Jurisdiction { get; set; } = default!;
    public DateTime IncorporationDate { get; set; }

    public int PropertyHolds { get; set; } // 🔥 important
}