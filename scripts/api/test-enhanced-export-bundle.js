const axios = require('axios');
const fs = require('fs');
const path = require('path');

const BASE_URL = 'https://localhost:5001';

// Configure axios to ignore SSL certificate for development
const axiosConfig = {
    httpsAgent: new (require('https').Agent)({
        rejectUnauthorized: false
    })
};

/**
 * Test enhanced export bundle functionality with time-based filtering and observation codes
 */
async function testEnhancedExportBundle() {
    console.log('🚀 Testing Enhanced FHIR Export Bundle API...\n');

    const headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
    };

    try {
        // Test 1: Export weight observations for last 10 measurements
        console.log('📊 Test 1: Export weight observations for last 10 measurements');
        const weightResponse = await axios.get(
            `${BASE_URL}/fhir/$export-bundle?resourceType=Observation&observationCode=29463-7&maxObservationsPerPatient=10&sortOrder=desc`,
            { ...axiosConfig, headers }
        );
        console.log(`✅ Weight observations: ${weightResponse.data.entry?.length || 0} found`);
        
        // Save weight observations to file
        if (weightResponse.data.entry?.length > 0) {
            fs.writeFileSync('weight_observations.json', JSON.stringify(weightResponse.data, null, 2));
            console.log('💾 Weight observations saved to weight_observations.json');
        }

        // Test 2: Export observations for last 7 days
        console.log('\n📅 Test 2: Export observations for last 7 days');
        const weekResponse = await axios.get(
            `${BASE_URL}/fhir/$export-bundle?resourceType=Observation&timePeriod=days&timePeriodCount=7&latestOnly=true`,
            { ...axiosConfig, headers }
        );
        console.log(`✅ Last 7 days observations: ${weekResponse.data.entry?.length || 0} found`);

        // Test 3: Export observations for last 30 days
        console.log('\n📅 Test 3: Export observations for last 30 days');
        const monthResponse = await axios.get(
            `${BASE_URL}/fhir/$export-bundle?resourceType=Observation&timePeriod=days&timePeriodCount=30`,
            { ...axiosConfig, headers }
        );
        console.log(`✅ Last 30 days observations: ${monthResponse.data.entry?.length || 0} found`);

        // Test 4: Export observations for last 1 year
        console.log('\n📅 Test 4: Export observations for last 1 year');
        const yearResponse = await axios.get(
            `${BASE_URL}/fhir/$export-bundle?resourceType=Observation&timePeriod=years&timePeriodCount=1`,
            { ...axiosConfig, headers }
        );
        console.log(`✅ Last 1 year observations: ${yearResponse.data.entry?.length || 0} found`);

        // Test 5: Export specific time range
        console.log('\n📅 Test 5: Export observations for specific time range');
        const startDate = '2024-01-01T00:00:00Z';
        const endDate = '2024-12-31T23:59:59Z';
        const rangeResponse = await axios.get(
            `${BASE_URL}/fhir/$export-bundle?resourceType=Observation&startDate=${startDate}&endDate=${endDate}&observationCode=29463-7`,
            { ...axiosConfig, headers }
        );
        console.log(`✅ Date range observations: ${rangeResponse.data.entry?.length || 0} found`);

        // Test 6: Export different observation types
        console.log('\n🔬 Test 6: Export different observation types');
        const observationTypes = [
            { code: '29463-7', name: 'Body Weight', system: 'http://loinc.org' },
            { code: '8302-2', name: 'Body Height', system: 'http://loinc.org' },
            { code: '85354-9', name: 'Blood Pressure Systolic', system: 'http://loinc.org' },
            { code: '8462-4', name: 'Blood Pressure Diastolic', system: 'http://loinc.org' },
            { code: '8867-4', name: 'Heart Rate', system: 'http://loinc.org' },
            { code: '2708-6', name: 'Oxygen Saturation', system: 'http://loinc.org' }
        ];

        for (const obsType of observationTypes) {
            try {
                const response = await axios.get(
                    `${BASE_URL}/fhir/$export-bundle?resourceType=Observation&observationCode=${obsType.code}&observationSystem=${obsType.system}&maxObservationsPerPatient=5&timePeriod=days&timePeriodCount=30`,
                    { ...axiosConfig, headers }
                );
                console.log(`✅ ${obsType.name}: ${response.data.entry?.length || 0} observations`);
            } catch (error) {
                console.log(`❌ ${obsType.name}: Error - ${error.message}`);
            }
        }

        // Test 7: Export with complex POST query
        console.log('\n📝 Test 7: Export with complex POST query');
        const complexQuery = {
            resourceType: 'Observation',
            observationCode: '29463-7',
            observationSystem: 'http://loinc.org',
            timePeriod: 'days',
            timePeriodCount: 30,
            maxObservationsPerPatient: 10,
            sortOrder: 'desc',
            latestOnly: true,
            bundleType: 'collection'
        };

        const postResponse = await axios.post(`${BASE_URL}/fhir/$export-bundle`, complexQuery, { ...axiosConfig, headers });
        console.log(`✅ Complex POST query: ${postResponse.data.entry?.length || 0} observations found`);

        // Test 8: Performance test with large dataset
        console.log('\n⚡ Test 8: Performance test with large dataset');
        const startTime = Date.now();
        const perfResponse = await axios.get(
            `${BASE_URL}/fhir/$export-bundle?resourceType=Observation&maxResources=1000&timePeriod=days&timePeriodCount=365`,
            { ...axiosConfig, headers }
        );
        const endTime = Date.now();
        const duration = endTime - startTime;
        console.log(`✅ Performance test: ${perfResponse.data.entry?.length || 0} observations in ${duration}ms`);

        // Test 9: Export with different time periods
        console.log('\n⏰ Test 9: Export with different time periods');
        const timePeriods = [
            { period: 'days', count: 7, name: 'Last 7 days' },
            { period: 'days', count: 30, name: 'Last 30 days' },
            { period: 'weeks', count: 4, name: 'Last 4 weeks' },
            { period: 'months', count: 6, name: 'Last 6 months' },
            { period: 'years', count: 1, name: 'Last 1 year' }
        ];

        for (const tp of timePeriods) {
            try {
                const response = await axios.get(
                    `${BASE_URL}/fhir/$export-bundle?resourceType=Observation&timePeriod=${tp.period}&timePeriodCount=${tp.count}&observationCode=29463-7`,
                    { ...axiosConfig, headers }
                );
                console.log(`✅ ${tp.name}: ${response.data.entry?.length || 0} observations`);
            } catch (error) {
                console.log(`❌ ${tp.name}: Error - ${error.message}`);
            }
        }

        // Test 10: Export with sorting and limiting
        console.log('\n📊 Test 10: Export with sorting and limiting');
        const sortResponse = await axios.get(
            `${BASE_URL}/fhir/$export-bundle?resourceType=Observation&observationCode=29463-7&maxObservationsPerPatient=5&sortOrder=asc&latestOnly=true`,
            { ...axiosConfig, headers }
        );
        console.log(`✅ Sorted observations: ${sortResponse.data.entry?.length || 0} found`);

        console.log('\n🎉 All enhanced export bundle tests completed successfully!');

    } catch (error) {
        console.error('❌ Error testing enhanced export bundle:', error.message);
        if (error.response) {
            console.error('Response status:', error.response.status);
            console.error('Response data:', error.response.data);
        }
    }
}

