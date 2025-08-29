const https = require('https');

// Disable SSL certificate verification for development
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const baseUrl = 'https://localhost:5001';

async function testLoginStepByStep() {
    console.log('🔍 Testing Login Process Step by Step\n');
    
    // Test 1: Health Check
    console.log('='.repeat(60));
    console.log('STEP 1: Health Check');
    console.log('='.repeat(60));
    const healthResult = await makeRequest('/health', 'GET');
    console.log(`Status: ${healthResult.statusCode}`);
    console.log(`Response: ${healthResult.data}`);
    console.log('');
    
    // Test 2: Check if user exists in database via API
    console.log('='.repeat(60));
    console.log('STEP 2: Check User Existence');
    console.log('='.repeat(60));
    console.log('Note: This would require a user lookup endpoint');
    console.log('For now, we\'ll test with different credentials to see patterns');
    console.log('');
    
    // Test 3: Test login with various scenarios
    const testScenarios = [
        {
            name: 'Valid User, Valid Password, Valid Tenant',
            data: { username: 'admin', password: 'password', tenantId: 'default' },
            expectedBehavior: 'Should succeed or give specific error'
        },
        {
            name: 'Valid User, Wrong Password, Valid Tenant',
            data: { username: 'admin', password: 'wrongpassword', tenantId: 'default' },
            expectedBehavior: 'Should fail with password error'
        },
        {
            name: 'Valid User, Valid Password, Wrong Tenant',
            data: { username: 'admin', password: 'password', tenantId: 'wrong-tenant' },
            expectedBehavior: 'Should fail with user not found'
        },
        {
            name: 'Wrong User, Valid Password, Valid Tenant',
            data: { username: 'wronguser', password: 'password', tenantId: 'default' },
            expectedBehavior: 'Should fail with user not found'
        }
    ];
    
    for (const scenario of testScenarios) {
        console.log('='.repeat(60));
        console.log(`STEP 3: ${scenario.name}`);
        console.log('='.repeat(60));
        console.log(`Expected: ${scenario.expectedBehavior}`);
        console.log(`Data: ${JSON.stringify(scenario.data)}`);
        
        const result = await makeLoginRequest(scenario.data);
        
        console.log(`Status Code: ${result.statusCode}`);
        console.log(`Response Headers: ${JSON.stringify(result.headers, null, 2)}`);
        console.log(`Response Body: ${result.data}`);
        
        if (result.statusCode === 200) {
            console.log('✅ SUCCESS - Login worked!');
            try {
                const response = JSON.parse(result.data);
                console.log(`   Success: ${response.success}`);
                console.log(`   Access Token: ${response.accessToken ? 'Present' : 'Missing'}`);
                console.log(`   Refresh Token: ${response.refreshToken ? 'Present' : 'Missing'}`);
                console.log(`   User Info: ${response.user ? 'Present' : 'Missing'}`);
            } catch (e) {
                console.log('   Could not parse JSON response');
            }
        } else {
            console.log('❌ FAILED');
            try {
                const error = JSON.parse(result.data);
                console.log(`   Error Type: ${error.type}`);
                console.log(`   Error Title: ${error.title}`);
                console.log(`   Error Detail: ${error.detail}`);
                console.log(`   Error Status: ${error.status}`);
            } catch (e) {
                console.log(`   Raw response: ${result.data}`);
            }
        }
        console.log('');
    }
    
    // Test 4: Check Swagger documentation
    console.log('='.repeat(60));
    console.log('STEP 4: Check API Documentation');
    console.log('='.repeat(60));
    const swaggerResult = await makeRequest('/swagger/v1/swagger.json', 'GET');
    console.log(`Status: ${swaggerResult.statusCode}`);
    if (swaggerResult.statusCode === 200) {
        try {
            const swagger = JSON.parse(swaggerResult.data);
            console.log(`   Title: ${swagger.info?.title}`);
            console.log(`   Version: ${swagger.info?.version}`);
            console.log(`   Total Paths: ${Object.keys(swagger.paths || {}).length}`);
            
            // Check for authentication endpoints
            const authEndpoints = Object.keys(swagger.paths || {}).filter(path => 
                path.includes('/auth') || path.includes('/login')
            );
            console.log(`   Auth Endpoints: ${authEndpoints.length}`);
            authEndpoints.forEach(endpoint => {
                console.log(`     - ${endpoint}`);
            });
        } catch (e) {
            console.log('   Could not parse swagger JSON');
        }
    }
    console.log('');
}

async function makeRequest(path, method, data = null) {
    return new Promise((resolve) => {
        const options = {
            hostname: 'localhost',
            port: 5001,
            path: path,
            method: method,
            headers: {
                'Accept': 'application/json'
            }
        };
        
        if (data) {
            const postData = JSON.stringify(data);
            options.headers['Content-Type'] = 'application/json';
            options.headers['Content-Length'] = Buffer.byteLength(postData);
        }
        
        const req = https.request(options, (res) => {
            let responseData = '';
            
            res.on('data', (chunk) => {
                responseData += chunk;
            });
            
            res.on('end', () => {
                resolve({ 
                    statusCode: res.statusCode, 
                    data: responseData,
                    headers: res.headers
                });
            });
        });
        
        req.on('error', (err) => {
            resolve({ 
                statusCode: 0, 
                data: `Request Error: ${err.message}`,
                headers: {}
            });
        });
        
        req.setTimeout(10000, () => {
            req.destroy();
            resolve({ 
                statusCode: 0, 
                data: 'Request Timeout',
                headers: {}
            });
        });
        
        if (data) {
            req.write(JSON.stringify(data));
        }
        req.end();
    });
}

async function makeLoginRequest(loginData) {
    return makeRequest('/api/auth/login', 'POST', loginData);
}

testLoginStepByStep().catch(console.error);
