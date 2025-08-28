using HealthTech.Application.Common.Interfaces;
using HealthTech.Application.PatientAccess.Queries;
using HealthTech.Domain.Entities;
using HealthTech.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HealthTech.Infrastructure.Common.Services;

/// <summary>
/// Patient access service implementation
/// </summary>
public class PatientAccessService : IPatientAccessService
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<PatientAccessService> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Application database context</param>
    /// <param name="logger">Logger</param>
    public PatientAccessService(IApplicationDbContext context, ILogger<PatientAccessService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Check if user can access patient data
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="patientId">Patient ID</param>
    /// <param name="scope">Required scope</param>
    /// <returns>True if access is allowed, false otherwise</returns>
    public async Task<bool> CanAccessPatientAsync(string userId, string patientId, string scope)
    {
        // Check if user has system-level access
        if (scope.StartsWith("system/"))
            return true;

        // Check if user has patient-level access
        if (scope.StartsWith("patient/"))
        {
            var hasAccess = await _context.PatientAccesses
                .Include(pa => pa.Patient)
                .FirstOrDefaultAsync(pa => 
                    pa.UserId.ToString() == userId && 
                    pa.Patient.FhirPatientId == patientId &&
                    pa.IsActive);

            return hasAccess != null;
        }

        // Check specific patient access
        if (scope.StartsWith($"patient/{patientId}."))
        {
            var hasAccess = await _context.PatientAccesses
                .Include(pa => pa.Patient)
                .FirstOrDefaultAsync(pa => 
                    pa.UserId.ToString() == userId && 
                    pa.Patient.FhirPatientId == patientId &&
                    pa.AccessLevel >= PatientAccessLevel.Read &&
                    pa.IsActive);

            return hasAccess != null;
        }

        return false;
    }

    /// <summary>
    /// Get all patients that user can access
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Collection of accessible patient IDs</returns>
    public async Task<IEnumerable<string>> GetAccessiblePatientsAsync(string userId)
    {
        return await _context.PatientAccesses
            .Include(pa => pa.Patient)
            .Where(pa => pa.UserId.ToString() == userId && pa.IsActive)
            .Select(pa => pa.Patient.FhirPatientId)
            .Distinct()
            .ToListAsync();
    }

    /// <summary>
    /// Grant access to patient for user
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <param name="userId">User ID</param>
    /// <param name="accessLevel">Access level</param>
    /// <param name="grantedBy">User who granted access</param>
    /// <param name="reason">Reason for granting access</param>
    /// <param name="expiresAt">When access expires (null = no expiration)</param>
    /// <param name="isEmergencyAccess">Whether this is emergency access</param>
    /// <param name="emergencyJustification">Emergency justification (required if emergency access)</param>
    /// <returns>Created patient access</returns>
    public async Task<PatientAccess> GrantPatientAccessAsync(
        string patientId,
        string userId,
        PatientAccessLevel accessLevel,
        string grantedBy,
        string? reason = null,
        DateTime? expiresAt = null,
        bool isEmergencyAccess = false,
        string? emergencyJustification = null)
    {
        // Find patient by FHIR ID
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.FhirPatientId == patientId);
        if (patient == null)
            throw new ArgumentException($"Patient with FHIR ID {patientId} not found", nameof(patientId));

        // Check if access already exists
        var existingAccess = await _context.PatientAccesses
            .FirstOrDefaultAsync(pa => 
                pa.PatientId == patient.Id && 
                pa.UserId.ToString() == userId &&
                pa.IsActive);

        if (existingAccess != null)
        {
            // Update existing access
            existingAccess.AccessLevel = accessLevel;
            existingAccess.ExpiresAt = expiresAt;
            existingAccess.IsEmergencyAccess = isEmergencyAccess;
            existingAccess.EmergencyJustification = emergencyJustification;
            existingAccess.ModifiedAt = DateTime.UtcNow;
            existingAccess.ModifiedBy = grantedBy;

            await _context.SaveChangesAsync(CancellationToken.None);
            
            _logger.LogInformation("Updated patient access for user {UserId} to patient {PatientId}", userId, patientId);
            return existingAccess;
        }

        // Create new access
        var patientAccess = new PatientAccess
        {
            PatientId = patient.Id,
            UserId = Guid.Parse(userId),
            AccessLevel = accessLevel,
            GrantedBy = grantedBy,
            Reason = reason,
            ExpiresAt = expiresAt,
            IsEmergencyAccess = isEmergencyAccess,
            EmergencyJustification = emergencyJustification,
            TenantId = "default" // TODO: Get from context
        };

        _context.PatientAccesses.Add(patientAccess);
        await _context.SaveChangesAsync(CancellationToken.None);
        
        _logger.LogInformation("Granted {AccessLevel} access to user {UserId} for patient {PatientId}", accessLevel, userId, patientId);
        return patientAccess;
    }

    /// <summary>
    /// Revoke access to patient for user
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <param name="userId">User ID</param>
    /// <param name="revokedBy">User who revoked access</param>
    /// <param name="reason">Reason for revocation</param>
    /// <returns>True if revoked, false otherwise</returns>
    public async Task<bool> RevokePatientAccessAsync(string patientId, string userId, string revokedBy, string? reason = null)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.FhirPatientId == patientId);
        if (patient == null)
            return false;

        var patientAccess = await _context.PatientAccesses
            .FirstOrDefaultAsync(pa => 
                pa.PatientId == patient.Id && 
                pa.UserId.ToString() == userId &&
                pa.IsActive);

        if (patientAccess == null)
            return false;

        // Soft delete the access
        patientAccess.IsActive = false;
        patientAccess.ModifiedAt = DateTime.UtcNow;
        patientAccess.ModifiedBy = revokedBy;

        await _context.SaveChangesAsync(CancellationToken.None);
        
        _logger.LogInformation("Revoked access for user {UserId} to patient {PatientId} by {RevokedBy}", userId, patientId, revokedBy);
        return true;
    }

    /// <summary>
    /// Get patient access records for user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>Collection of patient access records</returns>
    public async Task<IEnumerable<PatientAccess>> GetPatientAccessesForUserAsync(string userId, string tenantId)
    {
        return await _context.PatientAccesses
            .Include(pa => pa.Patient)
            .Where(pa => pa.UserId.ToString() == userId && pa.TenantId == tenantId && pa.IsActive)
            .ToListAsync();
    }

    /// <summary>
    /// Get patient access records for patient
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>Collection of patient access records</returns>
    public async Task<IEnumerable<PatientAccess>> GetPatientAccessesForPatientAsync(string patientId, string tenantId)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.FhirPatientId == patientId);
        if (patient == null)
            return new List<PatientAccess>();

        return await _context.PatientAccesses
            .Include(pa => pa.User)
            .Where(pa => pa.PatientId == patient.Id && pa.TenantId == tenantId && pa.IsActive)
            .ToListAsync();
    }

    /// <summary>
    /// Check if user has emergency access to patient
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="patientId">Patient ID</param>
    /// <returns>True if emergency access exists, false otherwise</returns>
    public async Task<bool> HasEmergencyAccessAsync(string userId, string patientId)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.FhirPatientId == patientId);
        if (patient == null)
            return false;

        var emergencyAccess = await _context.PatientAccesses
            .FirstOrDefaultAsync(pa => 
                pa.PatientId == patient.Id && 
                pa.UserId.ToString() == userId &&
                pa.IsEmergencyAccess &&
                pa.IsActive);

        return emergencyAccess != null;
    }

    /// <summary>
    /// Create emergency access to patient
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <param name="userId">User ID</param>
    /// <param name="grantedBy">User who granted emergency access</param>
    /// <param name="justification">Emergency justification</param>
    /// <param name="expiresAt">When emergency access expires</param>
    /// <returns>Created emergency access</returns>
    public async Task<PatientAccess> CreateEmergencyAccessAsync(
        string patientId,
        string userId,
        string grantedBy,
        string justification,
        DateTime expiresAt)
    {
        var patientAccess = await GrantPatientAccessAsync(
            patientId,
            userId,
            PatientAccessLevel.Emergency,
            grantedBy,
            "Emergency access",
            expiresAt,
            true,
            justification);

        _logger.LogWarning("Emergency access granted to user {UserId} for patient {PatientId} by {GrantedBy}", userId, patientId, grantedBy);
        return patientAccess;
    }

    /// <summary>
    /// Get access level for user to patient
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="patientId">Patient ID</param>
    /// <returns>Access level or null if no access</returns>
    public async Task<PatientAccessLevel?> GetAccessLevelAsync(string userId, string patientId)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.FhirPatientId == patientId);
        if (patient == null)
            return null;

        var patientAccess = await _context.PatientAccesses
            .FirstOrDefaultAsync(pa => 
                pa.PatientId == patient.Id && 
                pa.UserId.ToString() == userId &&
                pa.IsActive);

        return patientAccess?.AccessLevel;
    }

    /// <summary>
    /// Check if access is expired
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <param name="userId">User ID</param>
    /// <returns>True if access is expired, false otherwise</returns>
    public async Task<bool> IsAccessExpiredAsync(string patientId, string userId)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.FhirPatientId == patientId);
        if (patient == null)
            return true;

        var patientAccess = await _context.PatientAccesses
            .FirstOrDefaultAsync(pa => 
                pa.PatientId == patient.Id && 
                pa.UserId.ToString() == userId &&
                pa.IsActive);

        if (patientAccess == null)
            return true;

        return patientAccess.ExpiresAt.HasValue && patientAccess.ExpiresAt.Value < DateTime.UtcNow;
    }

    /// <summary>
    /// Extend access expiration
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <param name="userId">User ID</param>
    /// <param name="newExpiresAt">New expiration time</param>
    /// <param name="modifiedBy">User who modified the access</param>
    /// <returns>True if extended, false otherwise</returns>
    public async Task<bool> ExtendAccessAsync(string patientId, string userId, DateTime newExpiresAt, string modifiedBy)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.FhirPatientId == patientId);
        if (patient == null)
            return false;

        var patientAccess = await _context.PatientAccesses
            .FirstOrDefaultAsync(pa => 
                pa.PatientId == patient.Id && 
                pa.UserId.ToString() == userId &&
                pa.IsActive);

        if (patientAccess == null)
            return false;

        patientAccess.ExpiresAt = newExpiresAt;
        patientAccess.ModifiedAt = DateTime.UtcNow;
        patientAccess.ModifiedBy = modifiedBy;

        await _context.SaveChangesAsync(CancellationToken.None);
        
        _logger.LogInformation("Extended access for user {UserId} to patient {PatientId} until {NewExpiresAt}", userId, patientId, newExpiresAt);
        return true;
    }

    /// <summary>
    /// Check if user can grant access to patient
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="patientId">Patient ID</param>
    /// <param name="userRole">User role</param>
    /// <returns>True if can grant access, false otherwise</returns>
    public async Task<bool> CanGrantAccessAsync(string userId, string patientId, UserRole userRole)
    {
        // System administrators can grant access to any patient
        if (userRole == UserRole.SystemAdministrator)
            return true;

        // Healthcare providers can grant access to patients they have access to
        if (userRole == UserRole.HealthcareProvider)
        {
            var hasAccess = await CanAccessPatientAsync(userId, patientId, "patient/*");
            return hasAccess;
        }

        // Other roles cannot grant access
        return false;
    }

    /// <summary>
    /// Grant access to patient for user
    /// </summary>
    /// <param name="targetUserId">Target user ID</param>
    /// <param name="patientId">Patient ID</param>
    /// <param name="accessLevel">Access level</param>
    /// <param name="grantedBy">User who granted access</param>
    /// <param name="reason">Reason for granting access</param>
    /// <param name="expiresAt">When access expires (null = no expiration)</param>
    /// <returns>Access ID</returns>
    public async Task<string> GrantAccessAsync(string targetUserId, string patientId, PatientAccessLevel accessLevel, string grantedBy, string? reason = null, DateTime? expiresAt = null)
    {
        var patientAccess = await GrantPatientAccessAsync(
            patientId,
            targetUserId,
            accessLevel,
            grantedBy,
            reason,
            expiresAt);

        return patientAccess.Id.ToString();
    }

    /// <summary>
    /// Check if user can revoke access to patient
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="accessId">Access ID</param>
    /// <param name="userRole">User role</param>
    /// <returns>True if can revoke access, false otherwise</returns>
    public async Task<bool> CanRevokeAccessAsync(string userId, string accessId, UserRole userRole)
    {
        // System administrators can revoke any access
        if (userRole == UserRole.SystemAdministrator)
            return true;

        // Get the access record to check ownership
        var accessRecord = await _context.PatientAccesses
            .Include(pa => pa.Patient)
            .FirstOrDefaultAsync(pa => pa.Id.ToString() == accessId && pa.IsActive);

        if (accessRecord == null)
            return false;

        // Users can revoke access they granted
        if (accessRecord.GrantedBy == userId)
            return true;

        // Healthcare providers can revoke access to patients they have access to
        if (userRole == UserRole.HealthcareProvider)
        {
            var hasAccess = await CanAccessPatientAsync(userId, accessRecord.Patient.FhirPatientId, "patient/*");
            return hasAccess;
        }

        return false;
    }

    /// <summary>
    /// Revoke access to patient for user
    /// </summary>
    /// <param name="accessId">Access ID</param>
    /// <param name="revokedBy">User who revoked access</param>
    /// <param name="reason">Reason for revocation</param>
    /// <returns>True if revoked, false otherwise</returns>
    public async Task<bool> RevokeAccessAsync(string accessId, string revokedBy, string? reason = null)
    {
        var accessRecord = await _context.PatientAccesses
            .Include(pa => pa.Patient)
            .FirstOrDefaultAsync(pa => pa.Id.ToString() == accessId && pa.IsActive);

        if (accessRecord == null)
            return false;

        return await RevokePatientAccessAsync(
            accessRecord.Patient.FhirPatientId,
            accessRecord.UserId.ToString(),
            revokedBy,
            reason);
    }

    /// <summary>
    /// Check if user can view patient access records
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="patientId">Patient ID</param>
    /// <param name="userRole">User role</param>
    /// <returns>True if can view records, false otherwise</returns>
    public async Task<bool> CanViewAccessRecordsAsync(string userId, string? patientId, UserRole userRole)
    {
        // System administrators can view all records
        if (userRole == UserRole.SystemAdministrator)
            return true;

        // If specific patient ID is provided, check access to that patient
        if (!string.IsNullOrEmpty(patientId))
        {
            var hasAccess = await CanAccessPatientAsync(userId, patientId, "patient/*");
            return hasAccess;
        }

        // Healthcare providers can view records for patients they have access to
        if (userRole == UserRole.HealthcareProvider)
            return true;

        // Other roles cannot view access records
        return false;
    }

    /// <summary>
    /// Get patient access records with pagination
    /// </summary>
    /// <param name="patientId">Patient ID filter</param>
    /// <param name="userId">User ID filter</param>
    /// <param name="accessLevel">Access level filter</param>
    /// <param name="isActive">Active status filter</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Tuple of access records and total count</returns>
    public async Task<(List<PatientAccessInfo> AccessRecords, int TotalCount)> GetPatientAccessAsync(
        string? patientId,
        string? userId,
        PatientAccessLevel? accessLevel,
        bool? isActive,
        int page,
        int pageSize)
    {
        var query = _context.PatientAccesses
            .Include(pa => pa.Patient)
            .Include(pa => pa.User)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(patientId))
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.FhirPatientId == patientId);
            if (patient != null)
                query = query.Where(pa => pa.PatientId == patient.Id);
        }

        if (!string.IsNullOrEmpty(userId))
            query = query.Where(pa => pa.UserId.ToString() == userId);

        if (accessLevel.HasValue)
            query = query.Where(pa => pa.AccessLevel == accessLevel.Value);

        if (isActive.HasValue)
            query = query.Where(pa => pa.IsActive == isActive.Value);

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination
        var accessRecords = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .OrderByDescending(pa => pa.CreatedAt)
            .ToListAsync();

        // Convert to DTOs
        var patientAccessInfos = accessRecords.Select(pa => new PatientAccessInfo
        {
            Id = pa.Id.ToString(),
            UserId = pa.UserId.ToString(),
            UserName = pa.User?.DisplayName ?? "Unknown User",
            PatientId = pa.Patient?.FhirPatientId ?? "Unknown Patient",
            PatientName = pa.Patient?.DisplayName ?? "Unknown Patient",
            AccessLevel = pa.AccessLevel,
            Reason = pa.Reason,
            GrantedAt = pa.CreatedAt,
            GrantedBy = pa.GrantedBy,
            ExpiresAt = pa.ExpiresAt,
            IsActive = pa.IsActive
        }).ToList();

        return (patientAccessInfos, totalCount);
    }
}
