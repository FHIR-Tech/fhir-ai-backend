# Scripts Directory

This directory contains utility scripts, sample data, and tools for the FHIR-AI Backend project.

## Directory Structure

### `/samples/` - Sample Data Files
- **`/bundles/`** - FHIR Bundle sample files
  - `fhir_bundle_ntkien_2024.json` - Comprehensive FHIR bundle with multiple resources
- **`/resources/`** - Individual FHIR resource samples
- **`/test-data/`** - Test data for development and testing
  - `test_bundle_simple.json` - Simple test bundle for basic functionality
- **`SAMPLE_VALUES.md`** - Documentation of sample values and data structures

### `/database/` - Database Scripts
- **`init-db.sql`** - Database initialization script with schema setup

### `/api/` - API Testing Scripts
- **`sample-data-api.js`** - Node.js script for testing API endpoints with sample data
```Terminal
dotnet run --project src/HealthTech.API
node scripts/api/sample-data-api.js
```

### `/deployment/` - Deployment Scripts
- Deployment automation scripts (to be added)

## Usage Guidelines

### Sample Data
- Use descriptive names with date/version: `{resource-type}_{purpose}_{date}.json`
- Keep sample data realistic but anonymized
- Document any special requirements or dependencies

### Scripts
- All scripts should be executable and well-documented
- Include error handling and logging
- Use environment variables for configuration
- Follow the project's coding standards

### Database Scripts
- Always backup before running database scripts
- Test scripts in development environment first
- Include rollback procedures where possible

## Naming Conventions

- **Files**: `{purpose}_{description}_{date}.{extension}`
- **Directories**: Use kebab-case for multi-word directories
- **Scripts**: Use descriptive names that indicate their purpose

## Security Notes

- Never include real patient data in sample files
- Use environment variables for sensitive configuration
- Validate all input data before processing
- Follow the project's security guidelines
