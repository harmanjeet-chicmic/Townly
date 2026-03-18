namespace RealEstateInvesting.Application.Common.Interfaces;

/// <summary>
/// Provides the KYC_CLAIM topic (keccak256 of "KYC_CLAIM") for T-REX deployTREXSuite.
/// </summary>
public interface IKycClaimTopicProvider
{
    /// <summary>Returns the hex string (with 0x) for the KYC_CLAIM claim topic.</summary>
    string GetKycClaimTopicHex();
}
