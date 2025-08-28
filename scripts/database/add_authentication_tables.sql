-- Add missing columns to users table
ALTER TABLE users ADD COLUMN IF NOT EXISTS first_name VARCHAR(255) NOT NULL DEFAULT '';
ALTER TABLE users ADD COLUMN IF NOT EXISTS last_name VARCHAR(255) NOT NULL DEFAULT '';
ALTER TABLE users ADD COLUMN IF NOT EXISTS role INTEGER NOT NULL DEFAULT 0;
ALTER TABLE users ADD COLUMN IF NOT EXISTS status INTEGER NOT NULL DEFAULT 0;
ALTER TABLE users ADD COLUMN IF NOT EXISTS last_login_at TIMESTAMP WITH TIME ZONE;
ALTER TABLE users ADD COLUMN IF NOT EXISTS last_login_ip VARCHAR(45);
ALTER TABLE users ADD COLUMN IF NOT EXISTS failed_login_attempts INTEGER NOT NULL DEFAULT 0;
ALTER TABLE users ADD COLUMN IF NOT EXISTS locked_until TIMESTAMP WITH TIME ZONE;
ALTER TABLE users ADD COLUMN IF NOT EXISTS practitioner_id VARCHAR(255);
ALTER TABLE users ADD COLUMN IF NOT EXISTS created_by TEXT NOT NULL DEFAULT '';
ALTER TABLE users ADD COLUMN IF NOT EXISTS modified_at TIMESTAMP WITH TIME ZONE;
ALTER TABLE users ADD COLUMN IF NOT EXISTS modified_by TEXT;
ALTER TABLE users ADD COLUMN IF NOT EXISTS is_deleted BOOLEAN NOT NULL DEFAULT FALSE;
ALTER TABLE users ADD COLUMN IF NOT EXISTS deleted_at TIMESTAMP WITH TIME ZONE;
ALTER TABLE users ADD COLUMN IF NOT EXISTS deleted_by VARCHAR(255);
ALTER TABLE users ADD COLUMN IF NOT EXISTS version INTEGER NOT NULL DEFAULT 0;

-- Create patients table
CREATE TABLE IF NOT EXISTS patients (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id TEXT NOT NULL,
    fhir_patient_id VARCHAR(255) NOT NULL,
    first_name VARCHAR(255) NOT NULL,
    last_name VARCHAR(255) NOT NULL,
    date_of_birth TIMESTAMP WITH TIME ZONE,
    gender VARCHAR(10) NOT NULL,
    email VARCHAR(255),
    phone VARCHAR(50),
    address TEXT,
    status INTEGER NOT NULL,
    consent_given BOOLEAN NOT NULL,
    consent_date TIMESTAMP WITH TIME ZONE,
    consent_given_by VARCHAR(255),
    emergency_contact_name VARCHAR(255),
    emergency_contact_phone VARCHAR(50),
    emergency_contact_relationship VARCHAR(100),
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by TEXT NOT NULL,
    modified_at TIMESTAMP WITH TIME ZONE,
    modified_by TEXT,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_at TIMESTAMP WITH TIME ZONE,
    deleted_by VARCHAR(255),
    version INTEGER NOT NULL DEFAULT 0
);

