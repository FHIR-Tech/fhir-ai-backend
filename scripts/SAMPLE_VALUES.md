# FHIR-AI Backend Sample Values Reference

This document provides a comprehensive reference of all sample values used in the FHIR-AI Backend sample data generation.

## 📋 Patient Sample Values

### Patient 1: John Michael Smith
- **Name**: John Michael Smith
- **Gender**: male
- **Birth Date**: 1990-01-15
- **Address**: 123 Main St, Springfield, IL 62701
- **Phone**: +1-555-0123
- **Email**: john.smith@email.com
- **Marital Status**: Married
- **Emergency Contact**: Sarah Jane Smith (+1-555-0124)

### Patient 2: Emily Rose Johnson
- **Name**: Emily Rose Johnson
- **Gender**: female
- **Birth Date**: 1985-06-22
- **Address**: 456 Oak Ave, Springfield, IL 62702
- **Phone**: +1-555-0125
- **Email**: emily.johnson@email.com
- **Marital Status**: Single

### Patient 3: Robert David Williams
- **Name**: Robert David Williams
- **Gender**: male
- **Birth Date**: 1978-12-03
- **Address**: 789 Pine St, Springfield, IL 62703
- **Phone**: +1-555-0126
- **Email**: robert.williams@email.com
- **Marital Status**: Married

### Patient 4: Lisa Marie Brown
- **Name**: Lisa Marie Brown
- **Gender**: female
- **Birth Date**: 1992-03-18
- **Address**: 321 Elm St, Springfield, IL 62704
- **Phone**: +1-555-0127
- **Email**: lisa.brown@email.com
- **Marital Status**: Divorced

### Patient 5: Michael James Davis
- **Name**: Michael James Davis
- **Gender**: male
- **Birth Date**: 1965-09-30
- **Address**: 654 Maple Dr, Springfield, IL 62705
- **Phone**: +1-555-0128
- **Email**: michael.davis@email.com
- **Marital Status**: Married

## 🔬 Observation Sample Values

### 1. Blood Pressure
- **Type**: Blood Pressure Panel
- **LOINC Code**: 85354-9
- **Systolic**: 120 mmHg (LOINC: 8480-6)
- **Diastolic**: 80 mmHg (LOINC: 8462-4)
- **Unit System**: http://unitsofmeasure.org
- **Status**: final

### 2. Body Weight
- **Type**: Body Weight
- **LOINC Code**: 29463-7
- **Value**: 70 kg
- **Unit**: kg
- **Unit System**: http://unitsofmeasure.org
- **Status**: final

### 3. Body Height
- **Type**: Body Height
- **LOINC Code**: 8302-2
- **Value**: 175 cm
- **Unit**: cm
- **Unit System**: http://unitsofmeasure.org
- **Status**: final

### 4. Heart Rate
- **Type**: Heart Rate
- **LOINC Code**: 8867-4
- **Value**: 72 beats/min
- **Unit**: /min
- **Unit System**: http://unitsofmeasure.org
- **Status**: final

### 5. Temperature
- **Type**: Body Temperature
- **LOINC Code**: 8310-5
- **Value**: 37.0°C
- **Unit**: Cel
- **Unit System**: http://unitsofmeasure.org
- **Status**: final

## 🏥 Encounter Sample Values

### General Medical Examination
- **Type**: General Medical Examination
- **SNOMED Code**: 185389000
- **Status**: finished
- **Class**: ambulatory (AMB)
- **Duration**: 1-4 hours
- **Reason**: General medical examination

## 💊 Condition Sample Values

### 1. Hypertensive Disorder
- **Name**: Hypertensive disorder
- **SNOMED Code**: 38341003
- **Clinical Status**: active
- **Verification Status**: confirmed
- **Category**: Problem List Item
- **Onset**: Random date between 2020-2023
- **Recorded**: Random date between 2023-present

### 2. Diabetes Mellitus
- **Name**: Diabetes mellitus
- **SNOMED Code**: 73211009
- **Clinical Status**: active
- **Verification Status**: confirmed
- **Category**: Problem List Item
- **Onset**: Random date between 2020-2023
- **Recorded**: Random date between 2023-present

## 💊 Medication Request Sample Values

### 1. Lisinopril 10 MG Oral Tablet
- **Name**: Lisinopril 10 MG Oral Tablet
- **RxNorm Code**: 197361
- **Status**: active
- **Intent**: order
- **Dosage**: Take 1 tablet daily
- **Route**: Oral (SNOMED: 26643006)
- **Frequency**: 1 per day
- **Period**: 1 day

### 2. Metformin 500 MG Oral Tablet
- **Name**: Metformin 500 MG Oral Tablet
- **RxNorm Code**: 860975
- **Status**: active
- **Intent**: order
- **Dosage**: Take 1 tablet twice daily with meals
- **Route**: Oral (SNOMED: 26643006)
- **Frequency**: 2 per day
- **Period**: 1 day

## 📊 Data Generation Summary

### Resource Distribution
- **Patients**: 5 records (one for each sample patient)
- **Observations**: 25 records (5 types × 5 patients)
- **Encounters**: 10 records (2 types × 5 patients)
- **Conditions**: 3 records (assigned to every other patient)
- **Medication Requests**: 3 records (assigned to patients with conditions)

### Total Resources: 46 FHIR resources

## 🔧 Technical Details

### API Configuration
- **Base URL**: https://localhost:52871
- **Tenant ID**: demo-tenant
- **FHIR Scopes**: user/* patient/*
- **Content Type**: application/json

### Data Variation
- **Observation Values**: ±10% random variation from base values
- **Dates**: Random dates within specified ranges
- **Patient Assignment**: Resources linked to created patient IDs

### FHIR Standards Used
- **LOINC**: For observation codes
- **SNOMED CT**: For conditions and encounter types
- **RxNorm**: For medication codes
- **UCUM**: For units of measurement
- **HL7 Terminology**: For status codes and categories

## 🚀 Quick Test Commands

### Test Single Patient Creation
```bash
curl -X POST "https://localhost:52871/fhir/Patient" \
  -H "X-Tenant-ID: demo-tenant" \
  -H "X-FHIR-Scopes: user/* patient/*" \
  -H "Content-Type: application/json" \
  -d '{
    "resourceType": "Patient",
    "name": [{"use": "official", "family": "Test", "given": ["John"]}],
    "gender": "male",
    "birthDate": "1990-01-01"
  }'
```

### Test Single Observation Creation
```bash
curl -X POST "https://localhost:52871/fhir/Observation" \
  -H "X-Tenant-ID: demo-tenant" \
  -H "X-FHIR-Scopes: user/* patient/*" \
  -H "Content-Type: application/json" \
  -d '{
    "resourceType": "Observation",
    "status": "final",
    "code": {"coding": [{"system": "http://loinc.org", "code": "8302-2", "display": "Body height"}]},
    "valueQuantity": {"value": 175.0, "unit": "cm", "system": "http://unitsofmeasure.org", "code": "cm"}
  }'
```

## 📝 Notes

- All sample data is fictional and for testing purposes only
- Values follow FHIR R4 standards and common healthcare practices
- Random variation is applied to make data more realistic
- Patient demographics represent diverse age groups and backgrounds
- Medical conditions and medications are common, realistic examples
