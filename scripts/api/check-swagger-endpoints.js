const https = require('https');

// Disable SSL certificate verification for development
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const baseUrl = 'https://localhost:5001';

async function getSwaggerDocumentation() {
    return new Promise((resolve) => {
        const url = `${baseUrl}/swagger/v1/swagger.json`;
        
        const req = https.get(url, (res) => {
            let data = '';
            
            res.on('data', (chunk) => {
                data += chunk;
            });
            
            res.on('end', () => {
                if (res.statusCode === 200) {
                    try {
                        const swagger = JSON.parse(data);
                        console.log('📖 Swagger Documentation Analysis:\n');
                        
                        console.log('🔗 Available Endpoints:');
                        if (swagger.paths) {
                            Object.keys(swagger.paths).forEach(path => {
                                const pathObj = swagger.paths[path];
                                Object.keys(pathObj).forEach(method => {
                                    const operation = pathObj[method];
                                    console.log(`   ${method.toUpperCase()} ${path} - ${operation.summary || operation.operationId || 'No description'}`);
                                });
                            });
                        }
                        
                        console.log('\n📋 API Information:');
                        console.log(`   Title: ${swagger.info?.title || 'N/A'}`);
                        console.log(`   Version: ${swagger.info?.version || 'N/A'}`);
                        console.log(`   Description: ${swagger.info?.description?.substring(0, 100) || 'N/A'}...`);
                        
                        console.log('\n🔐 Security Schemes:');
                        if (swagger.components?.securitySchemes) {
                            Object.keys(swagger.components.securitySchemes).forEach(scheme => {
                                console.log(`   ${scheme}: ${swagger.components.securitySchemes[scheme].type || 'N/A'}`);
                            });
                        }
                        
                        resolve({ success: true, swagger });
                    } catch (e) {
                        console.log('❌ Failed to parse Swagger JSON:', e.message);
                        resolve({ success: false, error: 'Invalid JSON' });
                    }
                } else {
                    console.log(`❌ Failed to get Swagger documentation: ${res.statusCode}`);
                    resolve({ success: false, statusCode: res.statusCode });
                }
            });
        });
        
        req.on('error', (err) => {
            console.log(`❌ Error getting Swagger documentation: ${err.message}`);
            resolve({ success: false, error: err.message });
        });
        
        req.setTimeout(10000, () => {
            console.log('⏰ Timeout getting Swagger documentation');
            req.destroy();
            resolve({ success: false, error: 'Timeout' });
        });
    });
}

async function checkSwaggerUI() {
    console.log('🔍 Checking Swagger Documentation...\n');
    
    const result = await getSwaggerDocumentation();
    
    if (result.success) {
        console.log('\n✨ Swagger documentation loaded successfully!');
        console.log('\n🌐 You can view the full Swagger UI at:');
        console.log(`   https://localhost:5001/index.html`);
    } else {
        console.log('\n❌ Failed to load Swagger documentation');
    }
}

checkSwaggerUI().catch(console.error);
