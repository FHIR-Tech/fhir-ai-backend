const https = require('https');

// Disable SSL certificate verification for development
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const baseUrl = 'https://localhost:5001';

const swaggerEndpoints = [
    { path: '/', name: 'Root' },
    { path: '/index.html', name: 'Index HTML' },
    { path: '/swagger', name: 'Swagger' },
    { path: '/swagger/index.html', name: 'Swagger Index' },
    { path: '/swagger/v1/swagger.json', name: 'Swagger JSON' },
    { path: '/api-docs', name: 'API Docs' },
    { path: '/docs', name: 'Docs' }
];

async function testEndpoint(path, name) {
    return new Promise((resolve) => {
        const url = `${baseUrl}${path}`;
        
        const req = https.get(url, (res) => {
            let data = '';
            
            res.on('data', (chunk) => {
                data += chunk;
            });
            
            res.on('end', () => {
                console.log(`✅ ${name} (${res.statusCode}):`);
                console.log(`   URL: ${url}`);
                console.log(`   Status: ${res.statusCode} ${res.statusMessage}`);
                console.log(`   Content-Type: ${res.headers['content-type'] || 'N/A'}`);
                console.log(`   Response Length: ${data.length} characters`);
                if (data.length > 0 && data.length < 500) {
                    console.log(`   Response Preview: ${data.substring(0, 200)}...`);
                }
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
            console.log('');
            req.destroy();
            resolve({ success: false, error: 'Timeout' });
        });
    });
}

async function testAllSwaggerEndpoints() {
    console.log('🔍 Testing Swagger UI and API Documentation Endpoints...\n');
    
    for (const endpoint of swaggerEndpoints) {
        await testEndpoint(endpoint.path, endpoint.name);
    }
    
    console.log('✨ Swagger routing test completed!');
}

testAllSwaggerEndpoints().catch(console.error);
