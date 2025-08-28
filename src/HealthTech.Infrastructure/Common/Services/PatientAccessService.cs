using HealthTech.Application.Common.Interfaces;
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
                    pa.AccessLevel >= PatientAccessLevel.ReadOnly &&
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
            PatientAccessLevel.EmergencyAccess,
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
}
