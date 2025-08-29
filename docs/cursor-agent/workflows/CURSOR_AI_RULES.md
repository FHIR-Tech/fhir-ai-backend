# Cursor AI Development Rules

## Overview

This document defines the mandatory rules and standards that Cursor AI must follow when working on the FHIR-AI Backend project. These rules ensure consistency, quality, and maintainability across all development activities.

## Domain Entity Standards

### Mandatory Field Organization

When creating or modifying domain entities, Cursor AI MUST follow this exact field ordering pattern:

```csharp
public class EntityName : BaseEntity
{
    // ========================================
    // FOREIGN KEY FIELDS
    // ========================================
    
    // ========================================
    // CORE IDENTITY FIELDS
    // ========================================
    
    // ========================================
    // BASIC INFORMATION FIELDS
    // ========================================
    
    // ========================================
    // STATUS & CONFIGURATION FIELDS
    // ========================================
    
    // ========================================
    // SECURITY & ACCESS FIELDS
    // ========================================
    
    // ========================================
    // TIMING FIELDS
    // ========================================
    
    // ========================================
    // ADDITIONAL DATA FIELDS
    // ========================================
    
    // ========================================
    // COMPUTED PROPERTIES
    // ========================================
    
    // ========================================
    // NAVIGATION PROPERTIES
    // ========================================
}
```

### Visual Separators Format
- Use exactly 39 equals signs (`=`) on each line
- Section name in UPPERCASE
- One blank line before and after each section
- Consistent 4-space indentation

### Field Documentation
Every field MUST have XML documentation:
```csharp
/// <summary>
/// Brief description of the field
/// </summary>
public string FieldName { get; set; }
```

## Architecture Principles

### Clean Architecture Pattern - Comprehensive Implementation Guide

#### API Layer Pattern Recognition (Critical Decision Matrix)
Cursor AI MUST automatically determine the correct API pattern based on the feature type:

**1. FHIR Healthcare Resources → Minimal API (MANDATORY)**
- **Trigger Keywords**: `Patient`, `Observation`, `Medication`, `Condition`, `Encounter`, `FHIR`, `healthcare`, `medical`, `clinical`
- **Pattern**: Minimal API Endpoints
- **Location**: `HealthTech.API/Endpoints/FhirEndpoints.cs`
- **Routes**: `/fhir/{resourceType}`, `/fhir/{resourceType}/{id}`
- **Reason**: FHIR standards require specific route patterns and response formats

**2. Business/System Resources → Controller (RECOMMENDED)**
- **Trigger Keywords**: `tenant`, `user`, `news`, `notification`, `report`, `audit`, `configuration`, `system`, `admin`
- **Pattern**: Traditional Controller
- **Location**: `HealthTech.API/Controllers/{Entity}Controller.cs`
- **Routes**: `/api/{entity}`, `/api/{entity}/{id}`
- **Reason**: Standard RESTful patterns, easier CRUD operations

**3. Special Operations → Minimal API (FLEXIBLE)**
- **Trigger Keywords**: `authentication`, `health`, `export`, `import`, `batch`, `bulk`
- **Pattern**: Minimal API Endpoints
- **Location**: `HealthTech.API/Endpoints/{Feature}Endpoints.cs`
- **Routes**: `/auth/*`, `/health/*`, `/export/*`
- **Reason**: Specialized operations with custom logic

**File Organization Structure**:
```
HealthTech.API/
├── Controllers/                    # Business/System Controllers
│   ├── TenantController.cs        # Tenant management
│   ├── UserController.cs          # User management  
│   ├── NewsController.cs          # News/Content management
│   ├── ReportController.cs        # Reports/Analytics
│   ├── ConfigurationController.cs # System configuration
│   └── AuditController.cs         # Audit/Logging
├── Endpoints/                     # FHIR & Special Endpoints
│   ├── FhirEndpoints.cs          # FHIR resources (Minimal API)
│   ├── AuthenticationEndpoints.cs # Auth flows (Minimal API)
│   ├── HealthEndpoints.cs        # Health checks (Minimal API)
│   └── ExportEndpoints.cs        # Export operations (Minimal API)
└── Middleware/                    # Cross-cutting concerns
    ├── ExceptionMiddleware.cs     # Global exception handling
    ├── ValidationMiddleware.cs    # Request validation
    └── SecurityMiddleware.cs      # Security & authentication
```

