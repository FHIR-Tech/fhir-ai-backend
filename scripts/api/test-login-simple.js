const axios = require('axios');

// Configuration
const BASE_URL = 'https://localhost:5001';

// Test different password combinations
const testCredentials = [
    { username: 'admin', password: 'admin123', tenantId: 'default' },
    { username: 'admin', password: 'admin', tenantId: 'default' },
    { username: 'admin', password: 'password', tenantId: 'default' },
    { username: 'admin', password: '123456', tenantId: 'default' },
    { username: 'admin', password: 'admin@demo.healthtech.com', tenantId: 'default' }
];

async function testLogin(credentials) {
    try {
        console.log(`🔐 Testing login with: ${credentials.username}/${credentials.password}`);
        
        const response = await axios.post(`${BASE_URL}/api/auth/login`, credentials, {
            headers: {
                'Content-Type': 'application/json'
            },
            httpsAgent: new (require('https').Agent)({
                rejectUnauthorized: false
            })
        });
        
        console.log(`✅ SUCCESS: ${response.status} - ${JSON.stringify(response.data)}`);
        return response.data;
    } catch (error) {
        console.log(`❌ FAILED: ${error.response?.status || 'Network Error'} - ${error.response?.data?.detail || error.message}`);
        return null;
    }
}

async function runTests() {
    console.log('🚀 Testing Login with Different Credentials...\n');
    
    for (const cred of testCredentials) {
        await testLogin(cred);
        console.log('---');
    }
    
    console.log('✨ Login tests completed!');
}

runTests().catch(console.error);
