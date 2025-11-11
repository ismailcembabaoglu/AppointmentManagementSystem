@echo off
echo ========================================
echo  Blazor WebAssembly Publish Script
echo ========================================
echo.

cd AppointmentManagementSystem.BlazorUI

echo [1/4] Temizleme...
dotnet clean

echo.
echo [2/4] Restore...
dotnet restore

echo.
echo [3/4] Building Release...
dotnet build -c Release

echo.
echo [4/4] Publishing...
dotnet publish -c Release -o ./publish

echo.
echo ========================================
echo  TAMAMLANDI!
echo ========================================
echo.
echo Publish Klasoru: /app/AppointmentManagementSystem.BlazorUI/publish/wwwroot
echo.
echo PLESK'E YUKLENECEK DOSYALAR:
echo - publish/wwwroot/ klasorunun TUM ICERIGINI yukleyin
echo - web.config dosyasinin yuklendiginden emin olun!
echo.
echo Detayli bilgi: /app/PLESK_DEPLOYMENT_GUIDE.md
echo.
pause
