using MediatR;

namespace HealthTech.Application.FhirResources.Commands;

/// <summary>
/// Command to import FHIR resources from a FHIR Bundle
/// </summary>
public record ImportFhirBundleCommand : IRequest<ImportFhirBundleResponse>
{
    /// <summary>
    /// FHIR Bundle JSON content
    /// </summary>
    public string BundleJson { get; init; } = string.Empty;

    /// <summary>
    /// Whether to validate FHIR resources before import
    /// </summary>
    public bool ValidateResources { get; init; } = true;
}

/// <summary>
/// Response for importing FHIR Bundle
/// </summary>
public record ImportFhirBundleResponse
{
    /// <summary>
    /// Total number of resources processed
    /// </summary>
    public int TotalProcessed { get; init; }

    /// <summary>
    /// Number of successfully imported resources
    /// </summary>
    public int SuccessfullyImported { get; init; }

    /// <summary>
    /// Number of resources that failed to import
    /// </summary>
    public int FailedToImport { get; init; }

    /// <summary>
    /// Import job ID for tracking
    /// </summary>
    public string ImportJobId { get; init; } = string.Empty;

    /// <summary>
    /// List of imported resource IDs
    /// </summary>
    public List<ImportedResource> ImportedResources { get; init; } = new();

    /// <summary>
    /// List of import errors
    /// </summary>
    public List<ImportError> Errors { get; init; } = new();
}

/// <summary>
/// Information about an imported resource
/// </summary>
public record ImportedResource
{
    /// <summary>
    /// Resource type
    /// </summary>
    public string ResourceType { get; init; } = string.Empty;

    /// <summary>
    /// Resource ID
    /// </summary>
    public string FhirId { get; init; } = string.Empty;

    /// <summary>
    /// Import status
    /// </summary>
    public ImportStatus Status { get; init; }

    /// <summary>
    /// Error message if import failed
    /// </summary>
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// Import error details
/// </summary>
public record ImportError
{
    /// <summary>
    /// Resource type
    /// </summary>
    public string ResourceType { get; init; } = string.Empty;

    /// <summary>
    /// Original resource ID from bundle
    /// </summary>
    public string? OriginalId { get; init; }

    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Error code
    /// </summary>
    public string ErrorCode { get; init; } = string.Empty;
}

/// <summary>
/// Import status enumeration
/// </summary>
public enum ImportStatus
{
    /// <summary>
    /// Successfully imported
    /// </summary>
    Success,

    /// <summary>
    /// Import failed
    /// </summary>
    Failed,

    /// <summary>
    /// Skipped (already exists)
    /// </summary>
    Skipped
}
