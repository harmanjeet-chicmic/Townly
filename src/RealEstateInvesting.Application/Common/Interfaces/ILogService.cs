using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Common.Interfaces;

public interface ILogService
{
    Task LogAsync(
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
    string correlationId
    );
}
