using MediatR;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System.Text.Json;

namespace HealthTech.Application.FhirResources.Commands.CreateFhirResource;

/// <summary>
/// Handler for CreateFhirResourceCommand
/// </summary>
public class CreateFhirResourceCommandHandler : IRequestHandler<CreateFhirResourceCommand, CreateFhirResourceResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Application database context</param>
    /// <param name="currentUserService">Current user service</param>
    public CreateFhirResourceCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Handle the command
    /// </summary>
    /// <param name="request">Command request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response</returns>
    public async Task<CreateFhirResourceResponse> Handle(CreateFhirResourceCommand request, CancellationToken cancellationToken)
    {
        // Parse FHIR resource
        var parser = new FhirJsonParser();
        var resource = parser.Parse<Resource>(request.ResourceJson);

        // Generate FHIR ID if not provided
        var fhirId = request.FhirId ?? Guid.NewGuid().ToString();

        // Create FHIR resource entity
        var fhirResource = new FhirResource
        {
            Id = Guid.NewGuid(),
            TenantId = _currentUserService.TenantId ?? string.Empty,
            ResourceType = request.ResourceType,
            FhirId = fhirId,
            VersionId = 1,
            ResourceJson = request.ResourceJson,
            Status = "active",
            LastUpdated = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId ?? "system",
            SearchParameters = ExtractSearchParameters(resource),
            Tags = ExtractTags(resource),
            SecurityLabels = ExtractSecurityLabels(resource)
        };

        // Add to database
        _context.FhirResources.Add(fhirResource);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateFhirResourceResponse
        {
            FhirId = fhirId,
            VersionId = fhirResource.VersionId,
            ResourceJson = request.ResourceJson
        };
    }

    private static string? ExtractSearchParameters(Resource resource)
    {
        // Extract common search parameters from FHIR resource
        var parameters = new Dictionary<string, object>();

        if (resource is Patient patient)
        {
            if (patient.Identifier?.Any() == true)
                parameters["identifier"] = patient.Identifier.First().Value;
            if (patient.Name?.Any() == true)
                parameters["name"] = patient.Name.First().Text ?? patient.Name.First().Given?.FirstOrDefault();
        }
        else if (resource is Observation observation)
        {
            if (observation.Code?.Coding?.Any() == true)
                parameters["code"] = observation.Code.Coding.First().Code;
            if (observation.Subject?.Reference != null)
                parameters["subject"] = observation.Subject.Reference;
        }

        return parameters.Any() ? JsonSerializer.Serialize(parameters) : null;
    }

    private static string? ExtractTags(Resource resource)
    {
        if (resource.Meta?.Tag?.Any() != true)
            return null;

        var tags = resource.Meta.Tag.Select(t => new { t.Code, t.Display, t.System }).ToList();
        return JsonSerializer.Serialize(tags);
    }

    private static string? ExtractSecurityLabels(Resource resource)
    {
        if (resource.Meta?.Security?.Any() != true)
            return null;

        var labels = resource.Meta.Security.Select(s => new { s.Code, s.Display, s.System }).ToList();
        return JsonSerializer.Serialize(labels);
    }
}
