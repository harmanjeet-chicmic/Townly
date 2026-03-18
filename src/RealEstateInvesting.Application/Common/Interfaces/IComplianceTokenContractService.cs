namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// T-REX / ERC-3643 compliant token contract: mint (Add Property Step 4).
/// Caller must have mint/agent role on the token contract (TRex:PrivateKey wallet).
/// </summary>
public interface IComplianceTokenContractService
{
    /// <summary>
    /// Mints property tokens to an address (e.g. vault). Requires agent/minter role on the token contract.
    /// </summary>
    /// <param name="tokenAddress">Property token contract address.</param>
    /// <param name="toAddress">Recipient address (typically the vault).</param>
    /// <param name="amountRaw">Amount to mint (raw units, e.g. "1000000000000000000").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Transaction hash of the mint call.</returns>
    Task<string> MintAsync(string tokenAddress, string toAddress, string amountRaw, CancellationToken cancellationToken = default);
}
