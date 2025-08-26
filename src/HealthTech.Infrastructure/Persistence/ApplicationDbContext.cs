using Microsoft.EntityFrameworkCore;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;
using HealthTech.Infrastructure.Persistence.Interceptors;

namespace HealthTech.Infrastructure.Persistence;

/// <summary>
/// Application database context
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="options">DbContext options</param>
    /// <param name="auditableEntitySaveChangesInterceptor">Auditable entity interceptor</param>
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    /// <summary>
    /// FHIR resources DbSet
    /// </summary>
    public DbSet<FhirResource> FhirResources => Set<FhirResource>();

    /// <summary>
    /// Audit events DbSet
    /// </summary>
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    /// <summary>
    /// Configure model
    /// </summary>
    /// <param name="modelBuilder">Model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Configure Row Level Security
        ConfigureRowLevelSecurity(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Configure interceptors
    /// </summary>
    /// <param name="optionsBuilder">Options builder</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
        base.OnConfiguring(optionsBuilder);
    }

    private static void ConfigureRowLevelSecurity(ModelBuilder modelBuilder)
    {
        // Configure RLS for FhirResource
        modelBuilder.Entity<FhirResource>()
            .ToTable("fhir_resources", t => t.HasComment("FHIR resources stored as JSONB"))
            .HasIndex(e => new { e.TenantId, e.ResourceType, e.FhirId })
            .IsUnique();

        modelBuilder.Entity<FhirResource>()
            .HasIndex(e => new { e.TenantId, e.ResourceType })
            .HasDatabaseName("IX_FhirResources_TenantId_ResourceType");

        modelBuilder.Entity<FhirResource>()
            .HasIndex(e => e.SearchParameters)
            .HasMethod("gin")
            .HasDatabaseName("IX_FhirResources_SearchParameters");

        // Configure RLS for AuditEvent
        modelBuilder.Entity<AuditEvent>()
            .ToTable("audit_events", t => t.HasComment("Audit events for compliance tracking"))
            .HasIndex(e => new { e.TenantId, e.EventTime })
            .HasDatabaseName("IX_AuditEvents_TenantId_EventTime");

        modelBuilder.Entity<AuditEvent>()
            .HasIndex(e => new { e.TenantId, e.UserId, e.EventTime })
            .HasDatabaseName("IX_AuditEvents_TenantId_UserId_EventTime");
    }
}
