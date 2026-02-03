namespace RealEstateInvesting.Application.Health.Queries;

public sealed class HealthStatusResult
{
    public string Status { get; init; } = default!;
    public Dictionary<string, string> Checks { get; init; } = new();
}