#### Core Principles (Immutable Standards)
1. **Dependency Rule**: Dependencies point inward only
2. **Abstraction Rule**: Inner layers define abstractions, outer layers implement them
3. **Independence Rule**: Domain layer has zero dependencies on external concerns
4. **Framework Independence**: Domain and Application layers are framework-agnostic

#### Layer Structure & Responsibilities

##### 1. Domain Layer (Innermost - No Dependencies)
**Purpose**: Core business logic and enterprise rules
**Location**: `HealthTech.Domain/`
**Dependencies**: None (zero external dependencies)

**Components**:
- **Entities**: Core business objects with identity
- **Value Objects**: Immutable objects without identity
- **Aggregates**: Clusters of related entities
- **Domain Services**: Business logic that doesn't belong to entities
- **Repositories (Interfaces)**: Data access abstractions
- **Domain Events**: Business events that occurred
- **Enums**: Business enumerations
- **Exceptions**: Domain-specific exceptions

**Naming Pattern**: `HealthTech.Domain.{Feature}`
**File Structure**:
```
HealthTech.Domain/
├── Entities/
│   ├── BaseEntity.cs
│   ├── Patient.cs
│   └── {EntityName}.cs
├── ValueObjects/
│   ├── Email.cs
│   └── {ValueObjectName}.cs
├── Aggregates/
│   ├── PatientAggregate.cs
│   └── {AggregateName}.cs
├── Repositories/
│   ├── IPatientRepository.cs
│   └── I{EntityName}Repository.cs
├── Services/
│   ├── IPatientDomainService.cs
│   └── I{DomainServiceName}.cs
├── Events/
│   ├── PatientCreatedEvent.cs
│   └── {EventName}.cs
├── Enums/
│   ├── PatientStatus.cs
│   └── {EnumName}.cs
└── Exceptions/
    ├── DomainException.cs
    └── {ExceptionName}.cs
```

##### 2. Application Layer (Business Use Cases)
**Purpose**: Application-specific business rules and orchestration
**Location**: `HealthTech.Application/`
**Dependencies**: Domain layer only

**Components**:
- **Commands**: Write operations (Create, Update, Delete)
- **Queries**: Read operations
- **Handlers**: Command and Query handlers
- **Validators**: Input validation using FluentValidation
- **DTOs**: Data Transfer Objects for API communication
- **Mappers**: Object mapping between layers
- **Services**: Application-specific business logic
- **Behaviors**: Cross-cutting concerns (logging, validation, caching)

**Naming Pattern**: `HealthTech.Application.{Feature}`
**File Structure**:
```
HealthTech.Application/
├── Common/
│   ├── Behaviors/
│   │   ├── ValidationBehavior.cs
│   │   ├── LoggingBehavior.cs
│   │   └── CachingBehavior.cs
│   ├── Interfaces/
│   │   ├── ICurrentUserService.cs
│   │   └── IDateTimeService.cs
│   ├── Models/
│   │   ├── Result.cs
│   │   └── PaginatedResult.cs
│   └── Exceptions/
│       └── ApplicationException.cs
├── {Feature}/
│   ├── Commands/
│   │   ├── Create{Entity}/
│   │   │   ├── Create{Entity}Command.cs
│   │   │   ├── Create{Entity}CommandHandler.cs
│   │   │   └── Create{Entity}CommandValidator.cs
│   │   └── Update{Entity}/
│   ├── Queries/
│   │   ├── Get{Entity}/
│   │   │   ├── Get{Entity}Query.cs
│   │   │   ├── Get{Entity}QueryHandler.cs
│   │   │   └── Get{Entity}QueryValidator.cs
│   │   └── Get{Entity}List/
│   ├── DTOs/
│   │   ├── {Entity}Dto.cs
│   │   └── Create{Entity}Request.cs
│   └── Mappers/
│       └── {Entity}Mapper.cs
└── DependencyInjection.cs
```

##### 3. Infrastructure Layer (External Concerns)
**Purpose**: External system interactions and data persistence
**Location**: `HealthTech.Infrastructure/`
**Dependencies**: Application and Domain layers

**Components**:
- **Persistence**: Database context, configurations, migrations
- **Repositories**: Concrete implementations of domain repositories
- **External Services**: Third-party API integrations
- **Authentication**: Identity and authorization implementations
- **Caching**: Cache implementations
- **Logging**: Logging implementations
- **File Storage**: File system or cloud storage implementations
- **Message Brokers**: Event bus implementations

