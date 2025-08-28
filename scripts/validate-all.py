#!/usr/bin/env python3
"""
Comprehensive Validation Script for FHIR-AI Backend
Runs all validation checks for the project
"""

import subprocess
import sys
import os

def run_command(command, description):
    """Run a command and return success status"""
    print(f"\n🔍 {description}")
    print("-" * 50)
    
    try:
        result = subprocess.run(command, shell=True, capture_output=True, text=True)
        print(result.stdout)
        if result.stderr:
            print("STDERR:", result.stderr)
        
        if result.returncode == 0:
            print("✅ Success")
            return True
        else:
            print("❌ Failed")
            return False
    except Exception as e:
        print(f"❌ Error: {e}")
        return False

def main():
    """Main validation function"""
    print("🚀 FHIR-AI Backend Comprehensive Validation")
    print("=" * 60)
    
    # Check if virtual environment is activated
    if not os.environ.get('VIRTUAL_ENV'):
        print("⚠️  Virtual environment not detected. Activating...")
        activate_script = os.path.join('.venv', 'bin', 'activate')
        if os.path.exists(activate_script):
            print("✅ Virtual environment found")
        else:
            print("❌ Virtual environment not found. Please run: python3 -m venv .venv && source .venv/bin/activate")
            return 1
    
    success_count = 0
    total_count = 0
    
    # 1. Validate YAML syntax
    total_count += 1
    if run_command("python3 scripts/validate-yaml.py", "YAML Syntax Validation"):
        success_count += 1
    
    # 2. Validate GitHub Actions workflows
    total_count += 1
    if run_command("python3 scripts/validate-github-actions.py", "GitHub Actions Workflow Validation"):
        success_count += 1
    
    # 3. Check .NET solution structure
    total_count += 1
    if run_command("dotnet --version", ".NET Version Check"):
        success_count += 1
    
    # 4. Validate solution file
    total_count += 1
    if run_command("dotnet sln list", ".NET Solution Validation"):
        success_count += 1
    
    # 5. Check Docker configuration
    total_count += 1
    if run_command("docker --version", "Docker Version Check"):
        success_count += 1
    
    # 6. Validate Docker Compose
    total_count += 1
    if run_command("docker-compose config", "Docker Compose Validation"):
        success_count += 1
    
    # 7. Check project structure
    total_count += 1
    if run_command("ls -la src/ tests/ docs/", "Project Structure Check"):
        success_count += 1
    
    print("\n" + "=" * 60)
    print(f"📊 Validation Summary: {success_count}/{total_count} passed")
    
    if success_count == total_count:
        print("🎉 All validations passed!")
        return 0
    else:
        print("⚠️  Some validations failed. Please review the issues above.")
        return 1

if __name__ == "__main__":
    sys.exit(main())
