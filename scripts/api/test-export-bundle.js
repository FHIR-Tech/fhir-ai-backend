const axios = require('axios');

const BASE_URL = 'http://localhost:5000';

async function testExportBundle() {
    console.log('Testing FHIR Export Bundle API...\n');

    try {
        // Test 1: Export all patients
        console.log('Test 1: Export all patients');
        const response1 = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Patient&maxResources=10`);
        console.log(`Status: ${response1.status}`);
        console.log(`Bundle contains ${response1.data.entry?.length || 0} resources\n`);

        // Test 2: Export with complex search (POST)
        console.log('Test 2: Export with complex search (POST)');
        const complexQuery = {
            resourceType: 'Patient',
            maxResources: 20,
            bundleType: 'collection'
        };
        
        const response2 = await axios.post(`${BASE_URL}/fhir/$export-bundle`, complexQuery);
        console.log(`Status: ${response2.status}`);
        console.log(`Bundle contains ${response2.data.entry?.length || 0} resources\n`);

        console.log('All tests completed successfully!');

    } catch (error) {
        console.error('Error:', error.message);
    }
}

testExportBundle();
