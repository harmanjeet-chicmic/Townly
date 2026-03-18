namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// Real Estate Vault Factory: deploy vault for a token + stablecoin, then resolve vault address via vaults(token).
/// </summary>
public interface IRealEstateVaultFactoryContractService
{
    /// <summary>
    /// Deploys a vault for the given token and stablecoin with pricePerShare (6 decimals typically).
    /// </summary>
    Task<string> DeployVaultAsync(string tokenAddress, string stablecoinAddress, string pricePerShareRaw, string adminAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the vault address for the given token (after deployVault).
    /// </summary>
    Task<string> GetVaultAsync(string tokenAddress, CancellationToken cancellationToken = default);
}
