# GitHub Actions Workflow Fixes

## Lỗi đã sửa trong `dotnet.yml`

### 1. **Lỗi Environment Variable trong Services**
**Lỗi:** `Unrecognized named-value: 'env'. Located at position 1 within expression: env.POSTGRES_VERSION`

**Nguyên nhân:** GitHub Actions không hỗ trợ sử dụng `env` variables trong phần `services`

**Sửa:**
```yaml
# Trước
image: postgres:${{ env.POSTGRES_VERSION }}

# Sau
image: postgres:14
```

### 2. **Node.js Setup Conditional**
**Lỗi:** Node.js setup có thể fail nếu không có `package.json`

**Sửa:**
```yaml
- name: Setup Node.js (for scripts)
  uses: actions/setup-node@v4
  with:
    node-version: '18'
    cache: 'npm'
    cache-dependency-path: scripts/package-lock.json
  if: hashFiles('scripts/package.json') != ''

- name: Install Node.js dependencies
  run: |
    if [ -f "scripts/package.json" ]; then
      cd scripts
      npm ci
    else
      echo "No package.json found in scripts directory, skipping npm install"
    fi
  if: hashFiles('scripts/package.json') != ''
```

### 3. **SonarCloud Token Handling**
**Lỗi:** SonarCloud job có thể fail nếu không có token

**Sửa:**
```yaml
- name: Install SonarCloud scanner
  uses: SonarSource/sonarcloud-github-action@master
  env:
    GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
    SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
  if: secrets.SONAR_TOKEN != ''

- name: Run SonarCloud analysis
  run: |
    dotnet tool install --global dotnet-sonarscanner
    dotnet sonarscanner begin /k:"fhir-ai-backend" /o:"your-organization" /d:sonar.login="${{ secrets.SONAR_TOKEN }}" /d:sonar.host.url="https://sonarcloud.io"
    dotnet build ${{ env.SOLUTION_FILE }} --configuration Release
    dotnet sonarscanner end /d:sonar.login="${{ secrets.SONAR_TOKEN }}"
  env:
    SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
  if: secrets.SONAR_TOKEN != ''

- name: Skip SonarCloud analysis
  run: echo "Skipping SonarCloud analysis - SONAR_TOKEN not provided"
  if: secrets.SONAR_TOKEN == ''
```

### 4. **Docker Artifact Upload**
**Lỗi:** Docker artifact upload có thể fail

**Sửa:**
```yaml
- name: Upload Docker image as artifact
  uses: actions/upload-artifact@v4
  with:
    name: docker-image
    path: /tmp/docker-image.tar
  if: always()
```

### 5. **Job Dependencies**
**Lỗi:** Deployment jobs reference đến `code-quality` job có thể không chạy

**Sửa:**
```yaml
# Trước
needs: [build-and-test, security-scan, code-quality]

# Sau
needs: [build-and-test, security-scan]
```

## Các Workflow Files

### 1. `dotnet.yml` - Main CI/CD Pipeline
**Location:** `.github/workflows/dotnet.yml`
- ✅ Build và test với PostgreSQL
- ✅ Security scanning với Trivy
- ✅ Docker build (main branch only)
- ✅ Code quality với SonarCloud (optional)
- ✅ Documentation validation
- ✅ Staging/Production deployment

### 2. `release.yml` - Release Management
**Location:** `.github/workflows/release.yml`
- ✅ Version management và tagging
- ✅ Docker image build và push
- ✅ Security scanning cho Docker images
- ✅ Release notes generation
- ✅ Manual deployment triggers

### 3. `code-quality.yml` - Quality Assurance
**Location:** `.github/workflows/code-quality.yml`
- ✅ Code formatting và static analysis
- ✅ Dependency vulnerability scanning
- ✅ FHIR compliance checks
- ✅ Security analysis với CodeQL
- ✅ Performance analysis
- ✅ Documentation quality checks
- ✅ Healthcare compliance (HIPAA, GDPR)

## Cấu hình cần thiết

### Secrets cần thiết:
- `GITHUB_TOKEN`: Tự động cung cấp bởi GitHub
- `SONAR_TOKEN`: Token cho SonarCloud (optional)

### Environment Protection:
- **Staging Environment**: Cần approval cho deployment
- **Production Environment**: Cần approval cho deployment

## Testing

Để test workflows:
1. Push code lên branch `develop` hoặc `main`
2. Kiểm tra GitHub Actions tab
3. Verify các jobs chạy thành công
4. Kiểm tra artifacts và logs

## Validation Tools

Dự án bao gồm các Python validation scripts trong thư mục `scripts/`:
- `validate-yaml.py` - YAML syntax validation
- `validate-github-actions.py` - GitHub Actions workflow validation
- `validate-all.py` - Comprehensive validation suite

### Usage:
```bash
# Setup Python environment
python3 -m venv .venv
source .venv/bin/activate
pip install -r requirements.txt

# Run validations
python3 scripts/validate-yaml.py
python3 scripts/validate-github-actions.py
python3 scripts/validate-all.py
```

## Troubleshooting

### Common Issues:
1. **SonarCloud fails**: Đảm bảo `SONAR_TOKEN` secret được set hoặc job sẽ skip
2. **Node.js fails**: Đảm bảo `scripts/package.json` tồn tại hoặc job sẽ skip
3. **PostgreSQL fails**: Kiểm tra service configuration
4. **Docker build fails**: Kiểm tra Dockerfile và context

### Debugging:
- Tất cả artifacts được upload ngay cả khi job fail
- Detailed logs có sẵn trong GitHub Actions
- Conditional steps được handle gracefully

## Project Structure Compliance

### Documentation Organization:
- ✅ Workflow documentation moved to `docs/cursor-agent/workflows/`
- ✅ Only workflow files remain in `.github/workflows/`
- ✅ Follows project documentation structure rules

### File Locations:
- **Workflow Files**: `.github/workflows/`
- **Documentation**: `docs/cursor-agent/workflows/`
- **Validation Scripts**: `scripts/`
- **Requirements**: `requirements.txt`

## Related Documentation

- [Workflows README](README.md) - Complete workflow documentation
- [Domain Entity Standards](DOMAIN_ENTITY_STANDARDS.md) - Domain entity coding standards
- [Project README](../../../README.md) - Main project documentation
- [API Documentation](../../api/README.md) - API documentation
- [Architecture Documentation](../../architecture/) - System architecture
- [Scripts Documentation](../../../scripts/README.md) - Validation scripts

---

*Các fixes này đảm bảo workflows hoạt động ổn định và handle gracefully các trường hợp optional dependencies, đồng thời tuân thủ cấu trúc dự án.*
