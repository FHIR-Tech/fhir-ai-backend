# Clean Architecture Reference Guide

## Overview

This document defines the **immutable Clean Architecture standards** for the FHIR-AI Backend project. These standards should **NEVER change**, regardless of Microsoft's framework updates or new patterns. They ensure consistency, maintainability, and proper separation of concerns.

## Core Principles (Immutable)

### 1. Dependency Rule
**Dependencies point inward only**
- Domain layer has zero external dependencies
- Application layer depends only on Domain
- Infrastructure layer depends on Application and Domain
- API layer depends on Application and Infrastructure

### 2. Abstraction Rule
**Inner layers define abstractions, outer layers implement them**
- Domain defines repository interfaces
- Infrastructure implements repository interfaces
- Domain defines service interfaces
- Infrastructure implements service interfaces

### 3. Independence Rule
**Domain layer has zero dependencies on external concerns**
- No framework dependencies (EF Core, NHapi, etc.)
- No external library dependencies
- Pure business logic only
- Framework-agnostic

### 4. Framework Independence
**Domain and Application layers are framework-agnostic**
- Can be tested without any framework
- Can be used with any framework
- No framework-specific code

## Layer Structure & Responsibilities

### Domain Layer (Innermost)
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

**File Structure**:
```
HealthTech.Domain/
├── Entities/
│   └── {EntityName}.cs
├── ValueObjects/
│   └── {ValueObjectName}.cs
├── Aggregates/
│   └── {AggregateName}Aggregate.cs
├── Repositories/
│   └── I{EntityName}Repository.cs
├── Services/
│   └── I{ServiceName}Service.cs
├── Events/
│   └── {EventName}Event.cs
├── Enums/
│   └── {EnumName}.cs
└── Exceptions/
    └── {ExceptionName}Exception.cs
```

### Application Layer (Business Use Cases)
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

### Infrastructure Layer (External Concerns)
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

**File Structure**:
```
HealthTech.Infrastructure/
├── Persistence/
│   ├── ApplicationDbContext.cs
│   ├── Configurations/
│   │   └── {Entity}Configuration.cs
│   ├── Repositories/
│   │   └── {Entity}Repository.cs
│   └── Migrations/
├── Authentication/
│   ├── JwtTokenService.cs
│   ├── CurrentUserService.cs
│   └── AuthorizationService.cs
├── External/
│   └── {ExternalService}Client.cs
├── Caching/
│   ├── RedisCacheService.cs
│   └── MemoryCacheService.cs
├── Logging/
│   └── SerilogService.cs
├── FileStorage/
│   ├── AzureBlobStorageService.cs
│   └── {FileStorageName}Service.cs
└── DependencyInjection.cs
```

### API Layer (Presentation)
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

**File Structure**:
```
HealthTech.API/
├── Endpoints/
│   └── {Entity}Endpoints.cs
├── Controllers/
│   └── {ControllerName}Controller.cs
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

## Official I/O Patterns

### MediatR I/O Pattern (Standard for FHIR-AI Backend)

**Base Request Pattern**:
```csharp
public abstract class BaseRequest<TResponse> : IRequest<TResponse>
{
    public Guid RequestId { get; set; } = Guid.NewGuid();
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public string? CorrelationId { get; set; }
    public string? UserId { get; set; }
    public string? TenantId { get; set; }
}

public abstract class BasePagedRequest<TResponse> : BaseRequest<TResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string SortOrder { get; set; } = "asc";
    public string? SearchTerm { get; set; }
    public Dictionary<string, object>? Filters { get; set; }
}
```

**Base Response Pattern**:
```csharp
public abstract class BaseResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public Guid RequestId { get; set; }
    public DateTime RespondedAt { get; set; } = DateTime.UtcNow;
    public int StatusCode { get; set; } = 200;
}

