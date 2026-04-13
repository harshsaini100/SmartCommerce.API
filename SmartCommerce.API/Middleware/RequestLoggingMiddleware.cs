public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // BEFORE request goes to next middleware / controller
        _logger.LogInformation("Incoming Request: {method} {url}",
            context.Request.Method,
            context.Request.Path);

        await _next(context); // 🔥 VERY IMPORTANT

        // AFTER response comes back
        _logger.LogInformation("Outgoing Response: {statusCode}",
            context.Response.StatusCode);
    }
}