namespace RealEstateInvesting.Application.Health.Queries;

public interface IHealthCheckService
{
    Task<HealthStatusResult> CheckAsync(
        CancellationToken cancellationToken);
}