public class PagedResponse<T> : BaseResponse
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}
```

### FluentValidation Integration

**Base Validator Pattern**:
```csharp
public abstract class BaseValidator<T> : AbstractValidator<T>
{
    protected BaseValidator()
    {
        // Common validation rules
        When(x => x is BaseRequest<object> request, () =>
        {
            RuleFor(x => ((BaseRequest<object>)x).RequestId)
                .NotEmpty()
                .WithMessage("Request ID is required");
        });
    }
}

public class PaginationValidator : AbstractValidator<BasePagedRequest<object>>
{
    public PaginationValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000)
            .WithMessage("Page size must be between 1 and 1000");

        RuleFor(x => x.SortOrder)
            .Must(x => x?.ToLower() == "asc" || x?.ToLower() == "desc")
            .WithMessage("Sort order must be 'asc' or 'desc'");
    }
}
```

## Implementation Guidelines

### 1. Request Implementation

**For Simple Operations (Commands/Queries)**:
```csharp
// Inherit from BaseRequest<TResponse>
public class CreatePatientCommand : BaseRequest<CreatePatientResponse>
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public DateTime DateOfBirth { get; init; }
    public string Gender { get; init; }
}

public class GetPatientQuery : BaseRequest<GetPatientResponse>
{
    public Guid PatientId { get; init; }
}
```

**For Paged Operations (List Queries)**:
```csharp
// Inherit from BasePagedRequest<TResponse>
public class GetPatientsQuery : BasePagedRequest<GetPatientsResponse>
{
    public string? SearchTerm { get; init; }
    public string? Gender { get; init; }
    public DateTime? DateOfBirthFrom { get; init; }
    public DateTime? DateOfBirthTo { get; init; }
}
```

### 2. Response Implementation

**For Simple Operations**:
```csharp
// Inherit from BaseResponse
public class CreatePatientResponse : BaseResponse
{
    public Guid PatientId { get; set; }
    public string FullName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GetPatientResponse : BaseResponse
{
    public PatientDto? Patient { get; set; }
}
```

**For Paged Operations**:
```csharp
// Inherit from PagedResponse<T>
public class GetPatientsResponse : PagedResponse<PatientDto>
{
    // Additional properties specific to patient list if needed
    public int ActivePatientsCount { get; set; }
    public int InactivePatientsCount { get; set; }
}
```

### 3. Handler Implementation

**Command Handler**:
```csharp
public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, CreatePatientResponse>
{
    private readonly IPatientRepository _patientRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreatePatientCommandHandler(IPatientRepository patientRepository, ICurrentUserService currentUserService)
    {
        _patientRepository = patientRepository;
        _currentUserService = currentUserService;
    }

    public async Task<CreatePatientResponse> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        // Business logic implementation
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            TenantId = _currentUserService.TenantId ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        await _patientRepository.AddAsync(patient);

        return new CreatePatientResponse
        {
            IsSuccess = true,
            Message = "Patient created successfully",
            RequestId = request.RequestId,
            PatientId = patient.Id,
            FullName = $"{patient.FirstName} {patient.LastName}",
            CreatedAt = patient.CreatedAt
        };
    }
}
```

**Query Handler**:
```csharp
public class GetPatientsQueryHandler : IRequestHandler<GetPatientsQuery, GetPatientsResponse>
{
    private readonly IPatientRepository _patientRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetPatientsQueryHandler(IPatientRepository patientRepository, ICurrentUserService currentUserService)
    {
        _patientRepository = patientRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetPatientsResponse> Handle(GetPatientsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId ?? string.Empty;
        
        // Build query with filters
        var query = _patientRepository.GetQueryable(tenantId);
        
        // Apply search filters
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(p => p.FirstName.Contains(request.SearchTerm) || p.LastName.Contains(request.SearchTerm));
        }

        if (!string.IsNullOrEmpty(request.Gender))
        {
            query = query.Where(p => p.Gender == request.Gender);
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination and sorting
        var items = await query
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PatientDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender
            })
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new GetPatientsResponse
        {
            IsSuccess = true,
            Message = "Patients retrieved successfully",
            RequestId = request.RequestId,
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = totalPages,
            HasPreviousPage = request.PageNumber > 1,
            HasNextPage = request.PageNumber < totalPages
        };
    }
}
```

### 4. Validation Implementation

**Command Validator**:
```csharp
public class CreatePatientCommandValidator : BaseValidator<CreatePatientCommand>
{
    public CreatePatientCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("First name is required and cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Last name is required and cannot exceed 100 characters");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .LessThan(DateTime.Today)
            .WithMessage("Date of birth must be in the past");

