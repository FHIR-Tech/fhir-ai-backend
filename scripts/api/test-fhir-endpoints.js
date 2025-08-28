const https = require('https');
const http = require('http');

// Disable SSL certificate verification for development
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const baseUrl = 'https://localhost:5001';

const testEndpoints = [
    { path: '/health', name: 'Health Check', method: 'GET' },
    { path: '/swagger', name: 'Swagger UI', method: 'GET' },
    { path: '/fhir/metadata', name: 'FHIR Metadata', method: 'GET' },
    { path: '/fhir/Patient', name: 'FHIR Patient Search', method: 'GET' },
    { path: '/auth/login', name: 'Login Endpoint', method: 'POST' }
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
                console.log(`   Response: ${data.substring(0, 300)}${data.length > 300 ? '...' : ''}`);
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
            console.log(`   Error: Request timeout after 5 seconds`);
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

async function testAllEndpoints() {
    console.log('🚀 Testing FHIR-AI Backend FHIR Endpoints...\n');
    
    for (const endpoint of testEndpoints) {
        await testEndpoint(endpoint.path, endpoint.name, endpoint.method);
    }
    
    console.log('✨ FHIR Endpoints testing completed!');
}

testAllEndpoints().catch(console.error);