/**
 * Test specific use cases for healthcare scenarios
 */
async function testHealthcareUseCases() {
    console.log('\n🏥 Testing Healthcare Use Cases...\n');

    const headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
    };

    try {
        // Use Case 1: Patient weight trend analysis
        console.log('📈 Use Case 1: Patient weight trend analysis');
        const weightTrend = await axios.get(
            `${BASE_URL}/fhir/$export-bundle?resourceType=Observation&observationCode=29463-7&timePeriod=months&timePeriodCount=6&maxObservationsPerPatient=20&sortOrder=desc`,
            { ...axiosConfig, headers }
        );
        console.log(`✅ Weight trend data: ${weightTrend.data.entry?.length || 0} measurements`);

        // Use Case 2: Blood pressure monitoring
        console.log('\n💓 Use Case 2: Blood pressure monitoring');
        const bpSystolic = await axios.get(
            `${BASE_URL}/fhir/$export-bundle?resourceType=Observation&observationCode=85354-9&timePeriod=days&timePeriodCount=30&maxObservationsPerPatient=10`,
            { ...axiosConfig, headers }
        );
        const bpDiastolic = await axios.get(
            `${BASE_URL}/fhir/$export-bundle?resourceType=Observation&observationCode=8462-4&timePeriod=days&timePeriodCount=30&maxObservationsPerPatient=10`,
            { ...axiosConfig, headers }
        );
        console.log(`✅ Blood pressure data: ${bpSystolic.data.entry?.length || 0} systolic, ${bpDiastolic.data.entry?.length || 0} diastolic measurements`);

        // Use Case 3: Vital signs monitoring
        console.log('\n🫀 Use Case 3: Vital signs monitoring');
        const vitalSigns = await axios.get(
            `${BASE_URL}/fhir/$export-bundle?resourceType=Observation&timePeriod=days&timePeriodCount=7&latestOnly=true`,
            { ...axiosConfig, headers }
        );
        console.log(`✅ Vital signs data: ${vitalSigns.data.entry?.length || 0} measurements`);

        // Use Case 4: Long-term health tracking
        console.log('\n📊 Use Case 4: Long-term health tracking');
        const longTerm = await axios.get(
            `${BASE_URL}/fhir/$export-bundle?resourceType=Observation&timePeriod=years&timePeriodCount=1&observationCode=29463-7&maxObservationsPerPatient=50`,
            { ...axiosConfig, headers }
        );
        console.log(`✅ Long-term health data: ${longTerm.data.entry?.length || 0} measurements`);

        console.log('\n🏥 All healthcare use case tests completed!');

    } catch (error) {
        console.error('❌ Error testing healthcare use cases:', error.message);
    }
}

