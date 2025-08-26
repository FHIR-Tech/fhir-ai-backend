using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HealthTech.API.Swagger;

/// <summary>
/// Schema filter for FHIR resources to provide better documentation
/// </summary>
public class FhirResourceSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// Apply the filter to the schema
    /// </summary>
    /// <param name="schema">OpenAPI schema</param>
    /// <param name="context">Schema filter context</param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;

        // Add FHIR-specific examples and descriptions
        if (type.Name.Contains("FhirResource") || type.Name.Contains("Patient") || type.Name.Contains("Observation"))
        {
            schema.Description = "FHIR R4B compliant resource following HL7 FHIR specification";
            
            if (type.Name.Contains("Patient"))
            {
                schema.Example = new OpenApiObject
                {
                    ["resourceType"] = new OpenApiString("Patient"),
                    ["id"] = new OpenApiString("example-patient-123"),
                    ["identifier"] = new OpenApiArray
                    {
                        new OpenApiObject
                        {
                            ["system"] = new OpenApiString("https://healthtech.com/patients"),
                            ["value"] = new OpenApiString("MRN123456")
                        }
                    },
                    ["name"] = new OpenApiArray
                    {
                        new OpenApiObject
                        {
                            ["use"] = new OpenApiString("official"),
                            ["family"] = new OpenApiString("Smith"),
                            ["given"] = new OpenApiArray
                            {
                                new OpenApiString("John"),
                                new OpenApiString("Michael")
                            }
                        }
                    },
                    ["gender"] = new OpenApiString("male"),
                    ["birthDate"] = new OpenApiString("1990-01-15"),
                    ["address"] = new OpenApiArray
                    {
                        new OpenApiObject
                        {
                            ["use"] = new OpenApiString("home"),
                            ["type"] = new OpenApiString("physical"),
                            ["line"] = new OpenApiArray
                            {
                                new OpenApiString("123 Main St"),
                                new OpenApiString("Apt 4B")
                            },
                            ["city"] = new OpenApiString("Anytown"),
                            ["state"] = new OpenApiString("CA"),
                            ["postalCode"] = new OpenApiString("12345"),
                            ["country"] = new OpenApiString("US")
                        }
                    }
                };
            }
            else if (type.Name.Contains("Observation"))
            {
                schema.Example = new OpenApiObject
                {
                    ["resourceType"] = new OpenApiString("Observation"),
                    ["id"] = new OpenApiString("example-observation-456"),
                    ["status"] = new OpenApiString("final"),
                    ["category"] = new OpenApiArray
                    {
                        new OpenApiObject
                        {
                            ["coding"] = new OpenApiArray
                            {
                                new OpenApiObject
                                {
                                    ["system"] = new OpenApiString("http://terminology.hl7.org/CodeSystem/observation-category"),
                                    ["code"] = new OpenApiString("vital-signs"),
                                    ["display"] = new OpenApiString("Vital Signs")
                                }
                            }
                        }
                    },
                    ["code"] = new OpenApiObject
                    {
                        ["coding"] = new OpenApiArray
                        {
                            new OpenApiObject
                            {
                                ["system"] = new OpenApiString("http://loinc.org"),
                                ["code"] = new OpenApiString("8302-2"),
                                ["display"] = new OpenApiString("Body height")
                            }
                        }
                    },
                    ["subject"] = new OpenApiObject
                    {
                        ["reference"] = new OpenApiString("Patient/example-patient-123")
                    },
                    ["effectiveDateTime"] = new OpenApiString("2023-12-01T10:30:00Z"),
                    ["valueQuantity"] = new OpenApiObject
                    {
                        ["value"] = new OpenApiDouble(175.0),
                        ["unit"] = new OpenApiString("cm"),
                        ["system"] = new OpenApiString("http://unitsofmeasure.org"),
                        ["code"] = new OpenApiString("cm")
                    }
                };
            }
        }

        // Add examples for common DTOs
        if (type.Name.Contains("SearchFhirResourcesQuery"))
        {
            schema.Example = new OpenApiObject
            {
                ["resourceType"] = new OpenApiString("Patient"),
                ["skip"] = new OpenApiInteger(0),
                ["take"] = new OpenApiInteger(10)
            };
        }

        if (type.Name.Contains("CreateFhirResourceCommand"))
        {
            schema.Example = new OpenApiObject
            {
                ["resourceType"] = new OpenApiString("Patient"),
                ["fhirId"] = new OpenApiString("new-patient-789"),
                ["resourceJson"] = new OpenApiString("{\"resourceType\":\"Patient\",\"name\":[{\"family\":\"Doe\",\"given\":[\"Jane\"]}]}")
            };
        }

        // Add validation information
        if (schema.Properties != null)
        {
            foreach (var property in schema.Properties)
            {
                switch (property.Key.ToLower())
                {
                    case "resourcetype":
                        property.Value.Description = "FHIR resource type (e.g., Patient, Observation, Encounter)";
                        property.Value.Example = new OpenApiString("Patient");
                        break;
                    case "fhirid":
                        property.Value.Description = "Unique identifier for the FHIR resource";
                        property.Value.Example = new OpenApiString("patient-123");
                        break;
                    case "resourcejson":
                        property.Value.Description = "FHIR resource in JSON format following R4 specification";
                        property.Value.Example = new OpenApiString("{\"resourceType\":\"Patient\",\"name\":[{\"family\":\"Smith\",\"given\":[\"John\"]}]}");
                        break;
                    case "tenantid":
                        property.Value.Description = "Tenant identifier for multi-tenancy support";
                        property.Value.Example = new OpenApiString("demo-tenant");
                        break;
                    case "skip":
                        property.Value.Description = "Number of records to skip for pagination";
                        property.Value.Example = new OpenApiInteger(0);
                        break;
                    case "take":
                        property.Value.Description = "Number of records to return (max 100)";
                        property.Value.Example = new OpenApiInteger(10);
                        break;
                }
            }
        }
    }
}
