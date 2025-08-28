using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTech.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthenticationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "deleted_by",
                table: "audit_events",
                newName: "DeletedBy");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "audit_events",
                newName: "DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_AuditEvents_TenantId_UserId_EventTime",
                table: "audit_events",
                newName: "IX_audit_events_TenantId_UserId_EventTime");

            migrationBuilder.RenameIndex(
                name: "IX_AuditEvents_TenantId_EventTime",
                table: "audit_events",
                newName: "IX_audit_events_TenantId_EventTime");

            migrationBuilder.CreateTable(
                name: "patients",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    fhir_patient_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false, defaultValue: "Active"),
                    consent_given = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ConsentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ConsentGivenBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    emergency_contact_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    emergency_contact_phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    emergency_contact_relationship = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    tenant_id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patients", x => x.id);
                    table.CheckConstraint("ck_patients_gender", "gender IN ('Male', 'Female', 'Other', 'Unknown')");
                    table.CheckConstraint("ck_patients_status", "status IN ('Active', 'Inactive', 'Deceased', 'Unknown', 'Transferred', 'Discharged')");
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false, defaultValue: "Active"),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastLoginIp = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    failed_login_attempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    locked_until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    practitioner_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    tenant_id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.CheckConstraint("ck_users_role", "role IN ('SystemAdministrator', 'HealthcareProvider', 'Nurse', 'Patient', 'FamilyMember', 'Researcher', 'ITSupport', 'ReadOnlyUser', 'DataAnalyst', 'ITAdministrator', 'Guest')");
                    table.CheckConstraint("ck_users_status", "status IN ('Active', 'Inactive', 'Locked', 'Suspended', 'Pending', 'Expired', 'PendingVerification', 'Deleted')");
                });

            migrationBuilder.CreateTable(
                name: "patient_consents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    consent_type = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    granted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    granted_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    revoked_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    purpose = table.Column<string>(type: "text", nullable: false),
                    details = table.Column<string>(type: "text", nullable: true),
                    is_electronic_consent = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    consent_ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    tenant_id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient_consents", x => x.id);
                    table.CheckConstraint("ck_patient_consents_consent_type", "consent_type IN ('DataSharing', 'ResearchParticipation', 'EmergencyAccess', 'FamilyAccess', 'ThirdPartyAccess', 'MarketingCommunications', 'AutomatedDecisionMaking', 'DataPortability', 'DataRetention', 'TreatmentConsent')");
                    table.ForeignKey(
                        name: "FK_patient_consents_patients_patient_id",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "patient_accesses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    access_level = table.Column<string>(type: "text", nullable: false),
                    granted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    granted_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    reason = table.Column<string>(type: "text", nullable: true),
                    is_emergency_access = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    emergency_justification = table.Column<string>(type: "text", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    tenant_id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient_accesses", x => x.id);
                    table.CheckConstraint("ck_patient_accesses_access_level", "access_level IN ('Read', 'Write', 'Admin', 'Emergency', 'Research', 'Analytics')");
                    table.ForeignKey(
                        name: "FK_patient_accesses_patients_patient_id",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_patient_accesses_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_scopes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scope = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    granted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    granted_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_scopes", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_scopes_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    refresh_token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_accessed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    revocation_reason = table.Column<string>(type: "text", nullable: true),
                    tenant_id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_sessions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_patient_accesses_access_level",
                table: "patient_accesses",
                column: "access_level");

            migrationBuilder.CreateIndex(
                name: "idx_patient_accesses_emergency",
                table: "patient_accesses",
                column: "is_emergency_access");

            migrationBuilder.CreateIndex(
                name: "idx_patient_accesses_expires_at",
                table: "patient_accesses",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "idx_patient_accesses_granted_at",
                table: "patient_accesses",
                column: "granted_at");

            migrationBuilder.CreateIndex(
                name: "idx_patient_accesses_patient_id",
                table: "patient_accesses",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "idx_patient_accesses_tenant_id",
                table: "patient_accesses",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "idx_patient_accesses_user_id",
                table: "patient_accesses",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "uk_patient_accesses_unique",
                table: "patient_accesses",
                columns: new[] { "patient_id", "user_id", "access_level" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_patient_consents_active",
                table: "patient_consents",
                columns: new[] { "patient_id", "consent_type", "is_active" });

            migrationBuilder.CreateIndex(
                name: "idx_patient_consents_consent_type",
                table: "patient_consents",
                column: "consent_type");

            migrationBuilder.CreateIndex(
                name: "idx_patient_consents_expires_at",
                table: "patient_consents",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "idx_patient_consents_granted_at",
                table: "patient_consents",
                column: "granted_at");

            migrationBuilder.CreateIndex(
                name: "idx_patient_consents_is_active",
                table: "patient_consents",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "idx_patient_consents_patient_id",
                table: "patient_consents",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "idx_patient_consents_tenant_id",
                table: "patient_consents",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "idx_patients_consent",
                table: "patients",
                column: "consent_given");

            migrationBuilder.CreateIndex(
                name: "idx_patients_fhir_patient_id",
                table: "patients",
                column: "fhir_patient_id");

            migrationBuilder.CreateIndex(
                name: "idx_patients_name",
                table: "patients",
                columns: new[] { "last_name", "first_name" });

            migrationBuilder.CreateIndex(
                name: "idx_patients_status",
                table: "patients",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_patients_tenant_id",
                table: "patients",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "uk_patients_tenant_fhir_id",
                table: "patients",
                columns: new[] { "tenant_id", "fhir_patient_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_user_scopes_expires_at",
                table: "user_scopes",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "idx_user_scopes_granted_at",
                table: "user_scopes",
                column: "granted_at");

            migrationBuilder.CreateIndex(
                name: "idx_user_scopes_is_revoked",
                table: "user_scopes",
                column: "is_revoked");

            migrationBuilder.CreateIndex(
                name: "idx_user_scopes_scope",
                table: "user_scopes",
                column: "scope");

            migrationBuilder.CreateIndex(
                name: "idx_user_scopes_tenant_id",
                table: "user_scopes",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "idx_user_scopes_user_id",
                table: "user_scopes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "uk_user_scopes_unique",
                table: "user_scopes",
                columns: new[] { "user_id", "scope" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_user_sessions_expires_at",
                table: "user_sessions",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "idx_user_sessions_is_revoked",
                table: "user_sessions",
                column: "is_revoked");

            migrationBuilder.CreateIndex(
                name: "idx_user_sessions_last_accessed_at",
                table: "user_sessions",
                column: "last_accessed_at");

            migrationBuilder.CreateIndex(
                name: "idx_user_sessions_refresh_token",
                table: "user_sessions",
                column: "refresh_token");

            migrationBuilder.CreateIndex(
                name: "idx_user_sessions_tenant_id",
                table: "user_sessions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "idx_user_sessions_user_id",
                table: "user_sessions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "uk_user_sessions_session_token",
                table: "user_sessions",
                column: "session_token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_users_email",
                table: "users",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "idx_users_practitioner_id",
                table: "users",
                column: "practitioner_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_users_role",
                table: "users",
                column: "role");

            migrationBuilder.CreateIndex(
                name: "idx_users_status",
                table: "users",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_users_tenant_id",
                table: "users",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "idx_users_username",
                table: "users",
                column: "username");

            migrationBuilder.CreateIndex(
                name: "uk_users_tenant_email",
                table: "users",
                columns: new[] { "tenant_id", "email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_users_tenant_username",
                table: "users",
                columns: new[] { "tenant_id", "username" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "patient_accesses");

            migrationBuilder.DropTable(
                name: "patient_consents");

            migrationBuilder.DropTable(
                name: "user_scopes");

            migrationBuilder.DropTable(
                name: "user_sessions");

            migrationBuilder.DropTable(
                name: "patients");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.RenameColumn(
                name: "DeletedBy",
                table: "audit_events",
                newName: "deleted_by");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "audit_events",
                newName: "deleted_at");

            migrationBuilder.RenameIndex(
                name: "IX_audit_events_TenantId_UserId_EventTime",
                table: "audit_events",
                newName: "IX_AuditEvents_TenantId_UserId_EventTime");

            migrationBuilder.RenameIndex(
                name: "IX_audit_events_TenantId_EventTime",
                table: "audit_events",
                newName: "IX_AuditEvents_TenantId_EventTime");
        }
    }
}
