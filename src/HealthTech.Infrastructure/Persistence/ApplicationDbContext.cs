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
    /// Users DbSet
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Patients DbSet
    /// </summary>
    public DbSet<Patient> Patients => Set<Patient>();

    /// <summary>
    /// Patient access DbSet
    /// </summary>
    public DbSet<PatientAccess> PatientAccesses => Set<PatientAccess>();

    /// <summary>
    /// Patient consents DbSet
    /// </summary>
    public DbSet<PatientConsent> PatientConsents => Set<PatientConsent>();

    /// <summary>
    /// User scopes DbSet
    /// </summary>
    public DbSet<UserScope> UserScopes => Set<UserScope>();

    /// <summary>
    /// User sessions DbSet
    /// </summary>
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    /// <summary>
    /// Configure model
    /// </summary>
    /// <param name="modelBuilder">Model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure snake_case naming convention
        // foreach (var entity in modelBuilder.Model.GetEntityTypes())
        // {
        //     // Configure table names to snake_case
        //     entity.SetTableName(ConvertToSnakeCase(entity.GetTableName()));
            
        //     // Configure column names to snake_case
        //     foreach (var property in entity.GetProperties())
        //     {
        //         property.SetColumnName(ConvertToSnakeCase(property.GetColumnName()));
        //     }
        // }

        // Apply entity configurations (which handle snake_case naming)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
    /// <summary>
    /// Convert PascalCase to snake_case
    /// </summary>
    /// <param name="name">Name to convert</param>
    /// <returns>Snake case name</returns>
    private static string? ConvertToSnakeCase(string? name)
    {
        if (string.IsNullOrEmpty(name))
            return name;
            
        return string.Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
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
}
