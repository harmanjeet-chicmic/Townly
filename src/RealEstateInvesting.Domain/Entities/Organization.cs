using RealEstateInvesting.Domain.Common;

namespace RealEstateInvesting.Domain.Entities;

public class Organization : BaseEntity
{
    public string Name { get; private set; } = default!;

    public string EntityType { get; private set; } = default!; 
    public string RegistrationNumber { get; private set; } = default!;
    public string Jurisdiction { get; private set; } = default!;
    public DateTime IncorporationDate { get; private set; }

    private Organization() { }

    public Organization(
        Guid id,
        string name,
        string entityType,
        string registrationNumber,
        string jurisdiction,
        DateTime incorporationDate)
    {
        Id = id;
        Name = name;
        EntityType = entityType;
        RegistrationNumber = registrationNumber;
        Jurisdiction = jurisdiction;
        IncorporationDate = incorporationDate;
    }
}