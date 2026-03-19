using RealEstateInvesting.Domain.Common;

namespace RealEstateInvesting.Domain.Entities;

/// <summary>
/// Audit record for an async T-REX property activation job initiated via the external API.
/// Tracks the job ID, property ID, status, and deployment transaction hash returned by the node service.
/// </summary>
public class PropertyActivationRecord : BaseEntity
{
    /// <summary>Job ID returned by the external registration API.</summary>
    public Guid JobId { get; private set; }

    /// <summary>External property ID returned by the API (maps to our internal property).</summary>
    public Guid PropertyId { get; private set; }

    /// <summary>
    /// Job status code returned by the API.
    /// e.g. 1=PENDING_TREX, 2=TREX_DEPLOYING, 3=VAULT_DEPLOYING, 4=REGISTERING, 5=KYC_VERIFYING, 6=MINTING, 7=COMPLETED, 8=FAILED
    /// </summary>
    public int Status { get; private set; }

    /// <summary>T-REX deploy transaction hash returned by the API (may be null until mined).</summary>
    public string? TrexDeployTxHash { get; private set; }

    /// <summary>Admin user ID who triggered the property activation.</summary>
    public Guid CreatedBy { get; private set; }

    private PropertyActivationRecord() { }

    public static PropertyActivationRecord Create(
        Guid jobId,
        Guid propertyId,
        int status,
        string? trexDeployTxHash,
        Guid createdBy)
    {
        return new PropertyActivationRecord
        {
            JobId = jobId,
            PropertyId = propertyId,
            Status = status,
            TrexDeployTxHash = trexDeployTxHash,
            CreatedBy = createdBy
        };
    }

    /// <summary>
    /// Updates the record with the latest status from the property-register/status API.
    /// </summary>
    public void UpdateStatus(int status, string? trexDeployTxHash = null)
    {
        Status = status;
        if (trexDeployTxHash != null)
            TrexDeployTxHash = trexDeployTxHash;
        MarkUpdated();
    }
}
