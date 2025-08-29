const bcrypt = require('bcrypt');

// The hash from the database
const hash = '$2b$11$01JDQPwZAn3zXklioqfgEOK.zeAPowaAduX0dmaFoXh3A/jYVnTjW';

// Common passwords to test
const passwords = [
    'admin123',
    'password',
    'admin',
    '123456',
    'password123',
    'admin@123',
    'Admin123',
    'P@ssw0rd',
    'test123',
    'demo123'
];

console.log('🔐 Testing BCrypt Password Verification\n');
console.log(`Hash: ${hash}\n`);

for (const password of passwords) {
    const isValid = bcrypt.compareSync(password, hash);
    console.log(`${isValid ? '✅' : '❌'} "${password}" -> ${isValid ? 'MATCH' : 'NO MATCH'}`);
}

console.log('\n✨ Password testing completed!');
