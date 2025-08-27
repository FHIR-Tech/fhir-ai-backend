# Enhanced FHIR Export Bundle Guide

## Overview

The Enhanced FHIR Export Bundle endpoint provides advanced filtering capabilities for exporting FHIR resources, with special focus on time-based filtering and observation-specific queries for healthcare data analysis.

## Key Features

### 1. Time-Based Filtering
- **Relative Time Periods**: Filter by days, weeks, months, years
- **Absolute Date Ranges**: Filter by specific start and end dates
- **Flexible Combinations**: Mix relative and absolute time filtering

### 2. Observation-Specific Filtering
- **LOINC Code Filtering**: Filter by specific lab test codes
- **Patient-Specific Queries**: Filter observations for specific patients
- **Quantity Limiting**: Limit number of observations per patient
- **Sorting Options**: Ascending or descending order

### 3. Enhanced Performance
- **Optimized PostgreSQL Queries**: Fixed JSONB query issues
- **Efficient Date Filtering**: Uses indexed date fields
- **Smart Resource Limiting**: Prevents performance issues

## API Endpoints

### GET /fhir/$export-bundle
Simple query parameter-based exports with enhanced filtering.

### POST /fhir/$export-bundle
Complex JSON body-based exports for advanced use cases.

## Parameters

### Basic Parameters
- `resourceType`: FHIR resource type to export
- `fhirIds`: Comma-separated list of specific resource IDs
- `maxResources`: Maximum number of resources (default: 1000)
- `bundleType`: Bundle type (default: collection)
- `includeHistory`: Include resource history (default: false)
- `maxHistoryVersions`: Maximum history versions per resource (default: 10)
- `includeDeleted`: Include deleted resources (default: false)
- `format`: Export format (default: json)

### Time-Based Filtering Parameters
- `startDate`: Start date in ISO 8601 format
- `endDate`: End date in ISO 8601 format
- `timePeriod`: Time period (days, weeks, months, years)
- `timePeriodCount`: Number of time periods to look back

