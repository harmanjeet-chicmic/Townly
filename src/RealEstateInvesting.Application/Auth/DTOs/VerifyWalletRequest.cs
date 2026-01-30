// namespace RealEstateInvesting.Application.Auth.DTOs;

// public class VerifyWalletRequest
// {
//     public string WalletAddress { get; set; } = default!;
//     public string Signature { get; set; } = default!;
//     public long ChainId { get; set; }
// }
namespace RealEstateInvesting.Application.Auth.DTOs;

public class VerifyWalletRequest
{
    public string WalletAddress { get; set; } = null!;
    public int ChainId { get; set; }
    public string Signature { get; set; } = null!;

    // 🔥 EXACT string that wallet signed
    public string Message { get; set; } = null!;
}
