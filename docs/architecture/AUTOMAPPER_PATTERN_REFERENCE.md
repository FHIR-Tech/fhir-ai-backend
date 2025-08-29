# AutoMapper Pattern Reference Guide

## Overview

This document defines the **immutable AutoMapper Pattern standards** for the FHIR-AI Backend project. These standards should **NEVER change**, regardless of Microsoft's framework updates or new patterns. They ensure consistency, maintainability, and proper object mapping between layers in Clean Architecture.

## Core AutoMapper Principles (Immutable)

### 1. Layer Separation (Immutable Rule)
- **Domain Entities**: Never mapped to DTOs directly
- **Application Layer**: Maps between Domain entities and DTOs
- **Infrastructure Layer**: Maps between Domain entities and database models
- **API Layer**: Maps between DTOs and API models

### 2. Unidirectional Mapping (Immutable Rule)
- **Domain → DTO**: One-way mapping from Domain to DTO
- **DTO → Domain**: Separate mapping for DTO to Domain
- **No Bidirectional**: Avoid bidirectional mapping complexity
- **Clear Direction**: Always know the mapping direction

### 3. Immutable Mapping (Immutable Rule)
- **Records**: Use records for all DTOs and mapping models
- **Init-Only Properties**: All properties are init-only
- **No Side Effects**: Mappings never modify source objects
- **Pure Functions**: Mapping functions are pure and predictable

### 4. Profile Organization (Immutable Rule)
- **Feature-Based**: Organize profiles by feature/domain
- **Single Responsibility**: Each profile handles one domain
- **Assembly Registration**: Automatic registration via assembly scanning
- **Naming Convention**: `{Feature}MappingProfile`

## Implementation Patterns

### Profile Structure

#### Basic Profile Structure
```csharp
public class PatientMappingProfile : Profile
{
    public PatientMappingProfile()
    {
        // Domain Entity to DTO mappings
        CreateMap<Patient, PatientDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => CalculateAge(src.DateOfBirth)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // DTO to Domain Entity mappings
        CreateMap<CreatePatientRequest, Patient>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => PatientStatus.Active));

        // Update mappings
        CreateMap<UpdatePatientRequest, Patient>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // List mappings
        CreateMap<Patient, PatientListItemDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => CalculateAge(src.DateOfBirth)));
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

#### Complex Profile Structure
```csharp
public class FhirResourceMappingProfile : Profile
{
    public FhirResourceMappingProfile()
    {
        // FHIR Resource to Domain Entity
        CreateMap<Patient, FhirPatient>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => new HumanName
            {
                Use = NameUse.Official,
                Family = src.LastName,
                Given = new string[] { src.FirstName }
            }))
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.DateOfBirth.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.Telecom, opt => opt.MapFrom(src => new ContactPoint[]
            {
                new ContactPoint
                {
                    System = ContactPointSystem.Email,
                    Value = src.Email
                },
                new ContactPoint
                {
                    System = ContactPointSystem.Phone,
                    Value = src.PhoneNumber
                }
            }))
            .ForMember(dest => dest.Meta, opt => opt.MapFrom(src => new Meta
            {
                LastUpdated = src.UpdatedAt ?? src.CreatedAt,
                VersionId = src.Version.ToString()
            }));

        // Domain Entity to FHIR Resource
        CreateMap<FhirPatient, Patient>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name?.Given?.FirstOrDefault()))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Name?.Family))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateTime.Parse(src.BirthDate)))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => 
                src.Telecom?.FirstOrDefault(t => t.System == ContactPointSystem.Email)?.Value))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => 
                src.Telecom?.FirstOrDefault(t => t.System == ContactPointSystem.Phone)?.Value));
    }
}
```

### DTO Structures

#### Request DTOs
```csharp
public record CreatePatientRequest
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
}

