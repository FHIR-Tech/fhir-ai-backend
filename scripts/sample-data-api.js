/**
 * FHIR-AI Backend Sample Data Generator
 * 
 * This script populates the FHIR-AI Backend with realistic healthcare sample data
 * using the REST API endpoints. It creates a comprehensive dataset including:
 * - Patients with demographic information
 * - Observations (vital signs, lab results)
 * - Encounters (visits, procedures)
 * - Conditions (diagnoses, problems)
 * - MedicationRequests (prescriptions)
 * 
 * Usage:
 * 1. Start the FHIR-AI Backend application
 * 2. Run: node scripts/sample-data-api.js
 * 3. Check the console output for results
 */

const API_BASE_URL = 'http://localhost:52872'; // Update with your actual API URL
const TENANT_ID = 'f66ddff8-fb9a-4d5b-9a44-194c62389842';
const FHIR_SCOPES = 'user/cc8c304a-9e5e-4115-bc54-791636170890 patient/cc8c304a-9e5e-4115-bc54-791636170890';

// Sample data arrays
const samplePatients = [
    {
        resourceType: "Patient",
        name: [{ use: "official", family: "Smith", given: ["John", "Michael"] }],
        gender: "male",
        birthDate: "1990-01-15",
        address: [{ 
            use: "home", 
            type: "physical", 
            line: ["123 Main St"], 
            city: "Springfield", 
            state: "IL", 
            postalCode: "62701", 
            country: "US" 
        }],
        telecom: [
            { system: "phone", value: "+1-555-0123", use: "home" },
            { system: "email", value: "john.smith@email.com", use: "home" }
        ],
        maritalStatus: { coding: [{ system: "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus", code: "M", display: "Married" }] },
        contact: [{ 
            relationship: [{ coding: [{ system: "http://terminology.hl7.org/CodeSystem/v2-0131", code: "C", display: "Emergency Contact" }] }],
            name: { use: "official", family: "Smith", given: ["Sarah", "Jane"] },
            telecom: [{ system: "phone", value: "+1-555-0124", use: "mobile" }]
        }]
    },
    {
        resourceType: "Patient",
        name: [{ use: "official", family: "Johnson", given: ["Emily", "Rose"] }],
        gender: "female",
        birthDate: "1985-06-22",
        address: [{ 
            use: "home", 
            type: "physical", 
            line: ["456 Oak Ave"], 
            city: "Springfield", 
            state: "IL", 
            postalCode: "62702", 
            country: "US" 
        }],
        telecom: [
            { system: "phone", value: "+1-555-0125", use: "mobile" },
            { system: "email", value: "emily.johnson@email.com", use: "home" }
        ],
        maritalStatus: { coding: [{ system: "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus", code: "S", display: "Single" }] }
    },
    {
        resourceType: "Patient",
        name: [{ use: "official", family: "Williams", given: ["Robert", "David"] }],
        gender: "male",
        birthDate: "1978-12-03",
        address: [{ 
            use: "home", 
            type: "physical", 
            line: ["789 Pine St"], 
            city: "Springfield", 
            state: "IL", 
            postalCode: "62703", 
            country: "US" 
        }],
        telecom: [
            { system: "phone", value: "+1-555-0126", use: "work" },
            { system: "email", value: "robert.williams@email.com", use: "work" }
        ],
        maritalStatus: { coding: [{ system: "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus", code: "M", display: "Married" }] }
    },
    {
        resourceType: "Patient",
        name: [{ use: "official", family: "Brown", given: ["Lisa", "Marie"] }],
        gender: "female",
        birthDate: "1992-03-18",
        address: [{ 
            use: "home", 
            type: "physical", 
            line: ["321 Elm St"], 
            city: "Springfield", 
            state: "IL", 
            postalCode: "62704", 
            country: "US" 
        }],
        telecom: [
            { system: "phone", value: "+1-555-0127", use: "mobile" },
            { system: "email", value: "lisa.brown@email.com", use: "home" }
        ],
        maritalStatus: { coding: [{ system: "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus", code: "D", display: "Divorced" }] }
    },
    {
        resourceType: "Patient",
        name: [{ use: "official", family: "Davis", given: ["Michael", "James"] }],
        gender: "male",
        birthDate: "1965-09-30",
        address: [{ 
            use: "home", 
            type: "physical", 
            line: ["654 Maple Dr"], 
            city: "Springfield", 
            state: "IL", 
            postalCode: "62705", 
            country: "US" 
        }],
        telecom: [
            { system: "phone", value: "+1-555-0128", use: "home" },
            { system: "email", value: "michael.davis@email.com", use: "home" }
        ],
        maritalStatus: { coding: [{ system: "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus", code: "M", display: "Married" }] }
    }
];

