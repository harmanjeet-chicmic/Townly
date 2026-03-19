namespace RealEstateInvesting.Application.Properties.PropertyRegistrationApi;

/// <summary>
/// Request body for POST /v1/property-register (external T-REX property registration API).
/// </summary>
public class PropertyRegisterRequestDto
{
    public string PropertyId { get; set; } = default!; // UUID
    public string OwnerAddress { get; set; } = default!; // Ethereum wallet address
    public string PricePerShare { get; set; } = default!;
    public string MintAmount { get; set; } = default!;
    public string IpfsUri { get; set; } = default!;
    public string PropertyName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string PropertyType { get; set; } = default!;
}
