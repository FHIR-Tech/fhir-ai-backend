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

### 4. MediatR I/O Pattern Integration (Immutable Rule)
- **MediatR I/O Pattern**: Standard implementation for all CQRS operations
- **BaseRequest<TResponse>**: All requests inherit from BaseRequest
- **BaseResponse**: All responses inherit from BaseResponse
- **Pipeline Behaviors**: Cross-cutting concerns via behaviors
- **For complete implementation details, see `MEDIATR_IO_PATTERN_REFERENCE.md`**

## Official I/O Patterns

### MediatR I/O Pattern (Standard for FHIR-AI Backend)

**For complete MediatR I/O Pattern implementation details, see `MEDIATR_IO_PATTERN_REFERENCE.md`**

The FHIR-AI Backend uses MediatR I/O pattern as the standard implementation for all CQRS operations. This pattern provides:

- **BaseRequest<TResponse>**: Standard base class for all requests (commands and queries)
- **BasePagedRequest<TResponse>**: Standard base class for paged requests
- **BaseResponse**: Standard base class for all responses
- **PagedResponse<T>**: Standard base class for paged responses
- **Pipeline Behaviors**: Validation, logging, and caching behaviors
- **FluentValidation Integration**: Automatic request validation
- **Consistent Error Handling**: Standardized error responses
- **Multi-tenancy Support**: Built-in tenant isolation
- **Request Tracing**: Correlation IDs for debugging

### CQRS Pattern Integration

The CQRS pattern is implemented using MediatR I/O pattern with the following structure:

- **Commands**: Write operations inheriting from BaseRequest<TResponse>
- **Queries**: Read operations inheriting from BaseRequest<TResponse> or BasePagedRequest<TResponse>
- **Handlers**: IRequestHandler<TRequest, TResponse> implementations
- **Validation**: FluentValidation with BaseValidator<T> inheritance
- **Pipeline**: Cross-cutting concerns via MediatR pipeline behaviors

## Implementation Patterns

**For complete MediatR I/O Pattern implementation details, see `MEDIATR_IO_PATTERN_REFERENCE.md`**

The CQRS pattern implementation follows these key principles:

### 1. Command/Query Separation
- **Commands**: Write operations only (Create, Update, Delete)
- **Queries**: Read operations only (Get, List, Search)
- **Separate Models**: Command and Query models are distinct
- **No Shared State**: Commands and Queries don't share models

### 2. Handler Implementation
- **Command Handlers**: Handle single write operation
- **Query Handlers**: Handle single read operation
- **Single Responsibility**: One handler = one operation
- **Clear Intent**: Handler purpose is immediately clear

### 3. Validation Strategy
- **Input Validation**: At Application layer boundaries
- **Business Validation**: In Domain layer
- **FluentValidation**: For all input validation
- **Clear Messages**: User-friendly error messages

### 4. File Organization
- **Feature-based**: Organize by business features
- **Command/Query Separation**: Clear separation of concerns
- **Consistent Naming**: Descriptive and consistent naming
- **Proper Structure**: Follow established file structure patterns

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

### 4. MediatR I/O Pattern Integration (Immutable Rule)
- **MediatR I/O Pattern**: Standard implementation for all CQRS operations
- **BaseRequest<TResponse>**: All requests inherit from BaseRequest
- **BaseResponse**: All responses inherit from BaseResponse
- **Pipeline Behaviors**: Cross-cutting concerns via behaviors
- **For complete implementation details, see `MEDIATR_IO_PATTERN_REFERENCE.md`**

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
