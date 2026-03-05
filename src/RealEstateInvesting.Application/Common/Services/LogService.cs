using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Common.Services;

public class LogService : ILogService
{
    private readonly ILogRepository _logRepository;
    private const int MaxLogs = 30;


    public LogService(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task LogAsync(
        LogLevel level,
        string message,
        string? exception,
        string? stackTrace,
        string? endpoint,
        string? httpMethod,
        Guid? userId,
        string? requestBody,
        string? responseBody,
        int? responseStatus,
        string? ipAddress,
        string correlationId)
    {
        var logCount = await _logRepository.CountAsync();

        if (logCount >= MaxLogs)
        {
            var oldestLog = await _logRepository.GetOldestAsync();
            if (oldestLog != null)
            {
                await _logRepository.DeleteAsync(oldestLog);
            }
        }

        var log = new Log
        {
            Id = Guid.NewGuid(),
            CorrelationId = correlationId,
            Level = level,
            Message = message,
            Exception = exception,
            StackTrace = stackTrace,
            Endpoint = endpoint,
            HttpMethod = httpMethod,
            UserId = userId,
            RequestBody = requestBody,
            ResponseBody = responseBody,
            ResponseStatus = responseStatus,
            IPAddress = ipAddress,
            CreatedAt = DateTime.UtcNow
        };

        await _logRepository.AddAsync(log);
    }


}
