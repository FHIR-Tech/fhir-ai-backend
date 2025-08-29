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

#### FHIR Patient Endpoint (Minimal API)
```csharp
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;

public static class PatientEndpoints
{
    public static void MapPatientEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/fhir/Patient")
            .WithTags("FHIR Patient")
            .WithOpenApi();

        // GET /fhir/Patient/{id} - Read Patient
        group.MapGet("/{id}", async (
            string id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPatientQuery { FhirId = id };
            var result = await mediator.Send(query, cancellationToken);
            
            if (!result.IsSuccess)
                return Results.NotFound(new OperationOutcome
                {
                    Issue = new List<OperationOutcome.IssueComponent>
                    {
                        new()
                        {
                            Severity = OperationOutcome.IssueSeverity.Error,
                            Code = OperationOutcome.IssueType.NotFound,
                            Diagnostics = result.Error
                        }
                    }
                });

            return Results.Ok(result.Value);
        })
        .WithName("GetPatient")
        .WithSummary("Read Patient")
        .WithDescription("Retrieve a Patient resource by ID")
        .Produces<Patient>(200)
        .Produces<OperationOutcome>(404);

        // POST /fhir/Patient - Create Patient
        group.MapPost("/", async (
            [FromBody] Patient patient,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new CreatePatientCommand { FhirPatient = patient };
            var result = await mediator.Send(command, cancellationToken);
            
            if (!result.IsSuccess)
                return Results.BadRequest(new OperationOutcome
                {
                    Issue = new List<OperationOutcome.IssueComponent>
                    {
                        new()
                        {
                            Severity = OperationOutcome.IssueSeverity.Error,
                            Code = OperationOutcome.IssueType.Invalid,
                            Diagnostics = result.Error
                        }
                    }
                });

            return Results.Created($"/fhir/Patient/{result.Value.Id}", result.Value);
        })
        .WithName("CreatePatient")
        .WithSummary("Create Patient")
        .WithDescription("Create a new Patient resource")
        .Produces<Patient>(201)
        .Produces<OperationOutcome>(400);

        // PUT /fhir/Patient/{id} - Update Patient
        group.MapPut("/{id}", async (
            string id,
            [FromBody] Patient patient,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdatePatientCommand 
            { 
                FhirId = id, 
                FhirPatient = patient 
            };
            var result = await mediator.Send(command, cancellationToken);
            
            if (!result.IsSuccess)
                return Results.BadRequest(new OperationOutcome
                {
                    Issue = new List<OperationOutcome.IssueComponent>
                    {
                        new()
                        {
                            Severity = OperationOutcome.IssueSeverity.Error,
                            Code = OperationOutcome.IssueType.Invalid,
                            Diagnostics = result.Error
                        }
                    }
                });

            return Results.Ok(result.Value);
        })
        .WithName("UpdatePatient")
        .WithSummary("Update Patient")
        .WithDescription("Update an existing Patient resource")
        .Produces<Patient>(200)
        .Produces<OperationOutcome>(400);

        // DELETE /fhir/Patient/{id} - Delete Patient
        group.MapDelete("/{id}", async (
            string id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new DeletePatientCommand { FhirId = id };
            var result = await mediator.Send(command, cancellationToken);
            
            if (!result.IsSuccess)
                return Results.NotFound(new OperationOutcome
                {
                    Issue = new List<OperationOutcome.IssueComponent>
                    {
                        new()
                        {
                            Severity = OperationOutcome.IssueSeverity.Error,
                            Code = OperationOutcome.IssueType.NotFound,
                            Diagnostics = result.Error
                        }
                    }
                });

            return Results.NoContent();
        })
        .WithName("DeletePatient")
        .WithSummary("Delete Patient")
        .WithDescription("Delete a Patient resource")
        .Produces(204)
        .Produces<OperationOutcome>(404);

        // GET /fhir/Patient - Search Patients
        group.MapGet("/", async (
            [FromQuery] string? name,
            [FromQuery] string? identifier,
            [FromQuery] string? birthdate,
            [FromQuery] string? gender,
            [FromQuery] int? _count,
            [FromQuery] string? _offset,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new SearchPatientsQuery
            {
                Name = name,
                Identifier = identifier,
                Birthdate = birthdate,
                Gender = gender,
                Count = _count ?? 10,
                Offset = _offset
            };
            
            var result = await mediator.Send(query, cancellationToken);
            
            if (!result.IsSuccess)
                return Results.BadRequest(new OperationOutcome
                {
                    Issue = new List<OperationOutcome.IssueComponent>
                    {
                        new()
                        {
                            Severity = OperationOutcome.IssueSeverity.Error,
                            Code = OperationOutcome.IssueType.Invalid,
                            Diagnostics = result.Error
                        }
                    }
                });

            return Results.Ok(result.Value);
        })
        .WithName("SearchPatients")
        .WithSummary("Search Patients")
        .WithDescription("Search for Patient resources")
        .Produces<Bundle>(200)
        .Produces<OperationOutcome>(400);
    }
}

### FHIR R4B Commands and Queries

#### FHIR Patient Commands
```csharp
using Hl7.Fhir.Model;

