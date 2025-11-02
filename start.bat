@echo off
echo ============================================
echo Starting Appointment Management System
echo ============================================
echo.

REM Get the script directory
set SCRIPT_DIR=%~dp0

REM API directory
set API_DIR=%SCRIPT_DIR%AppointmentManagementSystem.API

REM Blazor directory
set BLAZOR_DIR=%SCRIPT_DIR%AppointmentManagementSystem.BlazorUI

echo 🔧 Restoring packages...
dotnet restore
echo.

echo 🏗️ Building solution...
dotnet build --no-restore
echo.

echo 🚀 Starting API...
start "API Server" cmd /k "cd /d %API_DIR% && dotnet run"

echo ⏳ Waiting for API to start (3 seconds)...
timeout /t 3 /nobreak >nul

echo 🌐 Starting Blazor UI...
start "Blazor UI" cmd /k "cd /d %BLAZOR_DIR% && set ASPNETCORE_HOSTINGSTARTUPASSEMBLIES= && dotnet run"

echo.
echo ============================================
echo ✅ Both services started!
echo ============================================
echo 📊 API: http://localhost:5089
echo 📊 Swagger: http://localhost:5089/swagger
echo 🌐 Blazor: http://localhost:5090
echo ============================================
echo.
echo Press any key to close this window...
pause >nul
