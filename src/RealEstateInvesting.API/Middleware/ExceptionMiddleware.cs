using System.Net;
using System.Text.Json;
using RealEstateInvesting.API.Contracts;

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
