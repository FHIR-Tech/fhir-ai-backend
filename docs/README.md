# Documentation Directory

This directory contains all technical documentation for the FHIR-AI Backend project.

## Directory Structure

### `/api/` - API Documentation
- **`FHIR_ENDPOINTS_IMPLEMENTATION.md`** - Detailed implementation guide for FHIR endpoints
- **`ENDPOINT_VERIFICATION_REPORT.md`** - Verification report for implemented endpoints

### `/architecture/` - Architecture Documentation
- System architecture diagrams and design decisions
- Clean Architecture implementation details
- Database schema documentation
- Security architecture documentation

### `/cursor-agent/` - Cursor AI Documentation
- **`CURSOR_AI_RULES.md`** - Comprehensive development rules and Clean Architecture standards
- **`DOMAIN_ENTITY_STANDARDS.md`** - Entity creation and organization standards
- Implementation reports and session logs

## Clean Architecture Standards (Immutable)

### Core Principles
The FHIR-AI Backend follows Clean Architecture principles with these **immutable standards** that should never change, regardless of Microsoft's framework updates:

#### 1. Layer Structure
```
HealthTech.Domain/        # Core business logic (zero dependencies)
HealthTech.Application/   # Use cases and orchestration (depends on Domain only)
HealthTech.Infrastructure/ # External concerns (depends on Application + Domain)
HealthTech.API/          # HTTP endpoints (depends on Application + Infrastructure)
```

#### 2. Dependency Rules (Never Violate)
- **Domain Layer**: Zero external dependencies
- **Application Layer**: Depends only on Domain
- **Infrastructure Layer**: Implements Domain interfaces
- **API Layer**: Uses Application handlers only
- **No Circular Dependencies**: Ever

#### 3. Framework Independence
- **Domain & Application**: Framework-agnostic
- **Infrastructure & API**: Can use framework-specific code
- **No Framework Coupling**: Domain never depends on EF Core, NHapi, etc.

#### 4. CQRS Pattern
- **Commands**: Write operations (Create, Update, Delete)
- **Queries**: Read operations
- **Separate Models**: Command and Query models are distinct
- **MediatR**: For command/query handling
- **Immutable Records**: All commands and queries use records with init-only properties
- **Result Pattern**: Consistent error handling with Result<T> pattern

#### 5. Repository Pattern
- **Domain**: Defines repository interfaces
- **Infrastructure**: Implements repository interfaces
- **Application**: Uses repository interfaces only

### Anti-Patterns (Never Allowed)
1. Anemic Domain Model (entities with no behavior)
2. Fat Controllers (business logic in API layer)
3. Repository Leakage (infrastructure concerns in Domain)
4. Direct Database Access (bypassing Application layer)
5. Circular Dependencies (any circular references)

### Validation Checklist
- [ ] Domain layer has zero external dependencies
- [ ] Application layer depends only on Domain
- [ ] Infrastructure layer implements Domain interfaces
- [ ] API layer uses Application handlers
- [ ] No circular dependencies exist
- [ ] All public APIs have proper documentation
- [ ] Validation occurs at appropriate layers
- [ ] Error handling follows established patterns
- [ ] Testing covers all business logic

**For complete Clean Architecture implementation details, see `/cursor-agent/CURSOR_AI_RULES.md`**

**For complete CQRS Pattern implementation details, see `/architecture/CQRS_PATTERN_REFERENCE.md`**

**For complete AutoMapper Pattern implementation details, see `/architecture/AUTOMAPPER_PATTERN_REFERENCE.md`**

## Documentation Standards

### File Naming
- Use descriptive names: `{feature}_{description}.md`
- Use kebab-case for multi-word filenames
- Include date/version in filename when appropriate

### Content Structure
- Start with a clear title and overview
- Include table of contents for long documents
- Use consistent heading hierarchy (H1, H2, H3)
- Include code examples where relevant
- Add diagrams and screenshots when helpful

### Maintenance
- Keep documentation up-to-date with code changes
- Review and update documentation regularly
- Link related documents together
- Include version information

## Documentation Types

### Technical Documentation
- API specifications and examples
- Database schema and migrations
- Configuration guides
- Deployment procedures

### Architecture Documentation
- System design decisions
- Component interactions
- Data flow diagrams
- Security considerations

### User Documentation
- Setup and installation guides
- Usage examples
- Troubleshooting guides
- Best practices

## Contributing

When adding new documentation:
1. Follow the established naming conventions
2. Place files in the appropriate subdirectory
3. Update this README if adding new categories
4. Ensure all links are working
5. Review for accuracy and completeness
6. **Always follow Clean Architecture principles** as defined in the immutable standards
