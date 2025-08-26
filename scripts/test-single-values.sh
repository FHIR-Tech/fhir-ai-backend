#!/bin/bash

# FHIR-AI Backend Single Value Tests
# This script tests individual FHIR resource creation with sample values

API_BASE_URL="https://localhost:52871"
TENANT_ID="demo-tenant"
FHIR_SCOPES="user/* patient/*"

echo "🏥 FHIR-AI Backend Single Value Tests"
echo "====================================="
echo ""

# Test 1: Create a single Patient
echo "📋 Test 1: Creating a single Patient..."
echo "----------------------------------------"

PATIENT_JSON='{
  "resourceType": "Patient",
  "name": [
    {
      "use": "official",
      "family": "TestPatient",
      "given": ["John", "Doe"]
    }
  ],
  "gender": "male",
  "birthDate": "1990-01-01",
  "address": [
    {
      "use": "home",
      "type": "physical",
      "line": ["123 Test St"],
      "city": "TestCity",
      "state": "TS",
      "postalCode": "12345",
      "country": "US"
    }
  ],
  "telecom": [
    {
      "system": "phone",
      "value": "+1-555-0001",
      "use": "home"
    },
    {
      "system": "email",
      "value": "john.doe@test.com",
      "use": "home"
    }
  ]
}'

echo "Patient Data:"
echo "$PATIENT_JSON" | jq '.'
echo ""

curl -X POST "$API_BASE_URL/fhir/Patient" \
  -H "accept: */*" \
  -H "X-Tenant-ID: $TENANT_ID" \
  -H "X-FHIR-Scopes: $FHIR_SCOPES" \
  -H "Content-Type: application/json" \
  -d "$PATIENT_JSON"

echo ""
echo ""

# Test 2: Create a single Observation (Blood Pressure)
echo "🔬 Test 2: Creating a single Observation (Blood Pressure)..."
echo "------------------------------------------------------------"

OBSERVATION_JSON='{
  "resourceType": "Observation",
  "status": "final",
  "code": {
    "coding": [
      {
        "system": "http://loinc.org",
        "code": "85354-9",
        "display": "Blood pressure panel with all children optional"
      }
    ]
  },
  "subject": {
    "reference": "Patient/test-patient-001"
  },
  "effectiveDateTime": "2024-01-15T10:30:00Z",
  "component": [
    {
      "code": {
        "coding": [
          {
            "system": "http://loinc.org",
            "code": "8480-6",
            "display": "Systolic blood pressure"
          }
        ]
      },
      "valueQuantity": {
        "value": 120,
        "unit": "mmHg",
        "system": "http://unitsofmeasure.org",
        "code": "mm[Hg]"
      }
    },
    {
      "code": {
        "coding": [
          {
            "system": "http://loinc.org",
            "code": "8462-4",
            "display": "Diastolic blood pressure"
          }
        ]
      },
      "valueQuantity": {
        "value": 80,
        "unit": "mmHg",
        "system": "http://unitsofmeasure.org",
        "code": "mm[Hg]"
      }
    }
  ]
}'

echo "Observation Data:"
echo "$OBSERVATION_JSON" | jq '.'
echo ""

curl -X POST "$API_BASE_URL/fhir/Observation" \
  -H "accept: */*" \
  -H "X-Tenant-ID: $TENANT_ID" \
  -H "X-FHIR-Scopes: $FHIR_SCOPES" \
  -H "Content-Type: application/json" \
  -d "$OBSERVATION_JSON"

echo ""
echo ""

# Test 3: Create a single Observation (Body Weight)
echo "⚖️ Test 3: Creating a single Observation (Body Weight)..."
echo "--------------------------------------------------------"

WEIGHT_JSON='{
  "resourceType": "Observation",
  "status": "final",
  "code": {
    "coding": [
      {
        "system": "http://loinc.org",
        "code": "29463-7",
        "display": "Body weight"
      }
    ]
  },
  "subject": {
    "reference": "Patient/test-patient-001"
  },
  "effectiveDateTime": "2024-01-15T10:30:00Z",
  "valueQuantity": {
    "value": 70.5,
    "unit": "kg",
    "system": "http://unitsofmeasure.org",
    "code": "kg"
  }
}'

echo "Weight Observation Data:"
echo "$WEIGHT_JSON" | jq '.'
echo ""

