namespace RealEstateInvesting.API.Contracts;

/// <summary>Request for Flow 6: Sell shares on chain.</summary>
public class SellSharesOnChainRequest
{
    /// <summary>Property ERC-20 token address.</summary>
    public string PropertyTokenAddress { get; set; } = string.Empty;

    /// <summary>Amount of shares to sell in token smallest units (e.g. 18 decimals). Example: "10000000000000000000" for 10 shares.</summary>
    public string AmountOfSharesRaw { get; set; } = string.Empty;
}
