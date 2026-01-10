using Asp.Versioning;
using IncidentManagement.Api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace IncidentManagement.Api.Controllers.V1;

/// <summary>
/// Orders API - Version 1
/// Uses URL-based versioning: /v1/orders
/// 
/// ARCHITECTURAL NOTE:
/// V1 is maintained for backward compatibility because we assume old clients will NEVER update.
/// Mobile apps, embedded systems, and legacy integrations may be pinned to V1 indefinitely.
/// We must maintain this contract until all consumers are migrated or deprecated.
/// 
/// Breaking changes (like requiring CustomerId instead of CustomerName) necessitate a new version.
/// </summary>
[ApiController]
[Route("v{version:apiVersion}/orders")]
[ApiVersion("1.0")]
public class OrdersV1Controller : ControllerBase
{
    private readonly ILogger<OrdersV1Controller> _logger;

    public OrdersV1Controller(ILogger<OrdersV1Controller> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates a new order (V1 contract)
    /// </summary>
    /// <remarks>
    /// V1 uses simple string-based customer identification.
    /// This was adequate for initial launch but doesn't scale for enterprise customers.
    /// V2 addresses this with proper GUID-based customer references.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult CreateOrder([FromBody] CreateOrderV1Request request)
    {
        // Validation is handled by ValidationActionFilter - no need for manual checks
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";

        _logger.LogInformation(
            "Creating order V1. CorrelationId: {CorrelationId}, Customer: {CustomerName}, Product: {ProductName}",
            correlationId,
            request.CustomerName,
            request.ProductName);

        // Simulate order creation
        var response = new OrderResponse
        {
            OrderId = Guid.NewGuid(),
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            ApiVersion = "1.0"
        };

        return CreatedAtAction(nameof(GetOrder), new { id = response.OrderId }, response);
    }

    /// <summary>
    /// Retrieves an order by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetOrder(Guid id)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";

        _logger.LogInformation(
            "Retrieving order V1. CorrelationId: {CorrelationId}, OrderId: {OrderId}",
            correlationId,
            id);

        // Simulate retrieval
        var response = new OrderResponse
        {
            OrderId = id,
            Status = "Completed",
            CreatedAt = DateTime.UtcNow.AddHours(-2),
            ApiVersion = "1.0"
        };

        return Ok(response);
    }
}
