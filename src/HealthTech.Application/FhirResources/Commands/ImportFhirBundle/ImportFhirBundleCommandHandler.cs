using MediatR;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Application.FhirResources.Commands.CreateFhirResource;
using HealthTech.Application.FhirResources.Commands.UpdateFhirResource;
using HealthTech.Application.FhirResources.Commands.DeleteFhirResource;
using HealthTech.Domain.Entities;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System.Text.Json;

namespace HealthTech.Application.FhirResources.Commands.ImportFhirBundle;

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

            // Process each entry in the bundle
            if (bundle.Entry != null)
            {
                foreach (var entry in bundle.Entry)
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
                                    ResourceJson = JsonSerializer.Serialize(resource, new JsonSerializerOptions
                                    {
                                        WriteIndented = false,
                                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                                    })
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
                                    ResourceJson = JsonSerializer.Serialize(resource, new JsonSerializerOptions
                                    {
                                        WriteIndented = false,
                                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                                    }),
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
}
