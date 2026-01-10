using System.ComponentModel.DataAnnotations;

namespace IncidentManagement.Api.DTOs;

/// <summary>
/// Request DTOs for Order API versioning demonstration.
/// </summary>
/// 
// V1 DTO - Simple order structure
public class CreateOrderV1Request
{
    [Required(ErrorMessage = "Customer name is required.")]
    [StringLength(100, MinimumLength = 2)]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Product name is required.")]
    public string ProductName { get; set; } = string.Empty;

    [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000.")]
    public int Quantity { get; set; }
}

// V2 DTO - Enhanced with additional fields for backward-incompatible changes
public class CreateOrderV2Request
{
    [Required(ErrorMessage = "Customer ID is required in V2.")]
    public Guid CustomerId { get; set; }

    [Required(ErrorMessage = "Product SKU is required in V2.")]
    [RegularExpression(@"^[A-Z]{3}-\d{4}$", ErrorMessage = "SKU must match format: ABC-1234")]
    public string ProductSKU { get; set; } = string.Empty;

    [Range(1, 10000, ErrorMessage = "Quantity must be between 1 and 10000.")]
    public int Quantity { get; set; }

    /// <summary>
    /// New field in V2 - demonstrates breaking change that requires version bump
    /// </summary>
    [Required]
    public string ShippingAddress { get; set; } = string.Empty;
}

public class OrderResponse
{
    public Guid OrderId { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public string ApiVersion { get; set; } = string.Empty;
}