### Observation-Specific Parameters
- `observationCode`: LOINC code for specific lab tests
- `observationSystem`: Coding system (e.g., http://loinc.org)
- `patientId`: Patient ID for filtering observations
- `maxObservationsPerPatient`: Limit observations per patient
- `sortOrder`: Sort order (asc, desc)
- `latestOnly`: Include only latest observation per patient

## Usage Examples

### 1. Export Weight Observations for Last 10 Measurements

```bash
GET /fhir/$export-bundle?resourceType=Observation&observationCode=29463-7&maxObservationsPerPatient=10&sortOrder=desc
```

**Response:**
```json
{
  "resourceType": "Bundle",
  "type": "collection",
  "timestamp": "2024-12-19T10:30:00Z",
  "total": 10,
  "entry": [
    {
      "resource": {
        "resourceType": "Observation",
        "id": "obs-123",
        "code": {
          "coding": [{
            "system": "http://loinc.org",
            "code": "29463-7",
            "display": "Body weight"
          }]
        },
        "valueQuantity": {
          "value": 70.5,
          "unit": "kg"
        },
        "effectiveDateTime": "2024-12-19T10:00:00Z"
      }
    }
  ]
}
```

### 2. Export Observations for Last 7 Days

```bash
GET /fhir/$export-bundle?resourceType=Observation&timePeriod=days&timePeriodCount=7&latestOnly=true
```

### 3. Export Observations for Last 30 Days

```bash
GET /fhir/$export-bundle?resourceType=Observation&timePeriod=days&timePeriodCount=30
```

### 4. Export Observations for Last 1 Year

```bash
GET /fhir/$export-bundle?resourceType=Observation&timePeriod=years&timePeriodCount=1
```

### 5. Export Observations for Specific Time Range

```bash
GET /fhir/$export-bundle?resourceType=Observation&startDate=2024-01-01T00:00:00Z&endDate=2024-12-31T23:59:59Z&observationCode=29463-7
```

### 6. Export Different Observation Types

#### Body Weight
```bash
GET /fhir/$export-bundle?resourceType=Observation&observationCode=29463-7&observationSystem=http://loinc.org&maxObservationsPerPatient=5&timePeriod=days&timePeriodCount=30
```

#### Body Height
```bash
GET /fhir/$export-bundle?resourceType=Observation&observationCode=8302-2&observationSystem=http://loinc.org&maxObservationsPerPatient=5&timePeriod=days&timePeriodCount=30
```

#### Blood Pressure Systolic
```bash
GET /fhir/$export-bundle?resourceType=Observation&observationCode=85354-9&observationSystem=http://loinc.org&maxObservationsPerPatient=5&timePeriod=days&timePeriodCount=30
```

#### Blood Pressure Diastolic
```bash
GET /fhir/$export-bundle?resourceType=Observation&observationCode=8462-4&observationSystem=http://loinc.org&maxObservationsPerPatient=5&timePeriod=days&timePeriodCount=30
```

#### Heart Rate
```bash
GET /fhir/$export-bundle?resourceType=Observation&observationCode=8867-4&observationSystem=http://loinc.org&maxObservationsPerPatient=5&timePeriod=days&timePeriodCount=30
```

#### Oxygen Saturation
```bash
GET /fhir/$export-bundle?resourceType=Observation&observationCode=2708-6&observationSystem=http://loinc.org&maxObservationsPerPatient=5&timePeriod=days&timePeriodCount=30
```

### 7. Complex POST Query

```bash
POST /fhir/$export-bundle
Content-Type: application/json

{
  "resourceType": "Observation",
  "observationCode": "29463-7",
  "observationSystem": "http://loinc.org",
  "timePeriod": "days",
  "timePeriodCount": 30,
  "maxObservationsPerPatient": 10,
  "sortOrder": "desc",
  "latestOnly": true,
  "bundleType": "collection"
}
```

## Common Observation Codes

| Code | Name | System |
|------|------|--------|
| 29463-7 | Body Weight | http://loinc.org |
| 8302-2 | Body Height | http://loinc.org |
| 85354-9 | Blood Pressure Systolic | http://loinc.org |
| 8462-4 | Blood Pressure Diastolic | http://loinc.org |
| 8867-4 | Heart Rate | http://loinc.org |
| 2708-6 | Oxygen Saturation | http://loinc.org |

## Healthcare Use Cases

### 1. Patient Weight Trend Analysis
Export weight measurements over time for trend analysis:

```bash
GET /fhir/$export-bundle?resourceType=Observation&observationCode=29463-7&timePeriod=months&timePeriodCount=6&maxObservationsPerPatient=20&sortOrder=desc
```

### 2. Blood Pressure Monitoring
Export blood pressure measurements for monitoring:

```bash
GET /fhir/$export-bundle?resourceType=Observation&observationCode=85354-9&timePeriod=days&timePeriodCount=30&maxObservationsPerPatient=10
```

### 3. Vital Signs Monitoring
Export latest vital signs for patient monitoring:

```bash
GET /fhir/$export-bundle?resourceType=Observation&timePeriod=days&timePeriodCount=7&latestOnly=true
```

### 4. Long-term Health Tracking
Export long-term health data for analysis:

```bash
GET /fhir/$export-bundle?resourceType=Observation&timePeriod=years&timePeriodCount=1&observationCode=29463-7&maxObservationsPerPatient=50
```

## Time Period Examples

### Relative Time Periods
- **Last 7 days**: `timePeriod=days&timePeriodCount=7`
- **Last 30 days**: `timePeriod=days&timePeriodCount=30`
- **Last 4 weeks**: `timePeriod=weeks&timePeriodCount=4`
- **Last 6 months**: `timePeriod=months&timePeriodCount=6`
- **Last 1 year**: `timePeriod=years&timePeriodCount=1`

### Absolute Date Ranges
- **Specific range**: `startDate=2024-01-01T00:00:00Z&endDate=2024-12-31T23:59:59Z`

## Performance Considerations

### 1. Resource Limits
- Default maximum: 1000 resources
- Adjust based on your needs
- Consider pagination for large datasets

### 2. Time Range Optimization
- Use specific date ranges for better performance
- Avoid very large time periods
- Consider using relative periods for recent data

### 3. Observation Filtering
- Always specify observation codes when filtering
- Use `maxObservationsPerPatient` to limit results
- Use `latestOnly=true` for current values only

## Error Handling

### Common Errors
- **400 Bad Request**: Invalid parameters
- **404 Not Found**: No resources match criteria
- **500 Internal Server Error**: Database or processing error

### Error Response Format
```json
{
  "error": "Error message",
  "details": "Additional error details",
  "timestamp": "2024-12-19T10:30:00Z"
}
```

## Testing

### Using the Test Scripts
```bash
# Run basic tests
node scripts/api/test-export-bundle.js

# Run enhanced tests
node scripts/api/test-enhanced-export-bundle.js
```

### Manual Testing with curl
```bash
# Test weight observations
curl -X GET "http://localhost:5000/fhir/\$export-bundle?resourceType=Observation&observationCode=29463-7&maxObservationsPerPatient=10" \
  -H "Content-Type: application/json"

# Test time-based filtering
curl -X GET "http://localhost:5000/fhir/\$export-bundle?resourceType=Observation&timePeriod=days&timePeriodCount=30" \
  -H "Content-Type: application/json"
```

## Best Practices

### 1. Always Specify Resource Type
```bash
# Good
GET /fhir/$export-bundle?resourceType=Observation&observationCode=29463-7

# Avoid
GET /fhir/$export-bundle?observationCode=29463-7
```

### 2. Use Appropriate Time Ranges
```bash
# Good - specific time range
GET /fhir/$export-bundle?resourceType=Observation&timePeriod=days&timePeriodCount=30

# Avoid - very large time range
GET /fhir/$export-bundle?resourceType=Observation&timePeriod=years&timePeriodCount=10
```

### 3. Limit Results Appropriately
```bash
# Good - reasonable limit
GET /fhir/$export-bundle?resourceType=Observation&maxObservationsPerPatient=10

# Avoid - very large limit
GET /fhir/$export-bundle?resourceType=Observation&maxObservationsPerPatient=1000
```

### 4. Use Latest Only When Appropriate
```bash
# Good - for current values
GET /fhir/$export-bundle?resourceType=Observation&latestOnly=true

# Good - for trend analysis
GET /fhir/$export-bundle?resourceType=Observation&maxObservationsPerPatient=20
```

## Troubleshooting

### Common Issues

#### 1. No Results Returned
- Check if resources exist in the database
- Verify time range parameters
- Ensure observation codes are correct

#### 2. Performance Issues
- Reduce time range
- Limit number of resources
- Use specific observation codes

#### 3. Invalid Parameters
- Check parameter names and values
- Ensure dates are in ISO 8601 format
- Verify observation codes exist

### Debug Tips
1. Start with simple queries
2. Add parameters one by one
3. Check response metadata
4. Use smaller time ranges for testing

## Support

For additional support or questions about the Enhanced Export Bundle functionality, please refer to:
- API Documentation: `/swagger`
- Test Scripts: `scripts/api/`
- Sample Data: `scripts/samples/`
