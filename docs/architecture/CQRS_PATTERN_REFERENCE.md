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

## Implementation Patterns

### Command Pattern

#### Command Structure
```csharp
public record CreatePatientCommand : IRequest<Result<PatientDto>>
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public DateTime DateOfBirth { get; init; }
    public string Email { get; init; }
    public string PhoneNumber { get; init; }
}
```

#### Command Handler Structure
```csharp
public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Result<PatientDto>>
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

    public async Task<Result<PatientDto>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Business logic validation
            if (await _repository.ExistsByEmailAsync(request.Email, cancellationToken))
            {
                return Result<PatientDto>.Failure("Patient with this email already exists");
            }

            // Create domain entity
            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                CreatedBy = _currentUserService.UserId,
                CreatedAt = DateTime.UtcNow,
                TenantId = _currentUserService.TenantId
            };

            // Persist to database
            var createdPatient = await _repository.AddAsync(patient, cancellationToken);

            // Map to DTO
            var patientDto = new PatientDto
            {
                Id = createdPatient.Id,
                FirstName = createdPatient.FirstName,
                LastName = createdPatient.LastName,
                DateOfBirth = createdPatient.DateOfBirth,
                Email = createdPatient.Email,
                PhoneNumber = createdPatient.PhoneNumber
            };

            _logger.LogInformation("Patient created successfully: {PatientId}", createdPatient.Id);
            return Result<PatientDto>.Success(patientDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating patient");
            return Result<PatientDto>.Failure("Failed to create patient");
        }
    }
}
```

#### Command Validator Structure
```csharp
public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(100)
            .WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(100)
            .WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .WithMessage("Date of birth is required")
            .LessThan(DateTime.UtcNow)
            .WithMessage("Date of birth must be in the past");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .MaximumLength(255)
            .WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("Invalid phone number format");
    }
}
```

### Query Pattern

#### Query Structure
```csharp
public record GetPatientQuery : IRequest<Result<PatientDto>>
{
    public Guid Id { get; init; }
}

public record GetPatientListQuery : IRequest<Result<PaginatedResult<PatientDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? SortBy { get; init; }
    public bool IsAscending { get; init; } = true;
}
```

#### Query Handler Structure
```csharp
public class GetPatientQueryHandler : IRequestHandler<GetPatientQuery, Result<PatientDto>>
{
    private readonly IPatientRepository _repository;
    private readonly ILogger<GetPatientQueryHandler> _logger;

    public GetPatientQueryHandler(
        IPatientRepository repository,
        ILogger<GetPatientQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<PatientDto>> Handle(GetPatientQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var patient = await _repository.GetByIdAsync(request.Id, cancellationToken);
            
            if (patient == null)
            {
                return Result<PatientDto>.Failure("Patient not found");
            }

            var patientDto = new PatientDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber
            };

            return Result<PatientDto>.Success(patientDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient with ID: {PatientId}", request.Id);
            return Result<PatientDto>.Failure("Failed to retrieve patient");
        }
    }
}
```

#### List Query Handler Structure
```csharp
public class GetPatientListQueryHandler : IRequestHandler<GetPatientListQuery, Result<PaginatedResult<PatientDto>>>
{
    private readonly IPatientRepository _repository;
    private readonly ILogger<GetPatientListQueryHandler> _logger;

    public GetPatientListQueryHandler(
        IPatientRepository repository,
        ILogger<GetPatientListQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<PatientDto>>> Handle(GetPatientListQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (patients, totalCount) = await _repository.GetPaginatedAsync(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm,
                request.SortBy,
                request.IsAscending,
                cancellationToken);

            var patientDtos = patients.Select(p => new PatientDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                DateOfBirth = p.DateOfBirth,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber
            }).ToList();

            var result = new PaginatedResult<PatientDto>
            {
                Items = patientDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            };

            return Result<PaginatedResult<PatientDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient list");
            return Result<PaginatedResult<PatientDto>>.Failure("Failed to retrieve patient list");
        }
    }
}
```

## Result Pattern

### Result Structure
```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public List<string>? ValidationErrors { get; }

    private Result(bool isSuccess, T? value, string? error, List<string>? validationErrors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        ValidationErrors = validationErrors;
    }

    public static Result<T> Success(T value) => new(true, value, null, null);
    public static Result<T> Failure(string error) => new(false, default, error, null);
    public static Result<T> ValidationFailure(List<string> errors) => new(false, default, null, errors);
}
```

### Paginated Result Structure
```csharp
public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
```

## Cross-Cutting Concerns

### Validation Behavior
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
            var errors = failures.Select(f => f.ErrorMessage).ToList();
            throw new ValidationException(failures);
        }

        return await next();
    }
}
```

### Logging Behavior
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
        _logger.LogInformation("Handling {RequestName} with {@Request}", typeof(TRequest).Name, request);
        
        var sw = Stopwatch.StartNew();
        var response = await next();
        sw.Stop();

        _logger.LogInformation("Handled {RequestName} in {ElapsedMilliseconds}ms", typeof(TRequest).Name, sw.ElapsedMilliseconds);
        
        return response;
    }
}
```

### Caching Behavior
```csharp
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(IDistributedCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Only cache queries, not commands
        if (request is not IRequest<TResponse> query) return await next();

        var cacheKey = $"{typeof(TRequest).Name}_{JsonSerializer.Serialize(request)}";
        var cachedResponse = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedResponse))
        {
            _logger.LogInformation("Returning cached response for {RequestName}", typeof(TRequest).Name);
            return JsonSerializer.Deserialize<TResponse>(cachedResponse)!;
        }

        var response = await next();

        // Cache successful responses for 5 minutes
        if (response is Result<TResponse> result && result.IsSuccess)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(response), options, cancellationToken);
        }

        return response;
    }
}
```

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

