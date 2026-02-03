using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.API.Contracts;
using RealEstateInvesting.Application.Auth.DTOs;
using RealEstateInvesting.Application.Auth.Interfaces;

namespace RealEstateInvesting.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IWalletNonceService _walletNonceService;
    private readonly IWalletAuthService _walletAuthService;

    public AuthController(
        IWalletNonceService walletNonceService,
        IWalletAuthService walletAuthService)
    {
        _walletNonceService = walletNonceService;
        _walletAuthService = walletAuthService;
    }

    /// <summary>
    /// Step 1: Generate nonce for wallet authentication
    /// </summary>
    // [HttpPost("wallet/nonce")]
    // public async Task<IActionResult> RequestNonce(
    //     [FromBody] RequestNonceRequest request)
    // {   
    //     Console.WriteLine("======================= chain id "+ request.ChainId);
    //     Console.WriteLine("==========================Walletaddress==="+ request.WalletAddress);
    //     var response = await _walletNonceService.GenerateNonceAsync(request);
    //     Console.WriteLine("==============================================");
    //      Console.WriteLine("APi hitted");
    //      Console.WriteLine("==============================================");
    //     return Ok(ApiResponse<RequestNonceResponse>.Success(
    //         response,
    //         "Nonce generated successfully"
    //     ));
    // }

    /// <summary>
    /// Step 2: Verify wallet signature & issue JWT
    /// </summary>
    /// 
    /// 
    [HttpPost("wallet/nonce")]
    public async Task<IActionResult> RequestNonce()
    {
        var response = await _walletNonceService.GenerateNonceAsync();

        return Ok(ApiResponse<RequestNonceResponse>.Success(
            response,
            "Nonce generated successfully"
        ));
    }

    [HttpPost("wallet/verify")]
    public async Task<IActionResult> VerifyWallet(
        [FromBody] VerifyWalletRequest request)
    {
        var response = await _walletAuthService.VerifySignatureAsync(request);

        return Ok(ApiResponse<AuthResponse>.Success(
            response,
            "Wallet authenticated successfully"
        ));
    }
}
