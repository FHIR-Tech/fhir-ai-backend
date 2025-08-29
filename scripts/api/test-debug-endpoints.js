const https = require('https');

// Disable SSL certificate verification for development
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

async function testDebugEndpoints() {
    console.log('🔍 Testing Debug Endpoints\n');
    
    // Test 1: User Lookup
    console.log('='.repeat(60));
    console.log('TEST 1: User Lookup');
    console.log('='.repeat(60));
    
    const userResult = await makeRequest('/api/debug/user/admin', 'GET');
    console.log(`Status: ${userResult.statusCode}`);
    console.log(`Response: ${userResult.data}`);
    console.log('');
    
    if (userResult.statusCode === 200) {
        try {
            const user = JSON.parse(userResult.data);
            console.log('✅ User found successfully');
            console.log(`   ID: ${user.id}`);
            console.log(`   Username: ${user.username}`);
            console.log(`   Email: ${user.email}`);
            console.log(`   Role: ${user.role}`);
            console.log(`   Status: ${user.status}`);
            console.log(`   TenantId: ${user.tenantId}`);
            console.log(`   IsDeleted: ${user.isDeleted}`);
            console.log('');
            
            // Test 2: Password Verification
            console.log('='.repeat(60));
            console.log('TEST 2: Password Verification');
            console.log('='.repeat(60));
            
            const passwordData = {
                username: 'admin',
                password: 'password',
                tenantId: 'default'
            };
            
            const passwordResult = await makeRequest('/api/debug/verify-password', 'POST', passwordData);
            console.log(`Status: ${passwordResult.statusCode}`);
            console.log(`Response: ${passwordResult.data}`);
            console.log('');
            
            if (passwordResult.statusCode === 200) {
                const passwordInfo = JSON.parse(passwordResult.data);
                console.log('✅ Password verification successful');
                console.log(`   Username: ${passwordInfo.username}`);
                console.log(`   Password Hash: ${passwordInfo.passwordHash}`);
                console.log(`   Is Valid: ${passwordInfo.isValid}`);
                console.log('');
                
                // Test 3: User Scopes
                console.log('='.repeat(60));
                console.log('TEST 3: User Scopes');
                console.log('='.repeat(60));
                
                const scopesResult = await makeRequest(`/api/debug/user-scopes/${user.id}`, 'GET');
                console.log(`Status: ${scopesResult.statusCode}`);
                console.log(`Response: ${scopesResult.data}`);
                console.log('');
                
                if (scopesResult.statusCode === 200) {
                    const scopesInfo = JSON.parse(scopesResult.data);
                    console.log('✅ User scopes retrieved successfully');
                    console.log(`   UserId: ${scopesInfo.userId}`);
                    console.log(`   Scopes Count: ${scopesInfo.count}`);
                    console.log(`   Scopes: ${scopesInfo.scopes.join(', ')}`);
                    console.log('');
                    
                    // Test 4: JWT Token Generation
                    console.log('='.repeat(60));
                    console.log('TEST 4: JWT Token Generation');
                    console.log('='.repeat(60));
                    
                    const tokenData = {
                        userId: user.id,
                        username: user.username,
                        email: user.email,
                        role: user.role,
                        tenantId: user.tenantId,
                        scopes: scopesInfo.scopes,
                        practitionerId: null
                    };
                    
                    const tokenResult = await makeRequest('/api/debug/generate-token', 'POST', tokenData);
                    console.log(`Status: ${tokenResult.statusCode}`);
                    console.log(`Response: ${tokenResult.data}`);
                    console.log('');
                    
                    if (tokenResult.statusCode === 200) {
                        const tokenInfo = JSON.parse(tokenResult.data);
                        console.log('✅ JWT token generated successfully');
                        console.log(`   Token Length: ${tokenInfo.length}`);
                        console.log(`   Token: ${tokenInfo.token.substring(0, 50)}...`);
                        console.log('');
                        
                        // Test 5: Refresh Token Creation
                        console.log('='.repeat(60));
                        console.log('TEST 5: Refresh Token Creation');
                        console.log('='.repeat(60));
                        
                        const refreshTokenData = {
                            userId: user.id,
                            tenantId: user.tenantId,
                            ipAddress: '127.0.0.1',
                            userAgent: 'Debug-Test'
                        };
                        
                        const refreshTokenResult = await makeRequest('/api/debug/create-refresh-token', 'POST', refreshTokenData);
                        console.log(`Status: ${refreshTokenResult.statusCode}`);
                        console.log(`Response: ${refreshTokenResult.data}`);
                        console.log('');
                        
                        if (refreshTokenResult.statusCode === 200) {
                            const refreshTokenInfo = JSON.parse(refreshTokenResult.data);
                            console.log('✅ Refresh token created successfully');
                            console.log(`   Refresh Token Length: ${refreshTokenInfo.length}`);
                            console.log(`   Refresh Token: ${refreshTokenInfo.refreshToken.substring(0, 50)}...`);
                            console.log('');
                        } else {
                            console.log('❌ Refresh token creation failed');
                        }
                    } else {
                        console.log('❌ JWT token generation failed');
                    }
                } else {
                    console.log('❌ User scopes retrieval failed');
                }
            } else {
                console.log('❌ Password verification failed');
            }
        } catch (e) {
            console.log('❌ Could not parse user data');
        }
    } else {
        console.log('❌ User lookup failed');
    }
}

async function makeRequest(path, method, data = null) {
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
            options.headers['Content-Type'] = 'application/json';
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

testDebugEndpoints().catch(console.error);

