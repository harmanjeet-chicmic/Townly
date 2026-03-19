namespace RealEstateInvesting.Domain.Enums;

/// <summary>
/// Maps T-REX property registration job status codes (from node service) to <see cref="PropertyStatus"/>.
/// </summary>
public static class TrexStatusMapper
{
    /// <summary>
    /// Maps the integer status returned by GET /v1/property-register/status to our PropertyStatus enum.
    /// </summary>
    public static PropertyStatus MapToPropertyStatus(int trexStatusCode) => (PropertyStatusTREX)trexStatusCode switch
    {
        PropertyStatusTREX.COMPLETED => PropertyStatus.Active,
        PropertyStatusTREX.PENDING_TREX => PropertyStatus.PENDING_TREX,
        PropertyStatusTREX.TREX_DEPLOYING => PropertyStatus.TREX_DEPLOYING,
        PropertyStatusTREX.VAULT_DEPLOYING => PropertyStatus.VAULT_DEPLOYING,
        PropertyStatusTREX.REGISTERING => PropertyStatus.REGISTERING,
        PropertyStatusTREX.KYC_VERIFYING => PropertyStatus.KYC_VERIFYING,
        PropertyStatusTREX.MINTING => PropertyStatus.MINTING,
        PropertyStatusTREX.FAILED => PropertyStatus.FAILED,
        _ => PropertyStatus.PENDING_TREX
    };
}