const sampleObservations = [
    // Blood Pressure
    {
        resourceType: "Observation",
        status: "final",
        code: {
            coding: [{
                system: "http://loinc.org",
                code: "85354-9",
                display: "Blood pressure panel with all children optional"
            }]
        },
        subject: { reference: "Patient/" },
        effectiveDateTime: "",
        component: [
            {
                code: {
                    coding: [{
                        system: "http://loinc.org",
                        code: "8480-6",
                        display: "Systolic blood pressure"
                    }]
                },
                valueQuantity: { value: 120, unit: "mmHg", system: "http://unitsofmeasure.org", code: "mm[Hg]" }
            },
            {
                code: {
                    coding: [{
                        system: "http://loinc.org",
                        code: "8462-4",
                        display: "Diastolic blood pressure"
                    }]
                },
                valueQuantity: { value: 80, unit: "mmHg", system: "http://unitsofmeasure.org", code: "mm[Hg]" }
            }
        ]
    },
    // Body Weight
    {
        resourceType: "Observation",
        status: "final",
        code: {
            coding: [{
                system: "http://loinc.org",
                code: "29463-7",
                display: "Body weight"
            }]
        },
        subject: { reference: "Patient/" },
        effectiveDateTime: "",
        valueQuantity: { value: 70, unit: "kg", system: "http://unitsofmeasure.org", code: "kg" }
    },
    // Body Height
    {
        resourceType: "Observation",
        status: "final",
        code: {
            coding: [{
                system: "http://loinc.org",
                code: "8302-2",
                display: "Body height"
            }]
        },
        subject: { reference: "Patient/" },
        effectiveDateTime: "",
        valueQuantity: { value: 175, unit: "cm", system: "http://unitsofmeasure.org", code: "cm" }
    },
    // Heart Rate
    {
        resourceType: "Observation",
        status: "final",
        code: {
            coding: [{
                system: "http://loinc.org",
                code: "8867-4",
                display: "Heart rate"
            }]
        },
        subject: { reference: "Patient/" },
        effectiveDateTime: "",
        valueQuantity: { value: 72, unit: "beats/min", system: "http://unitsofmeasure.org", code: "/min" }
    },
    // Temperature
    {
        resourceType: "Observation",
        status: "final",
        code: {
            coding: [{
                system: "http://loinc.org",
                code: "8310-5",
                display: "Body temperature"
            }]
        },
        subject: { reference: "Patient/" },
        effectiveDateTime: "",
        valueQuantity: { value: 37.0, unit: "C", system: "http://unitsofmeasure.org", code: "Cel" }
    }
];

