# Authentication Module

This directory contains the authentication-related application logic following Clean Architecture principles and CQRS pattern.

## Directory Structure

```
Authentication/
├── Commands/                    # Command objects (IRequest)
│   ├── Login/                  # Login command group
│   │   ├── LoginCommand.cs
│   │   ├── LoginCommandHandler.cs
│   │   └── LoginCommandValidator.cs
│   ├── Logout/                 # Logout command group
│   │   ├── LogoutCommand.cs
│   │   ├── LogoutCommandHandler.cs
│   │   └── LogoutCommandValidator.cs
│   └── RefreshToken/           # Refresh token command group
│       ├── RefreshTokenCommand.cs
│       ├── RefreshTokenCommandHandler.cs
│       └── RefreshTokenCommandValidator.cs
├── Queries/                    # Query objects (IRequest)
│   └── GetCurrentUser/         # Get current user query group
│       ├── GetCurrentUserQuery.cs
│       └── GetCurrentUserQueryHandler.cs
├── DTOs/                       # Data Transfer Objects
│   ├── UserInfo.cs
│   ├── LoginResponse.cs
│   ├── LogoutResponse.cs
│   ├── RefreshTokenResponse.cs
│   └── GetCurrentUserResponse.cs
└── README.md                   # This documentation
```

## Components

### Commands
Each command is organized in its own directory containing the command, handler, and validator:

#### Login Command Group
- **LoginCommand**: User authentication request
- **LoginCommandHandler**: Processes user authentication with JWT token generation
- **LoginCommandValidator**: Validates login credentials

#### Logout Command Group
- **LogoutCommand**: User session termination request
- **LogoutCommandHandler**: Handles user session termination
- **LogoutCommandValidator**: Validates logout session token

#### RefreshToken Command Group
- **RefreshTokenCommand**: Token refresh request
- **RefreshTokenCommandHandler**: Manages token refresh operations
- **RefreshTokenCommandValidator**: Validates refresh token

### Queries
Each query is organized in its own directory containing the query and handler:

#### GetCurrentUser Query Group
- **GetCurrentUserQuery**: Retrieve current authenticated user information
- **GetCurrentUserQueryHandler**: Retrieves current user information

### DTOs (Data Transfer Objects)
- **UserInfo**: Shared user information structure
- **LoginResponse**: Authentication response with tokens and user info
- **LogoutResponse**: Session termination response
- **RefreshTokenResponse**: Token refresh response
- **GetCurrentUserResponse**: Current user information response

## Design Principles

### Clean Architecture Compliance
- **Feature-based organization**: Each command/query has its own directory
- **Cohesion**: Related files (command, handler, validator) are grouped together
- **Separation of concerns**: Clear separation between commands, queries, and DTOs
- **Dependency direction**: All components depend only on interfaces from the Domain layer

### CQRS Pattern
- **Commands**: Write operations (login, logout, refresh token)
- **Queries**: Read operations (get current user)
- **Clear separation**: Each operation type has its own namespace and structure

### Validation Strategy
- **FluentValidation**: Each command has its own validator
- **Input validation**: Validators ensure data integrity before processing
- **Error handling**: Structured error responses with user-friendly messages

## Usage

### Dependency Injection
All handlers and validators are automatically registered with the DI container through the Application layer's dependency injection configuration.

### MediatR Integration
Commands and queries are processed through MediatR pipeline:
- Automatic validation through FluentValidation
- Handler execution with dependency injection
- Response mapping to DTOs

### Namespace Structure
```
HealthTech.Application.Authentication.Commands.Login
HealthTech.Application.Authentication.Commands.Logout
HealthTech.Application.Authentication.Commands.RefreshToken
HealthTech.Application.Authentication.Queries.GetCurrentUser
HealthTech.Application.Authentication.DTOs
```

### Error Handling
All handlers include comprehensive error handling with:
- Structured error responses
- Proper exception logging
- User-friendly error messages

## Security Considerations

- Password validation and hashing handled by infrastructure services
- JWT token generation and validation through dedicated services
- Session management with proper cleanup
- Multi-tenant support with tenant isolation
- Role-based access control integration

## Benefits of This Structure

1. **Feature Cohesion**: Related files are grouped together
2. **Easy Navigation**: Clear directory structure makes it easy to find files
3. **Scalability**: Easy to add new commands/queries following the same pattern
4. **Maintainability**: Each feature is self-contained
5. **Testability**: Easy to test individual features in isolation
