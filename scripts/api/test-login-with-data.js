const https = require('https');

// Disable SSL certificate verification for development
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const baseUrl = 'https://localhost:5001';

const loginData = {
    username: "admin",
    password: "admin123",
    tenantId: "default"
};

async function testLogin() {
    return new Promise((resolve) => {
        const url = `${baseUrl}/api/auth/login`;
        const postData = JSON.stringify(loginData);
        
        const options = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'Content-Length': Buffer.byteLength(postData)
            }
        };
        
        const req = https.request(url, options, (res) => {
            let data = '';
            
            res.on('data', (chunk) => {
                data += chunk;
            });
            
            res.on('end', () => {
                console.log(`🔐 Login Test (${res.statusCode}):`);
                console.log(`   URL: ${url}`);
                console.log(`   Method: POST`);
                console.log(`   Status: ${res.statusCode} ${res.statusMessage}`);
                console.log(`   Content-Type: ${res.headers['content-type'] || 'N/A'}`);
                console.log(`   Request Data: ${postData}`);
                console.log(`   Response: ${data}`);
                console.log('');
                
                if (res.statusCode === 200) {
                    try {
                        const response = JSON.parse(data);
                        if (response.accessToken) {
                            console.log('✅ Login successful! Token received.');
                            resolve({ success: true, token: response.accessToken, data: response });
                        } else {
                            console.log('⚠️ Login response received but no token found.');
                            resolve({ success: false, data: response });
                        }
                    } catch (e) {
                        console.log('❌ Failed to parse response as JSON.');
                        resolve({ success: false, error: 'Invalid JSON response' });
                    }
                } else {
                    console.log('❌ Login failed.');
                    resolve({ success: false, statusCode: res.statusCode, data });
                }
            });
        });
        
        req.on('error', (err) => {
            console.log(`❌ Login Error:`);
            console.log(`   URL: ${url}`);
            console.log(`   Error: ${err.message}`);
            console.log('');
            resolve({ success: false, error: err.message });
        });
        
        req.setTimeout(5000, () => {
            console.log(`⏰ Login Timeout:`);
            console.log(`   URL: ${url}`);
            console.log('');
            req.destroy();
            resolve({ success: false, error: 'Timeout' });
        });
        
        req.write(postData);
        req.end();
    });
}

async function testWithToken(token) {
    return new Promise((resolve) => {
        const url = `${baseUrl}/fhir/metadata`;
        
        const options = {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': 'application/json'
            }
        };
        
        const req = https.request(url, options, (res) => {
            let data = '';
            
            res.on('data', (chunk) => {
                data += chunk;
            });
            
            res.on('end', () => {
                console.log(`🔍 FHIR Metadata Test (${res.statusCode}):`);
                console.log(`   URL: ${url}`);
                console.log(`   Method: GET`);
                console.log(`   Status: ${res.statusCode} ${res.statusMessage}`);
                console.log(`   Content-Type: ${res.headers['content-type'] || 'N/A'}`);
                if (data.length > 0) {
                    console.log(`   Response: ${data.substring(0, 300)}${data.length > 300 ? '...' : ''}`);
                }
                console.log('');
                resolve({ success: res.statusCode === 200, statusCode: res.statusCode, data });
            });
        });
        
        req.on('error', (err) => {
            console.log(`❌ FHIR Metadata Error:`);
            console.log(`   URL: ${url}`);
            console.log(`   Error: ${err.message}`);
            console.log('');
            resolve({ success: false, error: err.message });
        });
        
        req.setTimeout(5000, () => {
            console.log(`⏰ FHIR Metadata Timeout:`);
            console.log(`   URL: ${url}`);
            console.log('');
            req.destroy();
            resolve({ success: false, error: 'Timeout' });
        });
        
        req.end();
    });
}

async function runTests() {
    console.log('🚀 Testing Login with Real Data...\n');
    
    // Test login
    const loginResult = await testLogin();
    
    if (loginResult.success && loginResult.token) {
        console.log('🎉 Login successful! Testing FHIR endpoints with token...\n');
        
        // Test FHIR metadata with token
        await testWithToken(loginResult.token);
    } else {
        console.log('❌ Login failed. Cannot test authenticated endpoints.');
    }
    
    console.log('✨ Authentication test completed!');
}

runTests().catch(console.error);
