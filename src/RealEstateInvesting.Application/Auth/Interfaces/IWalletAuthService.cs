using RealEstateInvesting.Application.Auth.DTOs;

namespace RealEstateInvesting.Application.Auth.Interfaces;

public interface IWalletAuthService
{
    Task<AuthResponse> VerifySignatureAsync(VerifyWalletRequest request);
}
