using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace IncidentManagement.Api.Filters;

/// <summary>
/// Global exception handler that translates all unhandled exceptions into standardized Problem Details responses.
/// Implements IExceptionHandler (introduced in .NET 8+) for cleaner exception handling in the middleware pipeline.
/// This is AOP (Aspect-Oriented Programming) - cross-cutting concern applied globally without controller pollution.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        // Extract correlation ID from HttpContext for tracing
        var correlationId = httpContext.Items["CorrelationId"]?.ToString() ?? "N/A";

        // Log the exception with correlation ID for distributed tracing
        _logger.LogError(exception, 
            "Unhandled exception occurred. CorrelationId: {CorrelationId}", 
            correlationId);

        // Map exception types to appropriate HTTP status codes and problem details
        var (statusCode, title, detail) = MapExceptionToResponse(exception);

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path,
            Extensions =
            {
                ["correlationId"] = correlationId,
                ["timestamp"] = DateTime.UtcNow
            }
        };

        // Add traceId for development/debugging
        if (httpContext.RequestServices.GetService<IHostEnvironment>()?.IsDevelopment() == true)
        {
            problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Return true to indicate the exception has been handled
        return true;
    }

    /// <summary>
    /// Maps exception types to HTTP status codes and user-friendly messages.
    /// Staff-level pattern: Avoid leaking internal details to clients in production.
    /// </summary>
    private static (HttpStatusCode StatusCode, string Title, string Detail) MapExceptionToResponse(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => (HttpStatusCode.BadRequest, 
                "Invalid Input", 
                "A required parameter was not provided."),

            ArgumentException => (HttpStatusCode.BadRequest, 
                "Invalid Input", 
                exception.Message),

            InvalidOperationException => (HttpStatusCode.Conflict, 
                "Operation Not Allowed", 
                "The requested operation cannot be performed in the current state."),

            UnauthorizedAccessException => (HttpStatusCode.Forbidden, 
                "Access Denied", 
                "You do not have permission to perform this action."),

            KeyNotFoundException => (HttpStatusCode.NotFound, 
                "Resource Not Found", 
                "The requested resource could not be found."),

            TimeoutException => (HttpStatusCode.RequestTimeout, 
                "Request Timeout", 
                "The operation took too long to complete."),

            // Default for any unhandled exception type
            _ => (HttpStatusCode.InternalServerError, 
                "Internal Server Error", 
                "An unexpected error occurred. Please contact support with the correlation ID.")
        };
    }
}
