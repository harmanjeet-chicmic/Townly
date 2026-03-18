namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// T-REX Token read/write helpers: identityRegistry(), paused(), unpause(), compliance().
/// </summary>
public interface ITokenReaderContractService
{
    Task<string> GetIdentityRegistryAsync(string tokenAddress, CancellationToken cancellationToken = default);
    Task<bool> GetPausedAsync(string tokenAddress, CancellationToken cancellationToken = default);
    Task<string> UnpauseAsync(string tokenAddress, CancellationToken cancellationToken = default);
    Task<string> GetComplianceAsync(string tokenAddress, CancellationToken cancellationToken = default);
}
