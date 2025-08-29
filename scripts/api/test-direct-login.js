const https = require('https');

// Disable SSL certificate verification for development
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const baseUrl = 'https://localhost:5001';

async function testDirectLogin() {
    console.log('🔐 Testing Direct Login\n');
    
    const testCases = [
        {
            name: 'Correct Credentials',
            data: { username: 'admin', password: 'password', tenantId: 'default' },
            expected: 'Success or specific error message'
        },
        {
            name: 'Wrong Password',
            data: { username: 'admin', password: 'wrongpassword', tenantId: 'default' },
            expected: 'Password verification error'
        },
        {
            name: 'Wrong Username',
            data: { username: 'wronguser', password: 'password', tenantId: 'default' },
            expected: 'User not found error'
        },
        {
            name: 'Wrong Tenant',
            data: { username: 'admin', password: 'password', tenantId: 'wrong-tenant' },
            expected: 'User not found error'
        }
    ];
    
    for (const testCase of testCases) {
        console.log(`🧪 ${testCase.name}`);
        console.log(`Expected: ${testCase.expected}`);
        console.log(`Data: ${JSON.stringify(testCase.data)}`);
        
        const result = await makeLoginRequest(testCase.data);
        
        console.log(`Status: ${result.statusCode}`);
        console.log(`Response: ${result.data}`);
        
        if (result.statusCode === 200) {
            console.log('✅ SUCCESS - Login worked!');
            try {
                const response = JSON.parse(result.data);
                console.log(`   Success: ${response.success}`);
                console.log(`   Access Token: ${response.accessToken ? 'Present' : 'Missing'}`);
            } catch (e) {
                console.log('   Could not parse JSON response');
            }
        } else {
            console.log('❌ FAILED');
            try {
                const error = JSON.parse(result.data);
                console.log(`   Error: ${error.detail}`);
            } catch (e) {
                console.log(`   Raw response: ${result.data}`);
            }
        }
        
        console.log(''.padEnd(60, '-'));
    }
}

async function makeLoginRequest(loginData) {
    return new Promise((resolve) => {
        const postData = JSON.stringify(loginData);
        
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

testDirectLogin().catch(console.error);
