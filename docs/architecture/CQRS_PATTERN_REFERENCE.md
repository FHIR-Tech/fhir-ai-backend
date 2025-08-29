# CQRS Pattern Reference Guide

## Overview

This document defines the **immutable CQRS (Command Query Responsibility Segregation) Pattern standards** for the FHIR-AI Backend project. These standards should **NEVER change**, regardless of Microsoft's framework updates or new patterns. They ensure consistency, maintainability, and proper separation of concerns between read and write operations.

## Core CQRS Principles (Immutable)

### 1. Command Query Separation (Immutable Rule)
- **Commands**: Write operations only (Create, Update, Delete)
- **Queries**: Read operations only (Get, List, Search)
- **No Shared State**: Commands and Queries don't share models
- **Separate Models**: Command and Query models are distinct

### 2. Single Responsibility (Immutable Rule)
- **Command Handlers**: Handle single write operation
- **Query Handlers**: Handle single read operation
- **No Mixed Operations**: One handler = one operation
- **Clear Intent**: Handler purpose is immediately clear

### 3. Immutability (Immutable Rule)
- **Commands**: Immutable records with init-only properties
- **Queries**: Immutable records with init-only properties
- **DTOs**: Immutable records for data transfer
- **No Side Effects**: Queries never modify state

### 4. MediatR Integration (Immutable Rule)
- **IRequest<T>**: All commands and queries implement IRequest
- **IRequestHandler<TRequest, TResponse>**: All handlers implement IRequestHandler
- **Pipeline Behaviors**: Cross-cutting concerns via behaviors
- **Assembly Registration**: Automatic registration via MediatR

## Official I/O Patterns

