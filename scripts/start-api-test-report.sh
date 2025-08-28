#!/bin/bash

# FHIR-AI Backend - Start API, Run Tests & Generate Report
# This script starts the API in background, runs all tests, and generates a comprehensive report

set -e

echo "🚀 FHIR-AI Backend - Start API, Test & Report"
echo "=============================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Configuration
PROJECT_ROOT="$(pwd)"
API_DIR="src/HealthTech.API"
DB_NAME="fhir-ai"
DB_USER="postgres"
DB_HOST="localhost"
DB_PORT="5432"
HTTP_PORT="5000"
HTTPS_PORT="5001"
API_BASE_URL="https://localhost:5001"
REPORT_DIR="$PROJECT_ROOT/reports"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
REPORT_FILE="$REPORT_DIR/api_test_report_$TIMESTAMP.md"

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

print_header() {
    echo -e "${PURPLE}[HEADER]${NC} $1"
}

print_step() {
    echo -e "${CYAN}[STEP]${NC} $1"
}

# Function to check if PostgreSQL is running
check_postgres() {
    print_status "Checking PostgreSQL status..."
    
    if pg_isready -h $DB_HOST -p $DB_PORT -U $DB_USER > /dev/null 2>&1; then
        print_success "PostgreSQL is running on $DB_HOST:$DB_PORT"
        return 0
    else
        print_error "PostgreSQL is not running on $DB_HOST:$DB_PORT"
        print_status "Please start PostgreSQL before running this script"
        return 1
    fi
}

# Function to check if database exists
check_database() {
    print_status "Checking database '$DB_NAME'..."
    
    if psql -h $DB_HOST -p $DB_PORT -U $DB_USER -d $DB_NAME -c "SELECT 1;" > /dev/null 2>&1; then
        print_success "Database '$DB_NAME' exists and is accessible"
        return 0
    else
        print_error "Database '$DB_NAME' does not exist or is not accessible"
        print_status "Please create the database or check permissions"
        return 1
    fi
}

# Function to kill existing dotnet processes
kill_dotnet_processes() {
    print_status "Checking for existing dotnet processes..."
    
    if pgrep -f "dotnet run" > /dev/null; then
        print_warning "Found existing dotnet processes, killing them..."
        pkill -f "dotnet run" || true
        sleep 2
        print_success "Existing dotnet processes killed"
    else
        print_status "No existing dotnet processes found"
    fi
}

# Function to start the API in background
start_api_background() {
    print_step "Starting FHIR-AI Backend API in background..."
    
    if [ ! -d "$API_DIR" ]; then
        print_error "API directory '$API_DIR' not found"
        print_status "Please run this script from the project root directory"
        return 1
    fi
    
    cd "$API_DIR"
    
    print_status "Building the application..."
    dotnet build --no-restore
    
    print_status "Starting the API in background..."
    print_status "API will be available at:"
    print_status "  HTTP:  http://localhost:$HTTP_PORT"
    print_status "  HTTPS: https://localhost:$HTTPS_PORT"
    print_status "  Swagger: https://localhost:$HTTPS_PORT/swagger/index.html"
    print_status "  Health: https://localhost:$HTTPS_PORT/health"
    echo ""
    
    # Start API in background
    dotnet run > /dev/null 2>&1 &
    API_PID=$!
    
    print_success "API started with PID: $API_PID"
    
    # Wait for API to start
    print_status "Waiting for API to start..."
    sleep 10
    
    # Test API health
    if curl -f -s $API_BASE_URL/health > /dev/null 2>&1; then
        print_success "API is healthy and responding"
        return 0
    else
        print_warning "API health check failed, but continuing..."
        return 1
    fi
}

