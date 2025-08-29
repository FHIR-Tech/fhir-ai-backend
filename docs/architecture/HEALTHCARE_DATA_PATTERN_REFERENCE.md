# Healthcare Data Pattern Reference Guide

## Overview

This document defines the **immutable Healthcare Data Pattern standards** for the FHIR-AI Backend project. These standards should **NEVER change**, regardless of Microsoft's framework updates or new patterns. They ensure consistency, maintainability, and proper handling of healthcare data in compliance with FHIR standards and healthcare regulations.

## Core Healthcare Data Principles (Immutable)

### 1. FHIR Compliance (Immutable Rule)
- **FHIR Resources**: All healthcare data must be FHIR-compliant
- **Resource Types**: Use standard FHIR resource types (Patient, Observation, Condition, etc.)
- **Data Types**: Use FHIR data types (HumanName, Address, ContactPoint, etc.)
- **Extensions**: Use FHIR extensions for custom data when needed

### 2. Healthcare Data Security (Immutable Rule)
- **PHI Protection**: Personal Health Information must be encrypted
- **Access Control**: Role-based access control (RBAC) for healthcare data
- **Audit Trail**: Complete audit trail for all healthcare data access
- **Data Minimization**: Only collect necessary healthcare data
- **Consent Management**: Patient consent tracking and management

### 3. Multi-Tenancy (Immutable Rule)
- **Tenant Isolation**: Healthcare data must be isolated by tenant
- **Row-Level Security**: Database-level tenant isolation
- **Cross-Tenant Access**: Controlled cross-tenant data sharing
- **Tenant Hierarchy**: Support for healthcare organization hierarchies

### 4. Data Integrity (Immutable Rule)
- **Versioning**: All healthcare data must be versioned
- **Immutable History**: Healthcare data history cannot be modified
- **Data Validation**: Comprehensive validation of healthcare data
- **Reference Integrity**: Maintain referential integrity across FHIR resources

## Implementation Patterns

### FHIR Resource Structure

