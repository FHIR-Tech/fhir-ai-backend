using MediatR;

namespace HealthTech.Application.FhirResources.Queries;

/// <summary>
/// Query to export FHIR resources as a bundle
/// </summary>
public record ExportFhirBundleQuery : IRequest<ExportFhirBundleResponse>
{
    /// <summary>
    /// FHIR resource type to export (optional - if null, exports all types)
    /// </summary>
    public string? ResourceType { get; init; }

    /// <summary>
    /// Specific FHIR resource IDs to export (optional - if null, exports all resources of the type)
    /// </summary>
    public IEnumerable<string>? FhirIds { get; init; }

    /// <summary>
    /// Search parameters to filter resources
    /// </summary>
    public Dictionary<string, string> SearchParameters { get; init; } = new();

    /// <summary>
    /// Maximum number of resources to include in the bundle
    /// </summary>
    public int MaxResources { get; init; } = 1000;

    /// <summary>
    /// Bundle type (collection, transaction, batch, searchset, history)
    /// </summary>
    public string BundleType { get; init; } = "collection";

    /// <summary>
    /// Include resource history in the bundle
    /// </summary>
    public bool IncludeHistory { get; init; } = false;

    /// <summary>
    /// Maximum number of history versions per resource
    /// </summary>
    public int MaxHistoryVersions { get; init; } = 10;

    /// <summary>
    /// Include deleted resources in the bundle
    /// </summary>
    public bool IncludeDeleted { get; init; } = false;

    /// <summary>
    /// Export format (json, xml)
    /// </summary>
    public string Format { get; init; } = "json";

    // === NEW PARAMETERS FOR TIME-BASED FILTERING ===

    /// <summary>
    /// Start date for time-based filtering (ISO 8601 format)
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// End date for time-based filtering (ISO 8601 format)
    /// </summary>
    public DateTime? EndDate { get; init; }

    /// <summary>
    /// Time period for filtering (days, weeks, months, years)
    /// </summary>
    public string? TimePeriod { get; init; }

    /// <summary>
    /// Number of time periods to look back
    /// </summary>
    public int? TimePeriodCount { get; init; }

    /// <summary>
    /// Observation code for filtering specific lab results
    /// </summary>
    public string? ObservationCode { get; init; }

    /// <summary>
    /// Observation system for filtering specific lab results
    /// </summary>
    public string? ObservationSystem { get; init; }

    /// <summary>
    /// Patient ID for filtering observations
    /// </summary>
    public string? PatientId { get; init; }

    /// <summary>
    /// Limit number of observations per patient
    /// </summary>
    public int? MaxObservationsPerPatient { get; init; }

    /// <summary>
    /// Sort order for observations (asc, desc)
    /// </summary>
    public string SortOrder { get; init; } = "desc";

    /// <summary>
    /// Include only latest observations per patient
    /// </summary>
    public bool LatestOnly { get; init; } = false;
}

/// <summary>
/// Response for exporting FHIR bundle
/// </summary>
public record ExportFhirBundleResponse
{
    /// <summary>
    /// FHIR Bundle JSON
    /// </summary>
    public string BundleJson { get; init; } = string.Empty;

    /// <summary>
    /// Bundle metadata
    /// </summary>
    public BundleMetadata Metadata { get; init; } = new();

    /// <summary>
    /// Bundle metadata
    /// </summary>
    public record BundleMetadata
    {
        /// <summary>
        /// Bundle type
        /// </summary>
        public string BundleType { get; init; } = string.Empty;

        /// <summary>
        /// Total number of resources in the bundle
        /// </summary>
        public int TotalResources { get; init; }

        /// <summary>
        /// Number of resource types included
        /// </summary>
        public int ResourceTypesCount { get; init; }

        /// <summary>
        /// Resource type breakdown
        /// </summary>
        public Dictionary<string, int> ResourceTypeBreakdown { get; init; } = new();

        /// <summary>
        /// Export timestamp
        /// </summary>
        public DateTime ExportTimestamp { get; init; }

        /// <summary>
        /// Export duration in milliseconds
        /// </summary>
        public long ExportDurationMs { get; init; }

        /// <summary>
        /// Bundle size in bytes
        /// </summary>
        public long BundleSizeBytes { get; init; }
    }
}
