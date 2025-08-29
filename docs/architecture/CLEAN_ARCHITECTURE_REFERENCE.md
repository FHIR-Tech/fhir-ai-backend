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

**For complete MediatR I/O Pattern implementation details, see `MEDIATR_IO_PATTERN_REFERENCE.md`**

The FHIR-AI Backend uses MediatR I/O pattern as the standard implementation for all CQRS operations. This pattern provides:

- **BaseRequest<TResponse>**: Standard base class for all requests
- **BasePagedRequest<TResponse>**: Standard base class for paged requests
- **BaseResponse**: Standard base class for all responses
- **PagedResponse<T>**: Standard base class for paged responses
- **Pipeline Behaviors**: Validation, logging, and caching behaviors
- **FluentValidation Integration**: Automatic request validation
- **Consistent Error Handling**: Standardized error responses
- **Multi-tenancy Support**: Built-in tenant isolation
- **Request Tracing**: Correlation IDs for debugging

## Implementation Guidelines

**For complete MediatR I/O Pattern implementation details, see `MEDIATR_IO_PATTERN_REFERENCE.md`**

The Clean Architecture implementation follows these key principles:

### 1. Layer Separation
- **Domain Layer**: Pure business logic with no external dependencies
- **Application Layer**: Use cases and orchestration using MediatR I/O pattern
- **Infrastructure Layer**: External concerns and framework implementations
- **API Layer**: HTTP concerns and presentation logic

### 2. Dependency Direction
- Dependencies point inward only
- Domain layer has zero external dependencies
- Application layer depends only on Domain
- Infrastructure layer implements Domain interfaces
- API layer uses Application handlers

### 3. CQRS Implementation
- Commands and Queries are separated
- Each handler has single responsibility
- MediatR I/O pattern is used for all operations
- Validation, logging, and caching via pipeline behaviors

### 4. File Organization
- Feature-based organization within layers
- Clear separation of concerns
- Consistent naming conventions
- Proper dependency injection configuration

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
