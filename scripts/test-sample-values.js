/**
 * FHIR-AI Backend Sample Values Test
 * 
 * This script demonstrates the sample values and tests API connectivity
 */

const API_BASE_URL = 'https://localhost:52871';
const TENANT_ID = 'f66ddff8-fb9a-4d5b-9a44-194c62389842';
const FHIR_SCOPES = 'user/* patient/*';

// Sample values demonstration
console.log('🏥 FHIR-AI Backend Sample Values');
console.log('================================\n');

console.log('📋 PATIENT SAMPLE VALUES:');
console.log('-------------------------');
const samplePatients = [
    {
        name: "John Michael Smith",
        gender: "male",
        birthDate: "1990-01-15",
        address: "123 Main St, Springfield, IL 62701",
        phone: "+1-555-0123",
        email: "john.smith@email.com",
        maritalStatus: "Married"
    },
    {
        name: "Emily Rose Johnson", 
        gender: "female",
        birthDate: "1985-06-22",
        address: "456 Oak Ave, Springfield, IL 62702",
        phone: "+1-555-0125",
        email: "emily.johnson@email.com",
        maritalStatus: "Single"
    },
    {
        name: "Robert David Williams",
        gender: "male", 
        birthDate: "1978-12-03",
        address: "789 Pine St, Springfield, IL 62703",
        phone: "+1-555-0126",
        email: "robert.williams@email.com",
        maritalStatus: "Married"
    },
    {
        name: "Lisa Marie Brown",
        gender: "female",
        birthDate: "1992-03-18", 
        address: "321 Elm St, Springfield, IL 62704",
        phone: "+1-555-0127",
        email: "lisa.brown@email.com",
        maritalStatus: "Divorced"
    },
    {
        name: "Michael James Davis",
        gender: "male",
        birthDate: "1965-09-30",
        address: "654 Maple Dr, Springfield, IL 62705", 
        phone: "+1-555-0128",
        email: "michael.davis@email.com",
        maritalStatus: "Married"
    }
];

samplePatients.forEach((patient, index) => {
    console.log(`${index + 1}. ${patient.name}`);
    console.log(`   Gender: ${patient.gender}`);
    console.log(`   Birth Date: ${patient.birthDate}`);
    console.log(`   Address: ${patient.address}`);
    console.log(`   Phone: ${patient.phone}`);
    console.log(`   Email: ${patient.email}`);
    console.log(`   Marital Status: ${patient.maritalStatus}`);
    console.log('');
});

console.log('🔬 OBSERVATION SAMPLE VALUES:');
console.log('-----------------------------');
const sampleObservations = [
    {
        type: "Blood Pressure",
        systolic: "120 mmHg",
        diastolic: "80 mmHg",
        code: "85354-9",
        description: "Blood pressure panel"
    },
    {
        type: "Body Weight",
        value: "70 kg",
        code: "29463-7", 
        description: "Body weight"
    },
    {
        type: "Body Height",
        value: "175 cm",
        code: "8302-2",
        description: "Body height"
    },
    {
        type: "Heart Rate",
        value: "72 beats/min",
        code: "8867-4",
        description: "Heart rate"
    },
    {
        type: "Temperature",
        value: "37.0°C",
        code: "8310-5",
        description: "Body temperature"
    }
];

sampleObservations.forEach((obs, index) => {
    console.log(`${index + 1}. ${obs.type}`);
    if (obs.type === "Blood Pressure") {
        console.log(`   Systolic: ${obs.systolic}`);
        console.log(`   Diastolic: ${obs.diastolic}`);
    } else {
        console.log(`   Value: ${obs.value}`);
    }
    console.log(`   LOINC Code: ${obs.code}`);
    console.log(`   Description: ${obs.description}`);
    console.log('');
});

console.log('🏥 ENCOUNTER SAMPLE VALUES:');
console.log('---------------------------');
const sampleEncounters = [
    {
        type: "General Medical Examination",
        status: "finished",
        class: "ambulatory",
        duration: "1-4 hours",
        code: "185389000"
    }
];

