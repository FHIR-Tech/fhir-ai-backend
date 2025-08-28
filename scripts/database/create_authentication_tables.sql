-- =====================================================
-- Authentication System Database Schema
-- Phase 4: Database Schema & Security
-- =====================================================

-- Enable required extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- =====================================================
-- Users Table
-- =====================================================
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    username VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(50) NOT NULL CHECK (role IN ('SystemAdministrator', 'HealthcareProvider', 'Nurse', 'Patient', 'FamilyMember', 'Researcher')),
    status VARCHAR(20) NOT NULL DEFAULT 'Active' CHECK (status IN ('Active', 'Inactive', 'Locked', 'Pending')),
    first_name VARCHAR(100),
    last_name VARCHAR(100),
    practitioner_id VARCHAR(100),
    last_login_at TIMESTAMP WITH TIME ZONE,
    failed_login_attempts INTEGER DEFAULT 0,
    locked_until TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by UUID,
    updated_by UUID,
    
    -- Constraints
    CONSTRAINT uk_users_tenant_username UNIQUE (tenant_id, username),
    CONSTRAINT uk_users_tenant_email UNIQUE (tenant_id, email),
    CONSTRAINT uk_users_practitioner_id UNIQUE (practitioner_id)
);

-- =====================================================
-- Patients Table
-- =====================================================
CREATE TABLE patients (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    fhir_patient_id VARCHAR(100) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    date_of_birth DATE NOT NULL,
    gender VARCHAR(10) CHECK (gender IN ('Male', 'Female', 'Other', 'Unknown')),
    status VARCHAR(20) NOT NULL DEFAULT 'Active' CHECK (status IN ('Active', 'Inactive', 'Deceased', 'Unknown')),
    consent_given BOOLEAN DEFAULT FALSE,
    emergency_contact_name VARCHAR(200),
    emergency_contact_phone VARCHAR(50),
    emergency_contact_relationship VARCHAR(100),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by UUID,
    updated_by UUID,
    
    -- Constraints
    CONSTRAINT uk_patients_tenant_fhir_id UNIQUE (tenant_id, fhir_patient_id),
    CONSTRAINT fk_patients_created_by FOREIGN KEY (created_by) REFERENCES users(id),
    CONSTRAINT fk_patients_updated_by FOREIGN KEY (updated_by) REFERENCES users(id)
);

-- =====================================================
-- Patient Access Table
-- =====================================================
CREATE TABLE patient_accesses (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    patient_id UUID NOT NULL,
    user_id UUID NOT NULL,
    access_level VARCHAR(20) NOT NULL CHECK (access_level IN ('ReadOnly', 'ReadWrite', 'FullAccess', 'EmergencyAccess', 'ResearchAccess')),
    granted_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    expires_at TIMESTAMP WITH TIME ZONE,
    is_emergency_access BOOLEAN DEFAULT FALSE,
    emergency_justification TEXT,
    granted_by UUID NOT NULL,
    revoked_at TIMESTAMP WITH TIME ZONE,
    revoked_by UUID,
    revocation_reason TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    -- Constraints
    CONSTRAINT fk_patient_accesses_patient_id FOREIGN KEY (patient_id) REFERENCES patients(id) ON DELETE CASCADE,
    CONSTRAINT fk_patient_accesses_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_patient_accesses_granted_by FOREIGN KEY (granted_by) REFERENCES users(id),
    CONSTRAINT fk_patient_accesses_revoked_by FOREIGN KEY (revoked_by) REFERENCES users(id),
    CONSTRAINT uk_patient_accesses_unique UNIQUE (patient_id, user_id, access_level)
);

