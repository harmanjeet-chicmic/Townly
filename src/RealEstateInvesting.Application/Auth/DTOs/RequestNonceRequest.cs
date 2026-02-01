// namespace RealEstateInvesting.Application.Auth.DTOs;

// public class RequestNonceRequest
// {
//     public string WalletAddress { get; set; } = default!;
//     public long ChainId { get; set; }
// }

namespace RealEstateInvesting.Application.Auth.DTOs;

public class RequestNonceRequest
{
    public string WalletAddress { get; set; } = null!;
    public long ChainId { get; set; }
}
