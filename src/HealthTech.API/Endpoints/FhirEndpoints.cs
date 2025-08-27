using HealthTech.Application.FhirResources.Commands.CreateFhirResource;
using HealthTech.Application.FhirResources.Commands.UpdateFhirResource;
using HealthTech.Application.FhirResources.Commands.DeleteFhirResource;
using HealthTech.Application.FhirResources.Commands.ImportFhirBundle;
using HealthTech.Application.FhirResources.Queries.GetFhirResource;
using HealthTech.Application.FhirResources.Queries.SearchFhirResources;
using HealthTech.Application.FhirResources.Queries.GetFhirResourceHistory;
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

        // POST /fhir
        group.MapPost("", async (
            CreateFhirResourceCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return Results.Created($"/fhir/{command.ResourceType}/{result.FhirId}", result);
        })
        .WithName("CreateFhirResourceSingle")
        .WithSummary("Create FHIR resource (auto-detect type)")
        .WithDescription(@"
Create a new FHIR resource. The resource type is automatically detected from the request body.

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
");

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
        .WithDescription(@"
Update an existing FHIR resource by ID.

### Request Body
The request body should contain the updated FHIR resource in JSON format following R4 specification.

### Examples

#### Update a Patient
```json
{
  ""resourceType"": ""Patient"",
  ""id"": ""patient-123"",
  ""name"": [
    {
      ""use"": ""official"",
      ""family"": ""Smith"",
      ""given"": [""John"", ""Michael"", ""Updated""]
    }
  ],
  ""gender"": ""male"",
  ""birthDate"": ""1990-01-15"",
  ""address"": [
    {
      ""use"": ""home"",
      ""type"": ""physical"",
      ""text"": ""123 Main St, Anytown, USA"",
      ""line"": [""123 Main St""],
      ""city"": ""Anytown"",
      ""state"": ""CA"",
      ""postalCode"": ""12345""
    }
  ]
}
```

### Response
Returns the updated resource with new version ID and metadata.
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
        .WithDescription(@"
Delete a FHIR resource (soft delete). The resource is marked as deleted but remains in the system for audit purposes.

### Examples
```
DELETE /fhir/Patient/patient-123
DELETE /fhir/Observation/obs-456
```

### Response
Returns deletion confirmation with timestamp and success status.

### Note
This is a soft delete operation. The resource remains in the database but is marked as deleted.
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
                operation.Parameters[1].Description = "Unique identifier for the FHIR resource to delete";
                operation.Parameters[1].Example = new Microsoft.OpenApi.Any.OpenApiString("patient-123");
            }
            
            return operation;
        });

        // POST /fhir/$import
        group.MapPost("/$import", async (
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
        .WithDescription(@"
Import FHIR resources from a standard HL7 FHIR Bundle. This endpoint supports importing complete patient records including:

### Supported FHIR Resources
- **Organization**: Healthcare facilities and providers
- **Patient**: Patient demographic and administrative information
- **Encounter**: Patient visits and interactions with healthcare providers
- **Observation**: Clinical measurements, lab results, and vital signs
- **Condition**: Diagnoses and medical conditions
- **MedicationRequest**: Prescriptions and medication orders
- **Immunization**: Vaccination records
- **Procedure**: Medical procedures performed
- **AllergyIntolerance**: Patient allergies and intolerances
- **FamilyMemberHistory**: Family medical history

### Request Options
You can provide the FHIR Bundle in two ways:

#### Option 1: JSON Body
Send the FHIR Bundle directly in the request body as JSON.

#### Option 2: File Upload
Upload a JSON file containing the FHIR Bundle using multipart form data.

### FHIR Bundle Format (HL7 Standard)
The FHIR Bundle must follow the HL7 FHIR R4 specification:

```json
{
  ""resourceType"": ""Bundle"",
  ""type"": ""collection"",
  ""timestamp"": ""2024-01-15T10:30:00Z"",
  ""entry"": [
    {
      ""resource"": {
        ""resourceType"": ""Organization"",
        ""id"": ""org-hospital"",
        ""name"": ""Hospital Name""
      }
    },
    {
      ""resource"": {
        ""resourceType"": ""Patient"",
        ""id"": ""patient-123"",
        ""name"": [
          {
            ""use"": ""official"",
            ""family"": ""Nguyễn"",
            ""given"": [""Trung"", ""Kiên""]
          }
        ],
        ""gender"": ""male"",
        ""birthDate"": ""1993-01-08""
      }
    },
    {
      ""resource"": {
        ""resourceType"": ""Encounter"",
        ""id"": ""encounter-1"",
        ""status"": ""finished"",
        ""subject"": {
          ""reference"": ""Patient/patient-123""
        },
        ""serviceProvider"": {
          ""reference"": ""Organization/org-hospital""
        }
      }
    }
  ]
}
```

### Bundle Types Supported
- **collection**: A set of resources (most common for patient records)
- **transaction**: A set of actions to be performed
- **batch**: A set of actions to be performed as a group
- **searchset**: Results from a search operation
- **history**: A list of historical versions of a resource

### File Requirements
- **Format**: JSON only (.json files)
- **Maximum size**: 50MB (for large patient records)
- **Content**: Valid FHIR Bundle following HL7 FHIR R4 specification

### Response
Returns import summary with statistics and detailed results for each resource.
")
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
        .WithDescription(@"
Get the version history of a FHIR resource.

### Examples
```
GET /fhir/Patient/patient-123/_history
GET /fhir/Patient/patient-123/_history?maxVersions=50
```

### Response Format
Returns a list of all versions of the resource with metadata including:
- Version ID
- Resource JSON at each version
- Creation and update timestamps
- Operation type (create, update, delete)

### Parameters
- **maxVersions**: Maximum number of versions to return (default: 100)
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
            
            if (operation.Parameters != null && operation.Parameters.Count > 3)
            {
                operation.Parameters[3].Description = "Maximum number of versions to return (default: 100)";
            }
            
            return operation;
        });
    }
}