        RuleFor(x => x.Gender)
            .IsInEnum()
            .WithMessage("Gender must be a valid value");
    }
}
```

**Query Validator**:
```csharp
public class GetPatientsQueryValidator : BaseValidator<GetPatientsQuery>
{
    public GetPatientsQueryValidator()
    {
        // Pagination validation is inherited from PaginationValidator
        RuleFor(x => x.SearchTerm)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.SearchTerm))
            .WithMessage("Search term cannot exceed 200 characters");

        RuleFor(x => x.Gender)
            .IsInEnum()
            .When(x => !string.IsNullOrEmpty(x.Gender))
            .WithMessage("Gender must be a valid value");

        RuleFor(x => x.DateOfBirthFrom)
            .LessThanOrEqualTo(x => x.DateOfBirthTo)
            .When(x => x.DateOfBirthFrom.HasValue && x.DateOfBirthTo.HasValue)
            .WithMessage("Date of birth from must be less than or equal to date of birth to");
    }
}
```

### 5. File Structure Guidelines

**Application Layer Structure**:
```
HealthTech.Application/
├── Common/
│   ├── Base/
│   │   ├── BaseRequest.cs
│   │   ├── BasePagedRequest.cs
│   │   ├── BaseResponse.cs
│   │   └── PagedResponse.cs
│   ├── Behaviors/
│   │   ├── ValidationBehavior.cs
│   │   ├── LoggingBehavior.cs
│   │   └── CachingBehavior.cs
│   ├── Interfaces/
│   │   ├── ICurrentUserService.cs
│   │   └── IDateTimeService.cs
│   └── Validators/
│       ├── BaseValidator.cs
│       └── PaginationValidator.cs
├── {Feature}/
│   ├── Commands/
│   │   ├── Create{Entity}/
│   │   │   ├── Create{Entity}Command.cs
│   │   │   ├── Create{Entity}CommandHandler.cs
│   │   │   ├── Create{Entity}CommandValidator.cs
│   │   │   └── Create{Entity}Response.cs
│   │   └── Update{Entity}/
│   ├── Queries/
│   │   ├── Get{Entity}/
│   │   │   ├── Get{Entity}Query.cs
│   │   │   ├── Get{Entity}QueryHandler.cs
│   │   │   ├── Get{Entity}QueryValidator.cs
│   │   │   └── Get{Entity}Response.cs
│   │   └── Get{Entity}List/
│   │       ├── Get{Entity}ListQuery.cs
│   │       ├── Get{Entity}ListQueryHandler.cs
│   │       ├── Get{Entity}ListQueryValidator.cs
│   │       └── Get{Entity}ListResponse.cs
│   └── DTOs/
│       └── {Entity}Dto.cs
└── DependencyInjection.cs
```

### 6. Dependency Injection Configuration

```csharp
// Application Layer DI
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

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

### 7. Best Practices

**Request Guidelines**:
- ✅ Always inherit from `BaseRequest<TResponse>` or `BasePagedRequest<TResponse>`
- ✅ Use `init` properties for immutability
- ✅ Include validation attributes when needed
- ✅ Use descriptive names: `CreatePatientCommand`, `GetPatientsQuery`

