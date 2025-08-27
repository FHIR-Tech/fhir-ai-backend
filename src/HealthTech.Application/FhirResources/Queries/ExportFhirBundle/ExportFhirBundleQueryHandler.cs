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

        // Apply search parameters if provided
        if (request.SearchParameters.Any())
        {
            foreach (var param in request.SearchParameters)
            {
                switch (param.Key.ToLower())
                {
                    case "identifier":
                        query = query.Where(r => r.SearchParameters.Contains(param.Value));
                        break;
                    case "name":
                        query = query.Where(r => r.SearchParameters.Contains(param.Value));
                        break;
                    case "code":
                        query = query.Where(r => r.SearchParameters.Contains(param.Value));
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
}
