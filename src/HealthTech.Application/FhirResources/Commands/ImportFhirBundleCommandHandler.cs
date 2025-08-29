using MediatR;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System.Text.Json;

namespace HealthTech.Application.FhirResources.Commands;

/// <summary>
/// Handler for ImportFhirBundleCommand
/// </summary>
public class ImportFhirBundleCommandHandler : IRequestHandler<ImportFhirBundleCommand, ImportFhirBundleResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISender _sender;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Application database context</param>
    /// <param name="currentUserService">Current user service</param>
    /// <param name="sender">MediatR sender</param>
    public ImportFhirBundleCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        ISender sender)
    {
        _context = context;
        _currentUserService = currentUserService;
        _sender = sender;
    }

    /// <summary>
    /// Handle the command
    /// </summary>
    /// <param name="request">Command request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response</returns>
    public async Task<ImportFhirBundleResponse> Handle(ImportFhirBundleCommand request, CancellationToken cancellationToken)
    {
        var importJobId = Guid.NewGuid().ToString();
        var importedResources = new List<ImportedResource>();
        var errors = new List<ImportError>();
        var totalProcessed = 0;
        var successfullyImported = 0;
        var failedToImport = 0;

        try
        {
            // Parse FHIR Bundle
            var parser = new FhirJsonParser();
            var bundle = parser.Parse<Bundle>(request.BundleJson);

            // Add audit event for bundle import
            var bundleAuditEvent = new Domain.Entities.AuditEvent
            {
                Id = Guid.NewGuid(),
                TenantId = _currentUserService.TenantId ?? string.Empty,
                EventType = "bundle-import",
                ResourceType = "Bundle",
                ResourceId = importJobId,
                UserId = _currentUserService.UserId ?? "system",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserId ?? "system",
                EventData = JsonSerializer.Serialize(new
                {
                    TotalEntries = bundle.Entry?.Count ?? 0,
                    BundleType = bundle.Type?.ToString(),
                    ImportJobId = importJobId
                })
            };

            _context.AuditEvents.Add(bundleAuditEvent);

            // Sort entries by dependencies for proper import order
            var sortedEntries = SortEntriesByDependencies(bundle.Entry?.ToList() ?? new List<Bundle.EntryComponent>());
            
            // Get all resource IDs in the bundle for reference checking
            var bundleResourceIds = new HashSet<string>();
            foreach (var entry in sortedEntries)
            {
                if (entry.Resource?.Id != null)
                {
                    var resourceKey = $"{entry.Resource.GetType().Name}/{entry.Resource.Id}";
                    bundleResourceIds.Add(resourceKey);
                }
            }

            // Process each entry in the bundle
            foreach (var entry in sortedEntries)
            {
                totalProcessed++;

                try
                {
                    var resource = entry.Resource;
                    if (resource == null)
                    {
                        errors.Add(new ImportError
                        {
                            ResourceType = "Unknown",
                            OriginalId = null,
                            Message = "Entry contains no resource",
                            ErrorCode = "INVALID_ENTRY"
                        });
                        failedToImport++;
                        continue;
                    }

                    var resourceType = resource.GetType().Name;
                    var resourceId = resource.Id;

                    // Determine operation type
                    var operation = entry.Request?.Method?.ToString() ?? "POST";

                    // Validate references before processing
                    if (operation?.ToUpper() != "DELETE")
                    {
                        var invalidReferences = ValidateReferences(resource, bundleResourceIds);
                        if (invalidReferences.Any())
                        {
                            errors.Add(new ImportError
                            {
                                ResourceType = resourceType,
                                OriginalId = resourceId,
                                Message = $"Invalid references found: {string.Join(", ", invalidReferences)}",
                                ErrorCode = "INVALID_REFERENCES"
                            });
                            failedToImport++;
                            continue;
                        }
                    }

                    string finalResourceId;
                    ImportStatus status;

                    switch (operation.ToUpper())
                    {
                        case "PUT":
                        case "PATCH":
                            // Update existing resource
                            var updateCommand = new UpdateFhirResourceCommand
                            {
                                ResourceType = resourceType,
                                FhirId = resourceId,
                                ResourceJson = SerializeFhirResource(resource, resourceType)
                            };

                            var updateResult = await _sender.Send(updateCommand, cancellationToken);
                            finalResourceId = updateResult.FhirId;
                            status = ImportStatus.Success;
                            break;

                        case "DELETE":
                            // Handle delete operation
                            var deleteCommand = new DeleteFhirResourceCommand
                            {
                                ResourceType = resourceType,
                                FhirId = resourceId
                            };

                            await _sender.Send(deleteCommand, cancellationToken);
                            finalResourceId = resourceId;
                            status = ImportStatus.Success;
                            break;

                        default:
                            // Create new resource
                            var createCommand = new CreateFhirResourceCommand
                            {
                                ResourceType = resourceType,
                                ResourceJson = SerializeFhirResource(resource, resourceType),
                                FhirId = resourceId
                            };

                            var createResult = await _sender.Send(createCommand, cancellationToken);
                            finalResourceId = createResult.FhirId;
                            status = ImportStatus.Success;
                            break;
                    }

                    importedResources.Add(new ImportedResource
                    {
                        ResourceType = resourceType,
                        FhirId = finalResourceId,
                        Status = status,
                        ErrorMessage = null
                    });

                    successfullyImported++;
                }
                catch (Exception ex)
                {
                    var resourceType = entry.Resource?.GetType().Name ?? "Unknown";
                    var resourceId = entry.Resource?.Id;

                    errors.Add(new ImportError
                    {
                        ResourceType = resourceType,
                        OriginalId = resourceId,
                        Message = ex.Message,
                        ErrorCode = ex.GetType().Name
                    });

                    failedToImport++;
                }
            }

            // Save all changes
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            errors.Add(new ImportError
            {
                ResourceType = "Bundle",
                OriginalId = null,
                Message = $"Failed to parse bundle: {ex.Message}",
                ErrorCode = "BUNDLE_PARSE_ERROR"
            });

            failedToImport++;
        }

        return new ImportFhirBundleResponse
        {
            TotalProcessed = totalProcessed,
            SuccessfullyImported = successfullyImported,
            FailedToImport = failedToImport,
            ImportJobId = importJobId,
            ImportedResources = importedResources,
            Errors = errors
        };
    }

    /// <summary>
    /// Sort bundle entries by dependencies to ensure proper import order
    /// </summary>
    /// <param name="entries">Bundle entries</param>
    /// <returns>Sorted entries</returns>
    private List<Bundle.EntryComponent> SortEntriesByDependencies(List<Bundle.EntryComponent> entries)
    {
        if (entries == null || entries.Count == 1)
            return entries;

        var result = new List<Bundle.EntryComponent>();
        var processed = new HashSet<string>();
        var resourceIdToEntry = new Dictionary<string, Bundle.EntryComponent>();

        // Create mapping of resource ID to entry
        foreach (var entry in entries)
        {
            if (entry.Resource?.Id != null)
            {
                var key = $"{entry.Resource.GetType().Name}/{entry.Resource.Id}";
                resourceIdToEntry[key] = entry;
            }
        }

        // Define resource type priority (lower number = higher priority)
        var typePriority = new Dictionary<string, int>
        {
            // Foundation resources (should be imported first)
            { "Patient", 1 },
            { "Organization", 2 },
            { "Practitioner", 3 },
            { "Location", 4 },
            
            // Clinical resources (depend on foundation resources)
            { "Encounter", 5 },
            { "Condition", 6 },
            { "Observation", 7 },
            { "Procedure", 8 },
            { "MedicationRequest", 9 },
            { "AllergyIntolerance", 10 },
            
            // Document resources (depend on clinical resources)
            { "DocumentReference", 11 },
            { "Composition", 12 },
            
            // Default for other types
            { "default", 100 }
        };

        // Sort by type priority first, then by dependencies
        var sortedEntries = entries.OrderBy(e => 
        {
            var resourceType = e.Resource?.GetType().Name ?? "Unknown";
            return typePriority.GetValueOrDefault(resourceType, typePriority["default"]);
        }).ToList();

        foreach (var entry in sortedEntries)
        {
            if (entry.Resource?.Id == null)
            {
                result.Add(entry);
                continue;
            }

            var resourceKey = $"{entry.Resource.GetType().Name}/{entry.Resource.Id}";
            
            // Skip if already processed
            if (processed.Contains(resourceKey))
                continue;

            // Add this entry and mark as processed
            result.Add(entry);
            processed.Add(resourceKey);
        }

        return result;
    }

    /// <summary>
    /// Validate references in a resource against available resources in the bundle
    /// </summary>
    /// <param name="resource">Resource to validate</param>
    /// <param name="bundleResourceIds">Available resource IDs in the bundle</param>
    /// <returns>List of invalid references</returns>
    private List<string> ValidateReferences(Resource resource, HashSet<string> bundleResourceIds)
    {
        var invalidReferences = new List<string>();
        var references = FhirBundleReferenceHelper.ExtractReferences(resource);
        
        foreach (var reference in references)
        {
            if (reference.Reference != null)
            {
                var referenceType = FhirBundleReferenceHelper.GetResourceTypeFromReference(reference.Reference);
                var referenceId = FhirBundleReferenceHelper.GetResourceIdFromReference(reference.Reference);
                
                if (referenceType != null && referenceId != null)
                {
                    var resourceKey = $"{referenceType}/{referenceId}";
                    
                    // Check if reference exists in bundle
                    if (!bundleResourceIds.Contains(resourceKey))
                    {
                        // Check if it's a reference to an external resource (has # prefix)
                        if (!reference.Reference.StartsWith("#") && !reference.Reference.StartsWith("http"))
                        {
                            invalidReferences.Add(reference.Reference);
                        }
                    }
                }
            }
        }
        
        return invalidReferences;
    }
    
    /// <summary>
    /// Serialize FHIR resource with proper type information
    /// </summary>
    /// <param name="resource">FHIR resource</param>
    /// <param name="resourceType">Resource type name</param>
    /// <returns>Serialized JSON</returns>
    private string SerializeFhirResource(Resource resource, string resourceType)
    {
        // Use FHIR serializer with proper settings
        var serializer = new FhirJsonSerializer();
        var jsonString = serializer.SerializeToString(resource);
        
        return jsonString;
    }
}
