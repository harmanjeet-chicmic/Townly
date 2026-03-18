namespace RealEstateInvesting.API.Contracts;

/// <summary>Request for Flow 5: Buy shares on chain.</summary>
public class BuySharesOnChainRequest
{
    /// <summary>Property ERC-20 token address.</summary>
    public string PropertyTokenAddress { get; set; } = string.Empty;

    /// <summary>Amount of shares to buy in token smallest units (e.g. 18 decimals). Example: "10000000000000000000" for 10 shares.</summary>
    public string AmountOfSharesRaw { get; set; } = string.Empty;

    /// <summary>Stablecoin amount to approve for the marketplace in smallest units (e.g. 6 decimals). Example: "1000000" for 1 USDC.</summary>
    public string AmountStablecoinToApproveRaw { get; set; } = string.Empty;
}
