using MediatR;

namespace HealthTech.Application.FhirResources.Queries;

/// <summary>
/// Query to get a FHIR resource by type and ID
/// </summary>
public record GetFhirResourceQuery : IRequest<GetFhirResourceResponse?>
{
    /// <summary>
    /// FHIR resource type
    /// </summary>
    public string ResourceType { get; init; } = string.Empty;

    /// <summary>
    /// FHIR resource ID
    /// </summary>
    public string FhirId { get; init; } = string.Empty;
}

/// <summary>
/// Response for getting FHIR resource
/// </summary>
public record GetFhirResourceResponse
{
    /// <summary>
    /// FHIR resource ID
    /// </summary>
    public string FhirId { get; init; } = string.Empty;

    /// <summary>
    /// FHIR resource version
    /// </summary>
    public int VersionId { get; init; }

    /// <summary>
    /// FHIR resource JSON
    /// </summary>
    public string ResourceJson { get; init; } = string.Empty;

    /// <summary>
    /// Last updated timestamp
    /// </summary>
    public DateTime? LastUpdated { get; init; }
}
