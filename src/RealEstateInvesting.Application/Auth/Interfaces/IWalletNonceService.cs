using RealEstateInvesting.Application.Auth.DTOs;

namespace RealEstateInvesting.Application.Auth.Interfaces;

public interface IWalletNonceService
{
    Task<RequestNonceResponse> GenerateNonceAsync(RequestNonceRequest request);
}
