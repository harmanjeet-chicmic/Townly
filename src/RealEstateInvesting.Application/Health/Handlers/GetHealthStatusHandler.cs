using RealEstateInvesting.Application.Health.Queries;

namespace RealEstateInvesting.Application.Health.Handlers;

public sealed class GetHealthStatusHandler
{
    private readonly IHealthCheckService _healthCheckService;

    public GetHealthStatusHandler(IHealthCheckService healthCheckService)
    {   
        _healthCheckService = healthCheckService;
    }

    public Task<HealthStatusResult> HandleAsync(
        CancellationToken cancellationToken)
        => _healthCheckService.CheckAsync(cancellationToken);
}