**Naming Pattern**: `HealthTech.Infrastructure.{Concern}`
**File Structure**:
```
HealthTech.Infrastructure/
├── Persistence/
│   ├── ApplicationDbContext.cs
│   ├── Configurations/
│   │   ├── PatientConfiguration.cs
│   │   └── {Entity}Configuration.cs
│   ├── Repositories/
│   │   ├── PatientRepository.cs
│   │   └── {Entity}Repository.cs
│   └── Migrations/
├── Authentication/
│   ├── JwtTokenService.cs
│   ├── CurrentUserService.cs
│   └── AuthorizationService.cs
├── External/
│   ├── FhirApiClient.cs
│   └── {ExternalService}Client.cs
├── Caching/
│   ├── RedisCacheService.cs
│   └── MemoryCacheService.cs
├── Logging/
│   └── SerilogService.cs
├── FileStorage/
│   └── AzureBlobStorageService.cs
└── DependencyInjection.cs
```

##### 4. API Layer (Presentation)
**Purpose**: HTTP API endpoints and request/response handling
**Location**: `HealthTech.API/`
**Dependencies**: Application and Infrastructure layers

**Components**:
- **Endpoints**: Minimal API endpoints
- **Controllers**: Traditional controllers (if needed)
- **Middleware**: Request/response processing
- **Filters**: Action filters and exception filters
- **Models**: API-specific models and view models
- **Configuration**: API-specific configuration
- **Documentation**: OpenAPI/Swagger documentation

**Naming Pattern**: `HealthTech.API.{Feature}`
**File Structure**:
```
HealthTech.API/
├── Endpoints/
│   ├── PatientEndpoints.cs
│   └── {Entity}Endpoints.cs
├── Controllers/
│   ├── HealthController.cs
│   └── {ControllerName}.cs
├── Middleware/
│   ├── ExceptionHandlingMiddleware.cs
│   ├── RequestLoggingMiddleware.cs
│   └── TenantResolutionMiddleware.cs
├── Filters/
│   ├── ValidationFilter.cs
│   └── AuthorizationFilter.cs
├── Models/
│   ├── ApiResponse.cs
│   └── ErrorResponse.cs
├── Configuration/
│   ├── SwaggerConfiguration.cs
│   └── AuthenticationConfiguration.cs
└── Program.cs
```

#### Dependency Injection Configuration

**Domain Layer**: No DI configuration (no dependencies)
**Application Layer**: Register handlers, validators, behaviors
**Infrastructure Layer**: Register concrete implementations
**API Layer**: Register API-specific services

```csharp
// Application Layer DI
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

// Infrastructure Layer DI
services.AddDbContext<ApplicationDbContext>(options => 
    options.UseNpgsql(connectionString));
services.AddScoped<IPatientRepository, PatientRepository>();
services.AddScoped<ICurrentUserService, CurrentUserService>();

// API Layer DI
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddAuthentication();
services.AddAuthorization();
```

#### CQRS Pattern Implementation

**For complete CQRS implementation details, see `/architecture/CQRS_PATTERN_REFERENCE.md`**

**Pattern Recognition**: Cursor AI automatically applies CQRS for:
- **Business Entities**: `tenant`, `user`, `news`, `notification`, `report`
- **FHIR Resources**: `patient`, `observation`, `medication`, `condition`
- **System Operations**: `audit`, `configuration`, `export`, `import`

**Commands** (Write Operations):
```csharp
public record CreatePatientCommand : IRequest<Result<PatientDto>>
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public DateTime DateOfBirth { get; init; }
}

public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Result<PatientDto>>
{
    private readonly IPatientRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public async Task<Result<PatientDto>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        // Business logic implementation
    }
}
```

**Queries** (Read Operations):
```csharp
public record GetPatientQuery : IRequest<Result<PatientDto>>
{
    public Guid Id { get; init; }
}

public class GetPatientQueryHandler : IRequestHandler<GetPatientQuery, Result<PatientDto>>
{
    private readonly IPatientRepository _repository;

    public async Task<Result<PatientDto>> Handle(GetPatientQuery request, CancellationToken cancellationToken)
    {
        // Query implementation
    }
}
```