**Response Guidelines**:
- ✅ Always inherit from `BaseResponse` or `PagedResponse<T>`
- ✅ Set `IsSuccess`, `Message`, and `RequestId` in handlers
- ✅ Include relevant data in response
- ✅ Use DTOs for data transfer

**Handler Guidelines**:
- ✅ Implement `IRequestHandler<TRequest, TResponse>`
- ✅ Use dependency injection for dependencies
- ✅ Handle exceptions appropriately
- ✅ Return meaningful response messages

**Validation Guidelines**:
- ✅ Always inherit from `BaseValidator<T>`
- ✅ Use FluentValidation rules
- ✅ Provide clear error messages
- ✅ Validate business rules, not just data format

### 8. Error Handling

**Success Response**:
```csharp
return new CreatePatientResponse
{
    IsSuccess = true,
    Message = "Patient created successfully",
    RequestId = request.RequestId,
    StatusCode = 201,
    PatientId = patient.Id
};
```

**Error Response**:
```csharp
return new CreatePatientResponse
{
    IsSuccess = false,
    Message = "Failed to create patient",
    RequestId = request.RequestId,
    StatusCode = 400,
    Errors = new List<string> { "Patient with this identifier already exists" }
};
```

### 9. Testing Structure

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
│   └── {Entity}EndpointsTests.cs
├── Controllers/
│   └── {ControllerName}ControllerTests.cs
└── TestBase.cs
```

**Unit Test Example**:
```csharp
[Fact]
public async Task Handle_ValidCreatePatientCommand_ReturnsSuccessResponse()
{
    // Arrange
    var command = new CreatePatientCommand
    {
        FirstName = "John",
        LastName = "Doe",
        DateOfBirth = new DateTime(1990, 1, 1),
        Gender = "Male"
    };

    var handler = new CreatePatientCommandHandler(mockRepository, mockUserService);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal(command.RequestId, result.RequestId);
    Assert.NotEqual(Guid.Empty, result.PatientId);
}
```

## Immutable Standards (Never Change)

### 1. Dependency Direction (Immutable Rule)
- **Domain Layer**: Zero external dependencies
- **Application Layer**: Depends only on Domain
- **Infrastructure Layer**: Depends on Application and Domain
- **API Layer**: Depends on Application and Infrastructure
- **Violation**: Any outer layer dependency on inner layers is forbidden

### 2. Layer Responsibilities (Immutable Rule)
- **Domain**: Pure business logic, no framework code
- **Application**: Use cases and orchestration, no infrastructure concerns
- **Infrastructure**: External system interactions only
- **API**: HTTP concerns only, no business logic

### 3. Framework Independence (Immutable Rule)
- **Domain Layer**: Must be framework-agnostic
- **Application Layer**: Must be framework-agnostic (uses MediatR I/O pattern)
- **Infrastructure Layer**: Can use framework-specific code
- **API Layer**: Can use framework-specific code

### 4. Interface Segregation (Immutable Rule)
- **Repository Pattern**: Domain defines interfaces, Infrastructure implements
- **Service Pattern**: Domain defines interfaces, Infrastructure implements
- **No Direct Dependencies**: Application never depends on concrete implementations

### 5. Single Responsibility (Immutable Rule)
- **Entities**: Business objects with identity
- **Value Objects**: Immutable business concepts
- **Services**: Business logic that doesn't belong to entities
- **Repositories**: Data access abstractions
- **Handlers**: Single use case implementation (MediatR IRequestHandler)

### 6. Open/Closed Principle (Immutable Rule)
- **Open for Extension**: New features via new classes
- **Closed for Modification**: Existing code unchanged
- **Plugin Architecture**: New implementations via interfaces

### 7. Dependency Inversion (Immutable Rule)
- **High-Level Modules**: Don't depend on low-level modules
- **Abstractions**: Don't depend on details
- **Details**: Depend on abstractions

### 8. CQRS Separation (Immutable Rule)
- **Commands**: Write operations only (inherit from BaseRequest<TResponse>)
- **Queries**: Read operations only (inherit from BaseRequest<TResponse> or BasePagedRequest<TResponse>)
- **Separate Models**: Command and Query models are distinct
- **No Shared State**: Commands and Queries don't share models
- **MediatR I/O Pattern**: Standard implementation for all CQRS operations
- **For complete CQRS implementation details, see `CQRS_PATTERN_REFERENCE.md`**
- **For complete AutoMapper implementation details, see `AUTOMAPPER_PATTERN_REFERENCE.md`**
- **For complete Healthcare Data implementation details, see `HEALTHCARE_DATA_PATTERN_REFERENCE.md`**

### 9. Validation Strategy (Immutable Rule)
- **Input Validation**: At Application layer boundaries
- **Domain Validation**: Business rule validation in Domain
- **Infrastructure Validation**: Data integrity validation
- **API Validation**: Request format validation

### 10. Error Handling Strategy (Immutable Rule)
- **Domain Exceptions**: Business rule violations
- **Application Exceptions**: Use case failures
- **Infrastructure Exceptions**: Technical failures
- **API Exceptions**: HTTP-specific errors

### 11. Testing Strategy (Immutable Rule)
- **Unit Tests**: Test business logic in isolation
- **Integration Tests**: Test layer interactions
- **End-to-End Tests**: Test complete workflows
- **Test Independence**: Tests don't depend on each other

### 12. Configuration Strategy (Immutable Rule)
- **Domain**: No configuration dependencies
- **Application**: Minimal configuration for use cases
- **Infrastructure**: Framework and external system configuration
- **API**: HTTP and presentation configuration

### 13. Logging Strategy (Immutable Rule)
- **Domain**: No logging (pure business logic)
- **Application**: Business event logging
- **Infrastructure**: Technical event logging
- **API**: Request/response logging

### 14. Caching Strategy (Immutable Rule)
- **Domain**: No caching (pure business logic)
- **Application**: Business result caching
- **Infrastructure**: Technical caching (database, external APIs)
- **API**: Response caching

### 15. Security Strategy (Immutable Rule)
- **Domain**: Business security rules
- **Application**: Authorization logic
- **Infrastructure**: Authentication implementation
- **API**: Security headers and tokens

## Anti-Patterns to Avoid (Never Allowed)

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
11. **Multiple I/O Patterns**: Using different I/O patterns (only MediatR I/O allowed)
12. **Custom Request/Response**: Not inheriting from BaseRequest/BaseResponse
13. **Direct Handler Calls**: Bypassing MediatR pipeline
14. **Mixed Validation**: Not using FluentValidation with BaseValidator

## Validation Checklist (Must Pass Always)

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
- [ ] All requests inherit from BaseRequest<TResponse> or BasePagedRequest<TResponse>
- [ ] All responses inherit from BaseResponse or PagedResponse<T>
- [ ] All handlers implement IRequestHandler<TRequest, TResponse>
- [ ] All validators inherit from BaseValidator<T>
- [ ] MediatR pipeline behaviors are properly configured
- [ ] No direct handler calls bypassing MediatR

## Performance Considerations

1. **Async/Await**: Use throughout for I/O operations
2. **Caching**: Implement at appropriate layers
3. **Pagination**: For large data sets
4. **Projection**: Use DTOs to limit data transfer
5. **Indexing**: Proper database indexing strategy
6. **Connection Pooling**: Database connection management

## Security Implementation

1. **Authentication**: JWT tokens with proper validation
2. **Authorization**: Role-based and claim-based authorization
3. **Input Validation**: Comprehensive validation at all layers
4. **Audit Logging**: Track all data modifications
5. **Data Encryption**: Sensitive data encryption at rest and in transit
6. **Rate Limiting**: API rate limiting implementation

---

**This document defines the immutable Clean Architecture standards that should NEVER change, ensuring consistency and maintainability across the FHIR-AI Backend project.**
