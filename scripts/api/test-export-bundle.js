const axios = require('axios');

const BASE_URL = 'https://localhost:5001';

// Configure axios to ignore SSL certificate for development
const axiosConfig = {
    httpsAgent: new (require('https').Agent)({
        rejectUnauthorized: false
    })
};

async function testExportBundle() {
    console.log('🧪 Testing FHIR Export Bundle API with Enhanced Features...\n');

    try {
        // Test 1: Export all patients
        console.log('✅ Test 1: Export all patients');
        const response1 = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Patient&maxResources=10`, axiosConfig);
        console.log(`Status: ${response1.status}`);
        console.log(`Bundle contains ${response1.data.entry?.length || 0} resources\n`);

        // Test 2: Export with complex search (POST)
        console.log('✅ Test 2: Export with complex search (POST)');
        const complexQuery = {
            resourceType: 'Patient',
            maxResources: 20,
            bundleType: 'collection'
        };
        
        const response2 = await axios.post(`${BASE_URL}/fhir/$export-bundle`, complexQuery, axiosConfig);
        console.log(`Status: ${response2.status}`);
        console.log(`Bundle contains ${response2.data.entry?.length || 0} resources\n`);

        // === NEW TESTS FOR TIME-BASED FILTERING ===

        // Test 3: Export observations for last 30 days
        console.log('✅ Test 3: Export observations for last 30 days');
        const response3 = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Observation&timePeriod=days&timePeriodCount=30`, axiosConfig);
        console.log(`Status: ${response3.status}`);
        console.log(`Bundle contains ${response3.data.entry?.length || 0} observations\n`);

        // Test 4: Export weight observations for last 10 measurements
        console.log('✅ Test 4: Export weight observations for last 10 measurements');
        const response4 = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Observation&observationCode=29463-7&maxObservationsPerPatient=10&sortOrder=desc`, axiosConfig);
        console.log(`Status: ${response4.status}`);
        console.log(`Bundle contains ${response4.data.entry?.length || 0} weight observations\n`);

        // Test 5: Export observations for specific time range
        console.log('✅ Test 5: Export observations for specific time range');
        const startDate = '2024-01-01T00:00:00Z';
        const endDate = '2024-12-31T23:59:59Z';
        const response5 = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Observation&startDate=${startDate}&endDate=${endDate}&observationCode=29463-7`, axiosConfig);
        console.log(`Status: ${response5.status}`);
        console.log(`Bundle contains ${response5.data.entry?.length || 0} observations in date range\n`);

        // Test 6: Export observations for last 7 days
        console.log('✅ Test 6: Export observations for last 7 days');
        const response6 = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Observation&timePeriod=days&timePeriodCount=7&latestOnly=true`, axiosConfig);
        console.log(`Status: ${response6.status}`);
        console.log(`Bundle contains ${response6.data.entry?.length || 0} latest observations\n`);

        // Test 7: Export observations for last 4 weeks
        console.log('✅ Test 7: Export observations for last 4 weeks');
        const response7 = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Observation&timePeriod=weeks&timePeriodCount=4&observationCode=8302-2`, axiosConfig);
        console.log(`Status: ${response7.status}`);
        console.log(`Bundle contains ${response7.data.entry?.length || 0} height observations\n`);

        // Test 8: Export observations for last 6 months
        console.log('✅ Test 8: Export observations for last 6 months');
        const response8 = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Observation&timePeriod=months&timePeriodCount=6&observationCode=85354-9`, axiosConfig);
        console.log(`Status: ${response8.status}`);
        console.log(`Bundle contains ${response8.data.entry?.length || 0} blood pressure observations\n`);

        // Test 9: Export observations for last 1 year
        console.log('✅ Test 9: Export observations for last 1 year');
        const response9 = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Observation&timePeriod=years&timePeriodCount=1&observationCode=8867-4`, axiosConfig);
        console.log(`Status: ${response9.status}`);
        console.log(`Bundle contains ${response9.data.entry?.length || 0} heart rate observations\n`);

        // Test 10: Export observations with LOINC system
        console.log('✅ Test 10: Export observations with LOINC system');
        const response10 = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Observation&observationSystem=http://loinc.org&observationCode=2708-6&maxObservationsPerPatient=5`, axiosConfig);
        console.log(`Status: ${response10.status}`);
        console.log(`Bundle contains ${response10.data.entry?.length || 0} oxygen saturation observations\n`);

        console.log('🎉 All export bundle tests completed successfully!');

    } catch (error) {
        console.error('❌ Error testing export bundle:', error.message);
        if (error.response) {
            console.error('Response status:', error.response.status);
            console.error('Response data:', error.response.data);
        }
    }
}

// Test specific observation codes
async function testObservationCodes() {
    console.log('\n🔬 Testing Specific Observation Codes...\n');

    const observationCodes = [
        { code: '29463-7', name: 'Body Weight' },
        { code: '8302-2', name: 'Body Height' },
        { code: '85354-9', name: 'Blood Pressure Systolic' },
        { code: '8462-4', name: 'Blood Pressure Diastolic' },
        { code: '8867-4', name: 'Heart Rate' },
        { code: '2708-6', name: 'Oxygen Saturation' }
    ];

    for (const obs of observationCodes) {
        try {
            console.log(`Testing ${obs.name} (${obs.code})...`);
            const response = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Observation&observationCode=${obs.code}&maxObservationsPerPatient=5&timePeriod=days&timePeriodCount=30`, axiosConfig);
            console.log(`✅ ${obs.name}: ${response.data.entry?.length || 0} observations found`);
        } catch (error) {
            console.log(`❌ ${obs.name}: Error - ${error.message}`);
        }
    }
}

// Test time periods
async function testTimePeriods() {
    console.log('\n⏰ Testing Time Periods...\n');

    const timePeriods = [
        { period: 'days', count: 7, name: 'Last 7 days' },
        { period: 'days', count: 30, name: 'Last 30 days' },
        { period: 'weeks', count: 4, name: 'Last 4 weeks' },
        { period: 'months', count: 6, name: 'Last 6 months' },
        { period: 'years', count: 1, name: 'Last 1 year' }
    ];

    for (const tp of timePeriods) {
        try {
            console.log(`Testing ${tp.name}...`);
            const response = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Observation&timePeriod=${tp.period}&timePeriodCount=${tp.count}&observationCode=29463-7`, axiosConfig);
            console.log(`✅ ${tp.name}: ${response.data.entry?.length || 0} observations found`);
        } catch (error) {
            console.log(`❌ ${tp.name}: Error - ${error.message}`);
        }
    }
}

// Main test execution
async function runAllTests() {
    console.log('🚀 Starting Enhanced FHIR Export Bundle API Tests\n');
    
    await testExportBundle();
    await testObservationCodes();
    await testTimePeriods();
    
    console.log('\n🎯 All tests completed!');
}

// Export functions for use in other scripts
module.exports = {
    testExportBundle,
    testObservationCodes,
    testTimePeriods,
    runAllTests
};

// Run tests if this file is executed directly
if (require.main === module) {
    runAllTests().catch(console.error);
}