# Function to run all tests and capture output
run_all_tests() {
    print_step "Running comprehensive API tests..."
    
    # Create reports directory if it doesn't exist
    mkdir -p "$REPORT_DIR"
    
    # Initialize report file
    cat > "$REPORT_FILE" << EOF
# FHIR-AI Backend API Test Report

**Generated:** $(date)
**API Base URL:** $API_BASE_URL
**Test Duration:** $(date)

## Test Summary

EOF
    
    # Array of test scripts to run
    declare -a test_scripts=(
        "test-health-api.js"
        "check-swagger-endpoints.js"
        "test-authentication-endpoints.js"
        "test-fhir-endpoints.js"
        "comprehensive-api-test.js"
        "test-swagger-routing.js"
        "test-enhanced-export-bundle.js"
        "test-export-bundle.js"
        "test-import-export-bundle.js"
        "sample-data-api.js"
        "test-authentication-api.js"
        "test-login-with-data.js"
        "create-test-user.js"
        "seed-test-data.js"
        "export-bundle-api.js"
    )
    
    local total_tests=${#test_scripts[@]}
    local passed_tests=0
    local failed_tests=0
    
    echo "## Detailed Test Results" >> "$REPORT_FILE"
    echo "" >> "$REPORT_FILE"
    
    for script in "${test_scripts[@]}"; do
        print_status "Running $script..."
        
        # Create a temporary file for this test's output
        local temp_output=$(mktemp)
        local test_name=$(basename "$script" .js)
        
        echo "### $test_name" >> "$REPORT_FILE"
        echo "" >> "$REPORT_FILE"
        
        # Run the test and capture output
        if node "$PROJECT_ROOT/scripts/api/$script" > "$temp_output" 2>&1; then
            print_success "$script completed"
            echo "**Status:** ✅ PASSED" >> "$REPORT_FILE"
            ((passed_tests++))
        else
            print_warning "$script failed or had issues"
            echo "**Status:** ❌ FAILED" >> "$REPORT_FILE"
            ((failed_tests++))
        fi
        
        # Add test output to report (truncated for readability)
        echo "**Output:**" >> "$REPORT_FILE"
        echo '```' >> "$REPORT_FILE"
        head -50 "$temp_output" >> "$REPORT_FILE"
        if [ $(wc -l < "$temp_output") -gt 50 ]; then
            echo "..." >> "$REPORT_FILE"
            echo "(Output truncated - see full output in logs)" >> "$REPORT_FILE"
        fi
        echo '```' >> "$REPORT_FILE"
        echo "" >> "$REPORT_FILE"
        
        # Clean up temp file
        rm "$temp_output"
    done
    
    # Add summary to report
    cat >> "$REPORT_FILE" << EOF
## Test Summary

- **Total Tests:** $total_tests
- **Passed:** $passed_tests
- **Failed:** $failed_tests
- **Success Rate:** $((passed_tests * 100 / total_tests))%

## API Status

- **API Running:** ✅ Yes (PID: $API_PID)
- **Health Check:** ✅ Responding
- **Database:** ✅ Connected
- **Ports:** HTTP $HTTP_PORT, HTTPS $HTTPS_PORT

## Access Information

- **API Base:** $API_BASE_URL
- **Swagger UI:** $API_BASE_URL/swagger/index.html
- **Health Check:** $API_BASE_URL/health

## Next Steps

1. Review detailed test results above
2. Check API logs if needed: \`ps aux | grep dotnet\`
3. Stop API when done: \`./scripts/stop-api.sh\`
4. Access Swagger UI for manual testing

---
*Report generated by FHIR-AI Backend Test Suite*
EOF
    
    print_success "Test report generated: $REPORT_FILE"
    return 0
}

# Function to show final summary
show_final_summary() {
    echo ""
    echo "🎉 Complete Test Suite Finished!"
    echo "================================"
    echo ""
    echo "✅ API Status: Running in background (PID: $API_PID)"
    echo "✅ Database: PostgreSQL connected"
    echo "✅ Health Check: API responding"
    echo "✅ Tests: All completed"
    echo "✅ Report: Generated successfully"
    echo ""
    echo "📊 Test Report: $REPORT_FILE"
    echo ""
    echo "🔗 Access Information:"
    echo "   - API Base: $API_BASE_URL"
    echo "   - Swagger UI: $API_BASE_URL/swagger/index.html"
    echo "   - Health Check: $API_BASE_URL/health"
    echo ""
    echo "📋 Next Steps:"
    echo "   - Review report: cat $REPORT_FILE"
    echo "   - Open report: open $REPORT_FILE"
    echo "   - Stop API: ./scripts/stop-api.sh"
    echo "   - Check logs: ps aux | grep dotnet"
    echo ""
}

# Function to cleanup on exit
cleanup() {
    print_status "Cleaning up..."
    # Note: We don't stop the API here as user might want to keep it running
    print_status "API is still running. Use './scripts/stop-api.sh' to stop it."
}

# Set trap for cleanup
trap cleanup EXIT

# Main execution
main() {
    echo ""
    
    # Check PostgreSQL
    if ! check_postgres; then
        exit 1
    fi
    
    # Check database
    if ! check_database; then
        exit 1
    fi
    
    # Kill existing processes
    kill_dotnet_processes
    
    echo ""
    print_header "Starting comprehensive API test suite..."
    echo ""
    
    # Start API in background
    if start_api_background; then
        print_success "API started successfully"
    else
        print_error "Failed to start API"
        exit 1
    fi
    
    # Return to project root for running tests
    cd "$PROJECT_ROOT"
    
    # Run all tests and generate report
    if run_all_tests; then
        print_success "All tests completed and report generated"
    else
        print_warning "Some tests may have failed, but report was generated"
    fi
    
    # Show final summary
    show_final_summary
}

# Run main function
main "$@"
