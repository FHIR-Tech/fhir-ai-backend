const https = require('https');

// Disable SSL certificate verification for development
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const baseUrl = 'https://localhost:5001';

// Test data
const testLoginData = {
    username: "admin",
    password: "admin123",
    tenantId: "default"
};

const testPatientData = {
    resourceType: "Patient",
    id: "test-patient-1",
    identifier: [
        {
            system: "https://healthtech.com/patients",
            value: "PT001"
        }
    ],
    name: [
        {
            use: "official",
            family: "Doe",
            given: ["John"]
        }
    ],
    gender: "male",
    birthDate: "1990-01-01"
};

let authToken = null;

async function makeRequest(url, options = {}, data = null) {
    return new Promise((resolve) => {
        const req = https.request(url, options, (res) => {
            let responseData = '';
            
            res.on('data', (chunk) => {
                responseData += chunk;
            });
            
            res.on('end', () => {
                resolve({
                    success: res.statusCode >= 200 && res.statusCode < 300,
                    statusCode: res.statusCode,
                    statusMessage: res.statusMessage,
                    headers: res.headers,
                    data: responseData
                });
            });
        });
        
        req.on('error', (err) => {
            resolve({
                success: false,
                error: err.message
            });
        });
        
        req.setTimeout(10000, () => {
            req.destroy();
            resolve({
                success: false,
                error: 'Timeout'
            });
        });
        
        if (data) {
            req.write(data);
        }
        req.end();
    });
}

async function testHealthCheck() {
    console.log('🏥 Testing Health Check...');
    const result = await makeRequest(`${baseUrl}/health`);
    
    if (result.success) {
        console.log(`   ✅ Health Check: ${result.statusCode} - ${result.data}`);
    } else {
        console.log(`   ❌ Health Check Failed: ${result.error || result.statusCode}`);
    }
    console.log('');
}

async function testLogin() {
    console.log('🔐 Testing Login...');
    const loginData = JSON.stringify(testLoginData);
    
    const result = await makeRequest(`${baseUrl}/api/auth/login`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            'Content-Length': Buffer.byteLength(loginData)
        }
    }, loginData);
    
    if (result.success) {
        try {
            const response = JSON.parse(result.data);
            if (response.accessToken) {
                authToken = response.accessToken;
                console.log(`   ✅ Login Successful: Token received`);
                console.log(`   👤 User: ${response.user?.username || 'N/A'}`);
                console.log(`   🏢 Role: ${response.user?.role || 'N/A'}`);
            } else {
                console.log(`   ⚠️ Login Response: ${result.data}`);
            }
        } catch (e) {
            console.log(`   ❌ Login Failed: Invalid JSON response`);
        }
    } else {
        console.log(`   ❌ Login Failed: ${result.error || result.statusCode} - ${result.data}`);
    }
    console.log('');
}

async function testFHIREndpoints() {
    if (!authToken) {
        console.log('❌ Cannot test FHIR endpoints without authentication token');
        return;
    }
    
    console.log('🏥 Testing FHIR Endpoints...');
    
    // Test FHIR Metadata
    console.log('   📋 Testing FHIR Metadata...');
    const metadataResult = await makeRequest(`${baseUrl}/fhir/metadata`, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${authToken}`,
            'Accept': 'application/json'
        }
    });
    
    if (metadataResult.success) {
        console.log(`      ✅ FHIR Metadata: ${metadataResult.statusCode}`);
    } else {
        console.log(`      ❌ FHIR Metadata: ${metadataResult.error || metadataResult.statusCode}`);
    }
    
    // Test Create Patient
    console.log('   👤 Testing Create Patient...');
    const patientData = JSON.stringify(testPatientData);
    const createResult = await makeRequest(`${baseUrl}/fhir/Patient`, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${authToken}`,
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            'Content-Length': Buffer.byteLength(patientData)
        }
    }, patientData);
    
    if (createResult.success) {
        console.log(`      ✅ Create Patient: ${createResult.statusCode}`);
    } else {
        console.log(`      ❌ Create Patient: ${createResult.error || createResult.statusCode}`);
    }
    
    // Test Search Patients
    console.log('   🔍 Testing Search Patients...');
    const searchResult = await makeRequest(`${baseUrl}/fhir/Patient`, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${authToken}`,
            'Accept': 'application/json'
        }
    });
    
    if (searchResult.success) {
        console.log(`      ✅ Search Patients: ${searchResult.statusCode}`);
    } else {
        console.log(`      ❌ Search Patients: ${searchResult.error || searchResult.statusCode}`);
    }
    
    console.log('');
}

async function testPatientAccessEndpoints() {
    if (!authToken) {
        console.log('❌ Cannot test Patient Access endpoints without authentication token');
        return;
    }
    
    console.log('🔐 Testing Patient Access Endpoints...');
    
    // Test Grant Patient Access
    console.log('   ➕ Testing Grant Patient Access...');
    const grantData = JSON.stringify({
        targetUserId: "test-user-1",
        patientId: "test-patient-1",
        accessLevel: "Read",
        reason: "Test access grant"
    });
    
    const grantResult = await makeRequest(`${baseUrl}/api/auth/patient-access/grant`, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${authToken}`,
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            'Content-Length': Buffer.byteLength(grantData)
        }
    }, grantData);
    
    if (grantResult.success) {
        console.log(`      ✅ Grant Access: ${grantResult.statusCode}`);
    } else {
        console.log(`      ❌ Grant Access: ${grantResult.error || grantResult.statusCode}`);
    }
    
    // Test Get Patient Access
    console.log('   📋 Testing Get Patient Access...');
    const getAccessResult = await makeRequest(`${baseUrl}/api/auth/patient-access/test-patient-1`, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${authToken}`,
            'Accept': 'application/json'
        }
    });
    
    if (getAccessResult.success) {
        console.log(`      ✅ Get Access: ${getAccessResult.statusCode}`);
    } else {
        console.log(`      ❌ Get Access: ${getAccessResult.error || getAccessResult.statusCode}`);
    }
    
    console.log('');
}

async function testLogout() {
    if (!authToken) {
        console.log('❌ Cannot test logout without authentication token');
        return;
    }
    
    console.log('🚪 Testing Logout...');
    const result = await makeRequest(`${baseUrl}/api/auth/logout`, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${authToken}`,
            'Accept': 'application/json'
        }
    });
    
    if (result.success) {
        console.log(`   ✅ Logout Successful: ${result.statusCode}`);
    } else {
        console.log(`   ❌ Logout Failed: ${result.error || result.statusCode}`);
    }
    console.log('');
}

async function runComprehensiveTest() {
    console.log('🚀 Starting Comprehensive API Test...\n');
    
    // Test 1: Health Check
    await testHealthCheck();
    
    // Test 2: Login
    await testLogin();
    
    // Test 3: FHIR Endpoints (if authenticated)
    await testFHIREndpoints();
    
    // Test 4: Patient Access Endpoints (if authenticated)
    await testPatientAccessEndpoints();
    
    // Test 5: Logout
    await testLogout();
    
    console.log('✨ Comprehensive API Test Completed!');
    console.log('\n📊 Summary:');
    console.log(`   🔐 Authentication: ${authToken ? '✅ Success' : '❌ Failed'}`);
    console.log(`   🏥 FHIR Endpoints: ${authToken ? '✅ Tested' : '❌ Skipped'}`);
    console.log(`   🔐 Patient Access: ${authToken ? '✅ Tested' : '❌ Skipped'}`);
    console.log('\n🌐 Swagger UI: https://localhost:5001/swagger');
}

runComprehensiveTest().catch(console.error);