#### FHIR R4B Resource Implementation
```csharp
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System.Text.Json;

public class Patient : BaseEntity
{
    // ========================================
    // FHIR R4B RESOURCE STORAGE
    // ========================================
    
    /// <summary>
    /// FHIR R4B Patient resource as JSONB
    /// </summary>
    [Column(TypeName = "jsonb")]
    public JsonDocument FhirResource { get; set; } = JsonDocument.Parse("{}");
    
    /// <summary>
    /// FHIR resource ID (from FHIR resource)
    /// </summary>
    [MaxLength(64)]
    public string FhirId { get; set; } = string.Empty;
    
    /// <summary>
    /// FHIR resource version (from FHIR resource)
    /// </summary>
    public int FhirVersion { get; set; } = 1;
    
    /// <summary>
    /// FHIR resource last updated (from FHIR resource)
    /// </summary>
    public DateTime FhirLastUpdated { get; set; } = DateTime.UtcNow;
    
    // ========================================
    // CORE IDENTITY FIELDS
    // ========================================
    
    /// <summary>
    /// Patient's first name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Patient's last name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Patient's date of birth
    /// </summary>
    [Required]
    public DateTime DateOfBirth { get; set; }
    
    /// <summary>
    /// Patient's gender
    /// </summary>
    public Gender Gender { get; set; }
    
    // ========================================
    // CONTACT INFORMATION
    // ========================================
    
    /// <summary>
    /// Patient's email address
    /// </summary>
    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; set; }
    
    /// <summary>
    /// Patient's phone number
    /// </summary>
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    
    // ========================================
    // ADDRESS INFORMATION
    // ========================================
    
    /// <summary>
    /// Patient's address line 1
    /// </summary>
    [MaxLength(255)]
    public string? AddressLine1 { get; set; }
    
    /// <summary>
    /// Patient's address line 2
    /// </summary>
    [MaxLength(255)]
    public string? AddressLine2 { get; set; }
    
    /// <summary>
    /// Patient's city
    /// </summary>
    [MaxLength(100)]
    public string? City { get; set; }
    
    /// <summary>
    /// Patient's state/province
    /// </summary>
    [MaxLength(100)]
    public string? State { get; set; }
    
    /// <summary>
    /// Patient's postal code
    /// </summary>
    [MaxLength(20)]
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Patient's country
    /// </summary>
    [MaxLength(100)]
    public string? Country { get; set; }
    
    // ========================================
    // HEALTHCARE SPECIFIC FIELDS
    // ========================================
    
    /// <summary>
    /// Patient's medical record number
    /// </summary>
    [MaxLength(50)]
    public string? MedicalRecordNumber { get; set; }
    
    /// <summary>
    /// Patient's insurance information
    /// </summary>
    [Column(TypeName = "jsonb")]
    public InsuranceInfo? Insurance { get; set; }
    
    /// <summary>
    /// Patient's emergency contact information
    /// </summary>
    [Column(TypeName = "jsonb")]
    public EmergencyContact? EmergencyContact { get; set; }
    
    /// <summary>
    /// Patient's allergies
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<Allergy> Allergies { get; set; } = new();
    
    /// <summary>
    /// Patient's medications
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<Medication> Medications { get; set; } = new();
    
    // ========================================
    // STATUS & CONFIGURATION FIELDS
    // ========================================
    
    /// <summary>
    /// Patient's status
    /// </summary>
    public PatientStatus Status { get; set; } = PatientStatus.Active;
    
    /// <summary>
    /// Patient's deceased status
    /// </summary>
    public bool IsDeceased { get; set; } = false;
    
    /// <summary>
    /// Patient's deceased date
    /// </summary>
    public DateTime? DeceasedDate { get; set; }
    
    // ========================================
    // SECURITY & ACCESS FIELDS
    // ========================================
    
    /// <summary>
    /// Patient's consent status
    /// </summary>
    public ConsentStatus ConsentStatus { get; set; } = ConsentStatus.Pending;
    
    /// <summary>
    /// Patient's consent date
    /// </summary>
    public DateTime? ConsentDate { get; set; }
    
    /// <summary>
    /// Patient's data classification
    /// </summary>
    public DataClassification DataClassification { get; set; } = DataClassification.PHI;
    
    // ========================================
    // TIMING FIELDS
    // ========================================
    
    /// <summary>
    /// When the patient record was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the patient record was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// When the patient record was last accessed
    /// </summary>
    public DateTime? LastAccessedAt { get; set; }
    
    // ========================================
    // FHIR R4B HELPER METHODS
    // ========================================
    
    /// <summary>
    /// Get FHIR R4B Patient resource
    /// </summary>
    [NotMapped]
    public Hl7.Fhir.Model.Patient? FhirPatient
    {
        get
        {
            try
            {
                if (FhirResource == null) return null;
                var jsonString = FhirResource.RootElement.GetRawText();
                return FhirJsonParser.ParseResourceFromJson<Hl7.Fhir.Model.Patient>(jsonString);
            }
            catch
            {
                return null;
            }
        }
    }
    
    /// <summary>
    /// Set FHIR R4B Patient resource
    /// </summary>
    public void SetFhirPatient(Hl7.Fhir.Model.Patient patient)
    {
        var jsonString = FhirJsonSerializer.SerializeToString(patient);
        FhirResource = JsonDocument.Parse(jsonString);
        FhirId = patient.Id ?? string.Empty;
        FhirVersion = patient.Meta?.VersionId != null ? int.Parse(patient.Meta.VersionId) : 1;
        FhirLastUpdated = patient.Meta?.LastUpdated?.ToDateTime() ?? DateTime.UtcNow;
    }
    
    // ========================================
    // COMPUTED PROPERTIES
    // ========================================
    
    /// <summary>
    /// Patient's full name
    /// </summary>
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    /// <summary>
    /// Patient's age
    /// </summary>
    [NotMapped]
    public int Age
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
    
    /// <summary>
    /// Patient's full address
    /// </summary>
    [NotMapped]
    public string FullAddress
    {
        get
        {
            var parts = new List<string>();
            if (!string.IsNullOrEmpty(AddressLine1)) parts.Add(AddressLine1);
            if (!string.IsNullOrEmpty(AddressLine2)) parts.Add(AddressLine2);
            if (!string.IsNullOrEmpty(City)) parts.Add(City);
            if (!string.IsNullOrEmpty(State)) parts.Add(State);
            if (!string.IsNullOrEmpty(PostalCode)) parts.Add(PostalCode);
            if (!string.IsNullOrEmpty(Country)) parts.Add(Country);
            return string.Join(", ", parts);
        }
    }
    
    // ========================================
    // NAVIGATION PROPERTIES
    // ========================================
    
    /// <summary>
    /// Patient's observations
    /// </summary>
    public virtual ICollection<Observation> Observations { get; set; } = new List<Observation>();
    
    /// <summary>
    /// Patient's conditions
    /// </summary>
    public virtual ICollection<Condition> Conditions { get; set; } = new List<Condition>();
    
    /// <summary>
    /// Patient's encounters
    /// </summary>
    public virtual ICollection<Encounter> Encounters { get; set; } = new List<Encounter>();
}
```

### FHIR R4B Endpoint Implementation

