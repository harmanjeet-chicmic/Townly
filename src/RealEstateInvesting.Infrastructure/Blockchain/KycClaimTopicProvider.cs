using System.Text;
using RealEstateInvesting.Application.Common.Interfaces;

namespace RealEstateInvesting.Infrastructure.Blockchain;

/// <summary>
/// Returns keccak256("KYC_CLAIM") for T-REX claim config.
/// </summary>
public sealed class KycClaimTopicProvider : IKycClaimTopicProvider
{
    public string GetKycClaimTopicHex()
    {
        var hash = Nethereum.Util.Sha3Keccack.Current.CalculateHash(Encoding.UTF8.GetBytes("KYC_CLAIM"));
        return "0x" + Convert.ToHexString(hash).ToLowerInvariant();
    }
}
