const https = require('https');
const http = require('http');

// Disable SSL certificate verification for development
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const testEndpoints = [
    { url: 'http://localhost:5000/health', name: 'HTTP Health' },
    { url: 'https://localhost:5001/health', name: 'HTTPS Health' },
    { url: 'http://localhost:5000/', name: 'HTTP Swagger' },
    { url: 'https://localhost:5001/', name: 'HTTPS Swagger' }
];

async function testEndpoint(url, name) {
    return new Promise((resolve) => {
        const client = url.startsWith('https') ? https : http;
        
        const req = client.get(url, (res) => {
            let data = '';
            
            res.on('data', (chunk) => {
                data += chunk;
            });
            
            res.on('end', () => {
                console.log(`✅ ${name} (${res.statusCode}):`);
                console.log(`   URL: ${url}`);
                console.log(`   Status: ${res.statusCode} ${res.statusMessage}`);
                console.log(`   Headers: ${JSON.stringify(res.headers, null, 2)}`);
                console.log(`   Response: ${data.substring(0, 500)}${data.length > 500 ? '...' : ''}`);
                console.log('');
                resolve({ success: true, statusCode: res.statusCode, data });
            });
        });
        
        req.on('error', (err) => {
            console.log(`❌ ${name} - Error:`);
            console.log(`   URL: ${url}`);
            console.log(`   Error: ${err.message}`);
            console.log('');
            resolve({ success: false, error: err.message });
        });
        
        req.setTimeout(5000, () => {
            console.log(`⏰ ${name} - Timeout:`);
            console.log(`   URL: ${url}`);
            console.log(`   Error: Request timeout after 5 seconds`);
            console.log('');
            req.destroy();
            resolve({ success: false, error: 'Timeout' });
        });
    });
}

async function testAllEndpoints() {
    console.log('🚀 Testing FHIR-AI Backend API Endpoints...\n');
    
    for (const endpoint of testEndpoints) {
        await testEndpoint(endpoint.url, endpoint.name);
    }
    
    console.log('✨ Testing completed!');
}

testAllEndpoints().catch(console.error);
