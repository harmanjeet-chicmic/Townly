using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Application.Properties;

/// <summary>
/// Add Property Step 4: Supply the Vault — set identity for vault (optional) and mint property tokens to vault; audit in OnChainVaultSupplies.
/// </summary>
public sealed class SupplyVaultOnChainService : ISupplyVaultOnChainService
{
    private readonly IIdentityRegistryContractService _identityRegistry;
    private readonly IComplianceTokenContractService _complianceToken;
    private readonly IOnChainVaultSupplyRepository _vaultSupplyRepository;

    public SupplyVaultOnChainService(
        IIdentityRegistryContractService identityRegistry,
        IComplianceTokenContractService complianceToken,
        IOnChainVaultSupplyRepository vaultSupplyRepository)
    {
        _identityRegistry = identityRegistry;
        _complianceToken = complianceToken;
        _vaultSupplyRepository = vaultSupplyRepository;
    }

    /// <inheritdoc />
    public async Task<SupplyVaultResult> SupplyVaultOnChainAsync(
        string vaultAddress,
        string tokenAddress,
        string amountMintedRaw,
        string? vaultIdentityAddress,
        Guid? propertyId,
        Guid? performedByAdminId,
        CancellationToken cancellationToken = default)
    {
        string? identityTxHash = null;
        if (!string.IsNullOrWhiteSpace(vaultIdentityAddress))
        {
            identityTxHash = await _identityRegistry.UpdateIdentity(vaultAddress, vaultIdentityAddress, cancellationToken).ConfigureAwait(false);
        }

        var mintTxHash = await _complianceToken.MintAsync(tokenAddress, vaultAddress, amountMintedRaw, cancellationToken).ConfigureAwait(false);

        var supply = OnChainVaultSupply.Create(
            vaultAddress,
            tokenAddress,
            amountMintedRaw,
            identityTxHash,
            mintTxHash,
            propertyId,
            performedByAdminId);
        await _vaultSupplyRepository.AddAsync(supply, cancellationToken).ConfigureAwait(false);

        return new SupplyVaultResult
        {
            IdentityTxHash = identityTxHash,
            MintTxHash = mintTxHash
        };
    }
}
