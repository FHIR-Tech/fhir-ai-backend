using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HealthTech.API.Swagger;

/// <summary>
/// Swagger operation filter to provide default values and better documentation
/// </summary>
public class SwaggerDefaultValues : IOperationFilter
{
    /// <summary>
    /// Apply the filter to the operation
    /// </summary>
    /// <param name="operation">OpenAPI operation</param>
    /// <param name="context">Operation filter context</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        // Set operation ID if not already set
        if (string.IsNullOrEmpty(operation.OperationId))
        {
            operation.OperationId = apiDescription.ActionDescriptor.DisplayName;
        }

        // Add default responses
        if (!operation.Responses.ContainsKey("400"))
        {
            operation.Responses.Add("400", new OpenApiResponse
            {
                Description = "Bad Request - Invalid request parameters or body",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.Schema,
                                Id = "ProblemDetails"
                            }
                        }
                    }
                }
            });
        }

        if (!operation.Responses.ContainsKey("401"))
        {
            operation.Responses.Add("401", new OpenApiResponse
            {
                Description = "Unauthorized - Authentication required",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.Schema,
                                Id = "ProblemDetails"
                            }
                        }
                    }
                }
            });
        }

        if (!operation.Responses.ContainsKey("403"))
        {
            operation.Responses.Add("403", new OpenApiResponse
            {
                Description = "Forbidden - Insufficient permissions for the requested scope",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.Schema,
                                Id = "ProblemDetails"
                            }
                        }
                    }
                }
            });
        }

        if (!operation.Responses.ContainsKey("404"))
        {
            operation.Responses.Add("404", new OpenApiResponse
            {
                Description = "Not Found - Resource not found",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.Schema,
                                Id = "ProblemDetails"
                            }
                        }
                    }
                }
            });
        }

        if (!operation.Responses.ContainsKey("429"))
        {
            operation.Responses.Add("429", new OpenApiResponse
            {
                Description = "Too Many Requests - Rate limit exceeded",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.Schema,
                                Id = "ProblemDetails"
                            }
                        }
                    }
                }
            });
        }

        if (!operation.Responses.ContainsKey("500"))
        {
            operation.Responses.Add("500", new OpenApiResponse
            {
                Description = "Internal Server Error - Unexpected server error",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.Schema,
                                Id = "ProblemDetails"
                            }
                        }
                    }
                }
            });
        }

        // Add FHIR-specific headers
        if (operation.Parameters == null)
        {
            operation.Parameters = new List<OpenApiParameter>();
        }

        // Add tenant header parameter for all FHIR operations (only if not already present)
        if (apiDescription.RelativePath?.StartsWith("fhir") == true)
        {
            // Check if X-Tenant-ID parameter already exists
            var existingTenantParam = operation.Parameters.FirstOrDefault(p => p.Name == "X-Tenant-ID");
            if (existingTenantParam == null)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-Tenant-ID",
                    In = ParameterLocation.Header,
                    Description = "Tenant identifier for multi-tenancy support",
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Example = new Microsoft.OpenApi.Any.OpenApiString("demo-tenant")
                    }
                });
            }

            // Check if X-FHIR-Scopes parameter already exists
            var existingScopesParam = operation.Parameters.FirstOrDefault(p => p.Name == "X-FHIR-Scopes");
            if (existingScopesParam == null)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-FHIR-Scopes",
                    In = ParameterLocation.Header,
                    Description = "FHIR scopes for authorization (space-separated)",
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Example = new Microsoft.OpenApi.Any.OpenApiString("user/* patient/*")
                    }
                });
            }
        }
    }
}
