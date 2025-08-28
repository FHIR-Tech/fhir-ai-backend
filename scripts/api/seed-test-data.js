const https = require('https');

// Disable SSL certificate verification for development
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const baseUrl = 'https://localhost:5001';

// Test user data
const testUsers = [
    {
        username: "admin",
        password: "admin123",
        email: "admin@healthtech.com",
        firstName: "System",
        lastName: "Administrator",
        role: "SystemAdministrator",
        tenantId: "default"
    },
    {
        username: "doctor",
        password: "doctor123",
        email: "doctor@healthtech.com",
        firstName: "John",
        lastName: "Doctor",
        role: "HealthcareProvider",
        tenantId: "default"
    },
    {
        username: "nurse",
        password: "nurse123",
        email: "nurse@healthtech.com",
        firstName: "Jane",
        lastName: "Nurse",
        role: "HealthcareProvider",
        tenantId: "default"
    }
];

async function createUser(userData) {
    return new Promise((resolve) => {
        const url = `${baseUrl}/api/users`;
        const postData = JSON.stringify(userData);
        
        const options = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'Content-Length': Buffer.byteLength(postData)
            }
        };
        
        const req = https.request(url, options, (res) => {
            let data = '';
            
            res.on('data', (chunk) => {
                data += chunk;
            });
            
            res.on('end', () => {
                console.log(`👤 Create User ${userData.username} (${res.statusCode}):`);
                console.log(`   URL: ${url}`);
                console.log(`   Status: ${res.statusCode} ${res.statusMessage}`);
                if (data.length > 0) {
                    console.log(`   Response: ${data.substring(0, 200)}${data.length > 200 ? '...' : ''}`);
                }
                console.log('');
                resolve({ success: res.statusCode === 200 || res.statusCode === 201, statusCode: res.statusCode, data });
            });
        });
        
        req.on('error', (err) => {
            console.log(`❌ Create User ${userData.username} Error:`);
            console.log(`   URL: ${url}`);
            console.log(`   Error: ${err.message}`);
            console.log('');
            resolve({ success: false, error: err.message });
        });
        
        req.setTimeout(5000, () => {
            console.log(`⏰ Create User ${userData.username} Timeout:`);
            console.log(`   URL: ${url}`);
            console.log('');
            req.destroy();
            resolve({ success: false, error: 'Timeout' });
        });
        
        req.write(postData);
        req.end();
    });
}

async function testLoginAfterSeed() {
    return new Promise((resolve) => {
        const url = `${baseUrl}/api/auth/login`;
        const loginData = {
            username: "admin",
            password: "admin123",
            tenantId: "default"
        };
        const postData = JSON.stringify(loginData);
        
        const options = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'Content-Length': Buffer.byteLength(postData)
            }
        };
        
        const req = https.request(url, options, (res) => {
            let data = '';
            
            res.on('data', (chunk) => {
                data += chunk;
            });
            
            res.on('end', () => {
                console.log(`🔐 Test Login After Seed (${res.statusCode}):`);
                console.log(`   URL: ${url}`);
                console.log(`   Status: ${res.statusCode} ${res.statusMessage}`);
                if (data.length > 0) {
                    console.log(`   Response: ${data.substring(0, 300)}${data.length > 300 ? '...' : ''}`);
                }
                console.log('');
                resolve({ success: res.statusCode === 200, statusCode: res.statusCode, data });
            });
        });
        
        req.on('error', (err) => {
            console.log(`❌ Test Login Error:`);
            console.log(`   URL: ${url}`);
            console.log(`   Error: ${err.message}`);
            console.log('');
            resolve({ success: false, error: err.message });
        });
        
        req.setTimeout(5000, () => {
            console.log(`⏰ Test Login Timeout:`);
            console.log(`   URL: ${url}`);
            console.log('');
            req.destroy();
            resolve({ success: false, error: 'Timeout' });
        });
        
        req.write(postData);
        req.end();
    });
}

async function seedDatabase() {
    console.log('🌱 Seeding Database with Test Data...\n');
    
    let successCount = 0;
    
    for (const user of testUsers) {
        const result = await createUser(user);
        if (result.success) {
            successCount++;
        }
    }
    
    console.log(`📊 Seeding Results: ${successCount}/${testUsers.length} users created successfully.\n`);
    
    if (successCount > 0) {
        console.log('🔐 Testing login with seeded data...\n');
        await testLoginAfterSeed();
    }
    
    console.log('✨ Database seeding completed!');
}

seedDatabase().catch(console.error);
