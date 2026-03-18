namespace RealEstateInvesting.API.Contracts;

/// <summary>
/// Request body for Flow 4: Register property on T-REX Real Estate Registry.
/// </summary>
public class RegisterPropertyOnChainRequest
{
    /// <summary>Property owner/operator wallet address.</summary>
    public string ToAddress { get; set; } = string.Empty;

    /// <summary>Property metadata URI (e.g. ipfs://Qm...).</summary>
    public string Uri { get; set; } = string.Empty;

    /// <summary>Property ERC-20 token address.</summary>
    public string TokenAddress { get; set; } = string.Empty;

    /// <summary>Property vault address.</summary>
    public string VaultAddress { get; set; } = string.Empty;

    /// <summary>Optional: our internal Property ID to link to the on-chain registration.</summary>
    public Guid? PropertyId { get; set; }
}
