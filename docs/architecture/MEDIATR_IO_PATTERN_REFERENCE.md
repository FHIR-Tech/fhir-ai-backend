# MediatR I/O Pattern Reference Guide

## Overview

This document defines the **immutable MediatR I/O Pattern standards** for the FHIR-AI Backend project. These standards should **NEVER change**, regardless of Microsoft's framework updates or new patterns. They ensure consistency, maintainability, and proper implementation of the Mediator pattern using MediatR library.

## Core MediatR I/O Principles (Immutable)

### 1. Mediator Pattern Implementation (Immutable Rule)
- **Single Entry Point**: All application operations go through MediatR
- **Decoupled Communication**: Handlers don't know about each other
- **Pipeline Behaviors**: Cross-cutting concerns via behaviors
- **Assembly Registration**: Automatic registration via reflection

### 2. Request/Response Pattern (Immutable Rule)
- **IRequest<TResponse>**: All requests implement IRequest interface
- **IRequestHandler<TRequest, TResponse>**: All handlers implement IRequestHandler
- **Base Classes**: All requests inherit from BaseRequest or BasePagedRequest
- **Response Consistency**: All responses inherit from BaseResponse or PagedResponse

### 3. Pipeline Behavior Pattern (Immutable Rule)
- **ValidationBehavior**: Automatic request validation
- **LoggingBehavior**: Request/response logging
- **CachingBehavior**: Query result caching
- **Order Matters**: Behaviors execute in registration order

### 4. Immutability Pattern (Immutable Rule)
- **Init-Only Properties**: All request properties use init-only setters
- **Immutable Records**: Use records for DTOs and responses
- **No Side Effects**: Queries never modify state
- **Thread Safety**: Immutable objects are thread-safe

## Official MediatR I/O Patterns

### Base Request Pattern (Standard for FHIR-AI Backend)

**BaseRequest<TResponse>**:
```csharp
public abstract class BaseRequest<TResponse> : IRequest<TResponse>
{
    public Guid RequestId { get; set; } = Guid.NewGuid();
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public string? CorrelationId { get; set; }
    public string? UserId { get; set; }
    public string? TenantId { get; set; }
}
```

**BasePagedRequest<TResponse>**:
```csharp
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

### Base Response Pattern (Standard for FHIR-AI Backend)

**BaseResponse**:
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
```

**PagedResponse<T>**:
```csharp
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

### FluentValidation Integration Pattern

**BaseValidator<T>**:
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
```

