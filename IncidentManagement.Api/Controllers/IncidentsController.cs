using Asp.Versioning;
using IncidentManagement.Api.DTOs;
using IncidentManagement.Api.Interfaces;
using IncidentManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IncidentManagement.Api.Controllers;

/// <summary>
/// Incident Management Controller - Demonstrates AI/LLM Integration Readiness
/// 
/// ARCHITECTURAL HIGHLIGHTS:
/// 1. Uses IIncidentEnricher abstraction for future LLM integration
/// 2. Correlation ID flows through entire pipeline for distributed tracing
/// 3. Structured logging captures AI latency metrics
/// 4. Domain model separates raw input from AI-enriched data
/// 5. Validation handled by global filter (Fail Fast)
/// 
/// FUTURE AI INTEGRATION POINTS:
/// - Swap MockIncidentEnricher with Semantic Kernel/LangChain implementation
/// - Add streaming responses for real-time AI feedback to UI
/// - Implement confidence thresholds for human-in-the-loop scenarios
/// - Add A/B testing for different LLM models/prompts
/// </summary>
[ApiController]
[Route("incidents")]
[ApiVersion("1.0")]
public class IncidentsController : ControllerBase
{
    private readonly IIncidentEnricher _incidentEnricher;
    private readonly ILogger<IncidentsController> _logger;

    // In-memory storage for demo purposes
    // PRODUCTION: Replace with repository pattern + EF Core / Cosmos DB
    private static readonly List<IncidentTicket> _incidents = new();

    public IncidentsController(
        IIncidentEnricher incidentEnricher,
        ILogger<IncidentsController> logger)
    {
        _incidentEnricher = incidentEnricher;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new incident with AI-powered enrichment
    /// </summary>
    /// <remarks>
    /// WORKFLOW:
    /// 1. Validate input (handled by ValidationActionFilter)
    /// 2. Extract correlation ID from middleware
    /// 3. Call IIncidentEnricher.EnrichAsync (FUTURE: LLM integration point)
    /// 4. Create domain model with enriched data
    /// 5. Save and return response
    /// 
    /// OBSERVABILITY:
    /// - Correlation ID is logged at every step
    /// - AI enrichment duration is captured
    /// - All logs are structured for easy querying
    /// 
    /// This enables questions like:
    /// "Show me all incidents where AI enrichment took > 2 seconds"
    /// "What's the P95 latency for LLM calls in production?"
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(IncidentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateIncident([FromBody] CreateIncidentRequest request)
    {
        // Extract correlation ID from middleware context
        // This ID will flow through to the AI enrichment service for distributed tracing
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();

        _logger.LogInformation(
            "Incident creation started. CorrelationId: {CorrelationId}, DescriptionLength: {Length}",
            correlationId,
            request.UserDescription.Length);

        // Track end-to-end latency
        var totalStopwatch = Stopwatch.StartNew();

        // CRITICAL: This is where AI magic happens (currently mocked)
        // FUTURE: This will call Azure OpenAI, Semantic Kernel, LangChain, etc.
        var enrichmentStopwatch = Stopwatch.StartNew();
        var enrichmentResult = await _incidentEnricher.EnrichAsync(request.UserDescription, correlationId);
        enrichmentStopwatch.Stop();

        // Log AI enrichment performance - crucial for production monitoring
        _logger.LogInformation(
            "AI enrichment completed. CorrelationId: {CorrelationId}, " +
            "Severity: {Severity}, Tags: {Tags}, " +
            "EnrichmentDuration: {Duration}ms, Confidence: {Confidence}",
            correlationId,
            enrichmentResult.Severity,
            string.Join(", ", enrichmentResult.Tags),
            enrichmentStopwatch.ElapsedMilliseconds,
            enrichmentResult.ConfidenceScore);

        // Create domain model with enriched data
        var incident = new IncidentTicket
        {
            Id = Guid.NewGuid(),
            UserDescription = request.UserDescription,
            StructuredSummary = enrichmentResult.StructuredSummary,
            Severity = request.ManualSeverity ?? enrichmentResult.Severity, // Manual override takes precedence
            Tags = enrichmentResult.Tags,
            CreatedAt = DateTime.UtcNow,
            CorrelationId = correlationId
        };

        // Save to in-memory store (PRODUCTION: Use repository pattern)
        _incidents.Add(incident);

        totalStopwatch.Stop();

        _logger.LogInformation(
            "Incident created successfully. CorrelationId: {CorrelationId}, " +
            "IncidentId: {IncidentId}, TotalDuration: {Duration}ms",
            correlationId,
            incident.Id,
            totalStopwatch.ElapsedMilliseconds);

        // Map to response DTO
        var response = new IncidentResponse
        {
            Id = incident.Id,
            UserDescription = incident.UserDescription,
            StructuredSummary = incident.StructuredSummary,
            Severity = incident.Severity.ToString(),
            Tags = incident.Tags,
            CreatedAt = incident.CreatedAt,
            CorrelationId = incident.CorrelationId
        };

        return CreatedAtAction(nameof(GetIncident), new { id = incident.Id }, response);
    }

    /// <summary>
    /// Retrieves an incident by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IncidentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetIncident(Guid id)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";

        _logger.LogInformation(
            "Retrieving incident. CorrelationId: {CorrelationId}, IncidentId: {IncidentId}",
            correlationId,
            id);

        var incident = _incidents.FirstOrDefault(i => i.Id == id);

        if (incident == null)
        {
            _logger.LogWarning(
                "Incident not found. CorrelationId: {CorrelationId}, IncidentId: {IncidentId}",
                correlationId,
                id);

            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Incident Not Found",
                Detail = $"No incident exists with ID: {id}",
                Instance = HttpContext.Request.Path,
                Extensions = { ["correlationId"] = correlationId }
            });
        }

        var response = new IncidentResponse
        {
            Id = incident.Id,
            UserDescription = incident.UserDescription,
            StructuredSummary = incident.StructuredSummary,
            Severity = incident.Severity.ToString(),
            Tags = incident.Tags,
            CreatedAt = incident.CreatedAt,
            CorrelationId = incident.CorrelationId
        };

        return Ok(response);
    }

    /// <summary>
    /// Lists all incidents (with optional filtering)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<IncidentResponse>), StatusCodes.Status200OK)]
    public IActionResult ListIncidents([FromQuery] string? severity = null)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";

        _logger.LogInformation(
            "Listing incidents. CorrelationId: {CorrelationId}, Filter: {Severity}",
            correlationId,
            severity ?? "none");

        var query = _incidents.AsEnumerable();

        // Optional severity filter
        if (!string.IsNullOrEmpty(severity) && Enum.TryParse<IncidentSeverity>(severity, true, out var severityEnum))
        {
            query = query.Where(i => i.Severity == severityEnum);
        }

        var response = query.Select(i => new IncidentResponse
        {
            Id = i.Id,
            UserDescription = i.UserDescription,
            StructuredSummary = i.StructuredSummary,
            Severity = i.Severity.ToString(),
            Tags = i.Tags,
            CreatedAt = i.CreatedAt,
            CorrelationId = i.CorrelationId
        }).ToList();

        return Ok(response);
    }
}
