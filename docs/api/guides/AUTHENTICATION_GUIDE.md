# Authentication Guide

## Overview

The FHIR-AI Backend implements a comprehensive authentication and authorization system that is fully compliant with SMART on FHIR standards. This guide provides detailed information about the authentication mechanisms, API endpoints, and security features.

## Table of Contents

1. [Authentication Architecture](#authentication-architecture)
2. [SMART on FHIR Compliance](#smart-on-fhir-compliance)
3. [API Endpoints](#api-endpoints)
4. [Security Features](#security-features)
5. [User Management](#user-management)
6. [Patient Access Control](#patient-access-control)
7. [Testing](#testing)
8. [Deployment](#deployment)

## Authentication Architecture

### Core Components

The authentication system consists of several key components:

- **SMART on FHIR Authentication Handler**: Custom ASP.NET Core authentication handler
- **JWT Service**: Token generation, validation, and refresh
- **User Service**: User management and credential validation
- **Patient Access Service**: Fine-grained patient data access control
- **Current User Service**: Context-aware user information retrieval

### Authentication Flow

1. **Client Authentication**: Client authenticates via SMART on FHIR or direct login
2. **Token Generation**: System generates JWT token with user claims and scopes
3. **Request Authorization**: Each request is validated against user permissions
4. **Patient Access Verification**: Patient-specific access is checked for data operations
5. **Audit Logging**: All access attempts are logged for compliance

## SMART on FHIR Compliance

### Supported Scopes

The system supports the following FHIR-compliant scopes:

- `system/*` - System-level access (administrators)
- `user/*` - User-level access (healthcare providers)
- `patient/*` - Patient-level access (general patient data)
- `patient/{id}.*` - Specific patient access
- `user/Patient.read` - Read access to Patient resources
- `user/Patient.write` - Write access to Patient resources

### Scope Enforcement

Scopes are enforced at multiple levels:

1. **API Endpoint Level**: Using authorization policies
2. **Resource Level**: Using custom attributes
3. **Data Level**: Using Row-Level Security (RLS)

## API Endpoints

### Authentication Endpoints

#### POST /api/auth/login
Authenticate user and receive JWT token.

**Request Body:**
```json
{
  "username": "doctor@hospital.com",
  "password": "securepassword",
  "tenantId": "hospital-tenant-1"
}
```

**Response:**
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh-token-here",
  "expiresAt": "2024-12-20T10:30:00Z",
  "user": {
    "id": "user-guid",
    "username": "doctor@hospital.com",
    "email": "doctor@hospital.com",
    "role": "HealthcareProvider",
    "firstName": "John",
    "lastName": "Doe",
    "practitionerId": "practitioner-123",
    "scopes": ["patient/*.read", "user/*.read"]
  }
}
```

#### POST /api/auth/refresh
Refresh JWT token using refresh token.

**Request Body:**
```json
{
  "refreshToken": "refresh-token-here"
}
```

#### POST /api/auth/logout
Logout user and revoke session.

**Request Body:**
```json
{
  "sessionToken": "jwt-token-here",
  "reason": "User logout"
}
```

### Patient Access Endpoints

#### POST /api/auth/patient-access/grant
Grant access to patient data for a user.

**Request Body:**
```json
{
  "patientId": "patient-guid",
  "userId": "user-guid",
  "accessLevel": "ReadOnly",
  "expiresAt": "2024-12-27T10:30:00Z",
  "isEmergencyAccess": false,
  "purpose": "Clinical care and treatment"
}
```

#### POST /api/auth/patient-access/revoke
Revoke access to patient data.

**Request Body:**
```json
{
  "accessId": "access-guid",
  "reason": "Access no longer needed"
}
```

#### GET /api/auth/patient-access/{patientId}
Get list of users with access to patient data.

**Query Parameters:**
- `userId` (optional): Filter by specific user
- `accessLevel` (optional): Filter by access level
- `isActive` (optional): Filter by active status
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Page size (default: 20)

## Security Features

### Multi-Tenant Security

- **Tenant Isolation**: All data is isolated by tenant ID
- **Row-Level Security**: Database-level security policies
- **Cross-Tenant Protection**: Prevents unauthorized cross-tenant access

### Password Security

- **SHA256 Hashing**: Secure password hashing
- **Account Locking**: Automatic account locking after failed attempts
- **Password Policies**: Enforced password complexity requirements

### Session Management

- **JWT Tokens**: Secure token-based authentication
- **Refresh Tokens**: Secure token refresh mechanism
- **Session Tracking**: Complete session audit trail
- **Automatic Expiration**: Configurable token expiration

### Audit Trail

- **Access Logging**: All authentication attempts logged
- **Patient Access Logging**: All patient data access logged
- **Compliance Reporting**: HIPAA-compliant audit reports

## User Management

### User Roles

The system supports the following user roles:

1. **SystemAdministrator**: Full system access
2. **HealthcareProvider**: Healthcare provider access
3. **Nurse**: Nursing staff access
4. **Patient**: Patient self-service access
5. **FamilyMember**: Family member access
6. **Researcher**: Research access (with restrictions)

### User Status

Users can have the following statuses:

- **Active**: Normal access
- **Inactive**: Temporarily disabled
- **Locked**: Account locked due to security
- **Pending**: Awaiting activation

### User Scopes

Each user can have multiple FHIR scopes:

- Scopes are granted based on role and requirements
- Scopes can have expiration dates
- Scopes can be revoked at any time
- Emergency access scopes are available

## Patient Access Control

### Access Levels

The system supports the following access levels:

1. **ReadOnly**: Read-only access to patient data
2. **ReadWrite**: Read and write access to patient data
3. **FullAccess**: Complete access to patient data
4. **EmergencyAccess**: Emergency access with justification
5. **ResearchAccess**: Research access with restrictions

### Access Management

- **Granular Control**: Fine-grained access control per patient
- **Time-Limited Access**: Access can expire automatically
- **Emergency Access**: Break-glass access with justification
- **Access Auditing**: Complete audit trail of all access

### Consent Management

- **Patient Consent**: Track patient consent for data sharing
- **Consent Types**: Multiple consent types supported
- **Electronic Consent**: Support for electronic consent
- **Consent Revocation**: Patients can revoke consent

## Testing

### Unit Tests

Run unit tests for authentication components:

```bash
dotnet test tests/HealthTech.Application.Tests/FhirResources/Commands/Authentication/
dotnet test tests/HealthTech.Application.Tests/FhirResources/Commands/PatientAccess/
```

### API Tests

Run API tests for authentication endpoints:

```bash
cd scripts/api
npm install
node test-authentication-api.js
```

### Test Scenarios

The test suite covers:

1. **Valid Login**: Successful authentication
2. **Invalid Login**: Failed authentication attempts
3. **Token Refresh**: JWT token refresh
4. **Logout**: Session termination
5. **Patient Access**: Grant and revoke access
6. **Unauthorized Access**: Security validation
7. **Health Check**: System health verification

## Deployment

### Environment Configuration

Configure authentication settings in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "healthtech-fhir",
    "Audience": "healthtech-clients",
    "ExpirationHours": 1,
    "RefreshTokenExpirationDays": 30
  },
  "Database": {
    "ConnectionString": "your-connection-string"
  },
  "Security": {
    "EnableRowLevelSecurity": true,
    "EnableAuditLogging": true,
    "MaxFailedLoginAttempts": 5,
    "AccountLockoutMinutes": 30
  }
}
```

### Database Migration

Apply database migrations:

```bash
dotnet ef database update --project src/HealthTech.Infrastructure
```

### Security Checklist

Before deployment, ensure:

- [ ] JWT secret key is properly configured
- [ ] Database connection is secure
- [ ] Row-Level Security is enabled
- [ ] Audit logging is configured
- [ ] HTTPS is enabled
- [ ] CORS is properly configured
- [ ] Rate limiting is enabled
- [ ] Security headers are set

### Monitoring

Monitor authentication system:

- **Failed Login Attempts**: Track suspicious activity
- **Token Usage**: Monitor token generation and usage
- **Patient Access**: Audit patient data access
- **System Health**: Monitor authentication service health

## Troubleshooting

### Common Issues

1. **Invalid Token**: Check token expiration and signature
2. **Access Denied**: Verify user permissions and scopes
3. **Database Errors**: Check connection string and permissions
4. **Performance Issues**: Monitor database indexes and queries

### Support

For technical support:

- **Email**: support@healthtech.com
- **Documentation**: [API Documentation](README.md)
- **Issues**: [GitHub Issues](https://github.com/healthtech/fhir-ai-backend/issues)

## Compliance

### HIPAA Compliance

The authentication system is designed to meet HIPAA requirements:

- **Access Controls**: Role-based access control
- **Audit Logging**: Complete audit trail
- **Data Encryption**: Encrypted data transmission
- **User Authentication**: Secure authentication mechanisms

### FHIR Compliance

The system is fully compliant with FHIR standards:

- **SMART on FHIR**: OAuth2/OpenID Connect implementation
- **Scope Management**: FHIR-compliant scope handling
- **Resource Authorization**: Resource-level access control
- **Patient Privacy**: Patient data protection

---

*This guide covers the complete authentication system for the FHIR-AI Backend. For additional information, refer to the API documentation and implementation reports.*
