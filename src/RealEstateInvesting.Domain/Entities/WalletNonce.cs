// using RealEstateInvesting.Domain.Common;

// namespace RealEstateInvesting.Domain.Entities;

// public class WalletNonce : BaseEntity
// {
//     public string WalletAddress { get; private set; } = default!;
//     public string Nonce { get; private set; } = default!;
//     public string SiweMessage { get; private set; } = default!;
//     public long ChainId { get; private set; }
//     public DateTime ExpiresAt { get; private set; }
//     public bool IsUsed { get; private set; }

//     private WalletNonce() {}

//     public static WalletNonce Create(
//         string wallet,
//         long chainId,
//         string siweMessage)
//     {
//         return new WalletNonce
//         {
//             WalletAddress = wallet.ToLowerInvariant(),
//             Nonce = Guid.NewGuid().ToString("N"),
//             ChainId = chainId,
//             SiweMessage = siweMessage,
//             ExpiresAt = DateTime.UtcNow.AddMinutes(5),
//             IsUsed = false
//         };
//     }

//     public void MarkUsed()
//     {
//         IsUsed = true;
//         UpdatedAt = DateTime.UtcNow;
//     }

//     public bool IsExpired() => DateTime.UtcNow > ExpiresAt;
// }

// using RealEstateInvesting.Domain.Common;

// namespace RealEstateInvesting.Domain.Entities;

// public class WalletNonce : BaseEntity
// {
//     public string WalletAddress { get; private set; } = default!;
//     public string Nonce { get; private set; } = default!;
//     public long ChainId { get; private set; }
//     public DateTime ExpiresAt { get; private set; }
//     public bool IsUsed { get; private set; }

//     private WalletNonce() {}

//     public static WalletNonce Create(
//         string wallet,
//         long chainId,
//         string nonce)
//     {
//         return new WalletNonce
//         {
//             WalletAddress = wallet.ToLowerInvariant(),
//             Nonce = nonce,
//             ChainId = chainId,
//             ExpiresAt = DateTime.UtcNow.AddMinutes(5),
//             IsUsed = false,
//             CreatedAt = DateTime.UtcNow
//         };
//     }

//     public void MarkUsed()
//     {
//         IsUsed = true;
//         UpdatedAt = DateTime.UtcNow;
//     }

//     public bool IsExpired()
//         => DateTime.UtcNow > ExpiresAt;
// }
using RealEstateInvesting.Domain.Common;

namespace RealEstateInvesting.Domain.Entities;

public class WalletNonce : BaseEntity
{
    public string? WalletAddress { get; private set; }   // 👈 nullable
    public string Nonce { get; private set; } = default!;
    public long? ChainId { get; private set; }           // 👈 nullable
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }

    private WalletNonce() {}

    // 🔹 Existing flow (KEEP IT)
    public static WalletNonce Create(
        string wallet,
        long chainId,
        string nonce)
    {
        return new WalletNonce
        {
            WalletAddress = wallet.ToLowerInvariant(),
            ChainId = chainId,
            Nonce = nonce,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    // 🆕 Anonymous nonce (NEW)
    public static WalletNonce CreateAnonymous(string nonce)
    {
        return new WalletNonce
        {
            WalletAddress = null,
            ChainId = null,
            Nonce = nonce,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void AttachWallet(string wallet, long chainId)
    {
        WalletAddress = wallet.ToLowerInvariant();
        ChainId = chainId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkUsed()
    {
        IsUsed = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsExpired()
        => DateTime.UtcNow > ExpiresAt;
}