sampleEncounters.forEach((encounter, index) => {
    console.log(`${index + 1}. ${encounter.type}`);
    console.log(`   Status: ${encounter.status}`);
    console.log(`   Class: ${encounter.class}`);
    console.log(`   Duration: ${encounter.duration}`);
    console.log(`   SNOMED Code: ${encounter.code}`);
    console.log('');
});

console.log('💊 CONDITION SAMPLE VALUES:');
console.log('----------------------------');
const sampleConditions = [
    {
        name: "Hypertensive disorder",
        status: "active",
        verification: "confirmed",
        code: "38341003",
        category: "Problem List Item"
    },
    {
        name: "Diabetes mellitus", 
        status: "active",
        verification: "confirmed",
        code: "73211009",
        category: "Problem List Item"
    }
];

sampleConditions.forEach((condition, index) => {
    console.log(`${index + 1}. ${condition.name}`);
    console.log(`   Status: ${condition.status}`);
    console.log(`   Verification: ${condition.verification}`);
    console.log(`   SNOMED Code: ${condition.code}`);
    console.log(`   Category: ${condition.category}`);
    console.log('');
});

console.log('💊 MEDICATION REQUEST SAMPLE VALUES:');
console.log('------------------------------------');
const sampleMedications = [
    {
        name: "Lisinopril 10 MG Oral Tablet",
        status: "active",
        intent: "order",
        dosage: "Take 1 tablet daily",
        route: "Oral",
        code: "197361"
    },
    {
        name: "Metformin 500 MG Oral Tablet",
        status: "active", 
        intent: "order",
        dosage: "Take 1 tablet twice daily with meals",
        route: "Oral",
        code: "860975"
    }
];

sampleMedications.forEach((med, index) => {
    console.log(`${index + 1}. ${med.name}`);
    console.log(`   Status: ${med.status}`);
    console.log(`   Intent: ${med.intent}`);
    console.log(`   Dosage: ${med.dosage}`);
    console.log(`   Route: ${med.route}`);
    console.log(`   RxNorm Code: ${med.code}`);
    console.log('');
});

// Test API connectivity
async function testApiConnection() {
    console.log('🔍 TESTING API CONNECTION:');
    console.log('---------------------------');
    
    try {
        const response = await fetch(`${API_BASE_URL}/health`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'X-Tenant-ID': TENANT_ID,
                'X-FHIR-Scopes': FHIR_SCOPES
            }
        });
        
        if (response.ok) {
            const data = await response.json();
            console.log('✅ API Connection Successful!');
            console.log(`   Status: ${data.status}`);
            console.log(`   Timestamp: ${data.timestamp}`);
            console.log(`   API URL: ${API_BASE_URL}`);
        } else {
            console.log('❌ API Connection Failed');
            console.log(`   Status: ${response.status}`);
            console.log(`   Status Text: ${response.statusText}`);
        }
    } catch (error) {
        console.log('❌ API Connection Error:');
        console.log(`   Error: ${error.message}`);
        console.log(`   Make sure the FHIR-AI Backend is running on ${API_BASE_URL}`);
    }
    
    console.log('\n📊 SUMMARY:');
    console.log('------------');
    console.log(`• Patients: ${samplePatients.length} sample records`);
    console.log(`• Observations: ${sampleObservations.length} types (5 per patient = ${samplePatients.length * sampleObservations.length} total)`);
    console.log(`• Encounters: ${sampleEncounters.length} types (2 per patient = ${samplePatients.length * sampleEncounters.length} total)`);
    console.log(`• Conditions: ${sampleConditions.length} types (assigned to every other patient)`);
    console.log(`• Medications: ${sampleMedications.length} types (assigned to patients with conditions)`);
    
    const totalResources = samplePatients.length + 
                          (samplePatients.length * sampleObservations.length) + 
                          (samplePatients.length * sampleEncounters.length) + 
                          Math.ceil(samplePatients.length / 2) + 
                          Math.ceil(samplePatients.length / 2);
    
    console.log(`\n🎉 Total FHIR resources to be created: ${totalResources}`);
    console.log('\n✨ Run the full sample data generation with:');
    console.log('   node scripts/sample-data-api.js');
    console.log('   or');
    console.log('   .\\scripts\\run-sample-data.bat');
}

// Run the test
testApiConnection();
