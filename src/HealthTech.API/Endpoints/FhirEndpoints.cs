using HealthTech.Application.FhirResources.Commands.CreateFhirResource;
using HealthTech.Application.FhirResources.Commands.UpdateFhirResource;
using HealthTech.Application.FhirResources.Commands.DeleteFhirResource;
using HealthTech.Application.FhirResources.Commands.ImportFhirBundle;
using HealthTech.Application.FhirResources.Queries.GetFhirResource;
using HealthTech.Application.FhirResources.Queries.SearchFhirResources;
using HealthTech.Application.FhirResources.Queries.GetFhirResourceHistory;
using HealthTech.API.Swagger;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Text.Json;

namespace HealthTech.API.Endpoints;

/// <summary>
/// FHIR endpoints configuration
/// </summary>
public static class FhirEndpoints
{
    /// <summary>
    /// Map FHIR endpoints
    /// </summary>
    /// <param name="app">Web application</param>
    public static void MapFhirEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/fhir")
            .WithTags("FHIR")
            .WithOpenApi()
            .RequireAuthorization();

        // GET /fhir/{resourceType}
        group.MapGet("/{resourceType}", async (
            string resourceType,
            ISender sender,
            CancellationToken cancellationToken,
            int skip = 0,
            int take = 100) =>
        {
            var searchQuery = new SearchFhirResourcesQuery 
            { 
                ResourceType = resourceType,
                Skip = skip,
                Take = take
            };
            var result = await sender.Send(searchQuery, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("SearchFhirResources")
        .WithSummary("Search FHIR resources by type")
        .WithDescription(FhirEndpointDescriptions.SearchFhirResources)
        .WithOpenApi(operation =>
        {
            if (operation.Parameters != null && operation.Parameters.Count > 0)
            {
                operation.Parameters[0].Description = "FHIR resource type (e.g., Patient, Observation, Encounter)";
                operation.Parameters[0].Example = new Microsoft.OpenApi.Any.OpenApiString("Patient");
            }
            
            if (operation.Parameters != null && operation.Parameters.Count > 3)
            {
                operation.Parameters[3].Description = "Number of records to skip for pagination (default: 0)";
            }
            
            if (operation.Parameters != null && operation.Parameters.Count > 4)
            {
                operation.Parameters[4].Description = "Number of records to return, maximum 100 (default: 100)";
            }
            
            return operation;
        });

        // GET /fhir/{resourceType}/{id}
        group.MapGet("/{resourceType}/{id}", async (
            string resourceType,
            string id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetFhirResourceQuery { ResourceType = resourceType, FhirId = id };
            var result = await sender.Send(query, cancellationToken);
            
            if (result == null)
                return Results.NotFound();
                
            return Results.Ok(result);
        })
        .WithName("GetFhirResource")
        .WithSummary("Get FHIR resource by ID")
        .WithDescription(FhirEndpointDescriptions.GetFhirResource)
        .WithOpenApi(operation =>
        {
            if (operation.Parameters != null && operation.Parameters.Count > 0)
            {
                operation.Parameters[0].Description = "FHIR resource type (e.g., Patient, Observation, Encounter)";
                operation.Parameters[0].Example = new Microsoft.OpenApi.Any.OpenApiString("Patient");
            }
            
            if (operation.Parameters != null && operation.Parameters.Count > 1)
            {
                operation.Parameters[1].Description = "Unique identifier for the FHIR resource";
                operation.Parameters[1].Example = new Microsoft.OpenApi.Any.OpenApiString("patient-123");
            }
            
            return operation;
        });

        // POST /fhir/$auto-detect-type
        group.MapPost("/$auto-detect-type", async (
            CreateFhirResourceCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return Results.Created($"/fhir/{command.ResourceType}/{result.FhirId}", result);
        })
        .WithName("CreateFhirResourceAutoDetect")
        .WithSummary("Create FHIR resource with auto-detected type")
        .WithDescription(FhirEndpointDescriptions.CreateFhirResourceAutoDetect);

        // POST /fhir/{resourceType}
        group.MapPost("/{resourceType}", async (
            string resourceType,
            CreateFhirResourceCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var createCommand = command with { ResourceType = resourceType };
            var result = await sender.Send(createCommand, cancellationToken);
            return Results.Created($"/fhir/{resourceType}/{result.FhirId}", result);
        })
        .WithName("CreateFhirResource")
        .WithSummary("Create FHIR resource")
        .WithDescription(FhirEndpointDescriptions.CreateFhirResource)
        .WithOpenApi(operation =>
        {
            if (operation.Parameters != null && operation.Parameters.Count > 0)
            {
                operation.Parameters[0].Description = "FHIR resource type (e.g., Patient, Observation, Encounter)";
                operation.Parameters[0].Example = new Microsoft.OpenApi.Any.OpenApiString("Patient");
            }
            return operation;
        });

        // PUT /fhir/{resourceType}/{id}
        group.MapPut("/{resourceType}/{id}", async (
            string resourceType,
            string id,
            UpdateFhirResourceCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var updateCommand = command with { ResourceType = resourceType, FhirId = id };
            var result = await sender.Send(updateCommand, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("UpdateFhirResource")
        .WithSummary("Update FHIR resource")
        .WithDescription(FhirEndpointDescriptions.UpdateFhirResource)
        .WithOpenApi(operation =>
        {
            if (operation.Parameters != null && operation.Parameters.Count > 0)
            {
                operation.Parameters[0].Description = "FHIR resource type (e.g., Patient, Observation, Encounter)";
                operation.Parameters[0].Example = new Microsoft.OpenApi.Any.OpenApiString("Patient");
            }
            
            if (operation.Parameters != null && operation.Parameters.Count > 1)
            {
                operation.Parameters[1].Description = "Unique identifier for the FHIR resource to update";
                operation.Parameters[1].Example = new Microsoft.OpenApi.Any.OpenApiString("patient-123");
            }
            
            return operation;
        });

        // DELETE /fhir/{resourceType}/{id}
        group.MapDelete("/{resourceType}/{id}", async (
            string resourceType,
            string id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteFhirResourceCommand { ResourceType = resourceType, FhirId = id };
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("DeleteFhirResource")
        .WithSummary("Delete FHIR resource")
        .WithDescription(FhirEndpointDescriptions.DeleteFhirResource)
        .WithOpenApi(operation =>
        {
            if (operation.Parameters != null && operation.Parameters.Count > 0)
            {
                operation.Parameters[0].Description = "FHIR resource type (e.g., Patient, Observation, Encounter)";
                operation.Parameters[0].Example = new Microsoft.OpenApi.Any.OpenApiString("Patient");
            }
            
            if (operation.Parameters != null && operation.Parameters.Count > 1)
            {
                operation.Parameters[1].Description = "Unique identifier for the FHIR resource to delete";
                operation.Parameters[1].Example = new Microsoft.OpenApi.Any.OpenApiString("patient-123");
            }
            
            return operation;
        });

        // POST /fhir/$import-bundle
        group.MapPost("/$import-bundle", async (
            HttpContext httpContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            string bundleJson;
            bool validate = true; // Default to true
            string? description = null;
            
            // Check if the request is multipart form data (file upload)
            if (httpContext.Request.HasFormContentType)
            {
                var form = await httpContext.Request.ReadFormAsync(cancellationToken);
                var file = form.Files.FirstOrDefault(f => f.Name == "bundleFile");
                
                if (file != null && file.Length > 0)
                {
                    // Check file size (50MB limit for large FHIR bundles)
                    if (file.Length > 50 * 1024 * 1024)
                    {
                        return Results.BadRequest(new { error = "File size exceeds 50MB limit" });
                    }
                    
                    // Check file extension
                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (extension != ".json")
                    {
                        return Results.BadRequest(new { error = "Only .json files are supported for FHIR bundles" });
                    }
                    
                    using var reader = new StreamReader(file.OpenReadStream());
                    bundleJson = await reader.ReadToEndAsync(cancellationToken);
                }
                else
                {
                    return Results.BadRequest(new { error = "No FHIR bundle file provided" });
                }
                
                // Get optional parameters
                description = form["description"].FirstOrDefault();
                var validateStr = form["validate"].FirstOrDefault();
                validate = string.IsNullOrEmpty(validateStr) || bool.Parse(validateStr);
            }
            else
            {
                // Read the request body as JSON string
                using var reader = new StreamReader(httpContext.Request.Body);
                bundleJson = await reader.ReadToEndAsync(cancellationToken);
            }
            
            // Validate that this is a valid FHIR Bundle
            try
            {
                var bundle = JsonSerializer.Deserialize<JsonElement>(bundleJson);
                if (!bundle.TryGetProperty("resourceType", out var resourceType) || 
                    resourceType.GetString() != "Bundle")
                {
                    return Results.BadRequest(new { error = "Invalid FHIR Bundle: resourceType must be 'Bundle'" });
                }
                
                if (!bundle.TryGetProperty("entry", out var entry))
                {
                    return Results.BadRequest(new { error = "Invalid FHIR Bundle: missing 'entry' array" });
                }
                
                if (entry.ValueKind != JsonValueKind.Array)
                {
                    return Results.BadRequest(new { error = "Invalid FHIR Bundle: 'entry' must be an array" });
                }
            }
            catch (JsonException)
            {
                return Results.BadRequest(new { error = "Invalid JSON format in FHIR Bundle" });
            }
            
            var command = new ImportFhirBundleCommand 
            { 
                BundleJson = bundleJson,
                ValidateResources = validate
            };
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("ImportFhirBundle")
        .WithSummary("Import FHIR Bundle (HL7 Standard)")
        .WithDescription(FhirEndpointDescriptions.ImportFhirBundle)
        .WithOpenApi(operation =>
        {
            // Add support for both JSON body and file upload
            operation.RequestBody = new OpenApiRequestBody
            {
                Description = "FHIR Bundle as JSON or file upload",
                Required = true,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Description = "FHIR Bundle JSON object"
                        }
                    },
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["bundleFile"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary",
                                    Description = "JSON file containing FHIR Bundle"
                                }
                            }
                        }
                    }
                }
            };
            return operation;
        });

        // GET /fhir/{resourceType}/{id}/_history
        group.MapGet("/{resourceType}/{id}/_history", async (
            string resourceType,
            string id,
            ISender sender,
            CancellationToken cancellationToken,
            int maxVersions = 100) =>
        {
            var query = new GetFhirResourceHistoryQuery 
            { 
                ResourceType = resourceType, 
                FhirId = id,
                MaxVersions = maxVersions
            };
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetFhirResourceHistory")
        .WithSummary("Get FHIR resource history")
        .WithDescription(FhirEndpointDescriptions.GetFhirResourceHistory)
        .WithOpenApi(operation =>
        {
            if (operation.Parameters != null && operation.Parameters.Count > 0)
            {
                operation.Parameters[0].Description = "FHIR resource type (e.g., Patient, Observation, Encounter)";
                operation.Parameters[0].Example = new Microsoft.OpenApi.Any.OpenApiString("Patient");
            }
            
            if (operation.Parameters != null && operation.Parameters.Count > 1)
            {
                operation.Parameters[1].Description = "Unique identifier for the FHIR resource";
                operation.Parameters[1].Example = new Microsoft.OpenApi.Any.OpenApiString("patient-123");
            }
            
            if (operation.Parameters != null && operation.Parameters.Count > 3)
            {
                operation.Parameters[3].Description = "Maximum number of versions to return (default: 100)";
            }
            
            return operation;
        });
    }
}
