namespace RealEstateInvesting.Application.Auth.DTOs;

public class VerifyWalletTypedRequest
{
    public string WalletAddress { get; set; } = default!;
    public string Signature { get; set; } = default!;
    public long ChainId { get; set; }
}