#### FHIR Standard Endpoint Implementation
```csharp
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;

public static class FhirEndpoints
{
    public static void MapFhirEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/fhir")
            .WithTags("FHIR")
            .WithOpenApi()
            .RequireAuthorization();

        // ========================================
        // FHIR STANDARD RESOURCE ENDPOINTS
        // ========================================

        // GET /fhir/{resourceType} - Search Resources
        group.MapGet("/{resourceType}", async (
            string resourceType,
            ISender sender,
            CancellationToken cancellationToken,
            int skip = 0,
            int take = 100) =>
        {
            var searchQuery = new SearchFhirResourcesQuery 
            { 
                ResourceType = resourceType,
                Skip = skip,
                Take = take
            };
            var result = await sender.Send(searchQuery, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("SearchFhirResources")
        .WithSummary("Search FHIR resources by type")
        .WithDescription("Search for FHIR resources of the specified type");

        // GET /fhir/{resourceType}/{id} - Read Resource
        group.MapGet("/{resourceType}/{id}", async (
            string resourceType,
            string id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetFhirResourceQuery { ResourceType = resourceType, FhirId = id };
            var result = await sender.Send(query, cancellationToken);
            
            if (result == null)
                return Results.NotFound();
                
            return Results.Ok(result);
        })
        .WithName("GetFhirResource")
        .WithSummary("Get FHIR resource by ID")
        .WithDescription("Retrieve a specific FHIR resource by type and ID");

        // POST /fhir/{resourceType} - Create Resource
        group.MapPost("/{resourceType}", async (
            string resourceType,
            CreateFhirResourceCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var createCommand = command with { ResourceType = resourceType };
            var result = await sender.Send(createCommand, cancellationToken);
            return Results.Created($"/fhir/{resourceType}/{result.FhirId}", result);
        })
        .WithName("CreateFhirResource")
        .WithSummary("Create FHIR resource")
        .WithDescription("Create a new FHIR resource of the specified type");

        // PUT /fhir/{resourceType}/{id} - Update Resource
        group.MapPut("/{resourceType}/{id}", async (
            string resourceType,
            string id,
            UpdateFhirResourceCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var updateCommand = command with { ResourceType = resourceType, FhirId = id };
            var result = await sender.Send(updateCommand, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("UpdateFhirResource")
        .WithSummary("Update FHIR resource")
        .WithDescription("Update an existing FHIR resource");

        // DELETE /fhir/{resourceType}/{id} - Delete Resource
        group.MapDelete("/{resourceType}/{id}", async (
            string resourceType,
            string id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteFhirResourceCommand { ResourceType = resourceType, FhirId = id };
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("DeleteFhirResource")
        .WithSummary("Delete FHIR resource")
        .WithDescription("Delete a FHIR resource");

        // ========================================
        // FHIR STANDARD HISTORY ENDPOINTS
        // ========================================

        // GET /fhir/{resourceType}/{id}/_history - Resource History
        group.MapGet("/{resourceType}/{id}/_history", async (
            string resourceType,
            string id,
            ISender sender,
            CancellationToken cancellationToken,
            int maxVersions = 100) =>
        {
            var query = new GetFhirResourceHistoryQuery 
            { 
                ResourceType = resourceType, 
                FhirId = id,
                MaxVersions = maxVersions
            };
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetFhirResourceHistory")
        .WithSummary("Get FHIR resource history")
        .WithDescription("Get version history for a specific FHIR resource");

        // ========================================
        // FHIR STANDARD SYSTEM OPERATIONS ($)
        // ========================================

        // POST /fhir/$auto-detect-type - Auto-detect Resource Type
        group.MapPost("/$auto-detect-type", async (
            CreateFhirResourceCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return Results.Created($"/fhir/{command.ResourceType}/{result.FhirId}", result);
        })
        .WithName("CreateFhirResourceAutoDetect")
        .WithSummary("Create FHIR resource with auto-detected type")
        .WithDescription("Create a FHIR resource with automatic resource type detection");

        // POST /fhir/$import-bundle - Import FHIR Bundle
        group.MapPost("/$import-bundle", async (
            HttpContext httpContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            // Implementation for importing FHIR bundles
            var command = new ImportFhirBundleCommand();
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("ImportFhirBundle")
        .WithSummary("Import FHIR Bundle (HL7 Standard)")
        .WithDescription("Import a FHIR Bundle containing multiple resources");

        // GET /fhir/$export-bundle - Export FHIR Bundle
        group.MapGet("/$export-bundle", async (
            ISender sender,
            CancellationToken cancellationToken,
            string? resourceType = null,
            string? fhirIds = null,
            int maxResources = 1000,
            string bundleType = "collection",
            bool includeHistory = false,
            int maxHistoryVersions = 10,
            bool includeDeleted = false,
            string format = "json") =>
        {
            var query = new ExportFhirBundleQuery
            {
                ResourceType = resourceType,
                FhirIds = !string.IsNullOrEmpty(fhirIds) ? fhirIds.Split(',', StringSplitOptions.RemoveEmptyEntries) : null,
                MaxResources = maxResources,
                BundleType = bundleType,
                IncludeHistory = includeHistory,
                MaxHistoryVersions = maxHistoryVersions,
                IncludeDeleted = includeDeleted,
                Format = format
            };
            
            var result = await sender.Send(query, cancellationToken);
            return Results.Content(result.BundleJson, "application/json");
        })
        .WithName("ExportFhirBundle")
        .WithSummary("Export FHIR resources as a bundle")
        .WithDescription("Export FHIR resources as a Bundle in various formats");

        // POST /fhir/$export-bundle - Export FHIR Bundle (Complex Queries)
        group.MapPost("/$export-bundle", async (
            ExportFhirBundleQuery query,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(query, cancellationToken);
            return Results.Content(result.BundleJson, "application/json");
        })
        .WithName("ExportFhirBundlePost")
        .WithSummary("Export FHIR resources as a bundle (POST method for complex queries)")
        .WithDescription("Export FHIR resources as a Bundle using POST for complex query parameters");
    }
}

### FHIR R4B Commands and Queries

#### FHIR Standard Commands and Queries
```csharp
using Hl7.Fhir.Model;