/**
 * Generate sample data for testing
 */
async function generateSampleData() {
    console.log('\n📝 Generating sample data for testing...\n');

    const sampleObservations = [
        {
            resourceType: 'Observation',
            code: {
                coding: [{
                    system: 'http://loinc.org',
                    code: '29463-7',
                    display: 'Body weight'
                }]
            },
            subject: {
                reference: 'Patient/patient-123'
            },
            effectiveDateTime: '2024-12-19T10:00:00Z',
            valueQuantity: {
                value: 70.5,
                unit: 'kg',
                system: 'http://unitsofmeasure.org',
                code: 'kg'
            }
        },
        {
            resourceType: 'Observation',
            code: {
                coding: [{
                    system: 'http://loinc.org',
                    code: '8302-2',
                    display: 'Body height'
                }]
            },
            subject: {
                reference: 'Patient/patient-123'
            },
            effectiveDateTime: '2024-12-19T10:00:00Z',
            valueQuantity: {
                value: 175,
                unit: 'cm',
                system: 'http://unitsofmeasure.org',
                code: 'cm'
            }
        }
    ];

    const headers = {
        'Content-Type': 'application/fhir+json',
        'Accept': 'application/fhir+json'
    };

    for (const observation of sampleObservations) {
        try {
            const response = await axios.post(
                `${BASE_URL}/fhir/Observation`,
                observation,
                { ...axiosConfig, headers }
            );
            console.log(`✅ Created ${observation.code.coding[0].display} observation`);
        } catch (error) {
            console.log(`❌ Error creating ${observation.code.coding[0].display}: ${error.message}`);
        }
    }
}

// Main execution
async function main() {
    console.log('🚀 Starting Enhanced FHIR Export Bundle Testing Suite\n');
    
    // Generate sample data first
    await generateSampleData();
    
    // Run enhanced tests
    await testEnhancedExportBundle();
    
    // Run healthcare use cases
    await testHealthcareUseCases();
    
    console.log('\n🎯 All tests completed successfully!');
}

// Export functions
module.exports = {
    testEnhancedExportBundle,
    testHealthcareUseCases,
    generateSampleData,
    main
};

// Run if executed directly
if (require.main === module) {
    main().catch(console.error);
}
