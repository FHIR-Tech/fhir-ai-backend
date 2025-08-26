# FHIR-AI Backend

A modern, FHIR-compliant healthcare backend system built with .NET 8, PostgreSQL, and Clean Architecture principles. This system provides a robust foundation for healthcare applications with AI integration capabilities.

## 🏗️ Architecture

### Clean Architecture Layers

- **Domain Layer**: Core business entities and rules
- **Application Layer**: Use cases, commands, queries (CQRS pattern)
- **Infrastructure Layer**: External concerns (database, external APIs)
- **API Layer**: HTTP endpoints and controllers

### Technology Stack

- **.NET 8**: Latest LTS version with minimal APIs
- **PostgreSQL**: Primary database with JSONB for FHIR resources
- **Entity Framework Core**: ORM with code-first approach
- **MediatR**: CQRS pattern implementation
- **FluentValidation**: Input validation
- **Firely SDK**: FHIR resource handling
- **NHapi**: HL7 v2 integration
- **Docker**: Containerization and deployment

## 🚀 Features

### Core FHIR Features

- ✅ FHIR R4B compliant resource storage
- ✅ JSONB-based efficient querying
- ✅ Multi-tenant architecture with Row Level Security
- ✅ Full CRUD operations for all FHIR resources
- ✅ Search and filtering capabilities
- ✅ Version history tracking
- ✅ Audit logging for compliance

### Security & Compliance

- ✅ SMART on FHIR authentication
- ✅ Row Level Security (RLS) in PostgreSQL
- ✅ Comprehensive audit trail
- ✅ Data encryption at rest
- ✅ CORS configuration for healthcare domains
- ✅ Rate limiting and security headers

### Development Features

- ✅ Clean Architecture implementation
- ✅ CQRS pattern with MediatR
- ✅ Comprehensive unit and integration tests
- ✅ Docker containerization
- ✅ Health checks and monitoring
- ✅ OpenAPI/Swagger documentation

## 📁 Project Structure

```
fhir-ai-backend/
├── src/
│   ├── HealthTech.Domain/           # Domain entities and business rules
│   ├── HealthTech.Application/      # Use cases and application logic
│   ├── HealthTech.Infrastructure/   # External concerns and implementations
│   ├── HealthTech.API/             # HTTP API layer
│   └── HealthTech.Database/        # Database migrations and seeding
├── tests/
│   ├── HealthTech.Domain.Tests/
│   ├── HealthTech.Application.Tests/
│   └── HealthTech.API.Tests/
├── scripts/                        # Database initialization scripts
├── Dockerfile                      # Multi-stage Docker build
├── docker-compose.yml             # Development environment
└── README.md
```

## 🛠️ Getting Started

### Prerequisites

- .NET 8.0 SDK
- Docker and Docker Compose
- PostgreSQL 14+ (or use Docker)
- Visual Studio 2022 / VS Code

### Quick Start with Docker

1. **Clone the repository**

```bash
git clone https://github.com/your-org/fhir-ai-backend.git
cd fhir-ai-backend
```

2. **Start the development environment**

```bash
docker-compose up -d
```

3. **Access the application**
   - API: http://localhost:5000
   - Swagger UI: http://localhost:5000
   - Health Check: http://localhost:5000/health

### Local Development

1. **Restore dependencies**

```bash
dotnet restore
```

2. **Set up the database**

```bash
# Using Docker
docker run --name fhir-ai-postgres -e POSTGRES_DB=fhir-ai -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=Post:!@#4 -p 5432:5432 -d postgres:14

# Run database initialization
psql -h localhost -U postgres -d fhir-ai -f scripts/init-db.sql
```

3. **Update connection string**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=fhir-ai;Username=postgres;Password=Post:!@#4"
  }
}
```

4. **Run the application**

```bash
cd src/HealthTech.API
dotnet run
```

## 📚 API Documentation

### FHIR Endpoints

All FHIR endpoints follow the standard FHIR REST API pattern:

- `GET /fhir/{resourceType}` - Search resources
- `GET /fhir/{resourceType}/{id}` - Get specific resource
- `POST /fhir/{resourceType}` - Create new resource
- `PUT /fhir/{resourceType}/{id}` - Update resource
- `DELETE /fhir/{resourceType}/{id}` - Delete resource
- `GET /fhir/{resourceType}/{id}/_history` - Get resource history

### Authentication

The API supports SMART on FHIR authentication:

```bash
# Example request with Bearer token
curl -H "Authorization: Bearer YOUR_TOKEN" \
     -H "Content-Type: application/fhir+json" \
     http://localhost:5000/fhir/Patient
