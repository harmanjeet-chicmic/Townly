namespace RealEstateInvesting.Domain.Entities;

/// <summary>
/// Read model for external PropertyRegistrationJobs table maintained by node service.
/// </summary>
public class PropertyRegistrationJob
{
    public string Id { get; set; }
    public int Status { get; set; }
    public string PropertyId { get; set; }
    public string OwnerAddress { get; set; } = string.Empty;
    public string PricePerShare { get; set; } = string.Empty;
    public string MintAmount { get; set; } = string.Empty;
    public string IpfsUri { get; set; } = string.Empty;
    public string PropertyName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public string? TrexDeployTxHash { get; set; }
    public string? TokenAddress { get; set; }
    public string? VaultAddress { get; set; }
    public string? OnChainPropertyId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