-- =====================================================
-- Patient Consent Table
-- =====================================================
CREATE TABLE patient_consents (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    patient_id UUID NOT NULL,
    consent_type VARCHAR(50) NOT NULL CHECK (consent_type IN ('DataSharing', 'ResearchParticipation', 'EmergencyAccess', 'FamilyAccess', 'Marketing', 'ThirdPartyAccess')),
    is_active BOOLEAN DEFAULT TRUE,
    granted_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    revoked_at TIMESTAMP WITH TIME ZONE,
    purpose TEXT NOT NULL,
    scope TEXT,
    electronic_consent BOOLEAN DEFAULT FALSE,
    consent_document_url VARCHAR(500),
    witness_name VARCHAR(200),
    witness_signature TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by UUID,
    updated_by UUID,
    
    -- Constraints
    CONSTRAINT fk_patient_consents_patient_id FOREIGN KEY (patient_id) REFERENCES patients(id) ON DELETE CASCADE,
    CONSTRAINT fk_patient_consents_created_by FOREIGN KEY (created_by) REFERENCES users(id),
    CONSTRAINT fk_patient_consents_updated_by FOREIGN KEY (updated_by) REFERENCES users(id)
);

-- =====================================================
-- User Scopes Table
-- =====================================================
CREATE TABLE user_scopes (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    user_id UUID NOT NULL,
    scope VARCHAR(200) NOT NULL,
    granted_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    expires_at TIMESTAMP WITH TIME ZONE,
    granted_by UUID NOT NULL,
    revoked_at TIMESTAMP WITH TIME ZONE,
    revoked_by UUID,
    revocation_reason TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    -- Constraints
    CONSTRAINT fk_user_scopes_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_user_scopes_granted_by FOREIGN KEY (granted_by) REFERENCES users(id),
    CONSTRAINT fk_user_scopes_revoked_by FOREIGN KEY (revoked_by) REFERENCES users(id),
    CONSTRAINT uk_user_scopes_unique UNIQUE (user_id, scope)
);

-- =====================================================
-- User Sessions Table
-- =====================================================
CREATE TABLE user_sessions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    user_id UUID NOT NULL,
    session_token VARCHAR(500) NOT NULL,
    refresh_token VARCHAR(500),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    ip_address INET,
    user_agent TEXT,
    is_active BOOLEAN DEFAULT TRUE,
    revoked_at TIMESTAMP WITH TIME ZONE,
    revocation_reason TEXT,
    
    -- Constraints
    CONSTRAINT fk_user_sessions_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT uk_user_sessions_token UNIQUE (session_token)
);

-- =====================================================
-- Indexes for Performance
-- =====================================================

-- Users indexes
CREATE INDEX idx_users_tenant_id ON users(tenant_id);
CREATE INDEX idx_users_username ON users(username);
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_role ON users(role);
CREATE INDEX idx_users_status ON users(status);
CREATE INDEX idx_users_practitioner_id ON users(practitioner_id);

-- Patients indexes
CREATE INDEX idx_patients_tenant_id ON patients(tenant_id);
CREATE INDEX idx_patients_fhir_patient_id ON patients(fhir_patient_id);
CREATE INDEX idx_patients_name ON patients(last_name, first_name);
CREATE INDEX idx_patients_status ON patients(status);
CREATE INDEX idx_patients_consent ON patients(consent_given);

-- Patient Access indexes
CREATE INDEX idx_patient_accesses_tenant_id ON patient_accesses(tenant_id);
CREATE INDEX idx_patient_accesses_patient_id ON patient_accesses(patient_id);
CREATE INDEX idx_patient_accesses_user_id ON patient_accesses(user_id);
CREATE INDEX idx_patient_accesses_access_level ON patient_accesses(access_level);
CREATE INDEX idx_patient_accesses_granted_at ON patient_accesses(granted_at);
CREATE INDEX idx_patient_accesses_expires_at ON patient_accesses(expires_at);
CREATE INDEX idx_patient_accesses_emergency ON patient_accesses(is_emergency_access);

-- Patient Consent indexes
CREATE INDEX idx_patient_consents_tenant_id ON patient_consents(tenant_id);
CREATE INDEX idx_patient_consents_patient_id ON patient_consents(patient_id);
CREATE INDEX idx_patient_consents_consent_type ON patient_consents(consent_type);
CREATE INDEX idx_patient_consents_is_active ON patient_consents(is_active);
CREATE INDEX idx_patient_consents_granted_at ON patient_consents(granted_at);

-- User Scopes indexes
CREATE INDEX idx_user_scopes_tenant_id ON user_scopes(tenant_id);
CREATE INDEX idx_user_scopes_user_id ON user_scopes(user_id);
CREATE INDEX idx_user_scopes_scope ON user_scopes(scope);
CREATE INDEX idx_user_scopes_granted_at ON user_scopes(granted_at);
CREATE INDEX idx_user_scopes_expires_at ON user_scopes(expires_at);

