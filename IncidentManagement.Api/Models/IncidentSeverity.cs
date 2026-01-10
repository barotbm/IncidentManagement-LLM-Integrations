namespace IncidentManagement.Api.Models;

/// <summary>
/// Severity levels for incident classification.
/// Future AI integration point: LLM agents will analyze incident descriptions to auto-assign this.
/// </summary>
public enum IncidentSeverity
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}
