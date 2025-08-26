using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;

namespace HealthTech.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor for automatically setting audit fields on entities
/// </summary>
public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="currentUserService">Current user service</param>
    /// <param name="dateTime">DateTime service</param>
    public AuditableEntitySaveChangesInterceptor(ICurrentUserService currentUserService, IDateTime dateTime)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    /// <summary>
    /// Intercept saving changes
    /// </summary>
    /// <param name="eventData">Event data</param>
    /// <param name="result">Result</param>
    /// <returns>Interception result</returns>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Intercept saving changes async
    /// </summary>
    /// <param name="eventData">Event data</param>
    /// <param name="result">Result</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Interception result</returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = _currentUserService.UserId ?? "system";
                entry.Entity.CreatedAt = _dateTime.Now;
                entry.Entity.TenantId = _currentUserService.TenantId ?? string.Empty;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.ModifiedBy = _currentUserService.UserId;
                entry.Entity.ModifiedAt = _dateTime.Now;
                entry.Entity.Version++;
            }
        }
    }
}
