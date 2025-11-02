# Start Appointment Management System
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Starting Appointment Management System" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Get script directory
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Project directories
$ApiDir = Join-Path $ScriptDir "AppointmentManagementSystem.API"
$BlazorDir = Join-Path $ScriptDir "AppointmentManagementSystem.BlazorUI"

Write-Host "🔧 Restoring packages..." -ForegroundColor Yellow
dotnet restore
Write-Host ""

Write-Host "🏗️ Building solution..." -ForegroundColor Yellow
dotnet build --no-restore
Write-Host ""

Write-Host "🚀 Starting API..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$ApiDir'; Write-Host 'API Server Starting...' -ForegroundColor Cyan; dotnet run" -WindowStyle Normal

Write-Host "⏳ Waiting for API to start (3 seconds)..." -ForegroundColor Yellow
Start-Sleep -Seconds 3

Write-Host "🌐 Starting Blazor UI..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "`$env:ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=''; cd '$BlazorDir'; Write-Host 'Blazor UI Starting...' -ForegroundColor Cyan; dotnet run" -WindowStyle Normal

Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host "✅ Both services started!" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Green
Write-Host "📊 API: http://localhost:5089" -ForegroundColor White
Write-Host "📊 Swagger: http://localhost:5089/swagger" -ForegroundColor White
Write-Host "🌐 Blazor: http://localhost:5090" -ForegroundColor White
Write-Host "============================================" -ForegroundColor Green
Write-Host ""
Write-Host "Press any key to close this window..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