const sampleEncounters = [
    {
        resourceType: "Encounter",
        status: "finished",
        class: { system: "http://terminology.hl7.org/CodeSystem/v3-ActCode", code: "AMB", display: "ambulatory" },
        type: [{
            coding: [{
                system: "http://snomed.info/sct",
                code: "185389000",
                display: "General medical examination"
            }]
        }],
        subject: { reference: "Patient/" },
        period: { start: "", end: "" },
        reasonCode: [{
            coding: [{
                system: "http://snomed.info/sct",
                code: "185389000",
                display: "General medical examination"
            }]
        }]
    },
    {
        resourceType: "Encounter",
        status: "finished",
        class: { system: "http://terminology.hl7.org/CodeSystem/v3-ActCode", code: "AMB", display: "ambulatory" },
        type: [{
            coding: [{
                system: "http://snomed.info/sct",
                code: "185389000",
                display: "General medical examination"
            }]
        }],
        subject: { reference: "Patient/" },
        period: { start: "", end: "" },
        reasonCode: [{
            coding: [{
                system: "http://snomed.info/sct",
                code: "185389000",
                display: "General medical examination"
            }]
        }]
    }
];

const sampleConditions = [
    {
        resourceType: "Condition",
        clinicalStatus: {
            coding: [{
                system: "http://terminology.hl7.org/CodeSystem/condition-clinical",
                code: "active",
                display: "Active"
            }]
        },
        verificationStatus: {
            coding: [{
                system: "http://terminology.hl7.org/CodeSystem/condition-ver-status",
                code: "confirmed",
                display: "Confirmed"
            }]
        },
        category: [{
            coding: [{
                system: "http://terminology.hl7.org/CodeSystem/condition-category",
                code: "problem-list-item",
                display: "Problem List Item"
            }]
        }],
        code: {
            coding: [{
                system: "http://snomed.info/sct",
                code: "38341003",
                display: "Hypertensive disorder"
            }]
        },
        subject: { reference: "Patient/" },
        onsetDateTime: "",
        recordedDate: ""
    },
    {
        resourceType: "Condition",
        clinicalStatus: {
            coding: [{
                system: "http://terminology.hl7.org/CodeSystem/condition-clinical",
                code: "active",
                display: "Active"
            }]
        },
        verificationStatus: {
            coding: [{
                system: "http://terminology.hl7.org/CodeSystem/condition-ver-status",
                code: "confirmed",
                display: "Confirmed"
            }]
        },
        category: [{
            coding: [{
                system: "http://terminology.hl7.org/CodeSystem/condition-category",
                code: "problem-list-item",
                display: "Problem List Item"
            }]
        }],
        code: {
            coding: [{
                system: "http://snomed.info/sct",
                code: "73211009",
                display: "Diabetes mellitus"
            }]
        },
        subject: { reference: "Patient/" },
        onsetDateTime: "",
        recordedDate: ""
    }
];

const sampleMedicationRequests = [
    {
        resourceType: "MedicationRequest",
        status: "active",
        intent: "order",
        medicationCodeableConcept: {
            coding: [{
                system: "http://www.nlm.nih.gov/research/umls/rxnorm",
                code: "197361",
                display: "Lisinopril 10 MG Oral Tablet"
            }]
        },
        subject: { reference: "Patient/" },
        authoredOn: "",
        requester: { reference: "Practitioner/practitioner-001" },
        dosageInstruction: [{
            text: "Take 1 tablet daily",
            timing: {
                repeat: {
                    frequency: 1,
                    period: 1,
                    periodUnit: "d"
                }
            },
            route: {
                coding: [{
                    system: "http://snomed.info/sct",
                    code: "26643006",
                    display: "Oral route"
                }]
            },
            doseAndRate: [{
                type: {
                    coding: [{
                        system: "http://terminology.hl7.org/CodeSystem/dose-rate-type",
                        code: "ordered",
                        display: "Ordered"
                    }]
                },
                doseQuantity: {
                    value: 1,
                    unit: "tablet",
                    system: "http://unitsofmeasure.org",
                    code: "{tbl}"
                }
            }]
        }]
    },
    {
        resourceType: "MedicationRequest",
        status: "active",
        intent: "order",
        medicationCodeableConcept: {
            coding: [{
                system: "http://www.nlm.nih.gov/research/umls/rxnorm",
                code: "860975",
                display: "Metformin 500 MG Oral Tablet"
            }]
        },
        subject: { reference: "Patient/" },
        authoredOn: "",
        requester: { reference: "Practitioner/practitioner-001" },
        dosageInstruction: [{
            text: "Take 1 tablet twice daily with meals",
            timing: {
                repeat: {
                    frequency: 2,
                    period: 1,
                    periodUnit: "d"
                }
            },
            route: {
                coding: [{
                    system: "http://snomed.info/sct",
                    code: "26643006",
                    display: "Oral route"
                }]
            },
            doseAndRate: [{
                type: {
                    coding: [{
                        system: "http://terminology.hl7.org/CodeSystem/dose-rate-type",
                        code: "ordered",
                        display: "Ordered"
                    }]
                },
                doseQuantity: {
                    value: 1,
                    unit: "tablet",
                    system: "http://unitsofmeasure.org",
                    code: "{tbl}"
                }
            }]
        }]
    }
];

