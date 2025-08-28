const https = require('https');

// Disable SSL certificate verification for development
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const baseUrl = 'https://localhost:5001';

const authEndpoints = [
    { path: '/auth/login', name: 'Login', method: 'POST' },
    { path: '/auth/logout', name: 'Logout', method: 'POST' },
    { path: '/auth/refresh', name: 'Refresh Token', method: 'POST' },
    { path: '/auth/register', name: 'Register', method: 'POST' },
    { path: '/api/auth/login', name: 'API Login', method: 'POST' },
    { path: '/api/auth/logout', name: 'API Logout', method: 'POST' },
    { path: '/login', name: 'Simple Login', method: 'POST' },
    { path: '/logout', name: 'Simple Logout', method: 'POST' }
];

async function testEndpoint(path, name, method = 'GET') {
    return new Promise((resolve) => {
        const url = `${baseUrl}${path}`;
        const options = {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            }
        };
        
        const req = https.request(url, options, (res) => {
            let data = '';
            
            res.on('data', (chunk) => {
                data += chunk;
            });
            
            res.on('end', () => {
                console.log(`✅ ${name} (${res.statusCode}):`);
                console.log(`   URL: ${url}`);
                console.log(`   Method: ${method}`);
                console.log(`   Status: ${res.statusCode} ${res.statusMessage}`);
                console.log(`   Content-Type: ${res.headers['content-type'] || 'N/A'}`);
                if (data.length > 0) {
                    console.log(`   Response: ${data.substring(0, 200)}${data.length > 200 ? '...' : ''}`);
                }
                console.log('');
                resolve({ success: true, statusCode: res.statusCode, data });
            });
        });
        
        req.on('error', (err) => {
            console.log(`❌ ${name} - Error:`);
            console.log(`   URL: ${url}`);
            console.log(`   Method: ${method}`);
            console.log(`   Error: ${err.message}`);
            console.log('');
            resolve({ success: false, error: err.message });
        });
        
        req.setTimeout(5000, () => {
            console.log(`⏰ ${name} - Timeout:`);
            console.log(`   URL: ${url}`);
            console.log(`   Method: ${method}`);
            console.log('');
            req.destroy();
            resolve({ success: false, error: 'Timeout' });
        });
        
        // For POST requests, send empty body
        if (method === 'POST') {
            req.write('{}');
        }
        
        req.end();
    });
}

async function testAllAuthEndpoints() {
    console.log('🔐 Testing Authentication Endpoints...\n');
    
    for (const endpoint of authEndpoints) {
        await testEndpoint(endpoint.path, endpoint.name, endpoint.method);
    }
    
    console.log('✨ Authentication endpoints test completed!');
}

testAllAuthEndpoints().catch(console.error);
