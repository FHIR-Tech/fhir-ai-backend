using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HealthTech.API.Swagger;

/// <summary>
/// Operation filter to enhance file upload endpoints in Swagger documentation
/// </summary>
public class FileUploadOperationFilter : IOperationFilter
{
    /// <summary>
    /// Apply the operation filter
    /// </summary>
    /// <param name="operation">The operation to filter</param>
    /// <param name="context">The operation filter context</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check if this is the import endpoint
        if (operation.OperationId == "ImportFhirBundle")
        {
            // Ensure the request body is properly configured for file upload
            if (operation.RequestBody?.Content?.ContainsKey("multipart/form-data") == true)
            {
                var multipartContent = operation.RequestBody.Content["multipart/form-data"];
                
                // Add better description for file upload
                if (multipartContent.Schema?.Properties?.ContainsKey("bundleFile") == true)
                {
                    var fileSchema = multipartContent.Schema.Properties["bundleFile"];
                    fileSchema.Description = "HL7 FHIR Bundle JSON file. Must contain valid FHIR resources following R4 specification. Supports .json files up to 50MB.";
                    fileSchema.Example = new Microsoft.OpenApi.Any.OpenApiString("fhir_bundle_ntkien_2024.json");
                }
                
                // Add additional properties for better UX
                if (multipartContent.Schema?.Properties != null)
                {
                    // Add optional description field
                    multipartContent.Schema.Properties["description"] = new OpenApiSchema
                    {
                        Type = "string",
                        Description = "Optional description for this import operation",
                        Example = new Microsoft.OpenApi.Any.OpenApiString("Import patient data from external system")
                    };
                    
                    // Add optional validate parameter
                    multipartContent.Schema.Properties["validate"] = new OpenApiSchema
                    {
                        Type = "boolean",
                        Description = "Whether to validate FHIR resources before import (default: true)",
                        Default = new Microsoft.OpenApi.Any.OpenApiBoolean(true)
                    };
                }
            }
            
            // Add better response examples
            if (operation.Responses?.ContainsKey("200") == true)
            {
                var successResponse = operation.Responses["200"];
                successResponse.Description = "FHIR Bundle imported successfully";
                
                // Add example response
                successResponse.Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["importJobId"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Description = "Unique identifier for the import job",
                                    Example = new Microsoft.OpenApi.Any.OpenApiString("import-12345-abcde")
                                },
                                ["successfullyImported"] = new OpenApiSchema
                                {
                                    Type = "integer",
                                    Description = "Number of resources successfully imported",
                                    Example = new Microsoft.OpenApi.Any.OpenApiInteger(5)
                                },
                                ["failedToImport"] = new OpenApiSchema
                                {
                                    Type = "integer",
                                    Description = "Number of resources that failed to import",
                                    Example = new Microsoft.OpenApi.Any.OpenApiInteger(0)
                                },
                                ["importedResources"] = new OpenApiSchema
                                {
                                    Type = "array",
                                    Description = "List of successfully imported resources",
                                    Items = new OpenApiSchema
                                    {
                                        Type = "object",
                                        Properties = new Dictionary<string, OpenApiSchema>
                                        {
                                            ["resourceType"] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Example = new Microsoft.OpenApi.Any.OpenApiString("Patient")
                                            },
                                            ["fhirId"] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Example = new Microsoft.OpenApi.Any.OpenApiString("patient-123")
                                            },
                                            ["status"] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Example = new Microsoft.OpenApi.Any.OpenApiString("Success")
                                            }
                                        }
                                    }
                                },
                                ["errors"] = new OpenApiSchema
                                {
                                    Type = "array",
                                    Description = "List of import errors",
                                    Items = new OpenApiSchema
                                    {
                                        Type = "object",
                                        Properties = new Dictionary<string, OpenApiSchema>
                                        {
                                            ["resourceType"] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Example = new Microsoft.OpenApi.Any.OpenApiString("Patient")
                                            },
                                            ["fhirId"] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Example = new Microsoft.OpenApi.Any.OpenApiString("patient-456")
                                            },
                                            ["error"] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Example = new Microsoft.OpenApi.Any.OpenApiString("Invalid resource format")
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
            }
            
            // Add error responses
            if (operation.Responses?.ContainsKey("400") == true)
            {
                operation.Responses["400"].Description = "Invalid request - missing file or invalid JSON format";
            }
            
            if (operation.Responses?.ContainsKey("401") == true)
            {
                operation.Responses["401"].Description = "Unauthorized - missing or invalid authentication";
            }
            
            if (operation.Responses?.ContainsKey("500") == true)
            {
                operation.Responses["500"].Description = "Internal server error during import process";
            }
        }
    }
}
