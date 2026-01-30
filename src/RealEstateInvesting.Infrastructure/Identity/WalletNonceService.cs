// using Microsoft.EntityFrameworkCore;
// using RealEstateInvesting.Application.Auth.DTOs;
// using RealEstateInvesting.Application.Auth.Interfaces;
// using RealEstateInvesting.Domain.Entities;
// using RealEstateInvesting.Infrastructure.Persistence;

// namespace RealEstateInvesting.Infrastructure.Identity;

// public class WalletNonceService : IWalletNonceService
// {
//     private readonly AppDbContext _db;

//     public WalletNonceService(AppDbContext dbContext)
//     {
//         _db = dbContext;
//     }

//     public async Task<RequestNonceResponse> GenerateNonceAsync(
//     RequestNonceRequest request)
//     {
//         var wallet = request.WalletAddress.ToLowerInvariant();
//         var nonce = Guid.NewGuid().ToString("N");

//         var issuedAt = DateTime.UtcNow;
//         var expiresAt = issuedAt.AddMinutes(5);

//         var siweMessage = SiweMessageBuilder.Build(
//             domain: "com.townly.townly",
//             wallet: wallet,
//             chainId: request.ChainId,
//             nonce: nonce,
//             issuedAt: issuedAt,
//             expiresAt: expiresAt
//         );

//         // invalidate old nonces
//         var old = await _db.WalletNonces
//             .Where(x => x.WalletAddress == wallet && !x.IsUsed)
//             .ToListAsync();

//         foreach (var n in old)
//             n.MarkUsed();

//         var entity = WalletNonce.Create(
//             wallet,
//             request.ChainId,
//             siweMessage
//         );

//         _db.WalletNonces.Add(entity);
//         await _db.SaveChangesAsync();

//         return new RequestNonceResponse
//         {
//             Nonce = nonce,
//             SiweMessage = siweMessage
//         };
//     }

// }


using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Auth.DTOs;
using RealEstateInvesting.Application.Auth.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Infrastructure.Persistence;

namespace RealEstateInvesting.Infrastructure.Identity;

public class WalletNonceService : IWalletNonceService
{
    private readonly AppDbContext _db;

    public WalletNonceService(AppDbContext dbContext)
    {
        _db = dbContext;
    }

    public async Task<RequestNonceResponse> GenerateNonceAsync(
        RequestNonceRequest request)
    {
        var wallet = request.WalletAddress.ToLowerInvariant();

        // 1️⃣ Invalidate old nonces for this wallet
        var oldNonces = await _db.WalletNonces
            .Where(x => x.WalletAddress == wallet && !x.IsUsed)
            .ToListAsync();

        foreach (var n in oldNonces)
            n.MarkUsed();

        // 2️⃣ Generate new nonce
        var nonceValue = Guid.NewGuid().ToString("N");

        // 3️⃣ Create entity (NO message stored)
        var entity = WalletNonce.Create(
            wallet,
            request.ChainId,
            nonceValue
        );

        // ❗ BUG FIX: you were adding `nonce` (undefined)
        _db.WalletNonces.Add(entity);
        await _db.SaveChangesAsync();

        // 4️⃣ Return nonce to frontend
        return new RequestNonceResponse
        {
            Nonce = nonceValue
        };
    }
}
