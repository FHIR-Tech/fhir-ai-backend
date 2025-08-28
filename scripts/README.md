# FHIR-AI Backend Scripts

Scripts đơn giản để chạy và test FHIR-AI Backend API.

## 🚀 Quick Start

### Start API + Run Tests + Generate Report (Khuyến nghị)
```bash
./scripts/start-api-test-report.sh
```
Script này sẽ:
- Kiểm tra PostgreSQL và database
- Kill dotnet processes cũ
- Start API ở background
- Chạy tất cả test scripts
- Tạo report Markdown chi tiết
- Terminal trả về ngay

### Stop API
```bash
./scripts/stop-api.sh
```
Script này sẽ:
- Kiểm tra API có đang chạy không
- Dừng API gracefully
- Kiểm tra ports đã free chưa

## 📋 Prerequisites

### Required Software
- **PostgreSQL**: Chạy trên localhost:5432
- **.NET 8**: Để chạy API
- **Node.js**: Để chạy test scripts

### Database Setup
- Database: `fhir-ai`
- Username: `postgres`
- Password: `Post:!@#4`
- Host: `localhost:5432`

## 🔧 Configuration

### API Configuration
- **HTTP Port**: 5000 (redirects to HTTPS)
- **HTTPS Port**: 5001 (primary)
- **API Directory**: `src/HealthTech.API`
- **Test Directory**: `scripts/api`

## 📊 Test Scripts

### Available Test Scripts (15 scripts)
1. **test-health-api.js** - Health check và routing
2. **check-swagger-endpoints.js** - Swagger documentation
3. **test-authentication-endpoints.js** - Authentication endpoints
4. **test-fhir-endpoints.js** - FHIR endpoints
5. **comprehensive-api-test.js** - Comprehensive testing
6. **test-swagger-routing.js** - Swagger UI routing
7. **test-enhanced-export-bundle.js** - Enhanced export bundle
8. **test-export-bundle.js** - Export bundle
9. **test-import-export-bundle.js** - Import/export bundle
10. **sample-data-api.js** - Sample data generation
11. **test-authentication-api.js** - Authentication API
12. **test-login-with-data.js** - Login with data
13. **create-test-user.js** - Create test user
14. **seed-test-data.js** - Seed test data
15. **export-bundle-api.js** - Export bundle API

## 📝 Script Details

### start-api-test-report.sh
- Kiểm tra PostgreSQL và database
- Kill dotnet processes cũ
- Start API ở background
- Chạy tất cả 15 test scripts
- Tạo report Markdown chi tiết với timestamp
- Terminal trả về ngay

### stop-api.sh
- Kiểm tra API có đang chạy không
- Dừng API gracefully (SIGTERM trước, SIGKILL sau)
- Kiểm tra ports đã free chưa

## 🎉 Success Criteria

API được coi là chạy thành công khi:
- ✅ Health check trả về 200 OK
- ✅ Swagger UI accessible
- ✅ Database connected
- ✅ Tất cả tests pass

## 🔗 Access Information

Khi API chạy thành công:
- **API Base**: https://localhost:5001
- **Swagger UI**: https://localhost:5001/swagger/index.html
- **Health Check**: https://localhost:5001/health

## 📊 Report Files

Reports được tạo trong thư mục `reports/`:
```
reports/
└── api_test_report_YYYYMMDD_HHMMSS.md
```

Report bao gồm:
- Test summary với success rate
- Detailed test results
- API status
- Access information
- Next steps

## 🛠️ Troubleshooting

### PostgreSQL Not Running
```bash
# Start PostgreSQL (macOS with Homebrew)
brew services start postgresql@14
```

### Port Already in Use
```bash
# Check what's using the port
lsof -i :5000
lsof -i :5001

# Kill the process
kill -9 <PID>
```

### API Won't Start
```bash
# Check if dotnet is installed
dotnet --version

# Check if project builds
cd src/HealthTech.API
dotnet build
```

### Tests Failing
```bash
# Check if API is running
curl -f https://localhost:5001/health

# Check if Node.js is installed
node --version

# Install missing dependencies
cd scripts
npm install
```

## 📋 Workflow

### Development Workflow
1. **Start & Test**: `./scripts/start-api-test-report.sh`
2. **Review Report**: `cat reports/api_test_report_*.md`
3. **Continue Development**: API đang chạy background
4. **Stop When Done**: `./scripts/stop-api.sh`

### Manual Testing
1. **Start API**: `./scripts/start-api-test-report.sh`
2. **Access Swagger**: https://localhost:5001/swagger/index.html
3. **Test Manually**: Sử dụng Swagger UI
4. **Stop API**: `./scripts/stop-api.sh`

---

*Scripts được thiết kế để đơn giản và hiệu quả cho development workflow.*
