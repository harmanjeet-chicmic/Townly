namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// Modular Compliance: isModuleBound(module), addModule(module). Used to bind vault to compliance after mint.
/// </summary>
public interface IModularComplianceContractService
{
    Task<bool> IsModuleBoundAsync(string complianceAddress, string moduleAddress, CancellationToken cancellationToken = default);
    Task<string> AddModuleAsync(string complianceAddress, string moduleAddress, CancellationToken cancellationToken = default);
}
