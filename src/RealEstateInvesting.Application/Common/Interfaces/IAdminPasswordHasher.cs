namespace RealEstateInvesting.Application.Common.Interfaces;

public interface IAdminPasswordHasher
{
    bool Verify(string hashedPassword, string providedPassword);
}