#### Validation Pattern

```csharp
public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .LessThan(DateTime.UtcNow);
    }
}
```

#### Result Pattern

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string Error { get; }
    public List<string> ValidationErrors { get; }

    public static Result<T> Success(T value) => new(true, value, null, null);
    public static Result<T> Failure(string error) => new(false, default, error, null);
    public static Result<T> ValidationFailure(List<string> errors) => new(false, default, null, errors);
}
```

#### AutoMapper Pattern Implementation

**For complete AutoMapper implementation details, see `/architecture/AUTOMAPPER_PATTERN_REFERENCE.md`**

**Pattern Recognition**: Cursor AI automatically applies AutoMapper for:
- **Business Entities**: `tenant`, `user`, `news`, `notification`, `report` (Entity ↔ DTO)
- **FHIR Resources**: `patient`, `observation`, `medication`, `condition` (Entity ↔ FHIR Resource)
- **System Operations**: `audit`, `configuration`, `export`, `import` (Entity ↔ DTO)

**Profile Structure**:
```csharp
public class PatientMappingProfile : Profile
{
    public PatientMappingProfile()
    {
        // Domain Entity to DTO mappings
        CreateMap<Patient, PatientDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => CalculateAge(src.DateOfBirth)));

        // DTO to Domain Entity mappings
        CreateMap<CreatePatientRequest, Patient>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore());
    }

    private static int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }
}
```

**Mapper Service**:
```csharp
public interface IMapperService
{
    TDestination Map<TDestination>(object source);
    TDestination Map<TSource, TDestination>(TSource source);
    IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source);
    IQueryable<TDestination> ProjectTo<TSource, TDestination>(IQueryable<TSource> source);
}
```

#### Healthcare Data Implementation Pattern

**For complete Healthcare Data implementation details, see `/architecture/HEALTHCARE_DATA_PATTERN_REFERENCE.md`**

**Pattern Recognition**: Cursor AI automatically applies Healthcare Data Pattern for:
- **Trigger Keywords**: `Patient`, `Observation`, `Medication`, `Condition`, `Encounter`, `FHIR`, `healthcare`, `medical`, `clinical`
- **API Pattern**: Minimal API Endpoints (MANDATORY)
- **Data Format**: FHIR R4B Resources (MANDATORY)
- **SDK**: Local Hl7.Fhir.R4B SDK (MANDATORY)
- **Routes**: `/fhir/{resourceType}` (MANDATORY)
- **Validation**: FHIR R4B Validation (MANDATORY)

**Example**: When user requests "Create Patient API", Cursor AI automatically:
1. Uses Minimal API Endpoints (not Controller)
2. Implements FHIR R4B Patient resource
3. Uses local Hl7.Fhir.R4B SDK
4. Creates routes `/fhir/Patient`
5. Implements FHIR R4B validation

#### Cross-Cutting Concerns

**Logging Behavior**:
```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);
        var response = await next();
        _logger.LogInformation("Handled {RequestName}", typeof(TRequest).Name);
        return response;
    }
}
```

**Validation Behavior**:
```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}
```

#### Testing Structure

**Unit Tests**:
```
HealthTech.Application.Tests/
├── {Feature}/
│   ├── Commands/
│   │   ├── Create{Entity}CommandHandlerTests.cs
│   │   └── Update{Entity}CommandHandlerTests.cs
│   ├── Queries/
│   │   ├── Get{Entity}QueryHandlerTests.cs
│   │   └── Get{Entity}ListQueryHandlerTests.cs
│   └── Validators/
│       ├── Create{Entity}CommandValidatorTests.cs
│       └── Update{Entity}CommandValidatorTests.cs
```

**Integration Tests**:
```
HealthTech.API.Tests/
├── Endpoints/
│   ├── PatientEndpointsTests.cs
│   └── {Entity}EndpointsTests.cs
├── Controllers/
│   └── HealthControllerTests.cs
└── TestBase.cs
```

#### Architecture Validation Rules

1. **No Circular Dependencies**: Use dependency direction validation
2. **Layer Isolation**: Domain layer must have zero external dependencies
3. **Interface Segregation**: Repositories and services must be interface-based
4. **Single Responsibility**: Each class has one reason to change
5. **Open/Closed Principle**: Open for extension, closed for modification
6. **Dependency Inversion**: High-level modules don't depend on low-level modules

#### Performance Considerations

1. **Async/Await**: Use throughout for I/O operations
2. **Caching**: Implement at appropriate layers
3. **Pagination**: For large data sets
4. **Projection**: Use DTOs to limit data transfer
5. **Indexing**: Proper database indexing strategy
6. **Connection Pooling**: Database connection management

#### Security Implementation

1. **Authentication**: JWT tokens with proper validation
2. **Authorization**: Role-based and claim-based authorization
3. **Input Validation**: Comprehensive validation at all layers
4. **Audit Logging**: Track all data modifications
5. **Data Encryption**: Sensitive data encryption at rest and in transit
6. **Rate Limiting**: API rate limiting implementation

#### Immutable Clean Architecture Standards (Never Change)

These standards are fundamental to Clean Architecture and should NEVER be modified, regardless of Microsoft's framework updates or new patterns:

##### 1. Dependency Direction (Immutable Rule)
- **Domain Layer**: Zero external dependencies
- **Application Layer**: Depends only on Domain
- **Infrastructure Layer**: Depends on Application and Domain
- **API Layer**: Depends on Application and Infrastructure
- **Violation**: Any outer layer dependency on inner layers is forbidden

##### 2. Layer Responsibilities (Immutable Rule)
- **Domain**: Pure business logic, no framework code
- **Application**: Use cases and orchestration, no infrastructure concerns
- **Infrastructure**: External system interactions only
- **API**: HTTP concerns only, no business logic

##### 3. Framework Independence (Immutable Rule)
- **Domain Layer**: Must be framework-agnostic
- **Application Layer**: Must be framework-agnostic
- **Infrastructure Layer**: Can use framework-specific code
- **API Layer**: Can use framework-specific code

##### 4. Interface Segregation (Immutable Rule)
- **Repository Pattern**: Domain defines interfaces, Infrastructure implements
- **Service Pattern**: Domain defines interfaces, Infrastructure implements
- **No Direct Dependencies**: Application never depends on concrete implementations

##### 5. Single Responsibility (Immutable Rule)
- **Entities**: Business objects with identity
- **Value Objects**: Immutable business concepts
- **Services**: Business logic that doesn't belong to entities
- **Repositories**: Data access abstractions
- **Handlers**: Single use case implementation

##### 6. Open/Closed Principle (Immutable Rule)
- **Open for Extension**: New features via new classes
- **Closed for Modification**: Existing code unchanged
- **Plugin Architecture**: New implementations via interfaces

##### 7. Dependency Inversion (Immutable Rule)
- **High-Level Modules**: Don't depend on low-level modules
- **Abstractions**: Don't depend on details
- **Details**: Depend on abstractions

##### 8. CQRS Separation (Immutable Rule)
- **Commands**: Write operations only
- **Queries**: Read operations only
- **Separate Models**: Command and Query models are distinct
- **No Shared State**: Commands and Queries don't share models

##### 9. Validation Strategy (Immutable Rule)
- **Input Validation**: At Application layer boundaries
- **Domain Validation**: Business rule validation in Domain
- **Infrastructure Validation**: Data integrity validation
- **API Validation**: Request format validation

##### 10. Error Handling Strategy (Immutable Rule)
- **Domain Exceptions**: Business rule violations
- **Application Exceptions**: Use case failures
- **Infrastructure Exceptions**: Technical failures
- **API Exceptions**: HTTP-specific errors

##### 11. Testing Strategy (Immutable Rule)
- **Unit Tests**: Test business logic in isolation
- **Integration Tests**: Test layer interactions
- **End-to-End Tests**: Test complete workflows
- **Test Independence**: Tests don't depend on each other

##### 12. Configuration Strategy (Immutable Rule)
- **Domain**: No configuration dependencies
- **Application**: Minimal configuration for use cases
- **Infrastructure**: Framework and external system configuration
- **API**: HTTP and presentation configuration

##### 13. Logging Strategy (Immutable Rule)
- **Domain**: No logging (pure business logic)
- **Application**: Business event logging
- **Infrastructure**: Technical event logging
- **API**: Request/response logging

##### 14. Caching Strategy (Immutable Rule)
- **Domain**: No caching (pure business logic)
- **Application**: Business result caching
- **Infrastructure**: Technical caching (database, external APIs)
- **API**: Response caching

##### 15. Security Strategy (Immutable Rule)
- **Domain**: Business security rules
- **Application**: Authorization logic
- **Infrastructure**: Authentication implementation
- **API**: Security headers and tokens

#### Anti-Patterns to Avoid (Never Allowed)

1. **Anemic Domain Model**: Entities with no behavior
2. **Fat Controllers**: Business logic in API layer
3. **Repository Leakage**: Infrastructure concerns in Domain
4. **Framework Coupling**: Domain depends on EF Core, NHapi, etc.
5. **Direct Database Access**: Bypassing Application layer
6. **Mixed Responsibilities**: Single class handling multiple concerns
7. **Circular Dependencies**: Any circular reference between layers
8. **Tight Coupling**: Direct instantiation of concrete classes
9. **God Objects**: Classes with too many responsibilities
10. **Data Transfer Objects in Domain**: DTOs should be in Application layer only

#### Validation Checklist (Must Pass Always)

- [ ] Domain layer has zero external dependencies
- [ ] Application layer depends only on Domain
- [ ] Infrastructure layer implements Domain interfaces
- [ ] API layer uses Application handlers
- [ ] No circular dependencies exist
- [ ] All public APIs have proper documentation
- [ ] Validation occurs at appropriate layers
- [ ] Error handling follows established patterns
- [ ] Testing covers all business logic
- [ ] Configuration is layer-appropriate

### Clean Architecture Compliance
- **Domain Layer**: No external dependencies
- **Application Layer**: Business logic and use cases
- **Infrastructure Layer**: External concerns (database, APIs)
- **API Layer**: Controllers and endpoints only

### FHIR Compliance
- All FHIR resources stored as JSONB
- Proper versioning support
- Search parameters and security labels
- Audit trail following FHIR AuditEvent structure

### Security Standards
- Multi-tenancy with TenantId
- Row-Level Security ready
- Comprehensive audit logging
- Soft delete functionality
- Optimistic concurrency control

## Code Quality Standards

### Naming Conventions
- **Classes**: PascalCase
- **Properties**: PascalCase
- **Methods**: PascalCase
- **Variables**: camelCase
- **Constants**: UPPER_CASE
- **Enums**: PascalCase

### Validation Attributes
```csharp
[Required]
[MaxLength(255)]
[EmailAddress]
[Column(TypeName = "jsonb")]
[NotMapped]
```

### Type Usage
- **Primary Keys**: `Guid` with `uuid_generate_v4()` default
- **Foreign Keys**: `Guid` for entity relationships
- **Strings**: Use `string` with appropriate `MaxLength`
- **Dates**: Use `DateTime` or `DateTime?`
- **Booleans**: Use `bool` with descriptive names
- **Enums**: Use strongly-typed enums

## Database Standards

### Table Naming
- Use snake_case for table names
- Use snake_case for column names
- Include proper indexes and constraints

### Configuration Standards
```csharp
builder.ToTable("table_name");
builder.Property(e => e.PropertyName).HasColumnName("column_name");
builder.HasIndex(e => e.PropertyName).HasDatabaseName("idx_table_property");
```

### JSONB Usage
- Use for complex data structures
- Include GIN indexes for performance
- Document complex data structures

## API Design Standards

### Minimal APIs
- Use Minimal API pattern
- Include proper error handling
- Return ProblemDetails for errors
- Include OpenAPI documentation

### FHIR Endpoints
- All FHIR endpoints under `/fhir/...`
- Proper HTTP status codes
- Consistent error responses
- FHIR resource validation

## Testing Standards

### Unit Tests
- Test all handlers and business logic
- Test computed properties
- Test validation attributes
- Test enum values

### Integration Tests
- Test database operations
- Test entity configurations
- Test foreign key relationships
- Test unique constraints

## Documentation Standards

### XML Documentation
- All public APIs must have XML documentation
- Include parameter descriptions
- Include return value descriptions
- Include exception documentation

### README Files
- Comprehensive setup instructions
- Architecture documentation
- API documentation
- Deployment guides

## File Organization

### Project Structure
```
src/
├── HealthTech.API/           # API Layer
├── HealthTech.Application/   # Application Layer
├── HealthTech.Domain/        # Domain Layer
└── HealthTech.Infrastructure/ # Infrastructure Layer
```

### Documentation Structure
```
docs/
├── api/                      # API Documentation
├── architecture/             # Architecture Documentation
├── cursor-agent/             # Cursor Agent Documentation
└── deployment/               # Deployment Documentation
```

## Development Workflow

### Entity Creation Process
1. **Create Domain Entity** following field organization pattern
2. **Create Persistence Configuration** with proper mappings
3. **Create Application Handlers** (Commands/Queries)
4. **Create API Endpoints** with proper documentation
5. **Create Unit Tests** for all components
6. **Create Integration Tests** for database operations
7. **Update Documentation** with new features

### Code Review Checklist
- [ ] Field organization follows pattern
- [ ] Visual separators properly formatted
- [ ] XML documentation complete
- [ ] Validation attributes appropriate
- [ ] Navigation properties configured
- [ ] Computed properties marked with `[NotMapped]`
- [ ] Tests cover all functionality
- [ ] Documentation updated

## Prompt Templates

### Entity Creation Prompt
```
"Create a new domain entity following the FHIR-AI Backend field organization pattern:
1. Foreign Key Fields
2. Core Identity Fields  
3. Basic Information Fields
4. Status & Configuration Fields
5. Security & Access Fields
6. Timing Fields
7. Additional Data Fields
8. Computed Properties
9. Navigation Properties

