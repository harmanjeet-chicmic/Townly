using RealEstateInvesting.Application.Common.Interfaces;

namespace RealEstateInvesting.Application.Properties;

/// <summary>
/// Flow 4: Registers a property on T-REX Real Estate Registry and saves an audit record in DB.
/// </summary>
public sealed class PropertyRegistrationOnChainService : IPropertyRegistrationOnChainService
{
    private readonly IRealEstateRegistryContractService _registryContract;
    private readonly IOnChainPropertyRegistrationRepository _registrationRepository;

    public PropertyRegistrationOnChainService(
        IRealEstateRegistryContractService registryContract,
        IOnChainPropertyRegistrationRepository registrationRepository)
    {
        _registryContract = registryContract;
        _registrationRepository = registrationRepository;
    }

    /// <inheritdoc />
    public async Task<string> RegisterPropertyOnChainAsync(string toAddress,string uri, string tokenAddress,
                                                           string vaultAddress, Guid? propertyId,
                                                           Guid performedByAdminId, CancellationToken cancellationToken = default)
    {
        var txHash = await _registryContract.RegisterPropertyAsync(toAddress, uri, tokenAddress, vaultAddress, cancellationToken);

        await _registrationRepository.RecordRegistrationAsync(toAddress, uri, tokenAddress, vaultAddress,
                                                              txHash, propertyId, onChainPropertyId: null, 
                                                              performedByAdminId, cancellationToken);
        return txHash;
    }
}
