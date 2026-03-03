using Townly.AI.Infrastructure.HttpClients;
using Townly.AI.Services.Interfaces;

namespace Townly.AI.Services;

public class PortfolioContextBuilder : IPortfolioContextBuilder
{
    private readonly TownlyApiClient _apiClient;

    public PortfolioContextBuilder(TownlyApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<string> BuildAsync(Guid userId)
    {
        var summaryJson = await _apiClient.GetPortfolioSummaryAsync();
        var investmentsJson = await _apiClient.GetInvestmentsAsync();

        // For now, we pass structured JSON directly
        // Later we compress properly

        return $"""
        Portfolio Summary:
        {summaryJson}

        Investments:
        {investmentsJson}
        """;
    }
}