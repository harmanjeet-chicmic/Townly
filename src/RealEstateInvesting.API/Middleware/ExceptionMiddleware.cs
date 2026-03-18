using System.Net;
using System.Text.Json;
using RealEstateInvesting.API.Contracts;
using RealEstateInvesting.Application.Common.Exceptions;

namespace RealEstateInvesting.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "Unauthorized access exception");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

            var response = ApiResponse<object>.Failure(
                "Unauthorized access",
                context.Response.StatusCode,
                new List<ApiError>
                {
                    new ApiError
                    {
                        Code = "UNAUTHORIZED",
                        Message = ex.Message
                    }
                });

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch(InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation exception");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var response = ApiResponse<object>.Failure(
                "Invalid operation",
                context.Response.StatusCode,
                new List<ApiError>
                {
                    new ApiError
                    {
                        Code = "INVALID_OPERATION",
                        Message = ex.Message
                    }
                });

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch(Exception ex) when (ex is NotFoundException || ex is FileNotFoundException)
        {
            _logger.LogError(ex, "Resource not found");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;

            var response = ApiResponse<object>.Failure(
                "Resource not found",
                context.Response.StatusCode,
                new List<ApiError>
                {
                    new ApiError
                    {
                        Code = "NOT_FOUND",
                        Message = ex.Message
                    }
                });

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = ApiResponse<object>.Failure(
                "Something went wrong",
                context.Response.StatusCode,
                new List<ApiError>
                {
                    new ApiError
                    {
                        Code = "SERVER_ERROR",
                        Message = ex.Message
                    }
                });

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
