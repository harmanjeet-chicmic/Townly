namespace RealEstateInvesting.Application.Properties.PropertyRegistrationApi;

/// <summary>
/// Response from POST /v1/property-register.
/// </summary>
public class PropertyRegisterResponseDto
{
    public int StatusCode { get; set; }
    public bool Status { get; set; }
    public string Message { get; set; } = default!;
    public string Type { get; set; } = default!;
    public PropertyRegisterResponseDataDto? Data { get; set; }
}

public class PropertyRegisterResponseDataDto
{
    public Guid JobId { get; set; }
    public Guid PropertyId { get; set; }
    /// <summary>1=PENDING_TREX, 2=TREX_DEPLOYING, 3=VAULT_DEPLOYING, 4=REGISTERING, 5=KYC_VERIFYING, 6=MINTING, 7=COMPLETED, 8=FAILED</summary>
    public int Status { get; set; }
    public string? TrexDeployTxHash { get; set; }
}
