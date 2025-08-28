-- Add Row Level Security (RLS) policies for new tables
ALTER TABLE patients ENABLE ROW LEVEL SECURITY;
ALTER TABLE patient_accesses ENABLE ROW LEVEL SECURITY;
ALTER TABLE patient_consents ENABLE ROW LEVEL SECURITY;
ALTER TABLE user_scopes ENABLE ROW LEVEL SECURITY;
ALTER TABLE user_sessions ENABLE ROW LEVEL SECURITY;

-- Create RLS policies for patients table
DROP POLICY IF EXISTS patients_tenant_policy ON patients;
CREATE POLICY patients_tenant_policy ON patients
    FOR ALL
    USING (tenant_id = current_setting('app.current_tenant_id', true)::TEXT);

-- Create RLS policies for patient_accesses table
DROP POLICY IF EXISTS patient_accesses_tenant_policy ON patient_accesses;
CREATE POLICY patient_accesses_tenant_policy ON patient_accesses
    FOR ALL
    USING (tenant_id = current_setting('app.current_tenant_id', true)::TEXT);

-- Create RLS policies for patient_consents table
DROP POLICY IF EXISTS patient_consents_tenant_policy ON patient_consents;
CREATE POLICY patient_consents_tenant_policy ON patient_consents
    FOR ALL
    USING (tenant_id = current_setting('app.current_tenant_id', true)::TEXT);

-- Create RLS policies for user_scopes table
DROP POLICY IF EXISTS user_scopes_tenant_policy ON user_scopes;
CREATE POLICY user_scopes_tenant_policy ON user_scopes
    FOR ALL
    USING (tenant_id = current_setting('app.current_tenant_id', true)::TEXT);

-- Create RLS policies for user_sessions table
DROP POLICY IF EXISTS user_sessions_tenant_policy ON user_sessions;
CREATE POLICY user_sessions_tenant_policy ON user_sessions
    FOR ALL
    USING (tenant_id = current_setting('app.current_tenant_id', true)::TEXT);
