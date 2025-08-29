const https = require('https');

// Disable SSL certificate verification for development
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

async function testDebugLogin() {
    console.log('🔍 Debug Login Process\n');
    
    // Test 1: Check if we can get any response from login endpoint
    console.log('='.repeat(60));
    console.log('TEST 1: Basic Login Request');
    console.log('='.repeat(60));
    
    const loginData = {
        username: 'admin',
        password: 'password',
        tenantId: 'default'
    };
    
    const result = await makeLoginRequest(loginData);
    console.log(`Status Code: ${result.statusCode}`);
    console.log(`Response: ${result.data}`);
    
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
    
    // Test 2: Try with minimal data
    console.log('='.repeat(60));
    console.log('TEST 2: Minimal Login Data');
    console.log('='.repeat(60));
    
    const minimalData = {
        username: 'admin',
        password: 'password'
    };
    
    const minimalResult = await makeLoginRequest(minimalData);
    console.log(`Status Code: ${minimalResult.statusCode}`);
    console.log(`Response: ${minimalResult.data}`);
    console.log('');
    
    // Test 3: Try with different user
    console.log('='.repeat(60));
    console.log('TEST 3: Different User');
    console.log('='.repeat(60));
    
    const differentUserData = {
        username: 'doctor',
        password: 'doctor123',
        tenantId: 'default'
    };
    
    const differentUserResult = await makeLoginRequest(differentUserData);
    console.log(`Status Code: ${differentUserResult.statusCode}`);
    console.log(`Response: ${differentUserResult.data}`);
    console.log('');
}

async function makeLoginRequest(loginData) {
    return new Promise((resolve) => {
        const postData = JSON.stringify(loginData);
        
        const req = https.request('https://localhost:5001/api/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'Content-Length': Buffer.byteLength(postData)
            }
        }, (res) => {
            let data = '';
            
            res.on('data', (chunk) => {
                data += chunk;
            });
            
            res.on('end', () => {
                resolve({ 
                    statusCode: res.statusCode, 
                    data: data,
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
        
        req.write(postData);
        req.end();
    });
}

testDebugLogin().catch(console.error);
