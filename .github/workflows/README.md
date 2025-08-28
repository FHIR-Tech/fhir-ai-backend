# FHIR-AI Backend GitHub Actions Workflows

This directory contains GitHub Actions workflows for the FHIR-AI Backend project, implementing a comprehensive CI/CD pipeline with security, quality, and compliance checks.

## Workflow Overview

### 1. `dotnet.yml` - Main CI/CD Pipeline
**Triggers:** Push to `main`/`develop`, Pull Requests to `main`/`develop`

**Jobs:**
- **Build and Test**: Builds the solution, runs unit and integration tests with PostgreSQL
- **Security Scan**: Runs Trivy vulnerability scanner
- **Docker Build**: Builds Docker image (main branch only)
- **Code Quality**: Runs SonarCloud analysis
- **Documentation Check**: Validates documentation structure
- **Deploy Staging**: Deploys to staging environment (develop branch)
- **Deploy Production**: Deploys to production environment (main branch)

**Features:**
- Multi-platform Docker builds (linux/amd64, linux/arm64)
- PostgreSQL service for integration tests
- Code coverage collection
- Security vulnerability scanning
- Automated deployment to staging/production

### 2. `release.yml` - Release Management
**Triggers:** Release published, Manual workflow dispatch

**Jobs:**
- **Version and Tag**: Updates version numbers and creates Git tags
- **Build Release**: Builds and pushes Docker images to GitHub Container Registry
- **Security Scan Release**: Scans the built Docker image for vulnerabilities
- **Deploy Staging**: Deploys release to staging environment
- **Deploy Production**: Deploys release to production environment
- **Create Release Notes**: Generates and updates release notes
- **Notify Team**: Sends notifications on success/failure

**Features:**
- Automated version management
- Multi-platform Docker image builds
- Release notes generation
- Team notifications
- Manual deployment triggers

### 3. `code-quality.yml` - Quality Assurance
**Triggers:** Push to `main`/`develop`, Pull Requests, Weekly schedule

**Jobs:**
- **Code Quality Analysis**: Code formatting, static analysis, outdated package detection
- **Dependency Vulnerability Scan**: Scans for vulnerable NuGet packages
- **FHIR Compliance Check**: Validates FHIR resource schemas and compliance
- **Security Analysis**: CodeQL analysis and SAST scanning
- **Performance Analysis**: Performance testing and memory analysis
- **Documentation Quality Check**: Validates documentation completeness
- **Compliance Check**: HIPAA, GDPR, and FHIR compliance validation
- **Quality Report**: Generates comprehensive quality report

**Features:**
- Weekly dependency scanning
- FHIR-specific compliance checks
- Healthcare compliance validation (HIPAA, GDPR)
- Automated quality reporting
- PR comments with quality status

## Environment Configuration

### Required Secrets
- `GITHUB_TOKEN`: Automatically provided by GitHub
- `SONAR_TOKEN`: SonarCloud authentication token
- Environment-specific secrets for staging/production deployment

### Environment Protection
- **Staging Environment**: Requires approval for deployment
- **Production Environment**: Requires approval for deployment

## Usage

### Automatic Triggers
1. **Push to main/develop**: Triggers full CI/CD pipeline
2. **Pull Request**: Triggers build, test, and quality checks
3. **Release published**: Triggers release pipeline
4. **Weekly schedule**: Triggers dependency scanning

### Manual Triggers
1. **Release workflow**: Manual deployment with version specification
2. **Quality checks**: Can be triggered manually for specific branches

## Configuration

### Environment Variables
```yaml
DOTNET_VERSION: '8.0.x'
POSTGRES_VERSION: '14'
SOLUTION_FILE: 'HealthTech.FHIR-AI.sln'
REGISTRY: 'ghcr.io'
IMAGE_NAME: '${{ github.repository }}'
```

### Docker Configuration
- Multi-stage builds for optimized images
- Health checks included
- Non-root user for security
- Multi-platform support (amd64, arm64)

### Database Configuration
- PostgreSQL 14 for testing
- Health checks for service availability
- Test database with isolated credentials

## Security Features

### Vulnerability Scanning
- **Trivy**: Container and filesystem vulnerability scanning
- **CodeQL**: Static application security testing
- **Dependency scanning**: NuGet package vulnerability detection

### Compliance Checks
- **HIPAA**: Healthcare data protection compliance
- **GDPR**: Data protection and privacy compliance
- **FHIR**: Healthcare interoperability standards compliance

### Security Best Practices
- Non-root Docker containers
- Secrets management
- Environment protection rules
- Automated security scanning

## Quality Gates

### Code Quality
- Code formatting validation
- Static analysis
- Outdated package detection
- Documentation completeness

### Testing
- Unit tests with coverage
- Integration tests with database
- Performance tests
- FHIR compliance tests

### Documentation
- Required file structure validation
- API documentation checks
- Code documentation validation

## Deployment Strategy

### Staging Deployment
- Automatic deployment on develop branch
- Environment protection with approval
- Health check validation
- Rollback capability

### Production Deployment
- Manual deployment trigger
- Environment protection with approval
- Comprehensive testing requirements
- Release notes generation

## Monitoring and Notifications

### Health Checks
- Application health endpoints
- Database connectivity checks
- Docker container health monitoring

### Notifications
- Success/failure notifications
- Quality report generation
- PR comments with status
- Release notifications

## Troubleshooting

### Common Issues
1. **Build failures**: Check .NET version compatibility
2. **Test failures**: Verify PostgreSQL service availability
3. **Security scan failures**: Review vulnerability reports
4. **Deployment failures**: Check environment secrets and permissions

### Debugging
- Artifacts are uploaded for failed jobs
- Detailed logs available in GitHub Actions
- Quality reports provide comprehensive status

## Best Practices

### Development
- Always run tests locally before pushing
- Follow FHIR compliance guidelines
- Maintain documentation quality
- Use proper Git commit messages

### Deployment
- Test in staging before production
- Review security scan results
- Validate compliance requirements
- Monitor deployment health

### Security
- Regularly update dependencies
- Review vulnerability reports
- Follow security best practices
- Maintain compliance standards

## Support

For issues with workflows:
1. Check GitHub Actions logs
2. Review quality reports
3. Validate environment configuration
4. Contact the development team

---

*This workflow configuration ensures secure, compliant, and high-quality delivery of the FHIR-AI Backend application.*
