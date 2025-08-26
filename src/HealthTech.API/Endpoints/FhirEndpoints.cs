using HealthTech.Application.FhirResources.Commands.CreateFhirResource;
using HealthTech.Application.FhirResources.Queries.GetFhirResource;
using HealthTech.Application.FhirResources.Queries.SearchFhirResources;
using MediatR;
using Microsoft.AspNetCore.Authorization;

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
        .WithDescription("Search for FHIR resources of a specific type with optional search parameters");

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
        .WithDescription("Retrieve a specific FHIR resource by its type and ID");

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
        .WithDescription("Create a new FHIR resource of the specified type");

        // PUT /fhir/{resourceType}/{id} - TODO: Implement UpdateFhirResourceCommand
        // group.MapPut("/{resourceType}/{id}", async (
        //     string resourceType,
        //     string id,
        //     UpdateFhirResourceCommand command,
        //     ISender sender,
        //     CancellationToken cancellationToken) =>
        // {
        //     var updateCommand = command with { ResourceType = resourceType, FhirId = id };
        //     var result = await sender.Send(updateCommand, cancellationToken);
        //     return Results.Ok(result);
        // })
        // .WithName("UpdateFhirResource")
        // .WithSummary("Update FHIR resource")
        // .WithDescription("Update an existing FHIR resource");

        // DELETE /fhir/{resourceType}/{id} - TODO: Implement DeleteFhirResourceCommand
        // group.MapDelete("/{resourceType}/{id}", async (
        //     string resourceType,
        //     string id,
        //     ISender sender,
        //     CancellationToken cancellationToken) =>
        // {
        //     var command = new DeleteFhirResourceCommand { ResourceType = resourceType, FhirId = id };
        //     await sender.Send(command, cancellationToken);
        //     return Results.NoContent();
        // })
        // .WithName("DeleteFhirResource")
        // .WithSummary("Delete FHIR resource")
        // .WithDescription("Delete a FHIR resource (soft delete)");

        // GET /fhir/{resourceType}/{id}/_history - TODO: Implement GetFhirResourceHistoryQuery
        // group.MapGet("/{resourceType}/{id}/_history", async (
        //     string resourceType,
        //     string id,
        //     ISender sender,
        //     CancellationToken cancellationToken) =>
        // {
        //     var query = new GetFhirResourceHistoryQuery { ResourceType = resourceType, FhirId = id };
        //     var result = await sender.Send(query, cancellationToken);
        //     return Results.Ok(result);
        // })
        // .WithName("GetFhirResourceHistory")
        // .WithSummary("Get FHIR resource history")
        // .WithDescription("Get the version history of a FHIR resource");
    }
}
