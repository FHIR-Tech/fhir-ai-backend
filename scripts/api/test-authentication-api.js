const axios = require('axios');

// Configure axios to ignore SSL certificate verification for development
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

// Configuration
const BASE_URL = 'https://localhost:5001';
const API_KEY = 'dev-api-key-12345';

// Test data
const testUsers = {
    admin: {
        username: 'admin',
        password: 'password',
        tenantId: 'default'
    },
    doctor: {
        username: 'doctor',
        password: 'doctor123',
        tenantId: 'default'
    },
    nurse: {
        username: 'nurse',
        password: 'nurse123',
        tenantId: 'default'
    }
};

// Helper function to log results
function logResult(testName, success, message, data = null) {
    const status = success ? '✅ PASS' : '❌ FAIL';
    console.log(`${status} ${testName}`);
    if (message) console.log(`   ${message}`);
    if (data) console.log(`   Data: ${JSON.stringify(data, null, 2)}`);
    console.log('');
}

// Helper function to make authenticated requests
async function makeAuthenticatedRequest(url, method = 'GET', data = null, token = null) {
    const config = {
        method,
        url: `${BASE_URL}${url}`,
        headers: {
            'Content-Type': 'application/json',
            'X-API-Key': API_KEY
        }
    };

    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }

    if (data) {
        config.data = data;
    }

    try {
        const response = await axios(config);
        return { success: true, data: response.data, status: response.status };
    } catch (error) {
        return { 
            success: false, 
            error: error.response?.data || error.message, 
            status: error.response?.status 
        };
    }
}

// Test 1: Login with valid credentials
async function testValidLogin() {
    console.log('🧪 Testing Login with Valid Credentials');
    
    const loginData = {
        username: testUsers.admin.username,
        password: testUsers.admin.password,
        tenantId: testUsers.admin.tenantId
    };

    const result = await makeAuthenticatedRequest('/api/auth/login', 'POST', loginData);
    
    if (result.success && result.data.success) {
        logResult('Valid Login', true, 'Login successful', {
            token: result.data.token ? 'Present' : 'Missing',
            refreshToken: result.data.refreshToken ? 'Present' : 'Missing',
            user: result.data.user?.username
        });
        return result.data.token; // Return token for other tests
    } else {
        logResult('Valid Login', false, result.error?.detail || 'Login failed');
        return null;
    }
}

// Test 2: Login with invalid credentials
async function testInvalidLogin() {
    console.log('🧪 Testing Login with Invalid Credentials');
    
    const loginData = {
        username: 'invaliduser',
        password: 'wrongpassword',
        tenantId: 'test-tenant-1'
    };

    const result = await makeAuthenticatedRequest('/api/auth/login', 'POST', loginData);
    
    if (!result.success && result.status === 400) {
        logResult('Invalid Login', true, 'Properly rejected invalid credentials');
    } else {
        logResult('Invalid Login', false, 'Should have rejected invalid credentials');
    }
}

// Test 3: Refresh token
async function testRefreshToken(refreshToken) {
    console.log('🧪 Testing Token Refresh');
    
    if (!refreshToken) {
        logResult('Refresh Token', false, 'No refresh token available');
        return null;
    }

    const refreshData = {
        refreshToken: refreshToken
    };

    const result = await makeAuthenticatedRequest('/api/auth/refresh', 'POST', refreshData);
    
    if (result.success && result.data.success) {
        logResult('Refresh Token', true, 'Token refreshed successfully', {
            newToken: result.data.token ? 'Present' : 'Missing',
            newRefreshToken: result.data.refreshToken ? 'Present' : 'Missing'
        });
        return result.data.token;
    } else {
        logResult('Refresh Token', false, result.error?.detail || 'Token refresh failed');
        return null;
    }
}

// Test 4: Logout
async function testLogout(token) {
    console.log('🧪 Testing Logout');
    
    if (!token) {
        logResult('Logout', false, 'No token available');
        return;
    }

    const logoutData = {
        sessionToken: token,
        reason: 'Test logout'
    };

    const result = await makeAuthenticatedRequest('/api/auth/logout', 'POST', logoutData, token);
    
    if (result.success && result.data.success) {
        logResult('Logout', true, 'Logout successful');
    } else {
        logResult('Logout', false, result.error?.detail || 'Logout failed');
    }
}

