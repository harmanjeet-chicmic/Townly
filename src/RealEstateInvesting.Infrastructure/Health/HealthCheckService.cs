using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Application.Health.Queries;
using Microsoft.Extensions.Configuration;
using RealEstateInvesting.Infrastructure.Persistence;
namespace RealEstateInvesting.Infrastructure.Health;

public sealed class HealthCheckService : IHealthCheckService
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public HealthCheckService(
        AppDbContext dbContext,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<HealthStatusResult> CheckAsync(
        CancellationToken cancellationToken)
    {
        var checks = new Dictionary<string, string>();

        // ✅ Database check
        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync(
                "SELECT 1",
                cancellationToken);

            checks["database"] = "Ok";
        }
        catch
        {
            checks["database"] = "Failed";
        }

        // ✅ Qdrant config check (lightweight)
        var qdrantUrl = _configuration["Qdrant:Url"];
        checks["qdrant"] = string.IsNullOrWhiteSpace(qdrantUrl)
            ? "NotConfigured"
            : "Configured";

        var overallStatus = checks.Values.Any(v => v == "Failed")
            ? "Unhealthy"
            : "Healthy";

        return new HealthStatusResult
        {
            Status = overallStatus,
            Checks = checks
        };
    }
}
