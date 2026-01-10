using IncidentManagement.Api.Models;

namespace IncidentManagement.Api.Interfaces;

/// <summary>
/// CRITICAL ABSTRACTION FOR AI/LLM INTEGRATION.
/// 
/// This interface represents the integration point for AI-powered incident enrichment.
/// Current implementation is a mock, but this is designed to be swapped with:
/// - Semantic Kernel agents
/// - LangChain workflows
/// - Azure OpenAI Service
/// - Custom LLM orchestrators
/// 
/// The abstraction allows us to:
/// 1. Develop and test the API pipeline without LLM dependencies
/// 2. Swap implementations without changing controller code (Dependency Inversion Principle)
/// 3. A/B test different LLM models/prompts by injecting different implementations
/// 4. Mock for unit testing
/// 
/// FUTURE IMPLEMENTATION NOTES:
/// - Add retry policies for LLM API calls (Polly)
/// - Implement circuit breakers for fault tolerance
/// - Add telemetry for LLM latency tracking (correlation ID is key here)
/// - Consider streaming responses for real-time UI updates
/// </summary>
public interface IIncidentEnricher
{
    /// <summary>
    /// Enriches a raw incident description with AI-generated insights.
    /// </summary>
    /// <param name="description">Raw user-provided incident description</param>
    /// <param name="correlationId">Correlation ID for distributed tracing</param>
    /// <returns>Enriched incident data with structured summary, severity, and tags</returns>
    Task<EnrichmentResult> EnrichAsync(string description, string correlationId);
}

/// <summary>
/// Result object returned by the enrichment service.
/// Structured to contain all AI-generated metadata.
/// </summary>
public class EnrichmentResult
{
    /// <summary>
    /// AI-generated structured summary of the incident
    /// </summary>
    public string StructuredSummary { get; set; } = string.Empty;

    /// <summary>
    /// AI-determined severity level
    /// </summary>
    public IncidentSeverity Severity { get; set; }

    /// <summary>
    /// AI-extracted tags/categories
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Time taken by AI processing (for performance monitoring)
    /// </summary>
    public TimeSpan ProcessingDuration { get; set; }

    /// <summary>
    /// Confidence score from the AI model (0.0 - 1.0)
    /// FUTURE: Use this to flag low-confidence results for human review
    /// </summary>
    public double ConfidenceScore { get; set; }
}
