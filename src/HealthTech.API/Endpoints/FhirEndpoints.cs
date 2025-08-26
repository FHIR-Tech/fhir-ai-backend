using HealthTech.Application.FhirResources.Commands.CreateFhirResource;
using HealthTech.Application.FhirResources.Queries.GetFhirResource;
using HealthTech.Application.FhirResources.Queries.SearchFhirResources;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Any;

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
        .WithDescription(@"
Search for FHIR resources of a specific type with pagination support.

### Supported Resource Types
- **Patient**: Patient demographic and administrative information
- **Observation**: Clinical measurements and simple assertions
- **Encounter**: An interaction between a patient and healthcare provider
- **Condition**: Detailed information about conditions, problems, or diagnoses
- **MedicationRequest**: An order or request for medication
- **Procedure**: An action that is performed on or for a patient

### Examples
```
GET /fhir/Patient?skip=0&take=10
GET /fhir/Observation?skip=20&take=50
```

### Response Format
Returns a FHIR Bundle containing the matching resources with pagination metadata.
")
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
        .WithDescription(@"
Retrieve a specific FHIR resource by its type and ID.

### Examples
```
GET /fhir/Patient/patient-123
GET /fhir/Observation/obs-456
```

### Response Format
Returns the complete FHIR resource in JSON format following R4 specification.

### Error Responses
- **404 Not Found**: Resource not found or access denied
- **403 Forbidden**: Insufficient permissions for the requested resource
")
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
        .WithDescription(@"
Create a new FHIR resource of the specified type.

### Request Body
The request body should contain a valid FHIR resource in JSON format following R4 specification.

### Examples

#### Create a Patient
```json
{
  ""resourceType"": ""Patient"",
  ""name"": [
    {
      ""use"": ""official"",
      ""family"": ""Smith"",
      ""given"": [""John"", ""Michael""]
    }
  ],
  ""gender"": ""male"",
  ""birthDate"": ""1990-01-15""
}
```

#### Create an Observation
```json
{
  ""resourceType"": ""Observation"",
  ""status"": ""final"",
  ""code"": {
    ""coding"": [
      {
        ""system"": ""http://loinc.org"",
        ""code"": ""8302-2"",
        ""display"": ""Body height""
      }
    ]
  },
  ""valueQuantity"": {
    ""value"": 175.0,
    ""unit"": ""cm"",
    ""system"": ""http://unitsofmeasure.org"",
    ""code"": ""cm""
  }
}
```

### Response
Returns the created resource with assigned ID and metadata.
")
        .WithOpenApi(operation =>
        {
            if (operation.Parameters != null && operation.Parameters.Count > 0)
            {
                operation.Parameters[0].Description = "FHIR resource type (e.g., Patient, Observation, Encounter)";
                operation.Parameters[0].Example = new Microsoft.OpenApi.Any.OpenApiString("Patient");
            }
            return operation;
        });

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
