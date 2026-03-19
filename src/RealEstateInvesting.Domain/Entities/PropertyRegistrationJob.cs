namespace RealEstateInvesting.Domain.Entities;

/// <summary>
/// Read model for external PropertyRegistrationJobs table maintained by node service.
/// </summary>
public class PropertyRegistrationJob
{
    public string Id { get; set; } = string.Empty;
    public short Status { get; set; }
    public string PropertyId { get; set; } = string.Empty;
    public string OwnerAddress { get; set; } = string.Empty;
    public decimal? PricePerShare { get; set; }
    public decimal? MintAmount { get; set; }
    public string IpfsUri { get; set; } = string.Empty;
    public string PropertyName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? PropertyType { get; set; }
    public string? TrexDeployTxHash { get; set; }
    public string? TokenAddress { get; set; }
    public string? VaultAddress { get; set; }
    public long? OnChainPropertyId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}