// Utility functions
function getRandomDate(start, end) {
    return new Date(start.getTime() + Math.random() * (end.getTime() - start.getTime())).toISOString().split('T')[0];
}

function getRandomValue(min, max, decimals = 0) {
    const value = Math.random() * (max - min) + min;
    return decimals === 0 ? Math.round(value) : parseFloat(value.toFixed(decimals));
}

async function makeApiRequest(endpoint, method = 'GET', body = null) {
    const url = `${API_BASE_URL}${endpoint}`;
    const options = {
        method,
        headers: {
            'Content-Type': 'application/json',
            'X-Tenant-ID': TENANT_ID,
            'X-FHIR-Scopes': FHIR_SCOPES
        }
    };

    if (body) {
        options.body = JSON.stringify(body);
    }

    try {
        const response = await fetch(url, options);
        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }
        return await response.json();
    } catch (error) {
        console.error(`API request failed: ${error.message}`);
        throw error;
    }
}

async function createFhirResource(resourceType, resourceData) {
    try {
        const response = await makeApiRequest(`/fhir/${resourceType}`, 'POST', {
            resourceType: resourceType,
            resourceJson: JSON.stringify(resourceData)
        });
        console.log(`✅ Created ${resourceType} with ID: ${response.fhirId}`);
        return response.fhirId;
    } catch (error) {
        console.error(`❌ Failed to create ${resourceType}: ${error.message}`);
        return null;
    }
}

