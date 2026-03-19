namespace RealEstateInvesting.Application.Properties.PropertyRegistrationApi;

/// <summary>
/// Response from GET /v1/property-register/status?jobId={jobId}.
/// </summary>
public class PropertyRegisterJobStatusResponseDto
{
    public int StatusCode { get; set; }
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public string Type { get; set; } = default!;
    public PropertyRegisterJobStatusDataDto? Data { get; set; }
}

public class PropertyRegisterJobStatusDataDto
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    /// <summary>1=PENDING_TREX, 2=TREX_DEPLOYING, 3=VAULT_DEPLOYING, 4=REGISTERING, 5=KYC_VERIFYING, 6=MINTING, 7=COMPLETED, 8=FAILED</summary>
    public int Status { get; set; }
    public string? TrexDeployTxHash { get; set; }
    public string? TokenAddress { get; set; }
    public string? VaultAddress { get; set; }
    public string? OnChainPropertyId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? StatusLabel { get; set; }
}
