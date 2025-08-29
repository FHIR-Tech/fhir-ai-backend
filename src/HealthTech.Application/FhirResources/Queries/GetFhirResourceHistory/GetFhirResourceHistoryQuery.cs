using MediatR;

namespace HealthTech.Application.FhirResources.Queries.GetFhirResourceHistory;

/// <summary>
/// Query to get version history of a FHIR resource
/// </summary>
public record GetFhirResourceHistoryQuery : IRequest<GetFhirResourceHistoryResponse>
{
    /// <summary>
    /// FHIR resource type
    /// </summary>
    public string ResourceType { get; init; } = string.Empty;

    /// <summary>
    /// FHIR resource ID
    /// </summary>
    public string FhirId { get; init; } = string.Empty;

    /// <summary>
    /// Maximum number of versions to return
    /// </summary>
    public int MaxVersions { get; init; } = 100;
}

/// <summary>
/// Response for getting FHIR resource history
/// </summary>
public record GetFhirResourceHistoryResponse
{
    /// <summary>
    /// FHIR resource type
    /// </summary>
    public string ResourceType { get; init; } = string.Empty;

    /// <summary>
    /// FHIR resource ID
    /// </summary>
    public string FhirId { get; init; } = string.Empty;

    /// <summary>
    /// List of resource versions
    /// </summary>
    public List<FhirResourceVersion> Versions { get; init; } = new();

    /// <summary>
    /// Total number of versions
    /// </summary>
    public int TotalCount { get; init; }
}

/// <summary>
/// FHIR resource version information
/// </summary>
public record FhirResourceVersion
{
    /// <summary>
    /// Version ID
    /// </summary>
    public int VersionId { get; init; }

    /// <summary>
    /// FHIR resource JSON at this version
    /// </summary>
    public string ResourceJson { get; init; } = string.Empty;

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Last updated timestamp
    /// </summary>
    public DateTime LastUpdated { get; init; }

    /// <summary>
    /// Whether this version is the current version
    /// </summary>
    public bool IsCurrentVersion { get; init; }

    /// <summary>
    /// Operation that created this version (create, update, delete)
    /// </summary>
    public string Operation { get; init; } = string.Empty;
}