public record CreatePatientCommand : IRequest<Result<Patient>>
{
    public Hl7.Fhir.Model.Patient FhirPatient { get; init; } = new();
}

public record UpdatePatientCommand : IRequest<Result<Patient>>
{
    public string FhirId { get; init; } = string.Empty;
    public Hl7.Fhir.Model.Patient FhirPatient { get; init; } = new();
}

public record DeletePatientCommand : IRequest<Result<bool>>
{
    public string FhirId { get; init; } = string.Empty;
}

public record GetPatientQuery : IRequest<Result<Patient>>
{
    public string FhirId { get; init; } = string.Empty;
}

public record SearchPatientsQuery : IRequest<Result<Bundle>>
{
    public string? Name { get; init; }
    public string? Identifier { get; init; }
    public string? Birthdate { get; init; }
    public string? Gender { get; init; }
    public int Count { get; init; } = 10;
    public string? Offset { get; init; }
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
public interface IPatientRepository
{
    // FHIR CRUD Operations
    Task<Patient?> GetByFhirIdAsync(string fhirId, CancellationToken cancellationToken = default);
    Task<Patient> AddAsync(Patient patient, CancellationToken cancellationToken = default);
    Task<Patient> UpdateAsync(Patient patient, CancellationToken cancellationToken = default);
    Task DeleteAsync(string fhirId, CancellationToken cancellationToken = default);
    
    // FHIR Search Operations
    Task<IEnumerable<Patient>> SearchAsync(
        string? name = null,
        string? identifier = null,
        string? birthdate = null,
        string? gender = null,
        int count = 10,
        string? offset = null,
        CancellationToken cancellationToken = default);
    
    // FHIR History Operations
    Task<IEnumerable<Patient>> GetHistoryAsync(string fhirId, CancellationToken cancellationToken = default);
    
    // FHIR Validation
    Task<bool> ValidateFhirResourceAsync(Hl7.Fhir.Model.Patient patient, CancellationToken cancellationToken = default);
    