-- User Sessions indexes
CREATE INDEX idx_user_sessions_tenant_id ON user_sessions(tenant_id);
CREATE INDEX idx_user_sessions_user_id ON user_sessions(user_id);
CREATE INDEX idx_user_sessions_expires_at ON user_sessions(expires_at);
CREATE INDEX idx_user_sessions_is_active ON user_sessions(is_active);

-- =====================================================
-- Row Level Security (RLS) Policies
-- =====================================================

-- Enable RLS on all tables
ALTER TABLE users ENABLE ROW LEVEL SECURITY;
ALTER TABLE patients ENABLE ROW LEVEL SECURITY;
ALTER TABLE patient_accesses ENABLE ROW LEVEL SECURITY;
ALTER TABLE patient_consents ENABLE ROW LEVEL SECURITY;
ALTER TABLE user_scopes ENABLE ROW LEVEL SECURITY;
ALTER TABLE user_sessions ENABLE ROW LEVEL SECURITY;

-- Users RLS Policy
CREATE POLICY users_tenant_isolation ON users
    FOR ALL
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

-- Patients RLS Policy
CREATE POLICY patients_tenant_isolation ON patients
    FOR ALL
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

-- Patient Access RLS Policy
CREATE POLICY patient_accesses_tenant_isolation ON patient_accesses
    FOR ALL
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

-- Patient Consent RLS Policy
CREATE POLICY patient_consents_tenant_isolation ON patient_consents
    FOR ALL
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

-- User Scopes RLS Policy
CREATE POLICY user_scopes_tenant_isolation ON user_scopes
    FOR ALL
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

-- User Sessions RLS Policy
CREATE POLICY user_sessions_tenant_isolation ON user_sessions
    FOR ALL
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

-- =====================================================
-- Triggers for Audit Trail
-- =====================================================

-- Function to update updated_at timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Create triggers for updated_at
CREATE TRIGGER update_users_updated_at BEFORE UPDATE ON users
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_patients_updated_at BEFORE UPDATE ON patients
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_patient_accesses_updated_at BEFORE UPDATE ON patient_accesses
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_patient_consents_updated_at BEFORE UPDATE ON patient_consents
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_user_scopes_updated_at BEFORE UPDATE ON user_scopes
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- =====================================================
-- Sample Data for Testing
-- =====================================================

-- Insert sample tenant (for testing purposes)
-- Note: In production, this would come from your tenant management system
INSERT INTO users (id, tenant_id, username, email, password_hash, role, status, first_name, last_name)
VALUES (
    uuid_generate_v4(),
    uuid_generate_v4(),
    'admin',
    'admin@healthtech.com',
    crypt('admin123', gen_salt('bf')),
    'SystemAdministrator',
    'Active',
    'System',
    'Administrator'
);

-- =====================================================
-- Comments
-- =====================================================

COMMENT ON TABLE users IS 'User accounts for the FHIR-AI system';
COMMENT ON TABLE patients IS 'Patient information with consent tracking';
COMMENT ON TABLE patient_accesses IS 'Access control for patient data';
COMMENT ON TABLE patient_consents IS 'Patient consent management and tracking';
COMMENT ON TABLE user_scopes IS 'User permission scopes for FHIR resources';
COMMENT ON TABLE user_sessions IS 'User login sessions and tokens';

COMMENT ON COLUMN users.password_hash IS 'SHA256 hashed password';
COMMENT ON COLUMN users.practitioner_id IS 'FHIR Practitioner resource ID';
COMMENT ON COLUMN patients.fhir_patient_id IS 'FHIR Patient resource ID';
COMMENT ON COLUMN patient_accesses.access_level IS 'Level of access granted to user for patient data';
COMMENT ON COLUMN user_scopes.scope IS 'FHIR scope (e.g., patient/*.read, user/*.write)';
COMMENT ON COLUMN user_sessions.session_token IS 'JWT session token';
COMMENT ON COLUMN user_sessions.refresh_token IS 'JWT refresh token';
