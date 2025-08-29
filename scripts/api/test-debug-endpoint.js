const https = require('https');

// Disable SSL certificate verification for development
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

async function testDebugEndpoint() {
    console.log('🔍 Testing Debug Endpoint\n');
    
    // Test 1: Simple GET request to see if we can get any response
    console.log('='.repeat(50));
    console.log('TEST 1: Simple GET Request');
    console.log('='.repeat(50));
    
    const result = await makeRequest('/api/auth/login', 'GET');
    console.log(`Status: ${result.statusCode}`);
    console.log(`Response: ${result.data}`);
    console.log('');
    
    // Test 2: Try with different content types
    console.log('='.repeat(50));
    console.log('TEST 2: Different Content Types');
    console.log('='.repeat(50));
    
    const testData = { username: 'admin', password: 'password', tenantId: 'default' };
    
    // Test with application/json
    const jsonResult = await makeRequest('/api/auth/login', 'POST', testData, 'application/json');
    console.log('Content-Type: application/json');
    console.log(`Status: ${jsonResult.statusCode}`);
    console.log(`Response: ${jsonResult.data}`);
    console.log('');
    
    // Test with text/plain
    const textResult = await makeRequest('/api/auth/login', 'POST', testData, 'text/plain');
    console.log('Content-Type: text/plain');
    console.log(`Status: ${textResult.statusCode}`);
    console.log(`Response: ${textResult.data}`);
    console.log('');
}

async function makeRequest(path, method, data = null, contentType = 'application/json') {
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
        
        if (data && method === 'POST') {
            const postData = JSON.stringify(data);
            options.headers['Content-Type'] = contentType;
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
        
        if (data && method === 'POST') {
            req.write(JSON.stringify(data));
        }
        req.end();
    });
}

testDebugEndpoint().catch(console.error);
