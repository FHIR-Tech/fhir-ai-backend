const axios = require('axios');

// Configuration
const BASE_URL = 'https://localhost:5001';

async function testLogin() {
    try {
        console.log('🔐 Testing login with admin/admin123...');
        
        const loginData = {
            username: 'admin',
            password: 'admin123',
            tenantId: 'default'
        };
        
        console.log('📤 Request data:', JSON.stringify(loginData, null, 2));
        
        const response = await axios.post(`${BASE_URL}/api/auth/login`, loginData, {
            headers: {
                'Content-Type': 'application/json'
            },
            httpsAgent: new (require('https').Agent)({
                rejectUnauthorized: false
            }),
            validateStatus: function (status) {
                return status < 500; // Accept all status codes less than 500
            }
        });
        
        console.log('📥 Response status:', response.status);
        console.log('📥 Response headers:', response.headers);
        console.log('📥 Response data:', JSON.stringify(response.data, null, 2));
        
        if (response.status === 200) {
            console.log('✅ Login successful!');
            return response.data;
        } else {
            console.log('❌ Login failed with status:', response.status);
            return null;
        }
    } catch (error) {
        console.log('💥 Error during login:', error.message);
        if (error.response) {
            console.log('📥 Error response status:', error.response.status);
            console.log('📥 Error response data:', JSON.stringify(error.response.data, null, 2));
        }
        return null;
    }
}

async function testHealth() {
    try {
        console.log('\n🏥 Testing health endpoint...');
        
        const response = await axios.get(`${BASE_URL}/health`, {
            httpsAgent: new (require('https').Agent)({
                rejectUnauthorized: false
            })
        });
        
        console.log('✅ Health check successful:', response.data);
        return true;
    } catch (error) {
        console.log('❌ Health check failed:', error.message);
        return false;
    }
}

async function runTests() {
    console.log('🚀 Starting debug tests...\n');
    
    // Test health first
    const healthOk = await testHealth();
    if (!healthOk) {
        console.log('❌ API is not responding, stopping tests');
        return;
    }
    
    // Test login
    const loginResult = await testLogin();
    
    console.log('\n✨ Debug tests completed!');
}

runTests().catch(console.error);