// ========================================
// FHIR RESOURCE COMMANDS
// ========================================

public record CreateFhirResourceCommand : IRequest<Resource>
{
    public string ResourceType { get; init; } = string.Empty;
    public JsonDocument FhirResource { get; init; } = JsonDocument.Parse("{}");
}

public record UpdateFhirResourceCommand : IRequest<Resource>
{
    public string ResourceType { get; init; } = string.Empty;
    public string FhirId { get; init; } = string.Empty;
    public JsonDocument FhirResource { get; init; } = JsonDocument.Parse("{}");
}

public record DeleteFhirResourceCommand : IRequest<bool>
{
    public string ResourceType { get; init; } = string.Empty;
    public string FhirId { get; init; } = string.Empty;
}

// ========================================
// FHIR RESOURCE QUERIES
// ========================================

public record GetFhirResourceQuery : IRequest<Resource?>
{
    public string ResourceType { get; init; } = string.Empty;
    public string FhirId { get; init; } = string.Empty;
}

public record SearchFhirResourcesQuery : IRequest<Bundle>
{
    public string ResourceType { get; init; } = string.Empty;
    public int Skip { get; init; } = 0;
    public int Take { get; init; } = 100;
    public Dictionary<string, string> SearchParameters { get; init; } = new();
}

public record GetFhirResourceHistoryQuery : IRequest<Bundle>
{
    public string ResourceType { get; init; } = string.Empty;
    public string FhirId { get; init; } = string.Empty;
    public int MaxVersions { get; init; } = 100;
}

// ========================================
// FHIR SYSTEM OPERATIONS
// ========================================

public record ImportFhirBundleCommand : IRequest<Bundle>
{
    public string BundleJson { get; init; } = string.Empty;
    public bool ValidateResources { get; init; } = true;
    public string? Description { get; init; }
}

public record ExportFhirBundleQuery : IRequest<ExportFhirBundleResult>
{
    public string? ResourceType { get; init; }
    public string[]? FhirIds { get; init; }
    public int MaxResources { get; init; } = 1000;
    public string BundleType { get; init; } = "collection";
    public bool IncludeHistory { get; init; } = false;
    public int MaxHistoryVersions { get; init; } = 10;
    public bool IncludeDeleted { get; init; } = false;
    public string Format { get; init; } = "json";
    
    // Time-based filtering parameters
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? TimePeriod { get; init; }
    public int? TimePeriodCount { get; init; }
    public string? ObservationCode { get; init; }
    public string? ObservationSystem { get; init; }
    public string? PatientId { get; init; }
    public int? MaxObservationsPerPatient { get; init; }
    public string SortOrder { get; init; } = "desc";
    public bool LatestOnly { get; init; } = false;
}

public record ExportFhirBundleResult
{
    public string BundleJson { get; init; } = string.Empty;
    public int ResourceCount { get; init; }
    public string BundleType { get; init; } = string.Empty;
    public DateTime ExportedAt { get; init; } = DateTime.UtcNow;
}
```

### Healthcare Data Enums

```csharp
public enum Gender
{
    Unknown = 0,
    Male = 1,
    Female = 2,
    Other = 3
}

public enum PatientStatus
{
    Active = 1,
    Inactive = 2,
    Deceased = 3,
    Archived = 4
}

public enum ConsentStatus
{
    Pending = 0,
    Granted = 1,
    Denied = 2,
    Expired = 3,
    Withdrawn = 4
}

public enum DataClassification
{
    Public = 0,
    Internal = 1,
    Confidential = 2,
    PHI = 3,
    Restricted = 4
}
```

### Healthcare Data Value Objects

```csharp
public record InsuranceInfo
{
    public string? Provider { get; init; }
    public string? PolicyNumber { get; init; }
    public string? GroupNumber { get; init; }
    public DateTime? EffectiveDate { get; init; }
    public DateTime? ExpirationDate { get; init; }
    public string? SubscriberName { get; init; }
    public string? Relationship { get; init; }
}

public record EmergencyContact
{
    public string Name { get; init; } = string.Empty;
    public string Relationship { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Address { get; init; }
}

public record Allergy
{
    public string Substance { get; init; } = string.Empty;
    public string Reaction { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty;
    public DateTime? OnsetDate { get; init; }
    public string? Notes { get; init; }
}

public record Medication
{
    public string Name { get; init; } = string.Empty;
    public string Dosage { get; init; } = string.Empty;
    public string Frequency { get; init; } = string.Empty;
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Prescriber { get; init; }
    public string? Notes { get; init; }
}
```

### FHIR R4B Repository Pattern

```csharp
public interface IFhirResourceRepository
{
    // ========================================
    // FHIR CRUD OPERATIONS
    // ========================================
    
