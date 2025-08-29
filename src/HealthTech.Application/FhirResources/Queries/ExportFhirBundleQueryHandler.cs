using MediatR;
using HealthTech.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Diagnostics;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace HealthTech.Application.FhirResources.Queries;

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

        // Apply resource limit
        query = query.Take(request.MaxResources);

        // Get resources
        var resources = await query
            .OrderByDescending(r => r.LastUpdated)
            .ToListAsync(cancellationToken);

        // Create FHIR Bundle
        var bundle = new Bundle
        {
            Type = GetBundleType(request.BundleType),
            Timestamp = exportTimestamp,
            Entry = new List<Bundle.EntryComponent>()
        };

        // Add resources to bundle
        foreach (var resource in resources)
        {
            try
            {
                var fhirResource = ParseFhirResource(resource.ResourceJson, resource.ResourceType);
                if (fhirResource != null)
                {
                    bundle.Entry.Add(new Bundle.EntryComponent
                    {
                        Resource = fhirResource,
                        FullUrl = $"{resource.ResourceType}/{resource.FhirId}"
                    });
                }
            }
            catch (Exception ex)
            {
                // Log error but continue with other resources
                Console.WriteLine($"Error parsing resource {resource.FhirId}: {ex.Message}");
            }
        }

        // Serialize bundle
        var serializer = new FhirJsonSerializer();
        var bundleJson = serializer.SerializeToString(bundle);

        stopwatch.Stop();

        // Calculate metadata
        var resourceTypeBreakdown = resources
            .GroupBy(r => r.ResourceType)
            .ToDictionary(g => g.Key, g => g.Count());

        var metadata = new ExportFhirBundleResponse.BundleMetadata
        {
            BundleType = request.BundleType,
            TotalResources = bundle.Entry.Count,
            ResourceTypesCount = resourceTypeBreakdown.Count,
            ResourceTypeBreakdown = resourceTypeBreakdown,
            ExportTimestamp = exportTimestamp,
            ExportDurationMs = stopwatch.ElapsedMilliseconds,
            BundleSizeBytes = System.Text.Encoding.UTF8.GetByteCount(bundleJson)
        };

        return new ExportFhirBundleResponse
        {
            BundleJson = bundleJson,
            Metadata = metadata
        };
    }

    private IQueryable<Domain.Entities.FhirResource> ApplyTimeBasedFiltering(
        IQueryable<Domain.Entities.FhirResource> query, 
        ExportFhirBundleQuery request)
    {
        if (request.StartDate.HasValue)
        {
            query = query.Where(r => r.LastUpdated >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(r => r.LastUpdated <= request.EndDate.Value);
        }

        return query;
    }

    private Bundle.BundleType GetBundleType(string bundleType)
    {
        return bundleType.ToLower() switch
        {
            "collection" => Bundle.BundleType.Collection,
            "transaction" => Bundle.BundleType.Transaction,
            "batch" => Bundle.BundleType.Batch,
            "searchset" => Bundle.BundleType.Searchset,
            "history" => Bundle.BundleType.History,
            _ => Bundle.BundleType.Collection
        };
    }

    private Resource? ParseFhirResource(string resourceJson, string resourceType)
    {
        try
        {
            var parser = new FhirJsonParser();
            return parser.Parse<Resource>(resourceJson);
        }
        catch
        {
            return null;
        }
    }
}
