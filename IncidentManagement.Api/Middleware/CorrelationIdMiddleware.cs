namespace IncidentManagement.Api.Middleware;

/// <summary>
/// Middleware to attach a unique correlation ID to every request/response.
/// This ID flows through the entire request pipeline and is crucial for distributed tracing,
/// especially when integrating with AI/LLM services where we need to correlate enrichment latency.
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;
    private const string CorrelationIdHeader = "X-Correlation-Id";

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if correlation ID already exists in request headers (e.g., from API Gateway)
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

        // Store in HttpContext.Items for access throughout the request pipeline
        context.Items["CorrelationId"] = correlationId;

        // Add to response headers for client-side tracing
        context.Response.Headers.TryAdd(CorrelationIdHeader, correlationId);

        // Enrich structured logging context so all logs in this request include the correlation ID
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            _logger.LogInformation("Request started: {Method} {Path}", 
                context.Request.Method, 
                context.Request.Path);

            try
            {
                await _next(context);
            }
            finally
            {
                _logger.LogInformation("Request completed: {Method} {Path} - Status: {StatusCode}", 
                    context.Request.Method, 
                    context.Request.Path,
                    context.Response.StatusCode);
            }
        }
    }
}

/// <summary>
/// Extension method to register the middleware in the pipeline
/// </summary>
public static class CorrelationIdMiddlewareExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorrelationIdMiddleware>();
    }
}