    Task<Resource?> GetByFhirIdAsync(string resourceType, string fhirId, CancellationToken cancellationToken = default);
    Task<Resource> AddAsync(string resourceType, Resource resource, CancellationToken cancellationToken = default);
    Task<Resource> UpdateAsync(string resourceType, string fhirId, Resource resource, CancellationToken cancellationToken = default);
    Task DeleteAsync(string resourceType, string fhirId, CancellationToken cancellationToken = default);
    
    // ========================================
    // FHIR SEARCH OPERATIONS
    // ========================================
    
    Task<Bundle> SearchAsync(
        string resourceType,
        Dictionary<string, string> searchParameters,
        int skip = 0,
        int take = 100,
        CancellationToken cancellationToken = default);
    
    // ========================================
    // FHIR HISTORY OPERATIONS
    // ========================================
    
    Task<Bundle> GetHistoryAsync(
        string resourceType, 
        string fhirId, 
        int maxVersions = 100,
        CancellationToken cancellationToken = default);
    
    // ========================================
    // FHIR BUNDLE OPERATIONS
    // ========================================
    
    Task<Bundle> ImportBundleAsync(Bundle bundle, bool validateResources = true, CancellationToken cancellationToken = default);
    Task<Bundle> ExportBundleAsync(
        string? resourceType = null,
        string[]? fhirIds = null,
        int maxResources = 1000,
        string bundleType = "collection",
        bool includeHistory = false,
        int maxHistoryVersions = 10,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default);
    
    // ========================================
    // FHIR VALIDATION
    // ========================================
    
    Task<bool> ValidateFhirResourceAsync(Resource resource, CancellationToken cancellationToken = default);
    
    // ========================================
    // MULTI-TENANCY
    // ========================================
    
    Task<IEnumerable<Resource>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}

### FHIR R4B Validation

```csharp
using Hl7.Fhir.Model;
using Hl7.Fhir.Validation;

public class CreateFhirResourceCommandValidator : AbstractValidator<CreateFhirResourceCommand>
{
    private readonly FhirValidator _fhirValidator;

    public CreateFhirResourceCommandValidator(FhirValidator fhirValidator)
    {
        _fhirValidator = fhirValidator;

        RuleFor(x => x.ResourceType)
            .NotEmpty()
            .WithMessage("Resource type is required")
            .Must(IsValidFhirResourceType)
            .WithMessage("Invalid FHIR resource type");

        RuleFor(x => x.FhirResource)
            .NotNull()
            .WithMessage("FHIR resource is required")
            .MustAsync(ValidateFhirResourceAsync)
            .WithMessage("Invalid FHIR resource");

        RuleFor(x => x.FhirResource.RootElement.GetProperty("id").GetString())
            .Empty()
            .When(x => x.FhirResource.RootElement.TryGetProperty("id", out _))
            .WithMessage("Resource ID should not be provided for create operations");

        RuleFor(x => x.FhirResource.RootElement.GetProperty("meta").GetString())
            .Null()
            .When(x => x.FhirResource.RootElement.TryGetProperty("meta", out _))
            .WithMessage("Resource Meta should not be provided for create operations");
    }

    private bool IsValidFhirResourceType(string resourceType)
    {
        var validTypes = new[] { "Patient", "Observation", "Condition", "Encounter", "Medication", "Procedure" };
        return validTypes.Contains(resourceType);
    }

    private async Task<bool> ValidateFhirResourceAsync(JsonDocument resource, CancellationToken cancellationToken)
    {
        try
        {
            var jsonString = resource.RootElement.GetRawText();
            var fhirResource = FhirJsonParser.ParseResourceFromJson<Resource>(jsonString);
            var result = await _fhirValidator.ValidateAsync(fhirResource, cancellationToken);
            return result.IsValid;
        }
        catch
        {
            return false;
        }
    }
}

public class UpdateFhirResourceCommandValidator : AbstractValidator<UpdateFhirResourceCommand>
{
    private readonly FhirValidator _fhirValidator;

    public UpdateFhirResourceCommandValidator(FhirValidator fhirValidator)
    {
        _fhirValidator = fhirValidator;

        RuleFor(x => x.ResourceType)
            .NotEmpty()
            .WithMessage("Resource type is required")
            .Must(IsValidFhirResourceType)
            .WithMessage("Invalid FHIR resource type");

        RuleFor(x => x.FhirId)
            .NotEmpty()
            .WithMessage("FHIR ID is required for update operations");

        RuleFor(x => x.FhirResource)
            .NotNull()
            .WithMessage("FHIR resource is required")
            .MustAsync(ValidateFhirResourceAsync)
            .WithMessage("Invalid FHIR resource");

        RuleFor(x => x.FhirResource.RootElement.GetProperty("id").GetString())
            .Equal(x => x.FhirId)
            .When(x => x.FhirResource.RootElement.TryGetProperty("id", out _))
            .WithMessage("Resource ID must match the URL parameter");

        RuleFor(x => x.FhirResource.RootElement.GetProperty("meta").GetString())
            .NotNull()
            .When(x => x.FhirResource.RootElement.TryGetProperty("meta", out _))
            .WithMessage("Resource Meta is required for update operations");
    }

    private bool IsValidFhirResourceType(string resourceType)
    {
        var validTypes = new[] { "Patient", "Observation", "Condition", "Encounter", "Medication", "Procedure" };
        return validTypes.Contains(resourceType);
    }

    private async Task<bool> ValidateFhirResourceAsync(JsonDocument resource, CancellationToken cancellationToken)
    {
        try
        {
            var jsonString = resource.RootElement.GetRawText();
            var fhirResource = FhirJsonParser.ParseResourceFromJson<Resource>(jsonString);
            var result = await _fhirValidator.ValidateAsync(fhirResource, cancellationToken);
            return result.IsValid;
        }
        catch
        {
            return false;
        }
    }
}

### FHIR R4B Handler Implementation

```csharp
public class CreateFhirResourceCommandHandler : IRequestHandler<CreateFhirResourceCommand, Resource>
{
    private readonly IFhirResourceRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateFhirResourceCommandHandler> _logger;

    public CreateFhirResourceCommandHandler(
        IFhirResourceRepository repository,
        ICurrentUserService currentUserService,
        ILogger<CreateFhirResourceCommandHandler> logger)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Resource> Handle(CreateFhirResourceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Parse FHIR resource from JSON
            var jsonString = request.FhirResource.RootElement.GetRawText();
            var fhirResource = FhirJsonParser.ParseResourceFromJson<Resource>(jsonString);

            // Generate FHIR ID if not provided
            if (string.IsNullOrEmpty(fhirResource.Id))
            {
                fhirResource.Id = Guid.NewGuid().ToString();
            }

            // Set FHIR Meta
            fhirResource.Meta = new Meta
            {
                VersionId = "1",
                LastUpdated = DateTimeOffset.UtcNow,
                Profile = new[] { $"http://hl7.org/fhir/StructureDefinition/{request.ResourceType}" }
            };

            // Save to database
            var savedResource = await _repository.AddAsync(request.ResourceType, fhirResource, cancellationToken);

            _logger.LogInformation("Created FHIR {ResourceType} {FhirId} for tenant {TenantId}", 
                request.ResourceType, savedResource.Id, _currentUserService.TenantId);

            return savedResource;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating FHIR {ResourceType}", request.ResourceType);
            throw;
        }
    }
}

public class GetFhirResourceQueryHandler : IRequestHandler<GetFhirResourceQuery, Resource?>
{
    private readonly IFhirResourceRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GetFhirResourceQueryHandler> _logger;

    public GetFhirResourceQueryHandler(
        IFhirResourceRepository repository,
        ICurrentUserService currentUserService,
        ILogger<GetFhirResourceQueryHandler> logger)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Resource?> Handle(GetFhirResourceQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var resource = await _repository.GetByFhirIdAsync(request.ResourceType, request.FhirId, cancellationToken);
            
            if (resource == null)
            {
                return null;
            }

            _logger.LogInformation("Retrieved FHIR {ResourceType} {FhirId} for tenant {TenantId}", 
                request.ResourceType, request.FhirId, _currentUserService.TenantId);

            return resource;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving FHIR {ResourceType} {FhirId}", request.ResourceType, request.FhirId);
            throw;
        }
    }
}

public class SearchFhirResourcesQueryHandler : IRequestHandler<SearchFhirResourcesQuery, Bundle>
{
    private readonly IFhirResourceRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<SearchFhirResourcesQueryHandler> _logger;

