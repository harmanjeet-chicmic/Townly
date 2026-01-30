// using Microsoft.EntityFrameworkCore;
// using Nethereum.Signer;
// using RealEstateInvesting.Application.Auth.DTOs;
// using RealEstateInvesting.Application.Auth.Interfaces;
// using RealEstateInvesting.Application.Common.Interfaces;
// using RealEstateInvesting.Domain.Entities;
// using RealEstateInvesting.Infrastructure.Persistence;
// using Nethereum.Signer;
// namespace RealEstateInvesting.Infrastructure.Identity;

// public class WalletAuthService : IWalletAuthService
// {
//     private readonly AppDbContext _db;
//     private readonly IJwtService _jwtService;

//     public WalletAuthService(AppDbContext db, IJwtService jwtService)
//     {
//         _db = db;
//         _jwtService = jwtService;
//     }



//     public async Task<AuthResponse> VerifySignatureAsync(
//         VerifyWalletRequest request)
//     {
//         var wallet = request.WalletAddress.ToLowerInvariant();

//         var nonce = await _db.WalletNonces
//             .Where(x => x.WalletAddress == wallet && !x.IsUsed)
//             .OrderByDescending(x => x.CreatedAt)
//             .FirstOrDefaultAsync();

//         if (nonce == null || nonce.IsExpired())
//             throw new InvalidOperationException("Nonce invalid");

//         // 🔑 VERIFY EXACT STORED MESSAGE
//         var signer = new EthereumMessageSigner();
//         var recovered = signer
//             .EncodeUTF8AndEcRecover(nonce.SiweMessage, request.Signature)
//             .ToLowerInvariant();
//         Console.WriteLine("=======================================================");
//         Console.WriteLine("Recovered Addrress:"+recovered);
//         Console.WriteLine("Wallet Address : "+wallet);
//         Console.WriteLine("===================================================");
//         if (recovered != wallet)
//             throw new InvalidOperationException("Invalid wallet signature");

//         if (request.ChainId != nonce.ChainId)
//             throw new InvalidOperationException("Chain mismatch");

//         nonce.MarkUsed();

//         var user = await _db.Users
//             .FirstOrDefaultAsync(u => u.WalletAddress == wallet);

//         if (user == null)
//         {
//             user = User.Create(wallet, nonce.ChainId);
//             _db.Users.Add(user);
//         }

//         user.UpdateLastLogin();
//         await _db.SaveChangesAsync();

//         return new AuthResponse
//         {
//             AccessToken = _jwtService.GenerateToken(user),
//             ExpiresAt = DateTime.UtcNow.AddHours(6)
//         };
//     }

// }
using Microsoft.EntityFrameworkCore;
using Nethereum.Signer;
using RealEstateInvesting.Application.Auth.DTOs;
using RealEstateInvesting.Application.Auth.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Infrastructure.Persistence;
using RealEstateInvesting.Application.Common.Interfaces;
namespace RealEstateInvesting.Infrastructure.Identity;

public class WalletAuthService : IWalletAuthService
{
    private readonly AppDbContext _db;
    private readonly IJwtService _jwtService;

    public WalletAuthService(AppDbContext db, IJwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> VerifySignatureAsync(
        VerifyWalletRequest request)
    {
        var wallet = request.WalletAddress.ToLowerInvariant();

        var nonce = await _db.WalletNonces
            .Where(x =>
                x.WalletAddress == wallet &&
                x.ChainId == request.ChainId &&
                !x.IsUsed)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (nonce == null || nonce.IsExpired())
            throw new InvalidOperationException("Nonce invalid or expired");

        // 🔑 VERIFY EXACT SIGNED MESSAGE
        var signer = new EthereumMessageSigner();
        var recovered = signer
            .EncodeUTF8AndEcRecover(request.Message, request.Signature)
            .ToLowerInvariant();

        Console.WriteLine("=================================");
        Console.WriteLine($"Recovered : {recovered}");
        Console.WriteLine($"Expected  : {wallet}");
        Console.WriteLine("=================================");

        if (recovered != wallet)
            throw new InvalidOperationException("Invalid wallet signature");

        // Ensure nonce is present inside message (anti-replay)
        if (!request.Message.Contains($"Nonce: {nonce.Nonce}"))
            throw new InvalidOperationException("Nonce mismatch");

        nonce.MarkUsed();

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.WalletAddress == wallet);

        if (user == null)
        {
            user = User.Create(wallet, request.ChainId);
            _db.Users.Add(user);
        }

        user.UpdateLastLogin();
        await _db.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = _jwtService.GenerateToken(user),
            ExpiresAt = DateTime.UtcNow.AddHours(6)
        };
    }
}
