const https = require('https');

// Disable SSL certificate verification for development
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const baseUrl = 'https://localhost:5001';

async function testHealth() {
    return new Promise((resolve) => {
        const req = https.request(`${baseUrl}/health`, {
            method: 'GET',
            headers: {
                'Accept': 'application/json'
            }
        }, (res) => {
            let data = '';
            
            res.on('data', (chunk) => {
                data += chunk;
            });
            
            res.on('end', () => {
                console.log(`🏥 Health Check: ${res.statusCode} ${res.statusMessage}`);
                console.log(`   Response: ${data}`);
                console.log('');
                resolve({ success: res.statusCode === 200, data });
            });
        });
        
        req.on('error', (err) => {
            console.log(`❌ Health Check Error: ${err.message}`);
            resolve({ success: false, error: err.message });
        });
        
        req.end();
    });
}

async function testSwagger() {
    return new Promise((resolve) => {
        const req = https.request(`${baseUrl}/swagger/v1/swagger.json`, {
            method: 'GET',
            headers: {
                'Accept': 'application/json'
            }
        }, (res) => {
            let data = '';
            
            res.on('data', (chunk) => {
                data += chunk;
            });
            
            res.on('end', () => {
                console.log(`📚 Swagger: ${res.statusCode} ${res.statusMessage}`);
                if (res.statusCode === 200) {
                    try {
                        const swagger = JSON.parse(data);
                        console.log(`   Title: ${swagger.info?.title}`);
                        console.log(`   Version: ${swagger.info?.version}`);
                        console.log(`   Paths: ${Object.keys(swagger.paths || {}).length} endpoints`);
                    } catch (e) {
                        console.log(`   Could not parse swagger JSON`);
                    }
                }
                console.log('');
                resolve({ success: res.statusCode === 200, data });
            });
        });
        
        req.on('error', (err) => {
            console.log(`❌ Swagger Error: ${err.message}`);
            resolve({ success: false, error: err.message });
        });
        
        req.end();
    });
}

async function runTests() {
    console.log('🔍 Testing API and Database Connection\n');
    
    // Test 1: Health check
    console.log('='.repeat(50));
    console.log('TEST 1: Health Check');
    console.log('='.repeat(50));
    await testHealth();
    
    // Test 2: Swagger documentation
    console.log('='.repeat(50));
    console.log('TEST 2: Swagger Documentation');
    console.log('='.repeat(50));
    await testSwagger();
    
    console.log('✨ Connection tests completed!');
}

runTests().catch(console.error);
