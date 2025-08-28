#!/usr/bin/env python3
"""
GitHub Actions Validation Script for FHIR-AI Backend
Validates GitHub Actions workflows for common issues and best practices
"""

import yaml
import os
import sys
from pathlib import Path

def validate_github_actions_workflow(file_path):
    """Validate a GitHub Actions workflow file"""
    try:
        with open(file_path, 'r', encoding='utf-8') as file:
            workflow = yaml.safe_load(file)
        
        issues = []
        
        # Check required fields
        if 'name' not in workflow:
            issues.append("Missing 'name' field")
        
        # Check for 'on' field (triggers) - handle both string and boolean cases
        has_on_field = False
        for key in workflow.keys():
            if str(key).lower() == 'on' or key is True:  # 'on' might be parsed as True
                has_on_field = True
                break
        
        if not has_on_field:
            issues.append("Missing 'on' field (triggers)")
        
        if 'jobs' not in workflow:
            issues.append("Missing 'jobs' field")
        
        # Check jobs structure
        if 'jobs' in workflow:
            for job_name, job_config in workflow['jobs'].items():
                if 'runs-on' not in job_config:
                    issues.append(f"Job '{job_name}' missing 'runs-on' field")
                
                if 'steps' not in job_config:
                    issues.append(f"Job '{job_name}' missing 'steps' field")
                else:
                    # Check steps
                    for i, step in enumerate(job_config['steps']):
                        if 'name' not in step:
                            issues.append(f"Job '{job_name}' step {i+1} missing 'name' field")
                        
                        if 'uses' not in step and 'run' not in step:
                            issues.append(f"Job '{job_name}' step '{step.get('name', f'{i+1}')}' missing 'uses' or 'run' field")
        
        return True, issues
        
    except yaml.YAMLError as e:
        return False, [f"YAML syntax error: {str(e)}"]
    except Exception as e:
        return False, [f"Error reading file: {str(e)}"]

def find_workflow_files(directory):
    """Find all GitHub Actions workflow files"""
    workflow_files = []
    workflows_dir = os.path.join(directory, '.github', 'workflows')
    
    if os.path.exists(workflows_dir):
        for file in os.listdir(workflows_dir):
            if file.endswith(('.yml', '.yaml')):
                workflow_files.append(os.path.join(workflows_dir, file))
    
    return workflow_files

def main():
    """Main validation function"""
    print("🔍 GitHub Actions Workflow Validation")
    print("=" * 60)
    
    # Find all workflow files
    workflow_files = find_workflow_files('.')
    
    if not workflow_files:
        print("❌ No GitHub Actions workflow files found")
        return 1
    
    print(f"Found {len(workflow_files)} workflow file(s):")
    print()
    
    valid_count = 0
    invalid_count = 0
    
    for file_path in sorted(workflow_files):
        is_valid, issues = validate_github_actions_workflow(file_path)
        
        if is_valid and not issues:
            print(f"✅ {os.path.basename(file_path)}")
            valid_count += 1
        else:
            print(f"❌ {os.path.basename(file_path)}")
            if issues:
                for issue in issues:
                    print(f"   ⚠️  {issue}")
            print()
            invalid_count += 1
    
    print("=" * 60)
    print(f"Summary: {valid_count} valid, {invalid_count} with issues")
    
    if invalid_count > 0:
        print("❌ Validation failed")
        return 1
    else:
        print("✅ All GitHub Actions workflows are valid!")
        return 0

if __name__ == "__main__":
    sys.exit(main())