    public SearchFhirResourcesQueryHandler(
        IFhirResourceRepository repository,
        ICurrentUserService currentUserService,
        ILogger<SearchFhirResourcesQueryHandler> logger)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Bundle> Handle(SearchFhirResourcesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var bundle = await _repository.SearchAsync(
                request.ResourceType, 
                request.SearchParameters, 
                request.Skip, 
                request.Take, 
                cancellationToken);

            _logger.LogInformation("Searched FHIR {ResourceType} for tenant {TenantId}, found {Count} resources", 
                request.ResourceType, _currentUserService.TenantId, bundle.Entry?.Count ?? 0);

            return bundle;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching FHIR {ResourceType}", request.ResourceType);
            throw;
        }
    }
}
```

### Healthcare Data Security

```csharp
public class HealthcareDataSecurityService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<HealthcareDataSecurityService> _logger;

    public HealthcareDataSecurityService(
        ICurrentUserService currentUserService,
        ILogger<HealthcareDataSecurityService> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public bool CanAccessPatientData(Guid patientId, Guid tenantId)
    {
        // Check if user belongs to the same tenant
        if (_currentUserService.TenantId != tenantId)
        {
            _logger.LogWarning("User {UserId} attempted to access patient {PatientId} from different tenant {TenantId}", 
                _currentUserService.UserId, patientId, tenantId);
            return false;
        }

        // Check user permissions for healthcare data
        var hasPermission = _currentUserService.HasPermission("patient.read");
        if (!hasPermission)
        {
            _logger.LogWarning("User {UserId} does not have permission to read patient data", _currentUserService.UserId);
            return false;
        }

        return true;
    }

    public bool CanModifyPatientData(Guid patientId, Guid tenantId)
    {
        if (!CanAccessPatientData(patientId, tenantId))
            return false;

        // Check if user has write permission
        var hasWritePermission = _currentUserService.HasPermission("patient.write");
        if (!hasWritePermission)
        {
            _logger.LogWarning("User {UserId} does not have permission to modify patient data", _currentUserService.UserId);
            return false;
        }

        return true;
    }

    public void LogDataAccess(Guid patientId, string action, string details)
    {
        _logger.LogInformation("Healthcare data access: User {UserId} performed {Action} on patient {PatientId}. Details: {Details}", 
            _currentUserService.UserId, action, patientId, details);
    }
}
```

### Healthcare Data Audit Trail

```csharp
public class HealthcareDataAuditService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<HealthcareDataAuditService> _logger;

    public HealthcareDataAuditService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<HealthcareDataAuditService> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task LogDataAccessAsync(string resourceType, Guid resourceId, string action, string details, CancellationToken cancellationToken = default)
    {
        var auditEvent = new AuditEvent
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            UserId = _currentUserService.UserId,
            TenantId = _currentUserService.TenantId,
            ResourceType = resourceType,
            ResourceId = resourceId,
            Action = action,
            Details = details,
            IpAddress = _currentUserService.IpAddress,
            UserAgent = _currentUserService.UserAgent
        };

        _context.AuditEvents.Add(auditEvent);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Healthcare data audit: {Action} on {ResourceType} {ResourceId} by user {UserId}", 
            action, resourceType, resourceId, _currentUserService.UserId);
    }

    public async Task<IEnumerable<AuditEvent>> GetAuditTrailAsync(Guid resourceId, CancellationToken cancellationToken = default)
    {
        return await _context.AuditEvents
            .Where(ae => ae.ResourceId == resourceId)
            .OrderByDescending(ae => ae.Timestamp)
            .ToListAsync(cancellationToken);
    }
}
```

## File Organization

### FHIR R4B Structure
```
HealthTech.Domain/
├── Entities/
│   └── FhirResource.cs               # FHIR R4B entities
├── Repositories/
│   └── IFhirResourceRepository.cs    # FHIR repository interfaces
└── Services/
    ├── IFhirValidationService.cs     # FHIR R4B validation service
    ├── IFhirSecurityService.cs       # FHIR security service
    └── IFhirAuditService.cs          # FHIR audit service

HealthTech.Application/
├── FhirResources/
│   ├── Commands/
│   │   ├── CreateFhirResourceCommand.cs
│   │   ├── UpdateFhirResourceCommand.cs
│   │   ├── DeleteFhirResourceCommand.cs
│   │   ├── CreateFhirResourceCommandHandler.cs
│   │   ├── UpdateFhirResourceCommandHandler.cs
│   │   ├── DeleteFhirResourceCommandHandler.cs
│   │   ├── CreateFhirResourceCommandValidator.cs
│   │   └── UpdateFhirResourceCommandValidator.cs
│   ├── Queries/
│   │   ├── GetFhirResourceQuery.cs
│   │   ├── SearchFhirResourcesQuery.cs
│   │   ├── GetFhirResourceHistoryQuery.cs
│   │   ├── GetFhirResourceQueryHandler.cs
│   │   ├── SearchFhirResourcesQueryHandler.cs
│   │   └── GetFhirResourceHistoryQueryHandler.cs
│   └── Operations/
│       ├── ImportFhirBundleCommand.cs
│       ├── ExportFhirBundleQuery.cs
│       ├── ImportFhirBundleCommandHandler.cs
│       └── ExportFhirBundleQueryHandler.cs
└── Common/
    ├── FhirValidation/
    │   ├── FhirValidator.cs          # FHIR R4B validator
    │   └── FhirValidationBehavior.cs # FHIR validation behavior
    └── FhirSecurity/
        ├── FhirSecurityService.cs    # FHIR security service
        └── FhirAuditService.cs       # FHIR audit service

HealthTech.API/
├── Endpoints/
│   └── FhirEndpoints.cs              # FHIR standard endpoints (generic)
└── Middleware/
    ├── FhirExceptionMiddleware.cs    # FHIR error handling
    └── FhirSecurityMiddleware.cs     # FHIR security middleware
