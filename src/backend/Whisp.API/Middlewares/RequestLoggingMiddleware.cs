using System.Diagnostics;

namespace Whisp.API.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path;

        logger.LogInformation("Incoming request: {Method} {Path}", method, path);

        var stopwatch = Stopwatch.StartNew();

        await next(context);

        stopwatch.Stop();

        logger.LogInformation("Response: {StatusCode} {Method} {Path} in {ElapsedMs}ms",
            context.Response.StatusCode, method, path, stopwatch.ElapsedMilliseconds);
    }
}
