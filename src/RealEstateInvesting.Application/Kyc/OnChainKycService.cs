using RealEstateInvesting.Application.Common.Interfaces;

namespace RealEstateInvesting.Application.Kyc;

/// <summary>
/// Flow 1–3: Reads isVerified from chain; for updateIdentity/updateCountry calls contract then records in OnChainKycActions via repository.
/// </summary>
public sealed class OnChainKycService : IOnChainKycService
{
    private readonly IIdentityRegistryContractService _identityRegistry;
    private readonly IOnChainKycActionRepository _onChainKycActionRepository;

    public OnChainKycService(
        IIdentityRegistryContractService identityRegistry,
        IOnChainKycActionRepository onChainKycActionRepository)
    {
        _identityRegistry = identityRegistry;
        _onChainKycActionRepository = onChainKycActionRepository;
    }

    public Task<bool> IsVerifiedAsync(string address, CancellationToken cancellationToken = default)
        => _identityRegistry.IsVerified(address, cancellationToken);

    public async Task<string> UpdateIdentityOnChainAsync(string userAddress, string identityContractAddress, Guid performedByAdminId, CancellationToken cancellationToken = default)
    {
        var txHash = await _identityRegistry.UpdateIdentity(userAddress, identityContractAddress, cancellationToken);
        await _onChainKycActionRepository.RecordIdentityUpdateAsync(userAddress, identityContractAddress, txHash, performedByAdminId, cancellationToken);
        return txHash;
    }

    public async Task<string> UpdateCountryOnChainAsync(string userAddress, ushort countryCode, Guid performedByAdminId, CancellationToken cancellationToken = default)
    {
        var txHash = await _identityRegistry.UpdateCountry(userAddress, countryCode, cancellationToken);
        await _onChainKycActionRepository.RecordCountryUpdateAsync(userAddress, countryCode, txHash, performedByAdminId, cancellationToken);
        return txHash;
    }

    public async Task<string> RegisterIdentityOnChainAsync(string userAddress, string identityContractAddress, ushort countryCode, Guid performedByAdminId, CancellationToken cancellationToken = default)
    {
        var txHash = await _identityRegistry.RegisterIdentity(userAddress, identityContractAddress, countryCode, cancellationToken);
        await _onChainKycActionRepository.RecordRegisterIdentityAsync(userAddress, identityContractAddress, countryCode, txHash, performedByAdminId, cancellationToken);
        return txHash;
    }
}
