using Microsoft.EntityFrameworkCore;
using HealthTech.Domain.Entities;

namespace HealthTech.Application.Common.Interfaces;

/// <summary>
/// Application database context interface
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// FHIR resources DbSet
    /// </summary>
    DbSet<FhirResource> FhirResources { get; }

    /// <summary>
    /// Audit events DbSet
    /// </summary>
    DbSet<AuditEvent> AuditEvents { get; }

    /// <summary>
    /// Users DbSet
    /// </summary>
    DbSet<User> Users { get; }

    /// <summary>
    /// Patients DbSet
    /// </summary>
    DbSet<Patient> Patients { get; }

    /// <summary>
    /// Patient access DbSet
    /// </summary>
    DbSet<Domain.Entities.PatientAccess> PatientAccesses { get; }

    /// <summary>
    /// Patient consents DbSet
    /// </summary>
    DbSet<PatientConsent> PatientConsents { get; }

    /// <summary>
    /// User scopes DbSet
    /// </summary>
    DbSet<UserScope> UserScopes { get; }

    /// <summary>
    /// User sessions DbSet
    /// </summary>
    DbSet<UserSession> UserSessions { get; }

    /// <summary>
    /// Save changes to database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of affected rows</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
