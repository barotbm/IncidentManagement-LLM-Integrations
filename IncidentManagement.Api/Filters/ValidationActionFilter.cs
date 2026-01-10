using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IncidentManagement.Api.Filters;

/// <summary>
/// Action filter that implements "Fail Fast" validation strategy.
/// Intercepts all controller actions and validates ModelState BEFORE controller logic executes.
/// This is an AOP technique to eliminate repetitive "if (!ModelState.IsValid)" checks in every action.
/// Applied globally via AddControllers() configuration in Program.cs.
/// </summary>
public class ValidationActionFilter : IActionFilter
{
    private readonly ILogger<ValidationActionFilter> _logger;

    public ValidationActionFilter(ILogger<ValidationActionFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Executes BEFORE the controller action runs.
    /// If model validation fails, short-circuits the pipeline and returns 400 Bad Request.
    /// </summary>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var correlationId = context.HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";

            // Extract validation errors for structured logging
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .Select(e => new
                {
                    Field = e.Key,
                    Errors = e.Value!.Errors.Select(x => x.ErrorMessage).ToArray()
                })
                .ToList();

            _logger.LogWarning(
                "Model validation failed. CorrelationId: {CorrelationId}, Errors: {@ValidationErrors}",
                correlationId,
                errors);

            // Return standardized validation problem details (RFC 7807 compliant)
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred.",
                Instance = context.HttpContext.Request.Path,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };

            // Add correlation ID for tracing
            problemDetails.Extensions["correlationId"] = correlationId;
            problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

            // Short-circuit the pipeline - controller action will NOT execute
            context.Result = new BadRequestObjectResult(problemDetails);
        }
    }

    /// <summary>
    /// Executes AFTER the controller action completes.
    /// No-op in this implementation - used for post-processing if needed.
    /// </summary>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Could add response enrichment here if needed (e.g., performance metrics)
    }
}