### 1. CQRS I/O (Greg Young)

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
public abstract class BaseQuery<TResponse> : IRequest<TResponse>
{
    public Guid QueryId { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? CorrelationId { get; set; }
    public string? UserId { get; set; }
}

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

public abstract class BaseQueryResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public Guid QueryId { get; set; }
    public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;
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

### 2. MediatR I/O (Jimmy Bogard)

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

### 3. FluentValidation I/O (Official)

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

### Command Pattern

**Command Structure**:
- Implement `IRequest<TResponse>`
- Use immutable records with init-only properties
- Include validation using FluentValidation
- Return `Result<T>` or specific response types

**Command Handler Structure**:
- Implement `IRequestHandler<TRequest, TResponse>`
- Handle single write operation
- Include business logic validation
- Persist changes to database
- Map to DTO for response

**Command Validator Structure**:
- Extend `AbstractValidator<T>`
- Validate all input parameters
- Include business rule validation
- Provide clear error messages

### Query Pattern

**Query Structure**:
- Implement `IRequest<TResponse>`
- Use immutable records with init-only properties
- Include pagination, sorting, and filtering parameters
- Return `Result<T>` or specific response types

**Query Handler Structure**:
- Implement `IRequestHandler<TRequest, TResponse>`
- Handle single read operation
- Apply filters and pagination
- Map to DTO for response
- No state modifications

**List Query Handler Structure**:
- Handle paginated queries
- Apply sorting and filtering
- Calculate pagination metadata
- Return paged response

## Result Pattern

**Result Structure**:
- Use `Result<T>` for consistent error handling
- Include success/failure status
- Provide error messages and validation errors
- Support pagination for list operations

**Paginated Result Structure**:
- Include items collection
- Provide total count and pagination metadata
- Support previous/next page navigation
- Include page size and current page information

## Cross-Cutting Concerns

### Validation Behavior
- Automatically validate all requests
- Provide detailed validation error messages
- Support multiple validators per request
- Handle validation exceptions gracefully

### Logging Behavior
- Log all requests and responses
- Include correlation IDs for tracing
- Avoid logging sensitive information
- Include performance metrics

### Caching Behavior
- Cache query results only
- Use appropriate cache keys
- Include cache invalidation strategies
- Respect cache headers and TTL

## File Organization

### Command Structure
```
HealthTech.Application/
├── {Feature}/
│   ├── Commands/
│   │   ├── Create{Entity}/
│   │   │   ├── Create{Entity}Command.cs
│   │   │   ├── Create{Entity}CommandHandler.cs
│   │   │   └── Create{Entity}CommandValidator.cs
│   │   ├── Update{Entity}/
│   │   │   ├── Update{Entity}Command.cs
│   │   │   ├── Update{Entity}CommandHandler.cs
│   │   │   └── Update{Entity}CommandValidator.cs
│   │   └── Delete{Entity}/
│   │       ├── Delete{Entity}Command.cs
│   │       ├── Delete{Entity}CommandHandler.cs
│   │       └── Delete{Entity}CommandValidator.cs
```

### Query Structure
```
HealthTech.Application/
├── {Feature}/
│   ├── Queries/
│   │   ├── Get{Entity}/
│   │   │   ├── Get{Entity}Query.cs
│   │   │   ├── Get{Entity}QueryHandler.cs
│   │   │   └── Get{Entity}QueryValidator.cs
│   │   ├── Get{Entity}List/
│   │   │   ├── Get{Entity}ListQuery.cs
│   │   │   ├── Get{Entity}ListQueryHandler.cs
│   │   │   └── Get{Entity}ListQueryValidator.cs
│   │   └── Search{Entity}/
│   │       ├── Search{Entity}Query.cs
│   │       ├── Search{Entity}QueryHandler.cs
│   │       └── Search{Entity}QueryValidator.cs
```

## Dependency Injection Configuration

```csharp
// Application Layer DI
services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
    cfg.AddBehavior(typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(LoggingBehavior<,>));
    cfg.AddBehavior(typeof(CachingBehavior<,>));
});

services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
services.AddFluentValidationAutoValidation();
```

## Testing Structure

**Command Handler Tests**:
```
HealthTech.Application.Tests/
├── {Feature}/
│   ├── Commands/
│   └── Validators/
```

**Query Handler Tests**:
```
HealthTech.Application.Tests/
├── {Feature}/
│   ├── Queries/
│   └── Validators/
```

## Immutable Standards (Never Change)

### 1. Command Query Separation (Immutable Rule)
- **Commands**: Write operations only
- **Queries**: Read operations only
- **Separate Models**: Command and Query models are distinct
- **No Shared State**: Commands and Queries don't share models

### 2. Single Responsibility (Immutable Rule)
- **Command Handlers**: Handle single write operation
- **Query Handlers**: Handle single read operation
- **No Mixed Operations**: One handler = one operation
- **Clear Intent**: Handler purpose is immediately clear

### 3. Immutability (Immutable Rule)
- **Commands**: Immutable records with init-only properties
- **Queries**: Immutable records with init-only properties
- **DTOs**: Immutable records for data transfer
- **No Side Effects**: Queries never modify state

### 4. MediatR Integration (Immutable Rule)
- **IRequest<T>**: All commands and queries implement IRequest
- **IRequestHandler<TRequest, TResponse>**: All handlers implement IRequestHandler
- **Pipeline Behaviors**: Cross-cutting concerns via behaviors
- **Assembly Registration**: Automatic registration via MediatR

### 5. Validation Strategy (Immutable Rule)
- **Input Validation**: At Application layer boundaries
- **Business Validation**: In Domain layer
- **FluentValidation**: For all input validation
- **Clear Messages**: User-friendly error messages

### 6. Error Handling Strategy (Immutable Rule)
- **Result Pattern**: Consistent error handling
- **Validation Errors**: Detailed validation feedback
- **Business Errors**: Domain-specific error messages
- **Technical Errors**: Infrastructure error handling

### 7. Pagination Strategy (Immutable Rule)
- **Standard Parameters**: PageNumber, PageSize, SortBy, SortOrder
- **Metadata**: TotalCount, TotalPages, HasPreviousPage, HasNextPage
- **Validation**: Page size limits and validation
- **Performance**: Efficient pagination queries

### 8. Logging Strategy (Immutable Rule)
- **Request Logging**: All incoming requests
- **Response Logging**: All outgoing responses
- **Correlation IDs**: For request tracing
- **Sensitive Data**: Never log sensitive information

### 9. Caching Strategy (Immutable Rule)
- **Query Caching**: Cache read operations only
- **Cache Keys**: Appropriate cache key generation
- **Cache Invalidation**: Proper cache invalidation
- **Cache Headers**: Respect cache control headers

### 10. Testing Strategy (Immutable Rule)
- **Unit Tests**: Test handlers in isolation
- **Integration Tests**: Test with real dependencies
- **Validation Tests**: Test all validation rules
- **Performance Tests**: Test pagination and caching

## Anti-Patterns to Avoid (Never Allowed)

1. **Mixed Operations**: Commands that read data, queries that modify state
2. **Shared Models**: Commands and queries sharing the same model
3. **Fat Handlers**: Handlers with too many responsibilities
4. **Direct Repository Access**: Bypassing handlers for data access
5. **Synchronous Operations**: Blocking operations in async handlers
6. **Hardcoded Values**: Magic numbers and hardcoded strings
7. **Missing Validation**: Requests without proper validation
8. **Poor Error Handling**: Inconsistent error responses
9. **Inefficient Queries**: N+1 queries and poor pagination
10. **Missing Logging**: Operations without proper logging

## Validation Checklist (Must Pass Always)

- [ ] Commands and queries are properly separated
- [ ] Queries only perform read operations
- [ ] All handlers implement single responsibility
- [ ] All requests have proper validation
- [ ] All responses follow consistent patterns
- [ ] Pagination is properly implemented
- [ ] Error handling is consistent
- [ ] Logging covers all operations
- [ ] Caching is implemented for queries
- [ ] Testing covers all scenarios
- [ ] Performance is optimized

## Performance Considerations

1. **Async/Await**: Use throughout for I/O operations
2. **Pagination**: Implement efficient pagination
3. **Caching**: Cache query results appropriately
4. **Projection**: Use DTOs to limit data transfer
5. **Indexing**: Proper database indexing for queries
6. **Connection Pooling**: Efficient database connections management

## Security Implementation

1. **Input Validation**: Comprehensive validation at all layers
2. **Authorization**: Proper authorization checks
3. **Audit Logging**: Track all data modifications
4. **Data Protection**: Sensitive data encryption
5. **Rate Limiting**: API rate limiting for commands
6. **Correlation IDs**: Request tracing for security
7. **Tenant Isolation**: Ensure proper tenant isolation

---

**This document defines the immutable CQRS Pattern standards that should NEVER change, ensuring consistency and maintainability across the FHIR-AI Backend project.**

**For complete AutoMapper implementation details, see `AUTOMAPPER_PATTERN_REFERENCE.md`**
