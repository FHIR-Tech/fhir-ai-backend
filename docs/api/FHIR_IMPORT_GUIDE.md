# FHIR Import Endpoint Guide (HL7 Standard)

## Overview

The FHIR import endpoint allows you to import FHIR resources from a standard HL7 FHIR Bundle. This endpoint supports importing complete patient records including organizations, patients, encounters, observations, and other healthcare resources following the HL7 FHIR R4 specification.

## Endpoint Details

- **URL**: `POST /fhir/$import-bundle`
- **Authentication**: Required (API Key or JWT Bearer token)
- **Content-Type**: 
  - `application/json` (for JSON input)
  - `multipart/form-data` (for file upload)

## Usage Methods

### Method 1: JSON Body Input

Send the FHIR Bundle directly in the request body as JSON:

```bash
curl -X POST "https://your-api-url/fhir/$import-bundle" \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -H "X-Tenant-ID: your-tenant-id" \
  -d @sample-fhir-bundle.json
```

### Method 2: File Upload

Upload a JSON file containing the FHIR Bundle:

```bash
curl -X POST "https://your-api-url/fhir/$import-bundle" \
  -H "X-API-Key: your-api-key" \
  -H "X-Tenant-ID: your-tenant-id" \
  -F "bundleFile=@sample-fhir-bundle.json" \
  -F "description=Import patient data from external system" \
  -F "validate=true"
```

## File Upload Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `bundleFile` | File | Yes | JSON file containing FHIR Bundle |
| `description` | String | No | Optional description for the import operation |
| `validate` | Boolean | No | Whether to validate FHIR resources (default: true) |

## File Requirements

- **Supported formats**: `.json` only
- **Maximum file size**: 50MB (for large patient records)
- **Content**: Valid HL7 FHIR Bundle in JSON format following R4 specification

## FHIR Bundle Format (HL7 Standard)

The FHIR Bundle must follow the HL7 FHIR R4 specification. Here's an example of a complete patient record:

```json
{
  "resourceType": "Bundle",
  "type": "collection",
  "timestamp": "2024-01-15T10:30:00Z",
  "entry": [
    {
      "resource": {
        "resourceType": "Organization",
        "id": "org-hospital",
        "name": "Hospital Name"
      }
    },
    {
      "resource": {
        "resourceType": "Patient",
        "id": "patient-123",
        "name": [
          {
            "use": "official",
            "family": "Nguyễn",
            "given": ["Trung", "Kiên"]
          }
        ],
        "gender": "male",
        "birthDate": "1993-01-08"
      }
    },
    {
      "resource": {
        "resourceType": "Encounter",
        "id": "encounter-1",
        "status": "finished",
        "subject": {
          "reference": "Patient/patient-123"
        },
        "serviceProvider": {
          "reference": "Organization/org-hospital"
        }
      }
    }
  ]
}
```

## Supported FHIR Resources

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

## Bundle Types Supported

- **collection**: A set of resources (most common for patient records)
- **transaction**: A set of actions to be performed
- **batch**: A set of actions to be performed as a group
- **searchset**: Results from a search operation
- **history**: A list of historical versions of a resource

## Response Format

### Success Response (200)

```json
{
  "importJobId": "import-12345-abcde",
  "successfullyImported": 5,
  "failedToImport": 0,
  "importedResources": [
    {
      "resourceType": "Patient",
      "fhirId": "patient-123",
      "status": "Success"
    }
  ],
  "errors": []
}
```

### Error Response (400)

```json
{
  "error": "File size exceeds 50MB limit"
}
```

## Swagger UI Features

When using the Swagger UI interface:

1. **File Upload Interface**: Enhanced drag-and-drop file upload
2. **File Validation**: Automatic file type and size validation
3. **Visual Feedback**: Progress indicators and status messages
4. **Sample Data**: Pre-filled examples for testing

### Using Swagger UI

1. Navigate to the Swagger UI at your API root URL
2. Find the `POST /fhir/$import-bundle` endpoint
3. Click "Try it out"
4. Choose between JSON input or file upload
5. For file upload:
   - Click the file upload area or drag a file
   - Select your JSON file
   - Optionally add description and validation settings
6. Click "Execute"

## Testing

### Sample Files

Use the provided `sample-fhir-bundle.json` file for testing, or use the actual patient record file:

```bash
# Test with sample file
curl -X POST "https://your-api-url/fhir/$import-bundle" \
  -H "X-API-Key: demo-api-key-12345" \
  -H "X-Tenant-ID: demo-tenant" \
  -F "bundleFile=@sample-fhir-bundle.json"

# Test with actual patient record
curl -X POST "https://your-api-url/fhir/$import-bundle" \
  -H "X-API-Key: demo-api-key-12345" \
  -H "X-Tenant-ID: demo-tenant" \
  -F "bundleFile=@scripts/fhir_bundle_ntkien_2024.json"
```

### Validation

The endpoint performs the following validations:

1. **File Size**: Maximum 50MB
2. **File Type**: Only `.json` files
3. **JSON Format**: Valid JSON structure
4. **FHIR Bundle**: Valid FHIR Bundle format
5. **Resource Validation**: FHIR resource validation (if enabled)

## Error Handling

Common error scenarios:

| Error | Description | Solution |
|-------|-------------|----------|
| `No FHIR bundle file provided` | Missing file in upload | Ensure file is selected |
| `File size exceeds 50MB limit` | File too large | Reduce file size or split bundle |
| `Only .json files are supported for FHIR bundles` | Invalid file type | Use .json files only |
| `Invalid JSON format in FHIR Bundle` | Malformed JSON | Validate JSON structure |
| `Invalid FHIR Bundle: resourceType must be 'Bundle'` | Invalid bundle format | Check FHIR Bundle specification |
| `Invalid FHIR Bundle: missing 'entry' array` | Missing resources | Ensure bundle contains entry array |
| `Invalid FHIR Bundle: 'entry' must be an array` | Invalid entry format | Check entry array structure |

## Best Practices

1. **File Size**: Keep bundles under 50MB for optimal performance
2. **Resource Count**: Limit to 5000 resources per bundle
3. **Validation**: Enable validation for production imports
4. **Error Handling**: Check response for import errors
5. **Backup**: Always backup data before large imports
6. **Testing**: Test with small bundles before large imports
7. **FHIR Compliance**: Ensure all resources follow HL7 FHIR R4 specification
8. **References**: Use proper resource references within the bundle

## Security Considerations

- **Authentication**: Always use proper authentication
- **Tenant Isolation**: Ensure tenant context is set
- **File Validation**: Validate all uploaded files
- **Rate Limiting**: Respect API rate limits
- **Audit Logging**: All imports are logged for compliance

## Troubleshooting

### Common Issues

1. **File not uploading**: Check file size and format
2. **Authentication errors**: Verify API key and tenant ID
3. **Import failures**: Check FHIR resource validation
4. **Timeout errors**: Reduce bundle size or split into smaller bundles

### Getting Help

For technical support:
- Email: support@healthtech.com
- Documentation: https://healthtech.com/docs
- API Status: https://status.healthtech.com
