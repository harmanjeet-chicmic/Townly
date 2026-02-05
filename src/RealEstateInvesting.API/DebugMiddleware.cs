namespace RealEstateInvesting.API.RequestDebugMiddleware;
public class RequestDebugMiddleware
{
    private readonly RequestDelegate _next;

    public RequestDebugMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/api/properties")
            && context.Request.Method == "POST")
        {   
            Console.WriteLine("==========================================");
            Console.WriteLine("===== INCOMING REQUEST =====");
            Console.WriteLine($"Method: {context.Request.Method}");
            Console.WriteLine($"Path: {context.Request.Path}");

            Console.WriteLine("Headers:");
            foreach (var header in context.Request.Headers)
            {
                Console.WriteLine($"{header.Key}: {header.Value}");
            }

            Console.WriteLine($"Content-Type: {context.Request.ContentType}");
            Console.WriteLine("============================");
        }

        await _next(context);
    }
}