```

### Example FHIR Resource Creation

```bash
curl -X POST http://localhost:5000/fhir/Patient \
  -H "Content-Type: application/fhir+json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "resourceType": "Patient",
    "identifier": [{
      "system": "http://hospital.example.org/identifiers/patient",
      "value": "123456"
    }],
    "name": [{
      "family": "Smith",
      "given": ["John"]
    }],
    "gender": "male",
    "birthDate": "1990-01-01"
  }'
```

## 🧪 Testing

### Run Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/HealthTech.API.Tests/

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Structure

- **Unit Tests**: Test individual components and business logic
- **Integration Tests**: Test database operations and external dependencies
- **API Tests**: End-to-end testing of HTTP endpoints

## 🚀 Deployment

### Production Deployment

1. **Build the Docker image**

```bash
docker build -t fhir-ai-backend .
```

2. **Set environment variables**

```bash
export CONNECTION_STRING="your-production-connection-string"
export JWT_SECRET="your-super-secret-key"
```

3. **Run the container**

```bash
docker run -d \
   --name fhir-ai-api \
   -p 80:80 \
   -e ConnectionStrings__DefaultConnection="$CONNECTION_STRING" \
   -e JwtSettings__SecretKey="$JWT_SECRET" \
   fhir-ai-backend
```

### Environment Configuration

| Environment | Database              | Logging             | Authentication           |
| ----------- | --------------------- | ------------------- | ------------------------ |
| Development | Local PostgreSQL      | Console + File      | Development certificates |
| Staging     | Staging PostgreSQL    | Structured logging  | Staging certificates     |
| Production  | Production PostgreSQL | Centralized logging | Production certificates  |

## 🔒 Security

### Row Level Security (RLS)

The application implements RLS in PostgreSQL to ensure data isolation between tenants:

```sql
-- Example RLS policy
CREATE POLICY fhir_resources_tenant_policy ON fhir_resources
    FOR ALL
    USING (tenant_id = current_setting('app.current_tenant_id', true)::VARCHAR(255));
```

### Audit Logging

All operations are logged for compliance:

```csharp
// Example audit event
var auditEvent = new AuditEvent
{
    EventType = "API_ACCESS",
    Action = "C", // Create
    UserId = "user123",
    ResourceType = "Patient",
    ResourceId = "patient456"
};
```

## 📊 Performance

### Database Optimization

- **JSONB Indexes**: GIN indexes for efficient FHIR resource querying
- **Composite Indexes**: Optimized for common query patterns
- **Connection Pooling**: Efficient database connection management

### Caching Strategy

- **Redis Integration**: Ready for caching layer implementation
- **Response Caching**: HTTP response caching for static resources
- **Query Result Caching**: Caching frequently accessed FHIR resources

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines

- Follow Clean Architecture principles
- Write comprehensive tests for new features
- Use CQRS pattern for all business operations
- Implement proper error handling and validation
- Follow FHIR standards for resource handling
- Document all public APIs

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

- **Documentation**: [API Documentation](http://localhost:5000)
- **Issues**: [GitHub Issues](https://github.com/your-org/fhir-ai-backend/issues)
- **Discussions**: [GitHub Discussions](https://github.com/your-org/fhir-ai-backend/discussions)

## 🔮 Roadmap

- [ ] AI-powered FHIR resource analysis
- [ ] HL7 v2 to FHIR conversion
- [ ] Advanced search with Elasticsearch
- [ ] Real-time notifications
- [ ] Mobile API optimization
- [ ] GraphQL endpoint
- [ ] FHIR Bulk Data operations
- [ ] SMART on FHIR app launcher

---

**Built with ❤️ for the healthcare community**
