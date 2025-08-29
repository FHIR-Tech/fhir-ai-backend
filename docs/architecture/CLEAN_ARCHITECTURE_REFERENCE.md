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

### 1. Clean Architecture I/O (Uncle Bob)

**Base Request Pattern**:
```csharp
public abstract class BaseRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string SortOrder { get; set; } = "asc";
    public string? SearchTerm { get; set; }
}
```

**Base Response Pattern**:
```csharp
public abstract class BaseResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public int StatusCode { get; set; }
    public List<string> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
```

### 2. CQRS I/O (Greg Young)

**Base Command Pattern**:
```csharp
public abstract class BaseCommand<TResponse> : IRequest<TResponse>
{
    public Guid CommandId { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? CorrelationId { get; set; }
    public string? UserId { get; set; }
}
```

**Base Query Pattern**:
```csharp
public abstract class BasePagedQuery<TResponse> : BaseQuery<TResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string SortOrder { get; set; } = "asc";
    public string? SearchTerm { get; set; }
    public Dictionary<string, string>? Filters { get; set; }
}
```

**CQRS Response Patterns**:
```csharp
public abstract class BaseCommandResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public Guid CommandId { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}

public class PagedQueryResponse<T> : BaseQueryResponse
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

### 3. MediatR I/O (Jimmy Bogard)

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

**MediatR Response Pattern**:
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

### 4. FluentValidation I/O (Official)

**Base Validator Pattern**:
```csharp
public abstract class BaseValidator<T> : AbstractValidator<T>
{
    protected BaseValidator()
    {
        // Common validation rules
        When(x => x is BaseRequest request, () =>
        {
            RuleFor(x => ((BaseRequest)x).RequestId)
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

## Implementation Patterns

### CQRS Pattern

**Commands** (Write Operations):
- Implement `IRequest<TResponse>`
- Use immutable records with init-only properties
- Include validation using FluentValidation
- Return `Result<T>` or specific response types

**Queries** (Read Operations):
- Implement `IRequest<TResponse>`
- Use immutable records with init-only properties
- Include pagination, sorting, and filtering parameters
- Return `Result<T>` or specific response types

### Repository Pattern

**Domain Interface**:
- Define in Domain layer
- Include async methods with CancellationToken
- Focus on business operations, not technical details

**Infrastructure Implementation**:
- Implement in Infrastructure layer
- Use framework-specific code (EF Core, etc.)
- Handle technical concerns (connection, transactions)

### Validation Pattern

- Use FluentValidation for all input validation
- Validate at Application layer boundaries
- Include business rule validation in Domain layer
- Provide clear, user-friendly error messages

### Result Pattern

- Use `Result<T>` for consistent error handling
- Include success/failure status
- Provide error messages and validation errors
- Support pagination for list operations

### Cross-Cutting Concerns

**Logging Behavior**:
- Log all requests and responses
- Include correlation IDs for tracing
- Avoid logging sensitive information

**Validation Behavior**:
- Automatically validate all requests
- Provide detailed validation error messages
- Support multiple validators per request

**Caching Behavior**:
- Cache query results only
- Use appropriate cache keys
- Include cache invalidation strategies

## Dependency Injection Configuration

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

## Testing Structure

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
- **Application Layer**: Must be framework-agnostic
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
- **Handlers**: Single use case implementation

### 6. Open/Closed Principle (Immutable Rule)
- **Open for Extension**: New features via new classes
- **Closed for Modification**: Existing code unchanged
- **Plugin Architecture**: New implementations via interfaces

### 7. Dependency Inversion (Immutable Rule)
- **High-Level Modules**: Don't depend on low-level modules
- **Abstractions**: Don't depend on details
- **Details**: Depend on abstractions

### 8. CQRS Separation (Immutable Rule)
- **Commands**: Write operations only
- **Queries**: Read operations only
- **Separate Models**: Command and Query models are distinct
- **No Shared State**: Commands and Queries don't share models
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