public record UpdatePatientRequest
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
}
```

#### Response DTOs
```csharp
public record PatientDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public int Age { get; init; }
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record PatientListItemDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public int Age { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}
```

### Mapper Service Implementation

#### IMapper Service Interface
```csharp
public interface IMapperService
{
    TDestination Map<TDestination>(object source);
    TDestination Map<TSource, TDestination>(TSource source);
    TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
    IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source);
    IQueryable<TDestination> ProjectTo<TSource, TDestination>(IQueryable<TSource> source);
}
```

#### Mapper Service Implementation
```csharp
public class MapperService : IMapperService
{
    private readonly IMapper _mapper;

    public MapperService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public TDestination Map<TDestination>(object source)
    {
        return _mapper.Map<TDestination>(source);
    }

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        return _mapper.Map<TSource, TDestination>(source);
    }

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        return _mapper.Map(source, destination);
    }

    public IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source)
    {
        return _mapper.Map<IEnumerable<TSource>, IEnumerable<TDestination>>(source);
    }

    public IQueryable<TDestination> ProjectTo<TSource, TDestination>(IQueryable<TSource> source)
    {
        return _mapper.ProjectTo<TDestination>(source);
    }
}
```

### Usage in Handlers

#### Command Handler with Mapping
```csharp
public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Result<PatientDto>>
{
    private readonly IPatientRepository _repository;
    private readonly IMapperService _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreatePatientCommandHandler> _logger;

    public CreatePatientCommandHandler(
        IPatientRepository repository,
        IMapperService mapper,
        ICurrentUserService currentUserService,
        ILogger<CreatePatientCommandHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<PatientDto>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Map command to domain entity
            var patient = _mapper.Map<CreatePatientCommand, Patient>(request);
            
            // Set additional properties
            patient.Id = Guid.NewGuid();
            patient.CreatedBy = _currentUserService.UserId;
            patient.CreatedAt = DateTime.UtcNow;
            patient.TenantId = _currentUserService.TenantId;
            patient.Status = PatientStatus.Active;

            // Persist to database
            var createdPatient = await _repository.AddAsync(patient, cancellationToken);

            // Map to DTO for response
            var patientDto = _mapper.Map<Patient, PatientDto>(createdPatient);

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

#### Query Handler with Projection
```csharp
public class GetPatientListQueryHandler : IRequestHandler<GetPatientListQuery, Result<PaginatedResult<PatientListItemDto>>>
{
    private readonly IPatientRepository _repository;
    private readonly IMapperService _mapper;
    private readonly ILogger<GetPatientListQueryHandler> _logger;

    public GetPatientListQueryHandler(
        IPatientRepository repository,
        IMapperService mapper,
        ILogger<GetPatientListQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<PatientListItemDto>>> Handle(GetPatientListQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get queryable from repository
            var query = _repository.GetQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(p => 
                    p.FirstName.Contains(request.SearchTerm) || 
                    p.LastName.Contains(request.SearchTerm) ||
                    p.Email.Contains(request.SearchTerm));
            }

            // Apply sorting
            query = request.SortBy?.ToLower() switch
            {
                "name" => request.IsAscending 
                    ? query.OrderBy(p => p.FirstName).ThenBy(p => p.LastName)
                    : query.OrderByDescending(p => p.FirstName).ThenByDescending(p => p.LastName),
                "email" => request.IsAscending 
                    ? query.OrderBy(p => p.Email)
                    : query.OrderByDescending(p => p.Email),
                "createdat" => request.IsAscending 
                    ? query.OrderBy(p => p.CreatedAt)
                    : query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            // Get total count
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination and projection
            var patientDtos = await _mapper.ProjectTo<Patient, PatientListItemDto>(query)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var result = new PaginatedResult<PatientListItemDto>
            {
                Items = patientDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            };

            return Result<PaginatedResult<PatientListItemDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient list");
            return Result<PaginatedResult<PatientListItemDto>>.Failure("Failed to retrieve patient list");
        }
    }
}
```

## File Organization

### Profile Structure
```
HealthTech.Application/
├── Common/
│   ├── Mapping/
│   │   ├── IMapperService.cs
│   │   ├── MapperService.cs
│   │   └── Profiles/
│   │       ├── PatientMappingProfile.cs
│   │       ├── FhirResourceMappingProfile.cs
│   │       └── {Feature}MappingProfile.cs
```

### DTO Structure
```
HealthTech.Application/
├── {Feature}/
│   ├── DTOs/
│   │   ├── {Entity}Dto.cs
│   │   ├── {Entity}ListItemDto.cs
│   │   ├── Create{Entity}Request.cs
│   │   └── Update{Entity}Request.cs
```

## Dependency Injection Configuration

```csharp
// Application Layer DI
services.AddAutoMapper(cfg =>
{
    // Scan for profiles in the current assembly
    cfg.AddMaps(typeof(DependencyInjection).Assembly);
    
    // Configure validation
    cfg.ValidateInlineMaps = false;
    cfg.ValidateMaps = true;
});

// Register mapper service
services.AddScoped<IMapperService, MapperService>();
```

## Testing Structure

### Profile Tests
```csharp
public class PatientMappingProfileTests
{
    private readonly IMapper _mapper;

    public PatientMappingProfileTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PatientMappingProfile>();
        });

        configuration.AssertConfigurationIsValid();
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void Map_PatientToPatientDto_ShouldMapCorrectly()
    {
        // Arrange
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Email = "john.doe@example.com",
            PhoneNumber = "+1234567890",
            Status = PatientStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = _mapper.Map<PatientDto>(patient);

        // Assert
        Assert.Equal(patient.Id, result.Id);
        Assert.Equal("John Doe", result.FullName);
        Assert.Equal(30, result.Age);
        Assert.Equal(patient.Email, result.Email);
        Assert.Equal(patient.PhoneNumber, result.PhoneNumber);
        Assert.Equal("Active", result.Status);
    }

    [Fact]
    public void Map_CreatePatientRequestToPatient_ShouldMapCorrectly()
    {
        // Arrange
        var request = new CreatePatientRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            DateOfBirth = DateTime.UtcNow.AddYears(-25),
            Email = "jane.smith@example.com",
            PhoneNumber = "+0987654321"
        };

        // Act
        var result = _mapper.Map<Patient>(request);

        // Assert
        Assert.Equal(request.FirstName, result.FirstName);
        Assert.Equal(request.LastName, result.LastName);
        Assert.Equal(request.DateOfBirth, result.DateOfBirth);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.PhoneNumber, result.PhoneNumber);
        Assert.Equal(PatientStatus.Active, result.Status);
        Assert.Equal(Guid.Empty, result.Id); // Should be ignored
    }
}
```

### Mapper Service Tests
```csharp
public class MapperServiceTests
{
    private readonly IMapperService _mapperService;
    private readonly IMapper _mapper;

    public MapperServiceTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PatientMappingProfile>();
        });

        _mapper = configuration.CreateMapper();
        _mapperService = new MapperService(_mapper);
    }

    [Fact]
    public void Map_WithValidSource_ShouldReturnMappedObject()
    {
        // Arrange
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Email = "john.doe@example.com",
            PhoneNumber = "+1234567890",
            Status = PatientStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = _mapperService.Map<PatientDto>(patient);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(patient.Id, result.Id);
        Assert.Equal("John Doe", result.FullName);
    }

    [Fact]
    public void Map_WithEnumerable_ShouldReturnMappedEnumerable()
    {
        // Arrange
        var patients = new List<Patient>
        {
            new Patient { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
            new Patient { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" }
        };

        // Act
        var result = _mapperService.Map<Patient, PatientDto>(patients);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, dto => Assert.NotNull(dto.FullName));
    }
}
```

## Immutable AutoMapper Standards (Never Change)

### 1. Layer Separation (Immutable Rule)
- **Domain Entities**: Never mapped to DTOs directly
- **Application Layer**: Maps between Domain entities and DTOs
- **Infrastructure Layer**: Maps between Domain entities and database models
- **API Layer**: Maps between DTOs and API models

### 2. Unidirectional Mapping (Immutable Rule)
- **Domain → DTO**: One-way mapping from Domain to DTO
- **DTO → Domain**: Separate mapping for DTO to Domain
- **No Bidirectional**: Avoid bidirectional mapping complexity
- **Clear Direction**: Always know the mapping direction

### 3. Profile Organization (Immutable Rule)
- **Feature-Based**: Organize profiles by feature/domain
- **Single Responsibility**: Each profile handles one domain
- **Assembly Registration**: Automatic registration via assembly scanning
- **Naming Convention**: `{Feature}MappingProfile`

### 4. DTO Design (Immutable Rule)
- **Records**: Use records for all DTOs
- **Init-Only Properties**: All properties are init-only
- **Immutable**: DTOs are immutable after creation
- **Clear Purpose**: Each DTO has a specific purpose

### 5. Mapping Configuration (Immutable Rule)
- **Explicit Mapping**: Always define explicit mappings
- **Ignore Properties**: Explicitly ignore properties that shouldn't be mapped
- **Custom Resolvers**: Use custom resolvers for complex mappings
- **Validation**: Validate mapping configuration in tests

### 6. Performance (Immutable Rule)
- **ProjectTo**: Use ProjectTo for IQueryable projections
- **Lazy Loading**: Avoid lazy loading in mappings
- **Efficient Queries**: Ensure mappings don't cause N+1 queries
- **Caching**: Leverage AutoMapper's built-in caching

### 7. Testing (Immutable Rule)
- **Profile Tests**: Test all mapping profiles
- **Configuration Validation**: Validate mapping configuration
- **Edge Cases**: Test edge cases and null values
- **Integration Tests**: Test mappings in integration scenarios

## Anti-Patterns to Avoid (Never Allowed)

1. **Bidirectional Mapping**: Avoid complex bidirectional mappings
2. **Domain to DTO Direct Mapping**: Never map Domain entities directly to DTOs in Domain layer
3. **Circular References**: Avoid circular references in mappings
4. **Complex Logic in Mappings**: Keep mapping logic simple and predictable
5. **Lazy Loading in Mappings**: Don't trigger lazy loading during mapping
6. **Side Effects**: Mappings should never have side effects
7. **Nested Mappings**: Avoid deeply nested complex mappings
8. **Dynamic Mapping**: Don't use dynamic mapping features
9. **Global Mappings**: Avoid global mapping configurations
10. **Mapping in Domain**: Never use AutoMapper in Domain layer

## Validation Checklist (Must Pass Always)

- [ ] All mappings are unidirectional
- [ ] Domain entities are never mapped directly to DTOs
- [ ] All DTOs use records with init-only properties
- [ ] All profiles are feature-based and single responsibility
- [ ] All mappings are explicit and well-defined
- [ ] All profiles have comprehensive tests
- [ ] Mapping configuration is validated
- [ ] No circular references in mappings
- [ ] No side effects in mapping logic
- [ ] Performance considerations are addressed

## Performance Considerations

1. **ProjectTo**: Use ProjectTo for IQueryable projections to avoid loading full entities
2. **Efficient Queries**: Ensure mappings don't cause N+1 queries
3. **Lazy Loading**: Avoid lazy loading in mappings
4. **Caching**: Leverage AutoMapper's built-in caching
5. **Batch Operations**: Use batch operations for large datasets
6. **Memory Usage**: Be mindful of memory usage with large mappings

## Security Implementation

1. **Data Sanitization**: Sanitize data during mapping
2. **Sensitive Data**: Never map sensitive data to DTOs
3. **Authorization**: Check authorization before mapping sensitive data
4. **Audit Trail**: Maintain audit trail for data transformations
5. **Validation**: Validate mapped data after transformation
6. **Encryption**: Handle encrypted data appropriately in mappings

---

**This document defines the immutable AutoMapper Pattern standards that should NEVER change, ensuring consistency and maintainability across the FHIR-AI Backend project.**