    // Multi-tenancy
    Task<IEnumerable<Patient>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}

### FHIR R4B Validation

```csharp
using Hl7.Fhir.Model;
using Hl7.Fhir.Validation;

public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
{
    private readonly FhirValidator _fhirValidator;

    public CreatePatientCommandValidator(FhirValidator fhirValidator)
    {
        _fhirValidator = fhirValidator;

        RuleFor(x => x.FhirPatient)
            .NotNull()
            .WithMessage("FHIR Patient resource is required")
            .MustAsync(ValidateFhirPatientAsync)
            .WithMessage("Invalid FHIR Patient resource");

        RuleFor(x => x.FhirPatient.Id)
            .Empty()
            .WithMessage("Patient ID should not be provided for create operations");

        RuleFor(x => x.FhirPatient.Meta)
            .Null()
            .WithMessage("Patient Meta should not be provided for create operations");
    }

    private async Task<bool> ValidateFhirPatientAsync(Hl7.Fhir.Model.Patient patient, CancellationToken cancellationToken)
    {
        if (patient == null) return false;

        try
        {
            var result = await _fhirValidator.ValidateAsync(patient, cancellationToken);
            return result.IsValid;
        }
        catch
        {
            return false;
        }
    }
}

public class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
{
    private readonly FhirValidator _fhirValidator;

    public UpdatePatientCommandValidator(FhirValidator fhirValidator)
    {
        _fhirValidator = fhirValidator;

        RuleFor(x => x.FhirId)
            .NotEmpty()
            .WithMessage("FHIR ID is required for update operations");

        RuleFor(x => x.FhirPatient)
            .NotNull()
            .WithMessage("FHIR Patient resource is required")
            .MustAsync(ValidateFhirPatientAsync)
            .WithMessage("Invalid FHIR Patient resource");

        RuleFor(x => x.FhirPatient.Id)
            .Equal(x => x.FhirId)
            .WithMessage("Patient ID must match the URL parameter");

        RuleFor(x => x.FhirPatient.Meta)
            .NotNull()
            .WithMessage("Patient Meta is required for update operations");
    }

    private async Task<bool> ValidateFhirPatientAsync(Hl7.Fhir.Model.Patient patient, CancellationToken cancellationToken)
    {
        if (patient == null) return false;

        try
        {
            var result = await _fhirValidator.ValidateAsync(patient, cancellationToken);
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
public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Result<Patient>>
{
    private readonly IPatientRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreatePatientCommandHandler> _logger;

    public CreatePatientCommandHandler(
        IPatientRepository repository,
        ICurrentUserService currentUserService,
        ILogger<CreatePatientCommandHandler> logger)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<Patient>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Generate FHIR ID if not provided
            if (string.IsNullOrEmpty(request.FhirPatient.Id))
            {
                request.FhirPatient.Id = Guid.NewGuid().ToString();
            }

            // Set FHIR Meta
            request.FhirPatient.Meta = new Meta
            {
                VersionId = "1",
                LastUpdated = DateTimeOffset.UtcNow,
                Profile = new[] { "http://hl7.org/fhir/StructureDefinition/Patient" }
            };

            // Create domain entity
            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                TenantId = _currentUserService.TenantId,
                CreatedBy = _currentUserService.UserId,
                CreatedAt = DateTime.UtcNow
            };

            // Set FHIR resource
            patient.SetFhirPatient(request.FhirPatient);

            // Save to database
            var savedPatient = await _repository.AddAsync(patient, cancellationToken);

            _logger.LogInformation("Created FHIR Patient {FhirId} for tenant {TenantId}", 
                savedPatient.FhirId, _currentUserService.TenantId);

            return Result<Patient>.Success(savedPatient.FhirPatient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating FHIR Patient");
            return Result<Patient>.Failure("Failed to create patient");
        }
    }
}

public class GetPatientQueryHandler : IRequestHandler<GetPatientQuery, Result<Patient>>
{
    private readonly IPatientRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GetPatientQueryHandler> _logger;

    public GetPatientQueryHandler(
        IPatientRepository repository,
        ICurrentUserService currentUserService,
        ILogger<GetPatientQueryHandler> logger)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<Patient>> Handle(GetPatientQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var patient = await _repository.GetByFhirIdAsync(request.FhirId, cancellationToken);
            
            if (patient == null)
            {
                return Result<Patient>.Failure("Patient not found");
            }

            // Check tenant access
            if (patient.TenantId != _currentUserService.TenantId)
            {
                _logger.LogWarning("User {UserId} attempted to access patient {FhirId} from different tenant {TenantId}", 
                    _currentUserService.UserId, request.FhirId, patient.TenantId);
                return Result<Patient>.Failure("Access denied");
            }

            return Result<Patient>.Success(patient.FhirPatient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving FHIR Patient {FhirId}", request.FhirId);
            return Result<Patient>.Failure("Failed to retrieve patient");
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
└── Common/
    ├── FhirValidation/
    │   ├── FhirValidator.cs          # FHIR R4B validator
    │   └── FhirValidationBehavior.cs # FHIR validation behavior
    └── FhirSecurity/
        ├── FhirSecurityService.cs    # FHIR security service
        └── FhirAuditService.cs       # FHIR audit service

HealthTech.API/
├── Endpoints/
│   └── FhirEndpoints.cs              # FHIR endpoints
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
