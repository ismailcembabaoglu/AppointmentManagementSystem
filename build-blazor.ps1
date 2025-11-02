Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Building Blazor UI for Single Port Deployment" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $ScriptDir

Write-Host "🔧 Restoring Blazor packages..." -ForegroundColor Yellow
Set-Location AppointmentManagementSystem.BlazorUI
dotnet restore
Write-Host ""

Write-Host "🏗️ Publishing Blazor to API wwwroot..." -ForegroundColor Yellow
dotnet publish -c Release -o ../AppointmentManagementSystem.API/wwwroot/blazor
Write-Host ""

Write-Host "✅ Blazor built successfully!" -ForegroundColor Green
Write-Host "📁 Output: AppointmentManagementSystem.API/wwwroot/blazor" -ForegroundColor White
Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Now start the API:" -ForegroundColor White
Write-Host "  cd AppointmentManagementSystem.API" -ForegroundColor Gray
Write-Host "  dotnet run" -ForegroundColor Gray
Write-Host ""
Write-Host "Then open: http://localhost:5089" -ForegroundColor Yellow
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press any key to exit..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
