namespace IncidentManagement.Api.Models;

/// <summary>
/// Domain model representing an incident ticket.
/// Designed with AI extensibility in mind - raw user input is preserved separately from AI-enriched data.
/// </summary>
public class IncidentTicket
{
    public Guid Id { get; set; }

    /// <summary>
    /// Raw user-provided description of the incident.
    /// This is the input to the AI enrichment pipeline.
    /// </summary>
    public string UserDescription { get; set; } = string.Empty;

    /// <summary>
    /// AI-generated structured summary (nullable until enrichment completes).
    /// FUTURE: This will be populated by LLM agents (Semantic Kernel, LangChain, etc.)
    /// </summary>
    public string? StructuredSummary { get; set; }

    /// <summary>
    /// Severity level of the incident.
    /// FUTURE: Auto-populated by AI analysis of UserDescription.
    /// </summary>
    public IncidentSeverity Severity { get; set; }

    /// <summary>
    /// Tags/categories extracted from the incident.
    /// FUTURE: Auto-populated by AI semantic analysis (e.g., ["network", "authentication", "prod"]).
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Timestamp of incident creation.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Correlation ID for distributed tracing across AI enrichment calls.
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;
}
