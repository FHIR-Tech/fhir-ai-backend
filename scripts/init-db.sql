-- Initialize FHIR-AI Database with Row Level Security

-- Enable required extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Create tenant table for multi-tenancy
CREATE TABLE IF NOT EXISTS tenants (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    domain VARCHAR(255) UNIQUE NOT NULL,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Create users table
CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL REFERENCES tenants(id),
    username VARCHAR(255) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(tenant_id, username)
);

-- Create FHIR resources table with JSONB
CREATE TABLE IF NOT EXISTS fhir_resources (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id VARCHAR(255) NOT NULL,
    resource_type VARCHAR(100) NOT NULL,
    fhir_id VARCHAR(255) NOT NULL,
    version_id INTEGER NOT NULL DEFAULT 1,
    resource_json JSONB NOT NULL,
    status VARCHAR(50),
    last_updated TIMESTAMP WITH TIME ZONE,
    search_parameters JSONB,
    tags JSONB,
    security_labels JSONB,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(255) NOT NULL,
    modified_at TIMESTAMP WITH TIME ZONE,
    modified_by VARCHAR(255),
    is_deleted BOOLEAN DEFAULT false,
    version INTEGER DEFAULT 1,
    UNIQUE(tenant_id, resource_type, fhir_id, version_id)
);

-- Create audit events table
CREATE TABLE IF NOT EXISTS audit_events (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id VARCHAR(255) NOT NULL,
    event_type VARCHAR(100) NOT NULL,
    event_subtype VARCHAR(100),
    action VARCHAR(10) NOT NULL,
    outcome INTEGER NOT NULL,
    description TEXT,
    user_id VARCHAR(255) NOT NULL,
    user_display_name VARCHAR(255),
    user_ip_address VARCHAR(45),
    resource_type VARCHAR(100),
    resource_id VARCHAR(255),
    event_data JSONB,
    event_time TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(255) NOT NULL,
    modified_at TIMESTAMP WITH TIME ZONE,
    modified_by VARCHAR(255),
    is_deleted BOOLEAN DEFAULT false,
    version INTEGER DEFAULT 1
);

-- Create indexes for performance
CREATE INDEX IF NOT EXISTS idx_fhir_resources_tenant_type ON fhir_resources(tenant_id, resource_type);
CREATE INDEX IF NOT EXISTS idx_fhir_resources_tenant_id ON fhir_resources(tenant_id, resource_type, fhir_id);
CREATE INDEX IF NOT EXISTS idx_fhir_resources_search_params ON fhir_resources USING GIN(search_parameters);
CREATE INDEX IF NOT EXISTS idx_fhir_resources_tags ON fhir_resources USING GIN(tags);
CREATE INDEX IF NOT EXISTS idx_fhir_resources_security_labels ON fhir_resources USING GIN(security_labels);
CREATE INDEX IF NOT EXISTS idx_fhir_resources_last_updated ON fhir_resources(tenant_id, last_updated DESC);

CREATE INDEX IF NOT EXISTS idx_audit_events_tenant_time ON audit_events(tenant_id, event_time DESC);
CREATE INDEX IF NOT EXISTS idx_audit_events_tenant_user ON audit_events(tenant_id, user_id, event_time DESC);
CREATE INDEX IF NOT EXISTS idx_audit_events_resource ON audit_events(tenant_id, resource_type, resource_id, event_time DESC);

-- Enable Row Level Security
ALTER TABLE fhir_resources ENABLE ROW LEVEL SECURITY;
ALTER TABLE audit_events ENABLE ROW LEVEL SECURITY;

-- Create RLS policies for fhir_resources
CREATE POLICY fhir_resources_tenant_policy ON fhir_resources
    FOR ALL
    USING (tenant_id = current_setting('app.current_tenant_id', true)::VARCHAR(255));

-- Create RLS policies for audit_events
CREATE POLICY audit_events_tenant_policy ON audit_events
    FOR ALL
    USING (tenant_id = current_setting('app.current_tenant_id', true)::VARCHAR(255));

-- Create function to set current tenant
CREATE OR REPLACE FUNCTION set_current_tenant(tenant_id VARCHAR(255))
RETURNS VOID AS $$
BEGIN
    PERFORM set_config('app.current_tenant_id', tenant_id, false);
END;
$$ LANGUAGE plpgsql;

-- Create function to get current tenant
CREATE OR REPLACE FUNCTION get_current_tenant()
RETURNS VARCHAR(255) AS $$
BEGIN
    RETURN current_setting('app.current_tenant_id', true);
END;
$$ LANGUAGE plpgsql;

-- Insert sample tenant
INSERT INTO tenants (name, domain) VALUES 
('HealthTech Demo', 'demo.healthtech.com')
ON CONFLICT (domain) DO NOTHING;

-- Insert sample user
INSERT INTO users (tenant_id, username, email, password_hash) 
SELECT 
    t.id,
    'admin',
    'admin@demo.healthtech.com',
    crypt('admin123', gen_salt('bf'))
FROM tenants t 
WHERE t.domain = 'demo.healthtech.com'
ON CONFLICT (tenant_id, username) DO NOTHING;

-- Grant permissions
GRANT USAGE ON SCHEMA public TO postgres;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO postgres;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO postgres;
