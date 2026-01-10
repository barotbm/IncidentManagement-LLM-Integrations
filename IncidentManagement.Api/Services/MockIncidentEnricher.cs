using IncidentManagement.Api.Interfaces;
using IncidentManagement.Api.Models;
using System.Diagnostics;

namespace IncidentManagement.Api.Services;

/// <summary>
/// Mock implementation of IIncidentEnricher for development and testing.
/// 
/// PRODUCTION REPLACEMENT PLAN:
/// Replace this with a real LLM integration service that:
/// 1. Calls Azure OpenAI / Semantic Kernel / LangChain
/// 2. Uses prompt engineering to extract severity, tags, and summaries
/// 3. Implements retry logic and circuit breakers
/// 4. Caches results for identical descriptions (optional optimization)
/// 
/// The beauty of this abstraction: controllers never need to change.
/// Just swap the DI registration in Program.cs:
///   services.AddScoped<IIncidentEnricher, SemanticKernelIncidentEnricher>();
/// </summary>
public class MockIncidentEnricher : IIncidentEnricher
{
    private readonly ILogger<MockIncidentEnricher> _logger;

    public MockIncidentEnricher(ILogger<MockIncidentEnricher> logger)
    {
        _logger = logger;
    }

    public async Task<EnrichmentResult> EnrichAsync(string description, string correlationId)
    {
        // Simulate AI processing latency (100-500ms)
        var stopwatch = Stopwatch.StartNew();
        await Task.Delay(Random.Shared.Next(100, 500));

        _logger.LogInformation(
            "Mock enrichment started. CorrelationId: {CorrelationId}, DescriptionLength: {Length}",
            correlationId,
            description.Length);

        // Mock logic: Simple keyword-based classification
        // FUTURE: Replace with actual LLM prompt like:
        // "Analyze this incident: {description}. Return JSON with severity, tags, and summary."
        var severity = DetermineMockSeverity(description);
        var tags = ExtractMockTags(description);
        var summary = GenerateMockSummary(description, severity);

        stopwatch.Stop();

        _logger.LogInformation(
            "Mock enrichment completed. CorrelationId: {CorrelationId}, Severity: {Severity}, Duration: {Duration}ms",
            correlationId,
            severity,
            stopwatch.ElapsedMilliseconds);

        return new EnrichmentResult
        {
            StructuredSummary = summary,
            Severity = severity,
            Tags = tags,
            ProcessingDuration = stopwatch.Elapsed,
            ConfidenceScore = 0.85 // Mock confidence score
        };
    }

    private static IncidentSeverity DetermineMockSeverity(string description)
    {
        var lowerDesc = description.ToLowerInvariant();

        if (lowerDesc.Contains("critical") || lowerDesc.Contains("down") || lowerDesc.Contains("outage"))
            return IncidentSeverity.Critical;

        if (lowerDesc.Contains("urgent") || lowerDesc.Contains("high") || lowerDesc.Contains("production"))
            return IncidentSeverity.High;

        if (lowerDesc.Contains("medium") || lowerDesc.Contains("issue"))
            return IncidentSeverity.Medium;

        return IncidentSeverity.Low;
    }

    private static List<string> ExtractMockTags(string description)
    {
        var tags = new List<string>();
        var lowerDesc = description.ToLowerInvariant();

        // Simple keyword matching - FUTURE: Use LLM semantic extraction
        if (lowerDesc.Contains("network") || lowerDesc.Contains("connection"))
            tags.Add("network");

        if (lowerDesc.Contains("auth") || lowerDesc.Contains("login") || lowerDesc.Contains("password"))
            tags.Add("authentication");

        if (lowerDesc.Contains("database") || lowerDesc.Contains("sql") || lowerDesc.Contains("query"))
            tags.Add("database");

        if (lowerDesc.Contains("performance") || lowerDesc.Contains("slow") || lowerDesc.Contains("timeout"))
            tags.Add("performance");

        if (lowerDesc.Contains("production") || lowerDesc.Contains("prod"))
            tags.Add("production");
        else if (lowerDesc.Contains("staging") || lowerDesc.Contains("test"))
            tags.Add("staging");

        // Default tag if nothing matches
        if (tags.Count == 0)
            tags.Add("general");

        return tags;
    }

    private static string GenerateMockSummary(string description, IncidentSeverity severity)
    {
        // FUTURE: This will be replaced with LLM-generated summary
        var truncated = description.Length > 100 
            ? description[..100] + "..." 
            : description;

        return $"[MOCK AI SUMMARY] Severity: {severity}. Issue: {truncated}";
    }
}
