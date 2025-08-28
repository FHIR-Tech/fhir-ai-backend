#!/usr/bin/env python3
"""
YAML Validation Script for FHIR-AI Backend
Validates all YAML files in the project for syntax correctness
"""

import yaml
import os
import sys
from pathlib import Path

def validate_yaml_file(file_path):
    """Validate a single YAML file"""
    try:
        with open(file_path, 'r', encoding='utf-8') as file:
            yaml.safe_load(file)
        return True, None
    except yaml.YAMLError as e:
        return False, str(e)
    except Exception as e:
        return False, f"Error reading file: {str(e)}"

def find_yaml_files(directory):
    """Find all YAML files in the directory"""
    yaml_files = []
    for root, dirs, files in os.walk(directory):
        # Skip .git, node_modules and other hidden directories, but keep .github
        dirs[:] = [d for d in dirs if (d != '.git' and d != 'node_modules' and not (d.startswith('.') and d != '.github'))]
        
        for file in files:
            if file.endswith(('.yml', '.yaml')):
                yaml_files.append(os.path.join(root, file))
    return yaml_files

def main():
    """Main validation function"""
    print("🔍 FHIR-AI Backend YAML Validation")
    print("=" * 50)
    
    # Find all YAML files
    yaml_files = find_yaml_files('.')
    
    if not yaml_files:
        print("❌ No YAML files found")
        return 1
    
    print(f"Found {len(yaml_files)} YAML file(s):")
    print()
    
    valid_count = 0
    invalid_count = 0
    
    for file_path in sorted(yaml_files):
        is_valid, error = validate_yaml_file(file_path)
        
        if is_valid:
            print(f"✅ {file_path}")
            valid_count += 1
        else:
            print(f"❌ {file_path}")
            print(f"   Error: {error}")
            print()
            invalid_count += 1
    
    print("=" * 50)
    print(f"Summary: {valid_count} valid, {invalid_count} invalid")
    
    if invalid_count > 0:
        print("❌ Validation failed")
        return 1
    else:
        print("✅ All YAML files are valid!")
        return 0

if __name__ == "__main__":
    sys.exit(main())
