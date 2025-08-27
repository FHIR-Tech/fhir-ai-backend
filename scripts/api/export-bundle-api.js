const axios = require('axios');

// Configuration
const BASE_URL = 'http://localhost:5000';
const API_KEY = 'your-api-key'; // Replace with actual API key if required

// Headers for API requests
const headers = {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${API_KEY}`
};

/**
 * Test export bundle functionality
 */
async function testExportBundle() {
    console.log('🧪 Testing FHIR Export Bundle API...\n');

    try {
        // Test 1: Export all patients
        console.log('📋 Test 1: Export all patients');
        const response1 = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Patient&maxResources=10`, { headers });
        console.log(`✅ Status: ${response1.status}`);
        console.log(`📊 Bundle contains ${response1.data.entry?.length || 0} resources`);
        console.log(`📦 Bundle type: ${response1.data.type}`);
        console.log(`⏰ Timestamp: ${response1.data.timestamp}\n`);

        // Test 2: Export specific patient with history
        console.log('📋 Test 2: Export specific patient with history');
        const response2 = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Patient&fhirIds=patient-123&includeHistory=true&maxHistoryVersions=5`, { headers });
        console.log(`✅ Status: ${response2.status}`);
        console.log(`📊 Bundle contains ${response2.data.entry?.length || 0} resources`);
        console.log(`📦 Bundle type: ${response2.data.type}\n`);

        // Test 3: Export all resources of multiple types
        console.log('📋 Test 3: Export all resources (multiple types)');
        const response3 = await axios.get(`${BASE_URL}/fhir/$export-bundle?maxResources=50&bundleType=collection`, { headers });
        console.log(`✅ Status: ${response3.status}`);
        console.log(`📊 Bundle contains ${response3.data.entry?.length || 0} resources`);
        
        // Analyze resource types in bundle
        if (response3.data.entry) {
            const resourceTypes = response3.data.entry.map(entry => entry.resource?.resourceType).filter(Boolean);
            const typeCount = resourceTypes.reduce((acc, type) => {
                acc[type] = (acc[type] || 0) + 1;
                return acc;
            }, {});
            console.log(`📈 Resource type breakdown:`, typeCount);
        }
        console.log();

        // Test 4: Export with complex search (POST method)
        console.log('📋 Test 4: Export with complex search (POST)');
        const complexQuery = {
            resourceType: 'Patient',
            searchParameters: {
                name: 'Nguyễn',
                date: '2024-01-01'
            },
            maxResources: 20,
            includeHistory: false,
            bundleType: 'collection'
        };
        
        const response4 = await axios.post(`${BASE_URL}/fhir/$export-bundle`, complexQuery, { headers });
        console.log(`✅ Status: ${response4.status}`);
        console.log(`📊 Bundle contains ${response4.data.entry?.length || 0} resources`);
        console.log(`📦 Bundle type: ${response4.data.type}\n`);

        // Test 5: Export observations
        console.log('📋 Test 5: Export observations');
        const response5 = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Observation&maxResources=20`, { headers });
        console.log(`✅ Status: ${response5.status}`);
        console.log(`📊 Bundle contains ${response5.data.entry?.length || 0} resources`);
        console.log(`📦 Bundle type: ${response5.data.type}\n`);

        // Test 6: Export encounters
        console.log('📋 Test 6: Export encounters');
        const response6 = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Encounter&maxResources=20`, { headers });
        console.log(`✅ Status: ${response6.status}`);
        console.log(`📊 Bundle contains ${response6.data.entry?.length || 0} resources`);
        console.log(`📦 Bundle type: ${response6.data.type}\n`);

        console.log('🎉 All export bundle tests completed successfully!');

    } catch (error) {
        console.error('❌ Error testing export bundle:', error.message);
        if (error.response) {
            console.error('📄 Response data:', error.response.data);
            console.error('📊 Status:', error.response.status);
        }
    }
}

/**
 * Test export bundle with file download
 */
async function testExportBundleDownload() {
    console.log('💾 Testing Export Bundle Download...\n');

    try {
        // Export bundle and save to file
        console.log('📥 Downloading bundle as file...');
        const response = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Patient&maxResources=50`, { 
            headers,
            responseType: 'stream'
        });

        const fs = require('fs');
        const writer = fs.createWriteStream('exported_bundle.json');
        response.data.pipe(writer);

        return new Promise((resolve, reject) => {
            writer.on('finish', () => {
                console.log('✅ Bundle downloaded successfully to exported_bundle.json');
                resolve();
            });
            writer.on('error', reject);
        });

    } catch (error) {
        console.error('❌ Error downloading bundle:', error.message);
    }
}

/**
 * Test export bundle performance
 */
async function testExportBundlePerformance() {
    console.log('⚡ Testing Export Bundle Performance...\n');

    const startTime = Date.now();
    
    try {
        // Test with different resource limits
        const limits = [10, 50, 100, 500];
        
        for (const limit of limits) {
            const testStart = Date.now();
            console.log(`📊 Testing with ${limit} resources...`);
            
            const response = await axios.get(`${BASE_URL}/fhir/$export-bundle?maxResources=${limit}`, { headers });
            
            const testEnd = Date.now();
            const duration = testEnd - testStart;
            const bundleSize = JSON.stringify(response.data).length;
            
            console.log(`✅ Completed in ${duration}ms`);
            console.log(`📦 Bundle size: ${(bundleSize / 1024).toFixed(2)} KB`);
            console.log(`📊 Resources returned: ${response.data.entry?.length || 0}\n`);
        }

        const totalTime = Date.now() - startTime;
        console.log(`🎯 Total performance test completed in ${totalTime}ms`);

    } catch (error) {
        console.error('❌ Error in performance test:', error.message);
    }
}

// Main execution
async function main() {
    console.log('🚀 Starting FHIR Export Bundle API Tests\n');
    
    await testExportBundle();
    console.log('\n' + '='.repeat(50) + '\n');
    
    await testExportBundleDownload();
    console.log('\n' + '='.repeat(50) + '\n');
    
    await testExportBundlePerformance();
    
    console.log('\n🎉 All tests completed!');
}

// Run tests if this file is executed directly
if (require.main === module) {
    main().catch(console.error);
}

module.exports = {
    testExportBundle,
    testExportBundleDownload,
    testExportBundlePerformance
};