### Command Handler Tests
```csharp
public class CreatePatientCommandHandlerTests
{
    private readonly Mock<IPatientRepository> _repositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<ILogger<CreatePatientCommandHandler>> _loggerMock;
    private readonly CreatePatientCommandHandler _handler;

    public CreatePatientCommandHandlerTests()
    {
        _repositoryMock = new Mock<IPatientRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _loggerMock = new Mock<ILogger<CreatePatientCommandHandler>>();
        _handler = new CreatePatientCommandHandler(_repositoryMock.Object, _currentUserServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreatePatientCommand
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Email = "john.doe@example.com",
            PhoneNumber = "+1234567890"
        };

        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserServiceMock.Setup(x => x.TenantId).Returns(Guid.NewGuid());
        _repositoryMock.Setup(x => x.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var expectedPatient = new Patient { Id = Guid.NewGuid() };
        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPatient);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(expectedPatient.Id, result.Value.Id);
    }
}
```

### Query Handler Tests
```csharp
public class GetPatientQueryHandlerTests
{
    private readonly Mock<IPatientRepository> _repositoryMock;
    private readonly Mock<ILogger<GetPatientQueryHandler>> _loggerMock;
    private readonly GetPatientQueryHandler _handler;

    public GetPatientQueryHandlerTests()
    {
        _repositoryMock = new Mock<IPatientRepository>();
        _loggerMock = new Mock<ILogger<GetPatientQueryHandler>>();
        _handler = new GetPatientQueryHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingPatient_ReturnsSuccessResult()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var query = new GetPatientQuery { Id = patientId };

        var expectedPatient = new Patient
        {
            Id = patientId,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Email = "john.doe@example.com",
            PhoneNumber = "+1234567890"
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPatient);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(expectedPatient.Id, result.Value.Id);
        Assert.Equal(expectedPatient.FirstName, result.Value.FirstName);
    }
}
```

## Immutable CQRS Standards (Never Change)

### 1. Command Query Separation (Immutable Rule)
- **Commands**: Write operations only (Create, Update, Delete)
- **Queries**: Read operations only (Get, List, Search)
- **No Shared State**: Commands and Queries don't share models
- **Separate Models**: Command and Query models are distinct

### 2. Handler Responsibility (Immutable Rule)
- **Single Operation**: One handler = one operation
- **No Mixed Operations**: Commands don't read, Queries don't write
- **Clear Intent**: Handler purpose is immediately clear
- **Single Repository**: Each handler uses one repository

### 3. Immutability (Immutable Rule)
- **Records**: Use records for all commands and queries
- **Init-Only Properties**: All properties are init-only
- **No Side Effects**: Queries never modify state
- **Immutable DTOs**: All DTOs are immutable

### 4. Validation Strategy (Immutable Rule)
- **Command Validation**: Validate all commands
- **Query Validation**: Validate complex queries
- **FluentValidation**: Use FluentValidation for all validation
- **Pipeline Behavior**: Validation via pipeline behavior

### 5. Error Handling (Immutable Rule)
- **Result Pattern**: Use Result<T> for all responses
- **Consistent Errors**: Standardized error messages
- **Logging**: Log all errors appropriately
- **No Exceptions**: Don't throw exceptions for business logic

### 6. Performance (Immutable Rule)
- **Async/Await**: Use throughout for I/O operations
- **Caching**: Cache query results appropriately
- **Pagination**: Use pagination for large datasets
- **Projection**: Use DTOs to limit data transfer

### 7. Testing (Immutable Rule)
- **Unit Tests**: Test all handlers in isolation
- **Mock Dependencies**: Mock all external dependencies
- **Test Coverage**: Aim for >80% test coverage
- **Integration Tests**: Test complete workflows

## Anti-Patterns to Avoid (Never Allowed)

1. **Mixed Operations**: Commands that read data, Queries that write data
2. **Shared Models**: Using the same model for commands and queries
3. **Mutable Commands**: Commands with settable properties
4. **Side Effects in Queries**: Queries that modify state
5. **Complex Handlers**: Handlers with multiple responsibilities
6. **Direct Repository Access**: Bypassing handlers in API layer
7. **No Validation**: Commands or queries without validation
8. **Exception Throwing**: Throwing exceptions for business logic
9. **Synchronous Operations**: Using sync methods in async contexts
10. **No Error Handling**: Not handling errors in handlers

## Validation Checklist (Must Pass Always)

- [ ] Commands only perform write operations
- [ ] Queries only perform read operations
- [ ] All commands and queries use records
- [ ] All properties are init-only
- [ ] All commands have validators
- [ ] All handlers return Result<T>
- [ ] All handlers are async
- [ ] All handlers have proper error handling
- [ ] All handlers have unit tests
- [ ] No shared models between commands and queries

## Performance Considerations

1. **Async/Await**: Use throughout for I/O operations
2. **Caching**: Implement appropriate caching for queries
3. **Pagination**: Use pagination for large datasets
4. **Projection**: Use DTOs to limit data transfer
5. **Indexing**: Proper database indexing for queries
6. **Connection Pooling**: Efficient database connection management

## Security Implementation

1. **Authorization**: Check permissions in handlers
2. **Input Validation**: Validate all inputs thoroughly
3. **Audit Logging**: Log all command operations
4. **Data Encryption**: Encrypt sensitive data
5. **Rate Limiting**: Implement rate limiting for commands
6. **Tenant Isolation**: Ensure proper tenant isolation

---

**This document defines the immutable CQRS Pattern standards that should NEVER change, ensuring consistency and maintainability across the FHIR-AI Backend project.**

**For complete AutoMapper implementation details, see `AUTOMAPPER_PATTERN_REFERENCE.md`**