**PaginationValidator**:
```csharp
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

### 1. Command Implementation Pattern

**Command Structure**:
```csharp
public class CreatePatientCommand : BaseRequest<CreatePatientResponse>
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public DateTime DateOfBirth { get; init; }
    public string Gender { get; init; }
}
```

**Command Response**:
```csharp
public class CreatePatientResponse : BaseResponse
{
    public Guid PatientId { get; set; }
    public string FullName { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

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

### 2. Query Implementation Pattern

**Query Structure**:
```csharp
public class GetPatientQuery : BaseRequest<GetPatientResponse>
{
    public Guid PatientId { get; init; }
}
```

**Paged Query Structure**:
```csharp
public class GetPatientsQuery : BasePagedRequest<GetPatientsResponse>
{
    public string? SearchTerm { get; init; }
    public string? Gender { get; init; }
    public DateTime? DateOfBirthFrom { get; init; }
    public DateTime? DateOfBirthTo { get; init; }
}
```

**Query Response**:
```csharp
public class GetPatientResponse : BaseResponse
{
    public PatientDto? Patient { get; set; }
}
```

**Paged Query Response**:
```csharp
public class GetPatientsResponse : PagedResponse<PatientDto>
{
    // Additional properties specific to patient list if needed
    public int ActivePatientsCount { get; set; }
    public int InactivePatientsCount { get; set; }
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

### 3. Pipeline Behavior Implementation

**ValidationBehavior**:
```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
```

**LoggingBehavior**:
```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = request is BaseRequest<object> baseRequest ? baseRequest.RequestId : Guid.Empty;

        _logger.LogInformation("Handling {RequestName} with RequestId: {RequestId}", requestName, requestId);

        var sw = Stopwatch.StartNew();
        var response = await next();
        sw.Stop();

        _logger.LogInformation("Handled {RequestName} with RequestId: {RequestId} in {ElapsedMilliseconds}ms", 
            requestName, requestId, sw.ElapsedMilliseconds);

        return response;
    }
}
```

**CachingBehavior**:
```csharp
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(IMemoryCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Only cache queries, not commands
        if (request is not BaseRequest<TResponse> || request.GetType().Name.Contains("Command"))
        {
            return await next();
        }

        var cacheKey = $"{typeof(TRequest).Name}_{JsonSerializer.Serialize(request)}";
        
        if (_cache.TryGetValue(cacheKey, out TResponse? cachedResponse))
        {
            _logger.LogInformation("Returning cached response for {RequestName}", typeof(TRequest).Name);
            return cachedResponse!;
        }

        var response = await next();
        
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
            .SetAbsoluteExpiration(TimeSpan.FromHours(1));

        _cache.Set(cacheKey, response, cacheEntryOptions);
        
        return response;
    }
}
```

### 4. File Structure Guidelines

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

### 5. Dependency Injection Configuration

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

### 6. API Layer Integration

**Minimal API Endpoint**:
```csharp
app.MapPost("/api/patients", async (CreatePatientCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command);
    return result.IsSuccess ? Results.Created($"/api/patients/{result.PatientId}", result) : Results.BadRequest(result);
})
.WithName("CreatePatient")
.WithOpenApi();
```

**Controller Endpoint**:
```csharp
[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PatientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<CreatePatientResponse>> Create(CreatePatientCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.PatientId }, result) : BadRequest(result);
    }

    [HttpGet]
    public async Task<ActionResult<GetPatientsResponse>> Get([FromQuery] GetPatientsQuery query)
    {
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
```

## Best Practices

### Request Guidelines
- ✅ Always inherit from `BaseRequest<TResponse>` or `BasePagedRequest<TResponse>`
- ✅ Use `init` properties for immutability
- ✅ Include validation attributes when needed
- ✅ Use descriptive names: `CreatePatientCommand`, `GetPatientsQuery`
- ✅ Include correlation ID for tracing
- ✅ Set tenant ID for multi-tenancy

### Response Guidelines
- ✅ Always inherit from `BaseResponse` or `PagedResponse<T>`
- ✅ Set `IsSuccess`, `Message`, and `RequestId` in handlers
- ✅ Include relevant data in response
- ✅ Use DTOs for data transfer
- ✅ Set appropriate status codes
- ✅ Include error details when applicable

### Handler Guidelines
- ✅ Implement `IRequestHandler<TRequest, TResponse>`
- ✅ Use dependency injection for dependencies
- ✅ Handle exceptions appropriately
- ✅ Return meaningful response messages
- ✅ Use async/await for I/O operations
- ✅ Include proper cancellation token support

### Validation Guidelines
- ✅ Always inherit from `BaseValidator<T>`
- ✅ Use FluentValidation rules
- ✅ Provide clear error messages
- ✅ Validate business rules, not just data format
- ✅ Include custom validation logic when needed
- ✅ Use conditional validation with `When` clauses

### Pipeline Behavior Guidelines
- ✅ Register behaviors in correct order
- ✅ Use appropriate behavior types for different concerns
- ✅ Avoid logging sensitive information
- ✅ Implement proper cache key generation
- ✅ Handle validation exceptions gracefully

## Error Handling

### Success Response
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

### Error Response
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

### Exception Handling
```csharp
public async Task<CreatePatientResponse> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
{
    try
    {
        // Business logic implementation
        var patient = new Patient { /* ... */ };
        await _patientRepository.AddAsync(patient);

        return new CreatePatientResponse
        {
            IsSuccess = true,
            Message = "Patient created successfully",
            RequestId = request.RequestId,
            PatientId = patient.Id
        };
    }
    catch (ValidationException ex)
    {
        return new CreatePatientResponse
        {
            IsSuccess = false,
            Message = "Validation failed",
            RequestId = request.RequestId,
            StatusCode = 400,
            Errors = ex.Errors.Select(e => e.ErrorMessage).ToList()
        };
    }
    catch (Exception ex)
    {
        return new CreatePatientResponse
        {
            IsSuccess = false,
            Message = "An unexpected error occurred",
            RequestId = request.RequestId,
            StatusCode = 500,
            Errors = new List<string> { ex.Message }
        };
    }
}
```

## Testing Structure

### Unit Tests
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

### Unit Test Example
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

### Integration Tests
```csharp
[Fact]
public async Task Send_CreatePatientCommand_ReturnsSuccessResponse()
{
    // Arrange
    var command = new CreatePatientCommand
    {
        FirstName = "John",
        LastName = "Doe",
        DateOfBirth = new DateTime(1990, 1, 1),
        Gender = "Male"
    };

    // Act
    var result = await _mediator.Send(command);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotEqual(Guid.Empty, result.PatientId);
}
```

## Immutable Standards (Never Change)

### 1. MediatR I/O Pattern (Immutable Rule)
- **BaseRequest<TResponse>**: All requests inherit from BaseRequest
- **BaseResponse**: All responses inherit from BaseResponse
- **IRequestHandler<TRequest, TResponse>**: All handlers implement IRequestHandler
- **Pipeline Behaviors**: Cross-cutting concerns via behaviors only

### 2. Request/Response Structure (Immutable Rule)
- **RequestId**: Every request has a unique identifier
- **CorrelationId**: For request tracing
- **TenantId**: For multi-tenancy support
- **IsSuccess**: Every response indicates success/failure
- **Message**: Every response has a descriptive message

### 3. Validation Strategy (Immutable Rule)
- **FluentValidation**: All validation uses FluentValidation
- **BaseValidator<T>**: All validators inherit from BaseValidator
- **Automatic Validation**: ValidationBehavior handles all validation
- **Clear Messages**: User-friendly error messages

### 4. Pipeline Behavior Order (Immutable Rule)
- **ValidationBehavior**: First in pipeline
- **LoggingBehavior**: Second in pipeline
- **CachingBehavior**: Third in pipeline (for queries only)
- **Handler**: Last in pipeline

### 5. Error Handling Strategy (Immutable Rule)
- **Consistent Responses**: All responses follow same pattern
- **Status Codes**: Appropriate HTTP status codes
- **Error Details**: Detailed error information
- **Exception Handling**: Proper exception handling in handlers

### 6. Caching Strategy (Immutable Rule)
- **Query Only**: Cache queries, never commands
- **Cache Keys**: Appropriate cache key generation
- **TTL**: Reasonable time-to-live values
- **Invalidation**: Proper cache invalidation

### 7. Logging Strategy (Immutable Rule)
- **Request Logging**: Log all incoming requests
- **Response Logging**: Log all outgoing responses
- **Performance**: Include timing information
- **Sensitive Data**: Never log sensitive information

### 8. Testing Strategy (Immutable Rule)
- **Unit Tests**: Test handlers in isolation
- **Integration Tests**: Test with MediatR pipeline
- **Validation Tests**: Test all validation rules
- **Behavior Tests**: Test pipeline behaviors

## Anti-Patterns to Avoid (Never Allowed)

1. **Direct Handler Calls**: Bypassing MediatR pipeline
2. **Custom Request/Response**: Not inheriting from BaseRequest/BaseResponse
3. **Mixed I/O Patterns**: Using different I/O patterns
4. **Synchronous Operations**: Blocking operations in async handlers
5. **Missing Validation**: Requests without proper validation
6. **Poor Error Handling**: Inconsistent error responses
7. **Missing Logging**: Operations without proper logging
8. **Inappropriate Caching**: Caching commands or sensitive data
9. **Hardcoded Values**: Magic numbers and hardcoded strings
10. **Circular Dependencies**: Dependencies between handlers

## Validation Checklist (Must Pass Always)

- [ ] All requests inherit from BaseRequest<TResponse> or BasePagedRequest<TResponse>
- [ ] All responses inherit from BaseResponse or PagedResponse<T>
- [ ] All handlers implement IRequestHandler<TRequest, TResponse>
- [ ] All validators inherit from BaseValidator<T>
- [ ] MediatR pipeline behaviors are properly configured
- [ ] No direct handler calls bypassing MediatR
- [ ] All requests have proper validation
- [ ] All responses follow consistent patterns
- [ ] Error handling is consistent
- [ ] Logging covers all operations
- [ ] Caching is implemented appropriately
- [ ] Testing covers all scenarios
- [ ] Performance is optimized

## Performance Considerations

1. **Async/Await**: Use throughout for I/O operations
2. **Caching**: Implement appropriate caching strategies
3. **Pagination**: For large data sets
4. **Projection**: Use DTOs to limit data transfer
5. **Connection Pooling**: Database connection management
6. **Memory Management**: Proper disposal of resources

## Security Implementation

1. **Input Validation**: Comprehensive validation at all layers
2. **Authorization**: Proper authorization checks
3. **Audit Logging**: Track all data modifications
4. **Data Protection**: Sensitive data encryption
5. **Rate Limiting**: API rate limiting
6. **Correlation IDs**: Request tracing for security
7. **Tenant Isolation**: Ensure proper tenant isolation

---

**This document defines the immutable MediatR I/O Pattern standards that should NEVER change, ensuring consistency and maintainability across the FHIR-AI Backend project.**

**For complete Clean Architecture implementation details, see `CLEAN_ARCHITECTURE_REFERENCE.md`**
**For complete CQRS implementation details, see `CQRS_PATTERN_REFERENCE.md`**
**For complete AutoMapper implementation details, see `AUTOMAPPER_PATTERN_REFERENCE.md`**
**For complete Healthcare Data implementation details, see `HEALTHCARE_DATA_PATTERN_REFERENCE.md`**
