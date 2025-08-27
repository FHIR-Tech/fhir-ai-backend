namespace HealthTech.API.Swagger;

/// <summary>
/// FHIR endpoint descriptions for Swagger documentation
/// </summary>
public static class FhirEndpointDescriptions
{
    // Search endpoints
    public const string SearchFhirResources = @"
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
Returns a FHIR Bundle containing the matching resources with pagination metadata.";

    public const string GetFhirResource = @"
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
- **403 Forbidden**: Insufficient permissions for the requested resource";

    public const string CreateFhirResourceAutoDetect = @"
Create a new FHIR resource with automatic resource type detection from the request body.

### Request Body
The request body should contain a valid FHIR resource in JSON format following R4 specification.
The resource type is automatically detected from the 'resourceType' field in the JSON.

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

### Note
This endpoint automatically detects the resource type from the 'resourceType' field in the request body.
For explicit resource type creation, use POST /fhir/{resourceType} instead.";

    public const string CreateFhirResource = @"
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
Returns the created resource with assigned ID and metadata.";

    public const string UpdateFhirResource = @"
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
Returns the updated resource with new version ID and metadata.";

    public const string DeleteFhirResource = @"
Delete a FHIR resource (soft delete). The resource is marked as deleted but remains in the system for audit purposes.

### Examples
```
DELETE /fhir/Patient/patient-123
DELETE /fhir/Observation/obs-456
```

### Response
Returns deletion confirmation with timestamp and success status.

### Note
This is a soft delete operation. The resource remains in the database but is marked as deleted.";

    public const string ImportFhirBundle = @"
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
Returns import summary with statistics and detailed results for each resource.";

    public const string GetFhirResourceHistory = @"
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
- **maxVersions**: Maximum number of versions to return (default: 100)";

    public const string ExportFhirBundle = @"
Export FHIR resources as a standard HL7 FHIR Bundle. This endpoint supports exporting complete patient records including:

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
You can export resources in two ways:

#### Option 1: GET Method (Simple Queries)
Use query parameters for simple filtering and export operations.

#### Option 2: POST Method (Complex Queries)
Use POST method with JSON body for complex search parameters and advanced filtering.

### Examples

#### Export All Patients
```
GET /fhir/$export-bundle?resourceType=Patient&maxResources=100
```

#### Export Specific Patient with History
```
GET /fhir/$export-bundle?resourceType=Patient&fhirIds=patient-123&includeHistory=true&maxHistoryVersions=5
```

#### Export All Resources of Multiple Types
```
GET /fhir/$export-bundle?maxResources=500&bundleType=collection
```

#### Export with Complex Search (POST)
```json
POST /fhir/$export-bundle
{
  ""resourceType"": ""Patient"",
  ""searchParameters"": {
    ""name"": ""Nguyễn"",
    ""date"": ""2024-01-01""
  },
  ""maxResources"": 100,
  ""includeHistory"": true,
  ""bundleType"": ""collection""
}
```

### Bundle Types Supported
- **collection**: A set of resources (most common for patient records)
- **transaction**: A set of actions to be performed
- **batch**: A set of actions to be performed as a group
- **searchset**: Results from a search operation
- **history**: A list of historical versions of a resource

### Response Format
Returns a standard FHIR Bundle in JSON format following R4 specification:

```json
{
  ""resourceType"": ""Bundle"",
  ""type"": ""collection"",
  ""timestamp"": ""2024-01-15T10:30:00Z"",
  ""total"": 5,
  ""entry"": [
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
        ]
      },
      ""search"": {
        ""mode"": ""match""
      }
    }
  ]
}
```

### Parameters
- **resourceType**: FHIR resource type to export (optional - if null, exports all types)
- **fhirIds**: Comma-separated list of specific FHIR resource IDs to export
- **maxResources**: Maximum number of resources to include in the bundle (default: 1000)
- **bundleType**: Bundle type: collection, transaction, batch, searchset, history (default: collection)
- **includeHistory**: Include resource history in the bundle (default: false)
- **maxHistoryVersions**: Maximum number of history versions per resource (default: 10)
- **includeDeleted**: Include deleted resources in the bundle (default: false)
- **format**: Export format: json, xml (default: json)

### Performance Considerations
- Large exports are limited to 1000 resources by default
- History inclusion can significantly increase bundle size
- Consider using POST method for complex queries with multiple search parameters";

    public const string ExportFhirBundlePost = @"
Export FHIR resources as a bundle using POST method for complex queries with advanced filtering options.

### Use Cases
- Complex search parameters that cannot be expressed in URL query parameters
- Large parameter sets that exceed URL length limits
- Advanced filtering with multiple conditions
- Bulk export operations with specific criteria

### Request Body
```json
{
  ""resourceType"": ""Patient"",
  ""fhirIds"": [""patient-123"", ""patient-456""],
  ""searchParameters"": {
    ""name"": ""Nguyễn"",
    ""identifier"": ""12345"",
    ""date"": ""2024-01-01""
  },
  ""maxResources"": 500,
  ""bundleType"": ""collection"",
  ""includeHistory"": true,
  ""maxHistoryVersions"": 5,
  ""includeDeleted"": false,
  ""format"": ""json""
}
```

### Response
Returns a standard FHIR Bundle in JSON format following R4 specification.";
}
