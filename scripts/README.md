# FHIR-AI Backend Sample Data Generation

This directory contains scripts to populate the FHIR-AI Backend with realistic healthcare sample data using the REST API endpoints.

## Overview

The sample data generator creates a comprehensive dataset including:
- **Patients** with demographic information (names, addresses, contact details)
- **Observations** (vital signs, lab results, measurements)
- **Encounters** (medical visits, procedures)
- **Conditions** (diagnoses, problems)
- **MedicationRequests** (prescriptions)

## Prerequisites

1. **FHIR-AI Backend Application**: Make sure the backend is running
2. **Database**: Ensure PostgreSQL is running and the database is initialized
3. **Node.js** (optional): For running the JavaScript version
4. **PowerShell** (optional): For running the PowerShell version

## Quick Start

### Option 1: Windows Batch File (Recommended)
```bash
# Double-click or run from command line
scripts/run-sample-data.bat
```

### Option 2: Node.js Script
```bash
# Make sure Node.js is installed
node scripts/sample-data-api.js
```

### Option 3: PowerShell Script
```powershell
# Run PowerShell script
.\scripts\sample-data-api.ps1
```

## Configuration

### API URL
The scripts are configured to use `https://localhost:52871` by default. If your API is running on a different port, update the `API_BASE_URL` variable in the scripts.

### Tenant ID
Default tenant ID is `demo-tenant`. You can change this by modifying the `TENANT_ID` variable.

### FHIR Scopes
Default scopes are `user/* patient/*`. Modify the `FHIR_SCOPES` variable if needed.

## Sample Data Details

### Patients (5 records)
- John Michael Smith (Male, 1990-01-15)
- Emily Rose Johnson (Female, 1985-06-22)
- Robert David Williams (Male, 1978-12-03)
- Lisa Marie Brown (Female, 1992-03-18)
- Michael James Davis (Male, 1965-09-30)

Each patient includes:
- Full name and demographic information
- Address and contact details
- Marital status
- Emergency contacts (where applicable)

### Observations (5 types per patient = 25 total)
- **Blood Pressure**: Systolic and diastolic measurements
- **Body Weight**: Weight measurements in kg
- **Body Height**: Height measurements in cm
- **Heart Rate**: Heart rate in beats/min
- **Temperature**: Body temperature in Celsius

### Encounters (2 per patient = 10 total)
- General medical examinations
- Random dates between 2023-01-01 and current date
- 1-4 hour duration

### Conditions (3 total)
- Hypertensive disorder
- Diabetes mellitus
- Assigned to every other patient

### Medication Requests (3 total)
- Lisinopril 10 MG Oral Tablet
- Metformin 500 MG Oral Tablet
- Assigned to patients with conditions

## Expected Output

When successful, you should see output similar to:
```
🚀 Starting FHIR-AI Backend Sample Data Generation...

📋 Creating Patients...
✅ Created Patient with ID: patient-001
✅ Created Patient with ID: patient-002
...

🔬 Creating Observations...
✅ Created Observation with ID: obs-001
✅ Created Observation with ID: obs-002
...

📊 Sample Data Generation Complete!
=====================================
✅ Patients created: 5
✅ Observations created: 25
✅ Encounters created: 10
✅ Conditions created: 3
✅ Medication Requests created: 3

🎉 Total FHIR resources created: 46
```

## Troubleshooting

### Authentication Error
If you get authentication errors, ensure:
1. The backend application is running
2. The development authentication is properly configured
3. The API URL is correct

### Connection Error
If you get connection errors:
1. Check if the backend is running on the correct port
2. Verify the API URL in the script
3. Ensure no firewall is blocking the connection

### Database Error
If you get database errors:
1. Ensure PostgreSQL is running
2. Check that the database is initialized with `scripts/init-db.sql`
3. Verify connection strings in `appsettings.json`

## API Endpoints Used

The scripts use the following FHIR endpoints:
- `POST /fhir/Patient` - Create patients
- `POST /fhir/Observation` - Create observations
- `POST /fhir/Encounter` - Create encounters
- `POST /fhir/Condition` - Create conditions
- `POST /fhir/MedicationRequest` - Create medication requests
- `GET /fhir/Patient` - Verify patient creation
- `GET /fhir/Observation` - Verify observation creation

## Data Validation

After running the script, you can validate the data by:
1. Using the Swagger UI at `https://localhost:52871`
2. Making GET requests to retrieve the created resources
3. Checking the database directly

## Customization

To customize the sample data:
1. Modify the sample data arrays in the scripts
2. Add new resource types by extending the scripts
3. Change the data generation logic for different scenarios

## Security Notes

- The development authentication allows all requests for testing
- In production, proper OAuth2/SMART on FHIR authentication should be used
- The sample data contains realistic but fictional patient information
- No real patient data is used in the sample data
