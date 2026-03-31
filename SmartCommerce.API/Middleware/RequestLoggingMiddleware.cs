using System.Diagnostics;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        var method = context.Request.Method;
        var path = context.Request.Path;

        await _next(context);

        stopwatch.Stop();

        var statusCode = context.Response.StatusCode;

        Console.WriteLine(
            $"[{DateTime.UtcNow}] {method} {path} → {statusCode} in {stopwatch.ElapsedMilliseconds} ms"
        );
    }
}