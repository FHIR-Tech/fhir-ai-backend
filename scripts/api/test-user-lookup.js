const https = require('https');

// Disable SSL certificate verification for development
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const baseUrl = 'https://localhost:5001';

async function testUserLookup() {
    console.log('🔍 Testing User Lookup\n');
    
    // Test 1: Try to find user with correct credentials
    console.log('='.repeat(50));
    console.log('TEST: User Lookup with Correct Credentials');
    console.log('='.repeat(50));
    
    const loginData = {
        username: 'admin',
        password: 'password',
        tenantId: 'default'
    };
    
    const postData = JSON.stringify(loginData);
    
    return new Promise((resolve) => {
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
                console.log(`📡 Status: ${res.statusCode} ${res.statusMessage}`);
                console.log(`📡 Response: ${data}`);
                console.log('');
                
                if (res.statusCode === 200) {
                    console.log('✅ Login successful!');
                    try {
                        const response = JSON.parse(data);
                        console.log('   Success:', response.success);
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
                        
                        // Check if this is a user not found error or a different error
                        if (error.detail === 'Invalid username or password') {
                            console.log('   🔍 Analysis: User not found or password incorrect');
                        } else if (error.detail === 'An error occurred during login. Please try again.') {
                            console.log('   🔍 Analysis: User found but exception occurred during login process');
                        }
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

testUserLookup().catch(console.error);
