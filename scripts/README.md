# FHIR-AI Backend Scripts

This directory contains various utility scripts for the FHIR-AI Backend project, including validation tools, testing scripts, and automation helpers.

## Validation Scripts

### 1. `validate-yaml.py` - YAML Syntax Validation
Validates all YAML files in the project for syntax correctness.

**Usage:**
```bash
source .venv/bin/activate
python3 scripts/validate-yaml.py
```

**Features:**
- Finds all `.yml` and `.yaml` files
- Excludes `node_modules` and hidden directories
- Validates YAML syntax
- Provides detailed error reporting

### 2. `validate-github-actions.py` - GitHub Actions Validation
Validates GitHub Actions workflow files for common issues and best practices.

**Usage:**
```bash
source .venv/bin/activate
python3 scripts/validate-github-actions.py
```

**Features:**
- Validates workflow structure
- Checks required fields (`name`, `on`, `jobs`)
- Validates job and step configurations
- Handles YAML parsing quirks

### 3. `validate-all.py` - Comprehensive Validation
Runs all validation checks for the project.

**Usage:**
```bash
source .venv/bin/activate
python3 scripts/validate-all.py
```

**Features:**
- YAML syntax validation
- GitHub Actions workflow validation
- .NET solution validation
- Docker configuration validation
- Project structure validation

## Setup Requirements

### Python Environment
```bash
# Create virtual environment
python3 -m venv .venv

# Activate virtual environment
source .venv/bin/activate

# Install dependencies
pip install PyYAML
```

### Required Tools
- Python 3.8+
- .NET 8.0+
- Docker (optional, for Docker validation)
- Docker Compose (optional, for Docker validation)

## Validation Results

### YAML Files
- ✅ `.github/workflows/code-quality.yml`
- ✅ `.github/workflows/dotnet.yml`
- ✅ `.github/workflows/release.yml`
- ✅ `.github/workflows/test-simple.yml`
- ✅ `docker-compose.yml`

### GitHub Actions Workflows
- ✅ `code-quality.yml` - Quality assurance pipeline
- ✅ `dotnet.yml` - Main CI/CD pipeline
- ✅ `release.yml` - Release management
- ✅ `test-simple.yml` - Simple test workflow

## Common Issues and Solutions

### 1. YAML Parsing Issues
**Problem:** YAML parser reads `on` as boolean `True`
**Solution:** Script handles this automatically by checking for both string and boolean values

### 2. Missing Dependencies
**Problem:** PyYAML not installed
**Solution:** Install with `pip install PyYAML`

### 3. Virtual Environment
**Problem:** Python packages not found
**Solution:** Activate virtual environment with `source .venv/bin/activate`

### 4. Docker Not Available
**Problem:** Docker commands fail
**Solution:** Docker is optional for validation. Script will skip Docker checks if not available.

## Integration with CI/CD

These validation scripts can be integrated into the CI/CD pipeline:

```yaml
- name: Validate YAML files
  run: |
    source .venv/bin/activate
    python3 scripts/validate-yaml.py

- name: Validate GitHub Actions
  run: |
    source .venv/bin/activate
    python3 scripts/validate-github-actions.py
```

## Extending Validation

To add new validation checks:

1. Create a new Python script in the `scripts/` directory
2. Follow the naming convention: `validate-*.py`
3. Return appropriate exit codes (0 for success, 1 for failure)
4. Add to `validate-all.py` if needed

## Troubleshooting

### Script Not Found
```bash
# Make sure you're in the project root
cd /path/to/fhir-ai-backend

# Check if scripts exist
ls -la scripts/
```

### Permission Denied
```bash
# Make scripts executable
chmod +x scripts/*.py
```

### Python Path Issues
```bash
# Use absolute path
python3 /path/to/fhir-ai-backend/scripts/validate-yaml.py
```

### Virtual Environment Issues
```bash
# Recreate virtual environment
rm -rf .venv
python3 -m venv .venv
source .venv/bin/activate
pip install PyYAML
```

## Contributing

When adding new validation scripts:

1. Follow the existing naming conventions
2. Include proper error handling
3. Add documentation
4. Update this README
5. Test with various scenarios

---

*These scripts ensure code quality and consistency across the FHIR-AI Backend project.*
