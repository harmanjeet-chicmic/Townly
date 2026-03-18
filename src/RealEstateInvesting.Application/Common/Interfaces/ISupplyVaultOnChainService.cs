namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// Add Property Step 4 in T-REX Developer Guide: Supply the Vault — set identity for vault (optional) and mint property tokens to vault.
/// </summary>
public interface ISupplyVaultOnChainService
{
    /// <summary>
    /// Sets vault identity (if provided) then mints tokens to the vault and records the operation for audit.
    /// </summary>
    /// <param name="vaultAddress">Vault contract address.</param>
    /// <param name="tokenAddress">Property token contract address.</param>
    /// <param name="amountMintedRaw">Amount to mint to vault (raw string).</param>
    /// <param name="vaultIdentityAddress">Optional identity contract address for the vault; if provided, updateIdentity(vault, identity) is called first.</param>
    /// <param name="propertyId">Optional internal property ID.</param>
    /// <param name="performedByAdminId">Admin user ID performing the action.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Object with identityTxHash (if set) and mintTxHash.</returns>
    Task<SupplyVaultResult> SupplyVaultOnChainAsync(
        string vaultAddress,
        string tokenAddress,
        string amountMintedRaw,
        string? vaultIdentityAddress,
        Guid? propertyId,
        Guid? performedByAdminId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a Supply Vault on-chain operation.
/// </summary>
public class SupplyVaultResult
{
    /// <summary>Transaction hash for identity update (null if not performed).</summary>
    public string? IdentityTxHash { get; init; }

    /// <summary>Transaction hash for mint to vault.</summary>
    public string MintTxHash { get; init; } = default!;
}