curl -X POST "$API_BASE_URL/fhir/Observation" \
  -H "accept: */*" \
  -H "X-Tenant-ID: $TENANT_ID" \
  -H "X-FHIR-Scopes: $FHIR_SCOPES" \
  -H "Content-Type: application/json" \
  -d "$WEIGHT_JSON"

echo ""
echo ""

# Test 4: Create a single Observation (Body Height)
echo "📏 Test 4: Creating a single Observation (Body Height)..."
echo "--------------------------------------------------------"

HEIGHT_JSON='{
  "resourceType": "Observation",
  "status": "final",
  "code": {
    "coding": [
      {
        "system": "http://loinc.org",
        "code": "8302-2",
        "display": "Body height"
      }
    ]
  },
  "subject": {
    "reference": "Patient/test-patient-001"
  },
  "effectiveDateTime": "2024-01-15T10:30:00Z",
  "valueQuantity": {
    "value": 175.0,
    "unit": "cm",
    "system": "http://unitsofmeasure.org",
    "code": "cm"
  }
}'

echo "Height Observation Data:"
echo "$HEIGHT_JSON" | jq '.'
echo ""

curl -X POST "$API_BASE_URL/fhir/Observation" \
  -H "accept: */*" \
  -H "X-Tenant-ID: $TENANT_ID" \
  -H "X-FHIR-Scopes: $FHIR_SCOPES" \
  -H "Content-Type: application/json" \
  -d "$HEIGHT_JSON"

echo ""
echo ""

# Test 5: Create a single Condition
echo "🏥 Test 5: Creating a single Condition..."
echo "----------------------------------------"

CONDITION_JSON='{
  "resourceType": "Condition",
  "clinicalStatus": {
    "coding": [
      {
        "system": "http://terminology.hl7.org/CodeSystem/condition-clinical",
        "code": "active",
        "display": "Active"
      }
    ]
  },
  "verificationStatus": {
    "coding": [
      {
        "system": "http://terminology.hl7.org/CodeSystem/condition-ver-status",
        "code": "confirmed",
        "display": "Confirmed"
      }
    ]
  },
  "category": [
    {
      "coding": [
        {
          "system": "http://terminology.hl7.org/CodeSystem/condition-category",
          "code": "problem-list-item",
          "display": "Problem List Item"
        }
      ]
    }
  ],
  "code": {
    "coding": [
      {
        "system": "http://snomed.info/sct",
        "code": "38341003",
        "display": "Hypertensive disorder"
      }
    ]
  },
  "subject": {
    "reference": "Patient/test-patient-001"
  },
  "onsetDateTime": "2023-01-01",
  "recordedDate": "2024-01-15"
}'

echo "Condition Data:"
echo "$CONDITION_JSON" | jq '.'
echo ""

curl -X POST "$API_BASE_URL/fhir/Condition" \
  -H "accept: */*" \
  -H "X-Tenant-ID: $TENANT_ID" \
  -H "X-FHIR-Scopes: $FHIR_SCOPES" \
  -H "Content-Type: application/json" \
  -d "$CONDITION_JSON"

echo ""
echo ""

# Test 6: Retrieve created resources
echo "🔍 Test 6: Retrieving created resources..."
echo "------------------------------------------"

echo "Retrieving patients:"
curl -X GET "$API_BASE_URL/fhir/Patient?take=5" \
  -H "accept: */*" \
  -H "X-Tenant-ID: $TENANT_ID" \
  -H "X-FHIR-Scopes: $FHIR_SCOPES"

echo ""
echo ""

echo "Retrieving observations:"
curl -X GET "$API_BASE_URL/fhir/Observation?take=5" \
  -H "accept: */*" \
  -H "X-Tenant-ID: $TENANT_ID" \
  -H "X-FHIR-Scopes: $FHIR_SCOPES"

echo ""
echo ""

echo "Retrieving conditions:"
curl -X GET "$API_BASE_URL/fhir/Condition?take=5" \
  -H "accept: */*" \
  -H "X-Tenant-ID: $TENANT_ID" \
  -H "X-FHIR-Scopes: $FHIR_SCOPES"

echo ""
echo ""

echo "✅ Single value tests completed!"
echo "📊 Check the responses above to see the created FHIR resources."
echo "🔗 You can also view the data in Swagger UI: $API_BASE_URL"
