using Asp.Versioning;
using IncidentManagement.Api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace IncidentManagement.Api.Controllers.V2;

/// <summary>
/// Orders API - Version 2
/// Uses HEADER-based versioning: X-Version: 2.0
/// 
/// ARCHITECTURAL EVOLUTION:
/// V2 introduces breaking changes that couldn't be accommodated in V1:
/// 1. CustomerId (Guid) replaces CustomerName (string) for proper referential integrity
/// 2. ProductSKU with strict format validation replaces free-form ProductName
/// 3. ShippingAddress is now required (V1 assumed all orders were digital)
/// 
/// VERSIONING STRATEGY NOTES:
/// - URL versioning (V1) is easier for clients to discover and test
/// - Header versioning (V2) keeps URLs clean and is preferred for mature APIs
/// - Both strategies are demonstrated here for architectural reference
/// - Choose ONE strategy per organization for consistency
/// 
/// Why header-based here?
/// - Enterprise clients requested cleaner URLs for documentation
/// - Allows version negotiation without URL changes
/// - Better for API gateways that route based on headers
/// </summary>
[ApiController]
[Route("orders")]
[ApiVersion("2.0")]
public class OrdersV2Controller : ControllerBase
{
    private readonly ILogger<OrdersV2Controller> _logger;

    public OrdersV2Controller(ILogger<OrdersV2Controller> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates a new order (V2 contract - BREAKING CHANGES from V1)
    /// </summary>
    /// <remarks>
    /// This version requires:
    /// - X-Version: 2.0 header
    /// - CustomerId instead of CustomerName
    /// - Structured ProductSKU instead of free-form ProductName
    /// - ShippingAddress (new requirement)
    /// 
    /// Clients must explicitly opt-in to V2 by sending the version header.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult CreateOrder([FromBody] CreateOrderV2Request request)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";

        _logger.LogInformation(
            "Creating order V2. CorrelationId: {CorrelationId}, CustomerId: {CustomerId}, SKU: {ProductSKU}",
            correlationId,
            request.CustomerId,
            request.ProductSKU);

        // Simulate order creation with enhanced validation
        var response = new OrderResponse
        {
            OrderId = Guid.NewGuid(),
            Status = "Pending Shipment", // V2 has different status values
            CreatedAt = DateTime.UtcNow,
            ApiVersion = "2.0"
        };

        return CreatedAtAction(nameof(GetOrder), new { id = response.OrderId }, response);
    }

    /// <summary>
    /// Retrieves an order by ID (V2 endpoint)
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetOrder(Guid id)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";

        _logger.LogInformation(
            "Retrieving order V2. CorrelationId: {CorrelationId}, OrderId: {OrderId}",
            correlationId,
            id);

        var response = new OrderResponse
        {
            OrderId = id,
            Status = "Shipped",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            ApiVersion = "2.0"
        };

        return Ok(response);
    }
}
