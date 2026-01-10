using System.ComponentModel.DataAnnotations;

namespace IncidentManagement.Api.DTOs;

/// <summary>
/// Request DTO for creating a new incident ticket.
/// Demonstrates Data Annotations for declarative validation (Fail Fast strategy).
/// </summary>
public class CreateIncidentRequest
{
    /// <summary>
    /// User's description of the incident.
    /// This is the raw input that will be sent to AI enrichment services.
    /// </summary>
    [Required(ErrorMessage = "User description is required.")]
    [StringLength(5000, MinimumLength = 10, 
        ErrorMessage = "Description must be between 10 and 5000 characters.")]
    public string UserDescription { get; set; } = string.Empty;

    /// <summary>
    /// Optional manual severity override.
    /// If not provided, AI will determine severity.
    /// </summary>
    public IncidentManagement.Api.Models.IncidentSeverity? ManualSeverity { get; set; }
}

/// <summary>
/// Response DTO for incident creation.
/// Follows REST best practices - returns created resource with enriched data.
/// </summary>
public class IncidentResponse
{
    public Guid Id { get; set; }
    public string UserDescription { get; set; } = string.Empty;
    public string? StructuredSummary { get; set; }
    public string Severity { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
}
