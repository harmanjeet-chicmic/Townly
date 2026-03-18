namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// T-REX Factory: deploy full token suite (token + IR + compliance etc.). Emits TREXSuiteDeployed(token, ir, ...).
/// </summary>
public interface ITREXFactoryContractService
{
    /// <summary>
    /// Deploys a T-REX suite for a property. Returns token address and identity registry address from TREXSuiteDeployed event.
    /// </summary>
    Task<TREXSuiteDeployResult> DeployTREXSuiteAsync(TREXSuiteDeployRequest request, CancellationToken cancellationToken = default);
}

public class TREXSuiteDeployRequest
{
    public string Salt { get; set; } = default!;
    public string OwnerAddress { get; set; } = default!;
    public string TokenName { get; set; } = default!;
    public string TokenSymbol { get; set; } = default!;
    public byte Decimals { get; set; } = 6;
    /// <summary>Identity Registry Storage - use zero address for default.</summary>
    public string IrsAddress { get; set; } = "0x0000000000000000000000000000000000000000";
    /// <summary>ONCHAINID - use zero address for default.</summary>
    public string OnChainIdAddress { get; set; } = "0x0000000000000000000000000000000000000000";
    public IReadOnlyList<string> IrAgents { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> TokenAgents { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> ComplianceModules { get; set; } = Array.Empty<string>();
    public IReadOnlyList<byte[]> ComplianceSettings { get; set; } = Array.Empty<byte[]>();
    /// <summary>Claim topic (e.g. KYC_CLAIM keccak256).</summary>
    public string ClaimTopicHex { get; set; } = default!;
    /// <summary>Claim issuer address.</summary>
    public string ClaimIssuerAddress { get; set; } = default!;
}

public class TREXSuiteDeployResult
{
    public string TokenAddress { get; init; } = default!;
    public string IdentityRegistryAddress { get; init; } = default!;
    public string TransactionHash { get; init; } = default!;
}