-- Create patient_accesses table
CREATE TABLE IF NOT EXISTS patient_accesses (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id TEXT NOT NULL,
    patient_id UUID NOT NULL,
    user_id UUID NOT NULL,
    access_level INTEGER NOT NULL,
    granted_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    expires_at TIMESTAMP WITH TIME ZONE,
    is_emergency_access BOOLEAN NOT NULL DEFAULT FALSE,
    emergency_justification TEXT,
    granted_by TEXT NOT NULL,
    revoked_at TIMESTAMP WITH TIME ZONE,
    revoked_by TEXT,
    revocation_reason TEXT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by TEXT NOT NULL,
    modified_at TIMESTAMP WITH TIME ZONE,
    modified_by TEXT,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_at TIMESTAMP WITH TIME ZONE,
    deleted_by VARCHAR(255),
    version INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (patient_id) REFERENCES patients(id) ON DELETE CASCADE,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

-- Create patient_consents table
CREATE TABLE IF NOT EXISTS patient_consents (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id TEXT NOT NULL,
    patient_id UUID NOT NULL,
    consent_type INTEGER NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    granted_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    expires_at TIMESTAMP WITH TIME ZONE,
    purpose TEXT NOT NULL,
    scope TEXT,
    electronic_consent BOOLEAN NOT NULL DEFAULT FALSE,
    consent_document_url VARCHAR(500),
    witness_name VARCHAR(255),
    witness_signature TEXT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by TEXT NOT NULL,
    modified_at TIMESTAMP WITH TIME ZONE,
    modified_by TEXT,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_at TIMESTAMP WITH TIME ZONE,
    deleted_by VARCHAR(255),
    version INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (patient_id) REFERENCES patients(id) ON DELETE CASCADE
);

-- Create user_scopes table
CREATE TABLE IF NOT EXISTS user_scopes (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id TEXT NOT NULL,
    user_id UUID NOT NULL,
    scope VARCHAR(255) NOT NULL,
    granted_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    expires_at TIMESTAMP WITH TIME ZONE,
    granted_by TEXT NOT NULL,
    is_revoked BOOLEAN NOT NULL DEFAULT FALSE,
    revoked_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by TEXT NOT NULL,
    modified_at TIMESTAMP WITH TIME ZONE,
    modified_by TEXT,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_at TIMESTAMP WITH TIME ZONE,
    deleted_by VARCHAR(255),
    version INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

-- Create user_sessions table
CREATE TABLE IF NOT EXISTS user_sessions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id TEXT NOT NULL,
    user_id UUID NOT NULL,
    session_token VARCHAR(500) NOT NULL,
    refresh_token VARCHAR(500),
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    last_accessed_at TIMESTAMP WITH TIME ZONE,
    created_ip_address VARCHAR(45),
    user_agent TEXT,
    is_revoked BOOLEAN NOT NULL DEFAULT FALSE,
    revoked_at TIMESTAMP WITH TIME ZONE,
    revocation_reason TEXT,
    created_by TEXT NOT NULL,
    modified_at TIMESTAMP WITH TIME ZONE,
    modified_by TEXT,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_at TIMESTAMP WITH TIME ZONE,
    deleted_by VARCHAR(255),
    version INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

-- Create indexes for patients table
CREATE INDEX IF NOT EXISTS ix_patients_tenant_id ON patients(tenant_id);
CREATE INDEX IF NOT EXISTS ix_patients_fhir_patient_id ON patients(fhir_patient_id);
CREATE INDEX IF NOT EXISTS ix_patients_status ON patients(status);

-- Create indexes for patient_accesses table
CREATE INDEX IF NOT EXISTS ix_patient_accesses_tenant_id ON patient_accesses(tenant_id);
CREATE INDEX IF NOT EXISTS ix_patient_accesses_patient_id ON patient_accesses(patient_id);
CREATE INDEX IF NOT EXISTS ix_patient_accesses_user_id ON patient_accesses(user_id);

-- Create indexes for patient_consents table
CREATE INDEX IF NOT EXISTS ix_patient_consents_tenant_id ON patient_consents(tenant_id);
CREATE INDEX IF NOT EXISTS ix_patient_consents_patient_id ON patient_consents(patient_id);
CREATE INDEX IF NOT EXISTS ix_patient_consents_is_active ON patient_consents(is_active);

-- Create indexes for user_scopes table
CREATE INDEX IF NOT EXISTS ix_user_scopes_tenant_id ON user_scopes(tenant_id);
CREATE INDEX IF NOT EXISTS ix_user_scopes_user_id ON user_scopes(user_id);
CREATE INDEX IF NOT EXISTS ix_user_scopes_scope ON user_scopes(scope);

-- Create indexes for user_sessions table
CREATE INDEX IF NOT EXISTS ix_user_sessions_tenant_id ON user_sessions(tenant_id);
CREATE INDEX IF NOT EXISTS ix_user_sessions_user_id ON user_sessions(user_id);
CREATE INDEX IF NOT EXISTS ix_user_sessions_session_token ON user_sessions(session_token);

-- Create indexes for users table
CREATE INDEX IF NOT EXISTS ix_users_role ON users(role);
CREATE INDEX IF NOT EXISTS ix_users_status ON users(status);
CREATE INDEX IF NOT EXISTS ix_users_practitioner_id ON users(practitioner_id);

-- Add Row Level Security (RLS) policies for new tables
ALTER TABLE patients ENABLE ROW LEVEL SECURITY;
ALTER TABLE patient_accesses ENABLE ROW LEVEL SECURITY;
ALTER TABLE patient_consents ENABLE ROW LEVEL SECURITY;
ALTER TABLE user_scopes ENABLE ROW LEVEL SECURITY;
ALTER TABLE user_sessions ENABLE ROW LEVEL SECURITY;

-- Create RLS policies for patients table
CREATE POLICY IF NOT EXISTS patients_tenant_policy ON patients
    FOR ALL
    USING (tenant_id = current_setting('app.current_tenant_id', true)::TEXT);

-- Create RLS policies for patient_accesses table
CREATE POLICY IF NOT EXISTS patient_accesses_tenant_policy ON patient_accesses
    FOR ALL
    USING (tenant_id = current_setting('app.current_tenant_id', true)::TEXT);

-- Create RLS policies for patient_consents table
CREATE POLICY IF NOT EXISTS patient_consents_tenant_policy ON patient_consents
    FOR ALL
    USING (tenant_id = current_setting('app.current_tenant_id', true)::TEXT);

-- Create RLS policies for user_scopes table
CREATE POLICY IF NOT EXISTS user_scopes_tenant_policy ON user_scopes
    FOR ALL
    USING (tenant_id = current_setting('app.current_tenant_id', true)::TEXT);

-- Create RLS policies for user_sessions table
CREATE POLICY IF NOT EXISTS user_sessions_tenant_policy ON user_sessions
    FOR ALL
    USING (tenant_id = current_setting('app.current_tenant_id', true)::TEXT);
