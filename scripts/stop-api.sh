#!/bin/bash

# FHIR-AI Backend API Stop Script
# This script stops the API gracefully

set -e

echo "🛑 FHIR-AI Backend API Stop Script"
echo "=================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

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

# Function to check if API is running
check_api_running() {
    print_status "Checking if API is running..."
    
    if pgrep -f "dotnet run" > /dev/null; then
        print_success "API is running"
        return 0
    else
        print_warning "No API processes found"
        return 1
    fi
}

# Function to stop API gracefully
stop_api() {
    print_status "Stopping FHIR-AI Backend API..."
    
    # Get all dotnet run processes
    local pids=$(pgrep -f "dotnet run")
    
    if [ -z "$pids" ]; then
        print_warning "No API processes found to stop"
        return 0
    fi
    
    print_status "Found API processes: $pids"
    
    # Try graceful shutdown first
    print_status "Attempting graceful shutdown..."
    for pid in $pids; do
        kill $pid 2>/dev/null || true
    done
    
    # Wait a bit for graceful shutdown
    sleep 3
    
    # Check if processes are still running
    local remaining_pids=$(pgrep -f "dotnet run")
    if [ ! -z "$remaining_pids" ]; then
        print_warning "Some processes still running, forcing shutdown..."
        for pid in $remaining_pids; do
            kill -9 $pid 2>/dev/null || true
        done
        sleep 1
    fi
    
    # Final check
    if pgrep -f "dotnet run" > /dev/null; then
        print_error "Failed to stop all API processes"
        return 1
    else
        print_success "All API processes stopped successfully"
        return 0
    fi
}

# Function to check ports
check_ports() {
    print_status "Checking if ports are free..."
    
    local http_port=5000
    local https_port=5001
    
    if lsof -i :$http_port > /dev/null 2>&1; then
        print_warning "Port $http_port is still in use"
        return 1
    fi
    
    if lsof -i :$https_port > /dev/null 2>&1; then
        print_warning "Port $https_port is still in use"
        return 1
    fi
    
    print_success "Ports $http_port and $https_port are free"
    return 0
}

# Function to show final status
show_final_status() {
    echo ""
    echo "🛑 API Stop Complete!"
    echo "===================="
    echo ""
    
    if ! pgrep -f "dotnet run" > /dev/null; then
        echo "✅ API Status: Stopped"
    else
        echo "⚠️  API Status: Some processes may still be running"
    fi
    
    if check_ports; then
        echo "✅ Ports: Free"
    else
        echo "⚠️  Ports: Some ports may still be in use"
    fi
    
    echo ""
    echo "📊 Next Steps:"
    echo "   - Start API: ./scripts/start-api-background.sh"
    echo "   - Start API (interactive): ./scripts/start-api.sh"
    echo "   - Check processes: ps aux | grep dotnet"
    echo ""
}

# Main execution
main() {
    echo ""
    
    # Check if API is running
    if ! check_api_running; then
        print_warning "API is not running, nothing to stop"
        show_final_status
        exit 0
    fi
    
    # Stop API
    if stop_api; then
        print_success "API stopped successfully"
    else
        print_error "Failed to stop API completely"
    fi
    
    # Check ports
    check_ports
    
    # Show final status
    show_final_status
}

# Run main function
main "$@"
