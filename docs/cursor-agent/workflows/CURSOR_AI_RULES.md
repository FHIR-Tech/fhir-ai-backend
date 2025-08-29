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
1. Apply field organization pattern to new entities
2. Use correct section headers with proper formatting
3. Group related fields according to defined categories
4. Apply validation attributes based on field type
5. Include XML documentation for all fields
6. Follow naming conventions consistently

### Validation
- All new code must pass quality gates
- Documentation must be complete
- Tests must cover all functionality
- Architecture principles must be followed

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
