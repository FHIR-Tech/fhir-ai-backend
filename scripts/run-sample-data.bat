@echo off
echo ========================================
echo FHIR-AI Backend Sample Data Generator
echo ========================================
echo.

echo Starting sample data generation...
echo.

REM Check if Node.js is available
node --version >nul 2>&1
if %errorlevel% equ 0 (
    echo Using Node.js to generate sample data...
    node scripts/sample-data-api.js
) else (
    echo Node.js not found, trying PowerShell...
    powershell -ExecutionPolicy Bypass -File scripts/sample-data-api.ps1
)

echo.
echo Sample data generation completed!
echo.
pause
