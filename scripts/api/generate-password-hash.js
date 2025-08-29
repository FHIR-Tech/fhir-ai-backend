const bcrypt = require('bcrypt');

console.log('🔐 Generating Fresh Password Hash\n');

const password = 'password';
const saltRounds = 11;

console.log(`Password: "${password}"`);
console.log(`Salt Rounds: ${saltRounds}`);

const hash = bcrypt.hashSync(password, saltRounds);
console.log(`Generated Hash: ${hash}`);

// Verify the hash works
const isValid = bcrypt.compareSync(password, hash);
console.log(`Verification: ${isValid ? '✅ SUCCESS' : '❌ FAILED'}`);

console.log('\n📝 SQL Command to update user:');
console.log(`UPDATE users SET password_hash = '${hash}' WHERE username = 'admin' AND tenant_id = 'default';`);

console.log('\n✨ Hash generation completed!');