// Test 5: Grant patient access
async function testGrantPatientAccess(token) {
    console.log('🧪 Testing Grant Patient Access');
    
    if (!token) {
        logResult('Grant Patient Access', false, 'No token available');
        return;
    }

    const grantData = {
        patientId: '550e8400-e29b-41d4-a716-446655440000', // Test patient ID
        userId: '550e8400-e29b-41d4-a716-446655440001', // Test user ID
        accessLevel: 'ReadOnly',
        expiresAt: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString(), // 7 days from now
        isEmergencyAccess: false,
        purpose: 'Clinical care and treatment'
    };

    const result = await makeAuthenticatedRequest('/api/auth/patient-access/grant', 'POST', grantData, token);
    
    if (result.success && result.data.success) {
        logResult('Grant Patient Access', true, 'Patient access granted successfully', {
            accessId: result.data.accessId
        });
    } else {
        logResult('Grant Patient Access', false, result.error?.detail || 'Failed to grant patient access');
    }
}

// Test 6: Get patient access list
async function testGetPatientAccess(token) {
    console.log('🧪 Testing Get Patient Access List');
    
    if (!token) {
        logResult('Get Patient Access', false, 'No token available');
        return;
    }

    const patientId = '550e8400-e29b-41d4-a716-446655440000'; // Test patient ID
    const result = await makeAuthenticatedRequest(`/api/auth/patient-access/${patientId}`, 'GET', null, token);
    
    if (result.success && result.data.success) {
        logResult('Get Patient Access', true, 'Patient access list retrieved successfully', {
            totalCount: result.data.totalCount,
            accessListCount: result.data.accessList?.length || 0
        });
    } else {
        logResult('Get Patient Access', false, result.error?.detail || 'Failed to get patient access list');
    }
}

// Test 7: Revoke patient access
async function testRevokePatientAccess(token) {
    console.log('🧪 Testing Revoke Patient Access');
    
    if (!token) {
        logResult('Revoke Patient Access', false, 'No token available');
        return;
    }

    const revokeData = {
        accessId: '550e8400-e29b-41d4-a716-446655440002', // Test access ID
        reason: 'Access no longer needed'
    };

    const result = await makeAuthenticatedRequest('/api/auth/patient-access/revoke', 'POST', revokeData, token);
    
    if (result.success && result.data.success) {
        logResult('Revoke Patient Access', true, 'Patient access revoked successfully');
    } else {
        logResult('Revoke Patient Access', false, result.error?.detail || 'Failed to revoke patient access');
    }
}

// Test 8: Access protected endpoint without token
async function testUnauthorizedAccess() {
    console.log('🧪 Testing Unauthorized Access');
    
    const result = await makeAuthenticatedRequest('/api/auth/patient-access/550e8400-e29b-41d4-a716-446655440000', 'GET');
    
    if (!result.success && result.status === 401) {
        logResult('Unauthorized Access', true, 'Properly rejected unauthorized access');
    } else {
        logResult('Unauthorized Access', false, 'Should have rejected unauthorized access');
    }
}

// Test 9: Health check (public endpoint)
async function testHealthCheck() {
    console.log('🧪 Testing Health Check');
    
    const result = await makeAuthenticatedRequest('/health', 'GET');
    
    if (result.success && result.data.status === 'Healthy') {
        logResult('Health Check', true, 'Health check successful', {
            status: result.data.status,
            timestamp: result.data.timestamp
        });
    } else {
        logResult('Health Check', false, 'Health check failed');
    }
}

// Main test runner
async function runAllTests() {
    console.log('🚀 Starting Authentication API Tests\n');
    console.log('=' .repeat(50));
    
    let token = null;
    let refreshToken = null;

    try {
        // Test public endpoints first
        await testHealthCheck();
        await testUnauthorizedAccess();
        
        // Test authentication flow
        const loginResult = await testValidLogin();
        if (loginResult) {
            token = loginResult.token;
            refreshToken = loginResult.refreshToken;
        }
        
        await testInvalidLogin();
        
        // Test token refresh
        if (refreshToken) {
            const newToken = await testRefreshToken(refreshToken);
            if (newToken) {
                token = newToken; // Use the refreshed token
            }
        }
        
        // Test patient access operations
        await testGrantPatientAccess(token);
        await testGetPatientAccess(token);
        await testRevokePatientAccess(token);
        
        // Test logout
        await testLogout(token);
        
    } catch (error) {
        console.error('❌ Test execution failed:', error.message);
    }
    
    console.log('=' .repeat(50));
    console.log('🏁 Authentication API Tests Completed\n');
}

// Run tests if this script is executed directly
if (require.main === module) {
    runAllTests().catch(console.error);
}

module.exports = {
    runAllTests,
    testValidLogin,
    testInvalidLogin,
    testRefreshToken,
    testLogout,
    testGrantPatientAccess,
    testGetPatientAccess,
    testRevokePatientAccess,
    testUnauthorizedAccess,
    testHealthCheck
};
