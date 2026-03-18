namespace RealEstateInvesting.API.Contracts;

public class UpdateIdentityRequest
{
    public string UserAddress { get; set; } = string.Empty;
    public string IdentityContractAddress { get; set; } = string.Empty;
}
