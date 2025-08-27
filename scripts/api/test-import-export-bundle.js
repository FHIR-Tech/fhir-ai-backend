const axios = require('axios');
const fs = require('fs');
const path = require('path');

const BASE_URL = 'http://localhost:5000';

async function testImportExportBundle() {
    console.log('🧪 Testing FHIR Import/Export Bundle API...\n');

    try {
        // Step 1: Import test bundle
        console.log('📥 Step 1: Importing test bundle...');
        const bundlePath = path.join(__dirname, '../samples/bundles/export_test_bundle.json');
        const bundleData = fs.readFileSync(bundlePath, 'utf8');
        
        const importResponse = await axios.post(`${BASE_URL}/fhir/$import-bundle`, bundleData, {
            headers: { 'Content-Type': 'application/json' }
        });
        
        console.log(`✅ Import Status: ${importResponse.status}`);
        console.log(`📊 Imported ${importResponse.data.totalResources || 0} resources\n`);

        // Step 2: Export all resources
        console.log('📤 Step 2: Exporting all resources...');
        const exportResponse1 = await axios.get(`${BASE_URL}/fhir/$export-bundle?maxResources=100`);
        
        console.log(`✅ Export Status: ${exportResponse1.status}`);
        console.log(`📊 Exported ${exportResponse1.data.entry?.length || 0} resources`);
        console.log(`📦 Bundle type: ${exportResponse1.data.type}`);
        console.log(`⏰ Timestamp: ${exportResponse1.data.timestamp}\n`);

        // Step 3: Export specific resource types
        console.log('📤 Step 3: Exporting specific resource types...');
        
        // Export patients
        const patientExport = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Patient&maxResources=50`);
        console.log(`👥 Patients exported: ${patientExport.data.entry?.length || 0}`);
        
        // Export observations
        const observationExport = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Observation&maxResources=50`);
        console.log(`🔬 Observations exported: ${observationExport.data.entry?.length || 0}`);
        
        // Export encounters
        const encounterExport = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Encounter&maxResources=50`);
        console.log(`🏥 Encounters exported: ${encounterExport.data.entry?.length || 0}`);
        
        // Export conditions
        const conditionExport = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Condition&maxResources=50`);
        console.log(`🏥 Conditions exported: ${conditionExport.data.entry?.length || 0}\n`);

        // Step 4: Export with complex search (POST)
        console.log('📤 Step 4: Exporting with complex search (POST)...');
        const complexQuery = {
            resourceType: 'Patient',
            searchParameters: {
                name: 'Nguyễn'
            },
            maxResources: 20,
            bundleType: 'collection'
        };
        
        const complexExport = await axios.post(`${BASE_URL}/fhir/$export-bundle`, complexQuery);
        console.log(`✅ Complex export completed: ${complexExport.data.entry?.length || 0} resources\n`);

        // Step 5: Export with history
        console.log('📤 Step 5: Exporting with history...');
        const historyExport = await axios.get(`${BASE_URL}/fhir/$export-bundle?resourceType=Patient&includeHistory=true&maxHistoryVersions=5`);
        console.log(`✅ History export completed: ${historyExport.data.entry?.length || 0} resources\n`);

        // Step 6: Save exported bundle to file
        console.log('💾 Step 6: Saving exported bundle to file...');
        const exportedBundle = JSON.stringify(exportResponse1.data, null, 2);
        fs.writeFileSync('exported_bundle_result.json', exportedBundle);
        console.log('✅ Exported bundle saved to exported_bundle_result.json\n');

        console.log('🎉 All import/export tests completed successfully!');

    } catch (error) {
        console.error('❌ Error:', error.message);
        if (error.response) {
            console.error('📄 Response data:', error.response.data);
            console.error('📊 Status:', error.response.status);
        }
    }
}

// Run the test
testImportExportBundle();
