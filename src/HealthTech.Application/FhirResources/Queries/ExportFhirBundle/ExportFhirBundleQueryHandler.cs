using MediatR;
using HealthTech.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Diagnostics;

namespace HealthTech.Application.FhirResources.Queries.ExportFhirBundle;

/// <summary>
/// Handler for ExportFhirBundleQuery
/// </summary>
public class ExportFhirBundleQueryHandler : IRequestHandler<ExportFhirBundleQuery, ExportFhirBundleResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Application database context</param>
    /// <param name="currentUserService">Current user service</param>
    public ExportFhirBundleQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Handle the query
    /// </summary>
    /// <param name="request">Query request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response</returns>
    public async Task<ExportFhirBundleResponse> Handle(ExportFhirBundleQuery request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var tenantId = _currentUserService.TenantId ?? string.Empty;
        var exportTimestamp = DateTime.UtcNow;

        // Build base query
        var query = _context.FhirResources.AsQueryable();

        // Apply tenant filter
        query = query.Where(r => r.TenantId == tenantId);

        // Apply resource type filter
        if (!string.IsNullOrEmpty(request.ResourceType))
        {
            query = query.Where(r => r.ResourceType == request.ResourceType);
        }

        // Apply specific IDs filter
        if (request.FhirIds != null && request.FhirIds.Any())
        {
            query = query.Where(r => request.FhirIds.Contains(r.FhirId));
        }

        // Apply status filter (include/exclude deleted)
        if (!request.IncludeDeleted)
        {
            query = query.Where(r => r.Status != "deleted");
        }

        // Apply time-based filtering
        query = ApplyTimeBasedFiltering(query, request);

        // Apply search parameters if provided
        if (request.SearchParameters.Any())
        {
            foreach (var param in request.SearchParameters)
            {
                switch (param.Key.ToLower())
                {
                    case "identifier":
                        query = query.Where(r => r.SearchParameters != null && r.SearchParameters.Contains(param.Value));
                        break;
                    case "name":
                        query = query.Where(r => r.SearchParameters != null && r.SearchParameters.Contains(param.Value));
                        break;
                    case "code":
                        query = query.Where(r => r.SearchParameters != null && r.SearchParameters.Contains(param.Value));
                        break;
                    case "subject":
                        query = query.Where(r => r.ResourceJson.Contains(param.Value));
                        break;
                    case "date":
                        // Handle date range filtering
                        if (DateTime.TryParse(param.Value, out var date))
                        {
                            query = query.Where(r => r.LastUpdated >= date);
                        }
                        break;
                }
            }
        }

        // Apply observation-specific filtering
        query = ApplyObservationFiltering(query, request);

        // Apply resource limit
        query = query.Take(request.MaxResources);

        // Get resources
        var resources = await query
            .OrderByDescending(r => r.LastUpdated)
            .Select(r => new
            {
                r.ResourceType,
                r.FhirId,
                r.VersionId,
                r.ResourceJson,
                r.LastUpdated,
                r.Status
            })
            .ToListAsync(cancellationToken);

        // Apply observation-specific filtering in memory
        if (!string.IsNullOrEmpty(request.ObservationCode) || !string.IsNullOrEmpty(request.ObservationSystem) || !string.IsNullOrEmpty(request.PatientId))
        {
            resources = resources.Where(r => r.ResourceType == "Observation").ToList();
            
            if (!string.IsNullOrEmpty(request.ObservationCode))
            {
                resources = resources.Where(r => r.ResourceJson.Contains($"\"code\":\"{request.ObservationCode}\"")).ToList();
            }
            
            if (!string.IsNullOrEmpty(request.ObservationSystem))
            {
                resources = resources.Where(r => r.ResourceJson.Contains($"\"system\":\"{request.ObservationSystem}\"")).ToList();
            }
            
            if (!string.IsNullOrEmpty(request.PatientId))
            {
                resources = resources.Where(r => r.ResourceJson.Contains($"\"reference\":\"Patient/{request.PatientId}\"")).ToList();
            }
        }

        // Apply max observations per patient if specified
        if (request.MaxObservationsPerPatient.HasValue && request.MaxObservationsPerPatient.Value > 0)
        {
            var groupedResources = resources
                .GroupBy(r => ExtractPatientIdFromResource(r.ResourceJson))
                .SelectMany(g => g.Take(request.MaxObservationsPerPatient.Value))
                .ToList();
            resources = groupedResources;
        }

        // Apply sorting
        if (request.SortOrder.ToLower() == "asc")
        {
            resources = resources.OrderBy(r => r.LastUpdated).ToList();
        }
        else
        {
            resources = resources.OrderByDescending(r => r.LastUpdated).ToList();
        }

        // Get history if requested
        var historyResources = new List<object>();
        if (request.IncludeHistory)
        {
            var resourceIds = resources.Select(r => new { r.ResourceType, r.FhirId }).Distinct();
            
            foreach (var resourceId in resourceIds.Take(100)) // Limit to prevent performance issues
            {
                var history = await _context.FhirResources
                    .Where(r => r.TenantId == tenantId && 
                               r.ResourceType == resourceId.ResourceType && 
                               r.FhirId == resourceId.FhirId)
                    .OrderByDescending(r => r.VersionId)
                    .Take(request.MaxHistoryVersions)
                    .Select(r => new
                    {
                        r.ResourceType,
                        r.FhirId,
                        r.VersionId,
                        r.ResourceJson,
                        r.LastUpdated,
                        r.Status
                    })
                    .ToListAsync(cancellationToken);

                historyResources.AddRange(history);
            }
        }

        // Build bundle
        var bundle = BuildFhirBundle(resources, historyResources, request, exportTimestamp);

        // Calculate metadata
        var resourceTypeBreakdown = resources
            .GroupBy(r => r.ResourceType)
            .ToDictionary(g => g.Key, g => g.Count());

        stopwatch.Stop();

        var bundleJson = JsonSerializer.Serialize(bundle, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return new ExportFhirBundleResponse
        {
            BundleJson = bundleJson,
            Metadata = new ExportFhirBundleResponse.BundleMetadata
            {
                BundleType = request.BundleType,
                TotalResources = resources.Count + historyResources.Count,
                ResourceTypesCount = resourceTypeBreakdown.Count,
                ResourceTypeBreakdown = resourceTypeBreakdown,
                ExportTimestamp = exportTimestamp,
                ExportDurationMs = stopwatch.ElapsedMilliseconds,
                BundleSizeBytes = System.Text.Encoding.UTF8.GetByteCount(bundleJson)
            }
        };
    }

    /// <summary>
    /// Apply time-based filtering to the query
    /// </summary>
    /// <param name="query">Base query</param>
    /// <param name="request">Export request</param>
    /// <returns>Filtered query</returns>
    private static IQueryable<HealthTech.Domain.Entities.FhirResource> ApplyTimeBasedFiltering(
        IQueryable<HealthTech.Domain.Entities.FhirResource> query, 
        ExportFhirBundleQuery request)
    {
        // Calculate date range based on time period
        var (startDate, endDate) = CalculateDateRange(request);
        
        if (startDate.HasValue)
        {
            query = query.Where(r => r.LastUpdated >= startDate.Value);
        }
        
        if (endDate.HasValue)
        {
            query = query.Where(r => r.LastUpdated <= endDate.Value);
        }

        return query;
    }

    /// <summary>
    /// Calculate date range based on time period parameters
    /// </summary>
    /// <param name="request">Export request</param>
    /// <returns>Tuple of start and end dates</returns>
    private static (DateTime? startDate, DateTime? endDate) CalculateDateRange(ExportFhirBundleQuery request)
    {
        var now = DateTime.UtcNow;
        var startDate = request.StartDate;
        var endDate = request.EndDate;

        // If explicit dates are provided, use them
        if (startDate.HasValue || endDate.HasValue)
        {
            return (startDate, endDate);
        }

        // If time period is specified, calculate the range
        if (!string.IsNullOrEmpty(request.TimePeriod) && request.TimePeriodCount.HasValue)
        {
            endDate = now;
            
            startDate = request.TimePeriod.ToLower() switch
            {
                "days" => now.AddDays(-request.TimePeriodCount.Value),
                "weeks" => now.AddDays(-request.TimePeriodCount.Value * 7),
                "months" => now.AddMonths(-request.TimePeriodCount.Value),
                "years" => now.AddYears(-request.TimePeriodCount.Value),
                _ => null
            };
        }

        return (startDate, endDate);
    }

    /// <summary>
    /// Apply observation-specific filtering to the query
    /// </summary>
    /// <param name="query">Base query</param>
    /// <param name="request">Export request</param>
    /// <returns>Filtered query</returns>
    private static IQueryable<HealthTech.Domain.Entities.FhirResource> ApplyObservationFiltering(
        IQueryable<HealthTech.Domain.Entities.FhirResource> query, 
        ExportFhirBundleQuery request)
    {
        // If filtering for observations with specific codes
        if (!string.IsNullOrEmpty(request.ObservationCode) || !string.IsNullOrEmpty(request.ObservationSystem))
        {
            query = query.Where(r => r.ResourceType == "Observation");
            
            // For now, we'll filter after getting the data to avoid JSONB operator issues
            // This is a temporary solution until we implement proper JSONB indexing
        }

        // Filter by patient ID
        if (!string.IsNullOrEmpty(request.PatientId))
        {
            // For now, we'll filter after getting the data to avoid JSONB operator issues
            // This is a temporary solution until we implement proper JSONB indexing
        }

        return query;
    }

    /// <summary>
    /// Build FHIR bundle from resources
    /// </summary>
    /// <param name="resources">Resources to include</param>
    /// <param name="historyResources">History resources to include</param>
    /// <param name="request">Export request</param>
    /// <param name="timestamp">Bundle timestamp</param>
    /// <returns>FHIR bundle object</returns>
    private static object BuildFhirBundle(IEnumerable<object> resources, IEnumerable<object> historyResources, ExportFhirBundleQuery request, DateTime timestamp)
    {
        var allResources = resources.Concat(historyResources).ToList();
        var entries = new List<object>();

        foreach (var resource in allResources)
        {
            var resourceObj = JsonSerializer.Deserialize<JsonElement>(resource.GetType().GetProperty("ResourceJson")?.GetValue(resource)?.ToString() ?? "{}");
            
            var entry = new
            {
                resource = resourceObj,
                search = new
                {
                    mode = "match"
                }
            };

            entries.Add(entry);
        }

        return new
        {
            resourceType = "Bundle",
            type = request.BundleType,
            timestamp = timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            total = entries.Count,
            entry = entries
        };
    }

    /// <summary>
    /// Extracts the patient ID from a resource JSON.
    /// </summary>
    /// <param name="resourceJson">The JSON string of the resource.</param>
    /// <returns>The patient ID if found, otherwise null.</returns>
    private static string? ExtractPatientIdFromResource(string resourceJson)
    {
        try
        {
            var json = JsonDocument.Parse(resourceJson);
            var root = json.RootElement;

            if (root.TryGetProperty("subject", out var subjectProperty))
            {
                if (subjectProperty.TryGetProperty("reference", out var referenceProperty))
                {
                    var reference = referenceProperty.GetString();
                    if (reference != null && reference.StartsWith("Patient/"))
                    {
                        return reference.Substring(7); // Remove "Patient/"
                    }
                }
            }
        }
        catch (Exception)
        {
            // Log or handle the error appropriately
        }
        return null;
    }
}
