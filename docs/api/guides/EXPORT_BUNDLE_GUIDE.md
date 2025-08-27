# FHIR Export Bundle Guide

## Overview

Export FHIR resources as standard HL7 FHIR Bundles with filtering and search capabilities.

## Endpoints

### GET /fhir/$export-bundle
Export using query parameters for simple filtering.

**Parameters:**
- `resourceType`: FHIR resource type (optional)
- `fhirIds`: Comma-separated resource IDs (optional)
- `maxResources`: Maximum resources to export (default: 1000)
- `bundleType`: Bundle type (default: collection)
- `includeHistory`: Include resource history (default: false)
- `includeDeleted`: Include deleted resources (default: false)

**Examples:**
```bash
# Export all patients
GET /fhir/$export-bundle?resourceType=Patient&maxResources=100

# Export specific patients with history
GET /fhir/$export-bundle?resourceType=Patient&fhirIds=patient-123&includeHistory=true
```

### POST /fhir/$export-bundle
Export using JSON body for complex queries.

**Request Body:**
```json
{
  "resourceType": "Patient",
  "searchParameters": {
    "name": "Nguyễn"
  },
  "maxResources": 100,
  "bundleType": "collection"
}
```

## Testing

Run the test script:
```bash
node scripts/api/test-export-bundle.js
```

## Response Format

Returns a standard FHIR Bundle:
```json
{
  "resourceType": "Bundle",
  "type": "collection",
  "timestamp": "2024-12-19T10:30:00Z",
  "total": 5,
  "entry": [
    {
      "resource": {
        "resourceType": "Patient",
        "id": "patient-123"
      }
    }
  ]
}
```