```

## Immutable FHIR R4B Standards (Never Change)

### 1. FHIR R4B Compliance (Immutable Rule)
- **FHIR R4B Resources**: All healthcare data must use Hl7.Fhir.R4B
- **Resource Types**: Use standard FHIR R4B resource types (Patient, Observation, etc.)
- **Data Types**: Use FHIR R4B data types (HumanName, Address, ContactPoint, etc.)
- **Extensions**: Use FHIR R4B extensions for custom data
- **Local SDK**: Use local Hl7.Fhir.R4B SDK (readonly clone)

### 2. Healthcare Data Security (Immutable Rule)
- **PHI Protection**: Personal Health Information must be encrypted
- **Access Control**: Role-based access control (RBAC)
- **Audit Trail**: Complete audit trail for all healthcare data access
- **Data Minimization**: Only collect necessary healthcare data
- **Consent Management**: Patient consent tracking and management

### 3. Multi-Tenancy (Immutable Rule)
- **Tenant Isolation**: Healthcare data must be isolated by tenant
- **Row-Level Security**: Database-level tenant isolation
- **Cross-Tenant Access**: Controlled cross-tenant data sharing
- **Tenant Hierarchy**: Support for healthcare organization hierarchies

### 4. Data Integrity (Immutable Rule)
- **Versioning**: All healthcare data must be versioned
- **Immutable History**: Healthcare data history cannot be modified
- **Data Validation**: Comprehensive validation of healthcare data
- **Reference Integrity**: Maintain referential integrity across FHIR resources

### 5. Privacy and Consent (Immutable Rule)
- **Consent Tracking**: Track patient consent for data usage
- **Data Classification**: Classify data by sensitivity level
- **Access Logging**: Log all access to healthcare data
- **Data Retention**: Implement appropriate data retention policies

### 6. FHIR R4B Endpoint Standards (Immutable Rule)
- **FHIR Endpoints**: Use Minimal API endpoints with FHIR R4B resources
- **Route Structure**: Follow FHIR standard routes (/fhir/{ResourceType})
- **HTTP Methods**: Use standard FHIR HTTP methods (GET, POST, PUT, DELETE)
- **Response Format**: Return FHIR R4B resources or OperationOutcome
- **Search Parameters**: Implement FHIR standard search parameters
- **Versioning**: Support FHIR resource versioning
- **History**: Support FHIR resource history endpoints

### 7. FHIR Route Standards (Immutable Rule)
- **Base Path**: All FHIR endpoints must start with `/fhir`
- **Resource Endpoints**: `/fhir/{resourceType}` for resource operations
- **Instance Endpoints**: `/fhir/{resourceType}/{id}` for specific resource instances
- **History Endpoints**: `/fhir/{resourceType}/{id}/_history` for resource version history
- **System Endpoints**: `/fhir/$operation` for system-wide operations
- **Resource Type Endpoints**: `/fhir/{resourceType}/$operation` for resource-type specific operations

## Anti-Patterns to Avoid (Never Allowed)

1. **Non-FHIR R4B Data**: Storing healthcare data in non-FHIR R4B format
2. **Controller Pattern**: Using controllers instead of Minimal API endpoints
3. **Non-FHIR Routes**: Using non-standard FHIR route patterns
4. **Non-FHIR Responses**: Returning non-FHIR R4B resources or DTOs
5. **No FHIR Validation**: Healthcare data without FHIR R4B validation
6. **No Security**: Healthcare data without proper security measures
7. **No Audit Trail**: Healthcare data access without logging
8. **No Consent**: Using healthcare data without patient consent
9. **Cross-Tenant Leakage**: Healthcare data accessible across tenants
10. **No Versioning**: Healthcare data without FHIR versioning
11. **No Encryption**: PHI stored without encryption
12. **No Access Control**: Healthcare data without access controls
13. **No Data Classification**: Healthcare data without sensitivity classification
14. **External FHIR SDK**: Using external FHIR SDK instead of local clone

## Validation Checklist (Must Pass Always)

- [ ] All healthcare data uses FHIR R4B format
- [ ] FHIR endpoints follow standard FHIR route patterns
- [ ] FHIR R4B validation is implemented
- [ ] Minimal API endpoints are used (not controllers)
- [ ] FHIR R4B resources are returned (not DTOs)
- [ ] Local Hl7.Fhir.R4B SDK is used
- [ ] PHI is properly encrypted
- [ ] Access controls are implemented
- [ ] Audit trail is maintained
- [ ] Patient consent is tracked
- [ ] Multi-tenancy is enforced
- [ ] FHIR versioning is implemented
- [ ] Data classification is applied
- [ ] Security measures are in place

## Performance Considerations

1. **Indexing**: Proper indexing for healthcare data queries
2. **Caching**: Appropriate caching for frequently accessed data
3. **Pagination**: Pagination for large healthcare datasets
4. **Search Optimization**: Optimized search for healthcare data
5. **Data Archiving**: Archive old healthcare data appropriately
6. **Backup Strategy**: Comprehensive backup for healthcare data

## Security Implementation

1. **Encryption**: Encrypt all PHI at rest and in transit
2. **Access Control**: Implement role-based access control
3. **Audit Logging**: Log all healthcare data access
4. **Data Masking**: Mask sensitive data in logs
5. **Consent Management**: Track and enforce patient consent
6. **Data Classification**: Classify data by sensitivity level

---

**This document defines the immutable FHIR R4B Pattern standards that should NEVER change, ensuring strict compliance with HL7 FHIR standards and consistency across the FHIR-AI Backend project.**
