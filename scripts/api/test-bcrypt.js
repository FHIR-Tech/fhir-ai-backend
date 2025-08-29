const bcrypt = require('bcrypt');

console.log('🔐 Testing BCrypt Functionality\n');

// Test 1: Hash a password
const testPassword = 'password';
console.log(`Test Password: "${testPassword}"`);

const hash = bcrypt.hashSync(testPassword, 11);
console.log(`Generated Hash: ${hash}`);

// Test 2: Verify the hash
const isValid = bcrypt.compareSync(testPassword, hash);
console.log(`Verification Result: ${isValid ? '✅ SUCCESS' : '❌ FAILED'}`);

// Test 3: Test with the database hash
const dbHash = '$2b$11$01JDQPwZAn3zXklioqfgEOK.zeAPowaAduX0dmaFoXh3A/jYVnTjW';
console.log(`\nDatabase Hash: ${dbHash}`);

const dbIsValid = bcrypt.compareSync(testPassword, dbHash);
console.log(`Database Hash Verification: ${dbIsValid ? '✅ SUCCESS' : '❌ FAILED'}`);

// Test 4: Test with wrong password
const wrongPassword = 'wrongpassword';
const wrongIsValid = bcrypt.compareSync(wrongPassword, dbHash);
console.log(`Wrong Password Verification: ${wrongIsValid ? '❌ SHOULD FAIL' : '✅ CORRECTLY FAILED'}`);

console.log('\n✨ BCrypt testing completed!');
