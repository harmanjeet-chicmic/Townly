using System.Security.Claims;
using System.Text;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Enums;
using DomainLogLevel = RealEstateInvesting.Domain.Enums.LogLevel;
namespace RealEstateInvesting.Api.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;


    public RequestResponseLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ILogService logService)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

        context.Response.Headers["X-Correlation-ID"] = correlationId;

        var request = context.Request;

        string requestBody = "";

        request.EnableBuffering();

        if (request.ContentLength > 0)
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            requestBody = await reader.ReadToEndAsync();
            request.Body.Position = 0;
        }

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        string responseText = "";
        string? exceptionMessage = null;
        string? stackTrace = null;
        DomainLogLevel level = DomainLogLevel.Information;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            level = DomainLogLevel.Error;
            exceptionMessage = ex.Message;
            stackTrace = ex.StackTrace;

            throw;
        }
        finally
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            await responseBody.CopyToAsync(originalBodyStream);

            var endpoint = context.Request.Path.ToString();
            var method = context.Request.Method;
            var statusCode = context.Response.StatusCode;
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            Guid? userId = null;

            var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var parsedUserId))
            {
                userId = parsedUserId;
            }

            if (statusCode >= 500)
                level = DomainLogLevel.Error;
            else if (statusCode >= 400)
                level = DomainLogLevel.Warning;

            await logService.LogAsync(
                level,
                "API Request",
                exceptionMessage,
                stackTrace,
                endpoint,
                method,
                userId,
                requestBody,
                responseText,
                statusCode,
                ipAddress,
                correlationId
            );
        }
    }


}
