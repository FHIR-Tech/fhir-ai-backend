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
    /// Save changes to database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of affected rows</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
