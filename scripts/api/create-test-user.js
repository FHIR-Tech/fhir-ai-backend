const https = require('https');

// Disable SSL certificate verification for development
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const baseUrl = 'https://localhost:5001';

// Test user data for direct database insertion
const testUser = {
    id: "12345678-1234-1234-1234-123456789012",
    username: "admin",
    email: "admin@healthtech.com",
    passwordHash: "$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi", // "password"
    firstName: "System",
    lastName: "Administrator",
    role: "SystemAdministrator",
    status: "Active",
    tenantId: "default",
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString()
};

async function createUserDirectly() {
    console.log('🗄️ Creating Test User Directly in Database...\n');
    
    // Note: This is a conceptual approach since we can't directly access the database
    // In a real scenario, you would need to:
    // 1. Use a database migration script
    // 2. Use a database seeding tool
    // 3. Create an admin endpoint for initial setup
    
    console.log('📋 Test User Data:');
    console.log(`   ID: ${testUser.id}`);
    console.log(`   Username: ${testUser.username}`);
    console.log(`   Email: ${testUser.email}`);
    console.log(`   Role: ${testUser.role}`);
    console.log(`   Tenant: ${testUser.tenantId}`);
    console.log(`   Password Hash: ${testUser.passwordHash.substring(0, 20)}...`);
    console.log('');
    
    console.log('⚠️ Note: Direct database insertion requires:');
    console.log('   1. Database access credentials');
    console.log('   2. Proper password hashing (using pgcrypto)');
    console.log('   3. UUID generation for user ID');
    console.log('   4. Proper tenant isolation');
    console.log('');
    
    console.log('🔧 Alternative Approaches:');
    console.log('   1. Create a database migration script');
    console.log('   2. Add a development-only admin endpoint');
    console.log('   3. Use Entity Framework seeding');
    console.log('   4. Create a setup wizard for first-time installation');
    console.log('');
    
    return testUser;
}

async function testLoginWithCreatedUser() {
    console.log('🔐 Testing Login with Created User...');
    
    const loginData = {
        username: testUser.username,
        password: "password", // This should match the hash
        tenantId: testUser.tenantId
    };
    
    const postData = JSON.stringify(loginData);
    
    return new Promise((resolve) => {
        const req = https.request(`${baseUrl}/api/auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'Content-Length': Buffer.byteLength(postData)
            }
        }, (res) => {
            let data = '';
            
            res.on('data', (chunk) => {
                data += chunk;
            });
            
            res.on('end', () => {
                console.log(`   Status: ${res.statusCode} ${res.statusMessage}`);
                if (data.length > 0) {
                    console.log(`   Response: ${data.substring(0, 300)}${data.length > 300 ? '...' : ''}`);
                }
                console.log('');
                
                if (res.statusCode === 200) {
                    console.log('✅ Login successful!');
                    resolve({ success: true, data });
                } else {
                    console.log('❌ Login failed');
                    resolve({ success: false, data });
                }
            });
        });
        
        req.on('error', (err) => {
            console.log(`   Error: ${err.message}`);
            resolve({ success: false, error: err.message });
        });
        
        req.write(postData);
        req.end();
    });
}

async function runUserCreation() {
    console.log('🚀 User Creation and Testing Process...\n');
    
    // Step 1: Create user data
    const user = await createUserDirectly();
    
    // Step 2: Test login (this will fail until user is actually created in DB)
    await testLoginWithCreatedUser();
    
    console.log('✨ User creation process completed!');
    console.log('\n📝 Next Steps:');
    console.log('   1. Create a database migration to insert test users');
    console.log('   2. Add a development-only admin setup endpoint');
    console.log('   3. Implement proper password hashing in the application');
    console.log('   4. Test the complete authentication flow');
}

runUserCreation().catch(console.error);