Use proper visual separators and XML documentation. Include appropriate validation attributes and follow naming conventions."
```

### API Endpoint Prompt
```
"Create a Minimal API endpoint following FHIR-AI Backend standards:
- Use proper HTTP status codes
- Include error handling with ProblemDetails
- Add OpenAPI documentation
- Follow FHIR compliance guidelines
- Include proper validation
- Add comprehensive XML documentation"
```

### Test Creation Prompt
```
"Create comprehensive tests following FHIR-AI Backend standards:
- Unit tests for all business logic
- Integration tests for database operations
- Test all validation attributes
- Test computed properties
- Test enum values and conversions
- Include proper test data setup"
```

## Quality Gates

### Code Quality
- All entities follow field organization pattern
- Proper validation attributes applied
- XML documentation complete
- Naming conventions followed
- No compiler warnings
- **Pattern Recognition**: Correct API pattern applied (Controller vs Minimal API)
- **Architecture Compliance**: Clean Architecture layers respected
- **FHIR Compliance**: Healthcare data follows FHIR R4B standards

### Testing
- Unit test coverage > 80%
- Integration tests for database operations
- All validation attributes tested
- Computed properties tested

### Documentation
- XML documentation for all public APIs
- README files updated
- Architecture documentation current
- API documentation complete

## Enforcement

### Automatic Application
Cursor AI must automatically:
1. **Pattern Recognition**: Identify feature type and apply correct pattern
   - Healthcare/FHIR → Minimal API + FHIR R4B
   - Business/System → Controller + CQRS + AutoMapper
   - Special Operations → Minimal API + Custom Logic
2. Apply field organization pattern to new entities
3. Use correct section headers with proper formatting
4. Group related fields according to defined categories
5. Apply validation attributes based on field type
6. Include XML documentation for all fields
7. Follow naming conventions consistently
8. **Architecture Compliance**: Respect Clean Architecture layers
9. **FHIR Compliance**: Apply FHIR R4B standards for healthcare data

### Validation
- All new code must pass quality gates
- Documentation must be complete
- Tests must cover all functionality
- Architecture principles must be followed
- **Pattern Validation**: Correct API pattern applied for feature type
- **FHIR Validation**: Healthcare data follows FHIR R4B standards
- **Route Validation**: FHIR routes follow HL7 FHIR specification

## Integration with Development Tools

### IDE Configuration
- Enable XML documentation warnings
- Configure code formatting rules
- Enable naming convention checks
- Set up test coverage reporting

### CI/CD Integration
- Automated code quality checks
- Test coverage validation
- Documentation completeness checks
- Architecture compliance validation

## Troubleshooting

### Common Issues
1. **Field organization**: Ensure all entities follow the pattern
2. **Documentation**: Check XML documentation completeness
3. **Validation**: Verify validation attributes are appropriate
4. **Testing**: Ensure adequate test coverage

### Resolution Steps
1. Review the standards document
2. Check existing entity examples
3. Validate against quality gates
4. Update documentation as needed

---

*These rules ensure consistent, high-quality development across the FHIR-AI Backend project while maintaining Clean Architecture principles and FHIR compliance.*
