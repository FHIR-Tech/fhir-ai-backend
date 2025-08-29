const https = require('https');

// Disable SSL certificate verification for development
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const baseUrl = 'https://localhost:5001';

async function testLogin(username, password, tenantId) {
    return new Promise((resolve) => {
        const loginData = {
            username: username,
            password: password,
            tenantId: tenantId
        };
        
        const postData = JSON.stringify(loginData);
        
        console.log(`🔐 Testing login for user: ${username}`);
        console.log(`   Password: ${password}`);
        console.log(`   Tenant: ${tenantId}`);
        console.log(`   Request data: ${postData}`);
        console.log('');
        
        const req = https.request(`${baseUrl}/api/auth/login`, {
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
                console.log(`📡 Response Status: ${res.statusCode} ${res.statusMessage}`);
                console.log(`📡 Response Headers:`, res.headers);
                console.log(`📡 Response Body: ${data}`);
                console.log('');
                
                if (res.statusCode === 200) {
                    console.log('✅ Login successful!');
                    try {
                        const response = JSON.parse(data);
                        console.log('   Access Token:', response.accessToken ? 'Present' : 'Missing');
                        console.log('   Refresh Token:', response.refreshToken ? 'Present' : 'Missing');
                        console.log('   User Info:', response.user ? 'Present' : 'Missing');
                    } catch (e) {
                        console.log('   Could not parse response JSON');
                    }
                } else {
                    console.log('❌ Login failed');
                    try {
                        const error = JSON.parse(data);
                        console.log('   Error Type:', error.type);
                        console.log('   Error Title:', error.title);
                        console.log('   Error Detail:', error.detail);
                    } catch (e) {
                        console.log('   Could not parse error JSON');
                    }
                }
                console.log('');
                
                resolve({ success: res.statusCode === 200, data, statusCode: res.statusCode });
            });
        });
        
        req.on('error', (err) => {
            console.log(`❌ Request Error: ${err.message}`);
            resolve({ success: false, error: err.message });
        });
        
        req.setTimeout(10000, () => {
            console.log('⏰ Request Timeout');
            req.destroy();
            resolve({ success: false, error: 'Timeout' });
        });
        
        req.write(postData);
        req.end();
    });
}

async function runTests() {
    console.log('🚀 Starting Detailed Authentication Tests\n');
    
    // Test 1: Correct credentials
    console.log('='.repeat(60));
    console.log('TEST 1: Correct Credentials');
    console.log('='.repeat(60));
    await testLogin('admin', 'password', 'default');
    
    // Test 2: Wrong password
    console.log('='.repeat(60));
    console.log('TEST 2: Wrong Password');
    console.log('='.repeat(60));
    await testLogin('admin', 'wrongpassword', 'default');
    
    // Test 3: Wrong username
    console.log('='.repeat(60));
    console.log('TEST 3: Wrong Username');
    console.log('='.repeat(60));
    await testLogin('wronguser', 'password', 'default');
    
    // Test 4: Wrong tenant
    console.log('='.repeat(60));
    console.log('TEST 4: Wrong Tenant');
    console.log('='.repeat(60));
    await testLogin('admin', 'password', 'wrong-tenant');
    
    console.log('✨ All tests completed!');
}

runTests().catch(console.error);
