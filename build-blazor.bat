@echo off
echo ============================================
echo Building Blazor UI for Single Port Deployment
echo ============================================
echo.

cd %~dp0

echo 🔧 Restoring Blazor packages...
cd AppointmentManagementSystem.BlazorUI
dotnet restore
echo.

echo 🏗️ Publishing Blazor to API wwwroot...
dotnet publish -c Release -o ../AppointmentManagementSystem.API/wwwroot/blazor
echo.

echo ✅ Blazor built successfully!
echo 📁 Output: AppointmentManagementSystem.API/wwwroot/blazor
echo.
echo ============================================
echo Now start the API:
echo   cd AppointmentManagementSystem.API
echo   dotnet run
echo.
echo Then open: http://localhost:5089
echo ============================================
echo.
pause