async function generateSampleData() {
    console.log('🚀 Starting FHIR-AI Backend Sample Data Generation...\n');

    const createdPatients = [];
    const createdObservations = [];
    const createdEncounters = [];
    const createdConditions = [];
    const createdMedicationRequests = [];

    // Create Patients
    console.log('📋 Creating Patients...');
    for (const patientData of samplePatients) {
        const patientId = await createFhirResource('Patient', patientData);
        if (patientId) {
            createdPatients.push(patientId);
        }
    }

    // Create Observations for each patient
    console.log('\n🔬 Creating Observations...');
    for (const patientId of createdPatients) {
        for (const observationTemplate of sampleObservations) {
            const observationData = {
                ...observationTemplate,
                subject: { reference: `Patient/${patientId}` },
                effectiveDateTime: getRandomDate(new Date('2023-01-01'), new Date())
            };

            // Add some variation to the values
            if (observationData.valueQuantity) {
                const baseValue = observationData.valueQuantity.value;
                const variation = baseValue * 0.1; // 10% variation
                observationData.valueQuantity.value = getRandomValue(baseValue - variation, baseValue + variation, 1);
            }

            if (observationData.component) {
                observationData.component.forEach(comp => {
                    if (comp.valueQuantity) {
                        const baseValue = comp.valueQuantity.value;
                        const variation = baseValue * 0.1;
                        comp.valueQuantity.value = getRandomValue(baseValue - variation, baseValue + variation, 1);
                    }
                });
            }

            const observationId = await createFhirResource('Observation', observationData);
            if (observationId) {
                createdObservations.push(observationId);
            }
        }
    }

    // Create Encounters for each patient
    console.log('\n🏥 Creating Encounters...');
    for (const patientId of createdPatients) {
        for (const encounterTemplate of sampleEncounters) {
            const startDate = getRandomDate(new Date('2023-01-01'), new Date());
            const endDate = new Date(startDate);
            endDate.setHours(endDate.getHours() + getRandomValue(1, 4)); // 1-4 hour encounters

            const encounterData = {
                ...encounterTemplate,
                subject: { reference: `Patient/${patientId}` },
                period: { 
                    start: startDate + 'T09:00:00Z',
                    end: endDate.toISOString()
                }
            };

            const encounterId = await createFhirResource('Encounter', encounterData);
            if (encounterId) {
                createdEncounters.push(encounterId);
            }
        }
    }

    // Create Conditions for some patients
    console.log('\n🏥 Creating Conditions...');
    for (let i = 0; i < createdPatients.length; i += 2) { // Every other patient gets a condition
        const patientId = createdPatients[i];
        const conditionTemplate = sampleConditions[i % sampleConditions.length];
        
        const conditionData = {
            ...conditionTemplate,
            subject: { reference: `Patient/${patientId}` },
            onsetDateTime: getRandomDate(new Date('2020-01-01'), new Date('2023-01-01')),
            recordedDate: getRandomDate(new Date('2023-01-01'), new Date())
        };

        const conditionId = await createFhirResource('Condition', conditionData);
        if (conditionId) {
            createdConditions.push(conditionId);
        }
    }

    // Create Medication Requests for patients with conditions
    console.log('\n💊 Creating Medication Requests...');
    for (let i = 0; i < createdPatients.length; i += 2) {
        const patientId = createdPatients[i];
        const medicationTemplate = sampleMedicationRequests[i % sampleMedicationRequests.length];
        
        const medicationData = {
            ...medicationTemplate,
            subject: { reference: `Patient/${patientId}` },
            authoredOn: getRandomDate(new Date('2023-01-01'), new Date())
        };

        const medicationId = await createFhirResource('MedicationRequest', medicationData);
        if (medicationId) {
            createdMedicationRequests.push(medicationId);
        }
    }

    // Summary
    console.log('\n📊 Sample Data Generation Complete!');
    console.log('=====================================');
    console.log(`✅ Patients created: ${createdPatients.length}`);
    console.log(`✅ Observations created: ${createdObservations.length}`);
    console.log(`✅ Encounters created: ${createdEncounters.length}`);
    console.log(`✅ Conditions created: ${createdConditions.length}`);
    console.log(`✅ Medication Requests created: ${createdMedicationRequests.length}`);
    console.log('\n🎉 Total FHIR resources created:', 
        createdPatients.length + createdObservations.length + 
        createdEncounters.length + createdConditions.length + 
        createdMedicationRequests.length);

    // Test data retrieval
    console.log('\n🔍 Testing data retrieval...');
    try {
        const patients = await makeApiRequest('/fhir/Patient?take=5');
        console.log(`📋 Retrieved ${patients.resourceType === 'Bundle' ? patients.entry?.length || 0 : 0} patients`);
        
        const observations = await makeApiRequest('/fhir/Observation?take=5');
        console.log(`🔬 Retrieved ${observations.resourceType === 'Bundle' ? observations.entry?.length || 0 : 0} observations`);
    } catch (error) {
        console.error('❌ Data retrieval test failed:', error.message);
    }

    console.log('\n✨ Sample data generation completed successfully!');
    console.log('You can now explore the data through the Swagger UI or API endpoints.');
}

// Run the sample data generation
if (require.main === module) {
    generateSampleData().catch(console.error);
}

module.exports = { generateSampleData, makeApiRequest };
