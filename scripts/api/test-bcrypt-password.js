const bcrypt = require('bcrypt');

// The hash from the database
const storedHash = '$2a$06$sa/U2/2OU04MDaAgCZBIyeVW1WMfcgA7rdEH3ZMwp6IWMTpaIRkDK';

// Common passwords to test
const testPasswords = [
    'admin123',
    'admin',
    'password',
    '123456',
    'admin@demo.healthtech.com',
    'demo123',
    'test123',
    'healthtech',
    'fhir123',
    'health123'
];

async function testPassword(password) {
    try {
        const isValid = await bcrypt.compare(password, storedHash);
        console.log(`🔐 Testing: ${password} - ${isValid ? '✅ MATCH' : '❌ NO MATCH'}`);
        return isValid;
    } catch (error) {
        console.log(`❌ Error testing ${password}: ${error.message}`);
        return false;
    }
}

async function runTests() {
    console.log('🔍 Testing bcrypt password verification...\n');
    
    for (const password of testPasswords) {
        const isValid = await testPassword(password);
        if (isValid) {
            console.log(`\n🎉 FOUND MATCHING PASSWORD: ${password}`);
            break;
        }
    }
    
    console.log('\n✨ Password testing completed!');
}

runTests().catch(console.error);
