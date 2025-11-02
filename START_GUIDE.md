# 🚀 Hızlı Başlatma Kılavuzu

## Visual Studio Tooling Hatası Çözümü

Blazor başlatırken `Microsoft.WebTools.ApiEndpointDiscovery` hatası alıyorsanız, bu **çözüldü**! ✅

### Yapılan Düzeltmeler

1. ✅ `launchSettings.json` - Visual Studio tooling devre dışı
2. ✅ `BlazorAutoStartService` - Environment variable eklendi
3. ✅ Başlatma scriptleri oluşturuldu

---

## 3 Başlatma Yöntemi

### Yöntem 1: Scriptler (ÖNERİLEN) ⭐

En kolay ve güvenilir yöntem!

**Windows (Batch):**
```cmd
start.bat
```

**Windows (PowerShell):**
```powershell
.\start.ps1
```

**Linux/Mac:**
```bash
./start.sh
```

**Özellikler:**
- ✅ İki terminal/pencere açar
- ✅ API ve Blazor ayrı çalışır
- ✅ Visual Studio tooling hatası yok
- ✅ Tek tıkla çalıştır

---

### Yöntem 2: IHostedService (Otomatik)

API içinden otomatik Blazor başlatma.

**Kullanım:**
```bash
cd AppointmentManagementSystem.API
dotnet run
```

**Özellikler:**
- ✅ Tek komut
- ✅ Otomatik cleanup
- ⚠️ Debug zor olabilir

**Ayarlar:**
```json
// appsettings.json
{
  "BlazorUI": {
    "AutoStart": true  // false yaparak kapat
  }
}
```

---

### Yöntem 3: Manuel (Ayrı Terminaller)

**Terminal 1 - API:**
```bash
cd AppointmentManagementSystem.API
dotnet run
```

**Terminal 2 - Blazor:**
```bash
cd AppointmentManagementSystem.BlazorUI
set ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=
dotnet run
```

Linux/Mac için:
```bash
export ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=""
dotnet run
```

---

## 🐛 Visual Studio Tooling Hatası

### Hata Mesajı
```
System.IO.FileNotFoundException: Could not load file or assembly 
'Microsoft.WebTools.ApiEndpointDiscovery, Culture=neutral, PublicKeyToken=null'
```

### Çözüm ✅

**1. Environment Variable Ekle:**
```bash
# Windows CMD
set ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=

# Windows PowerShell
$env:ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=""

# Linux/Mac
export ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=""
```

**2. launchSettings.json (Otomatik Düzeltildi):**
```json
{
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "ASPNETCORE_HOSTINGSTARTUPASSEMBLIES": ""
  }
}
```

---

## 📊 URL'ler

Uygulamalar başladığında:

| Servis | URL |
|--------|-----|
| API | http://localhost:5089 |
| Swagger | http://localhost:5089/swagger |
| Blazor | http://localhost:5090 |

---

## ✅ Test Etme

### Test 1: Script ile Başlatma
```bash
# Windows
start.bat

# Mac/Linux  
./start.sh

# Beklenen: İki pencere açılmalı
# - API Server (Port 5089)
# - Blazor UI (Port 5090)
```

### Test 2: Blazor Erişim
```bash
# Tarayıcıda
http://localhost:5090

# Beklenen: Blazor app açılmalı
```

### Test 3: API Test
```bash
# Swagger UI
http://localhost:5089/swagger

# veya curl
curl http://localhost:5089/api/categories
```

---

## 🔧 Sorun Giderme

### Blazor hala hata veriyor

**Çözüm 1: Environment Variable**
```bash
# Her Blazor çalıştırmasında ekle
set ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=
dotnet run
```

**Çözüm 2: csproj Temizle**
```bash
cd AppointmentManagementSystem.BlazorUI
dotnet clean
dotnet build
dotnet run
```

**Çözüm 3: NuGet Cache Temizle**
```bash
dotnet nuget locals all --clear
dotnet restore
```

### Port conflict

**API (5089) kullanımda:**
```bash
# launchSettings.json'da port değiştir
"applicationUrl": "http://localhost:5088"
```

**Blazor (5090) kullanımda:**
```bash
# launchSettings.json'da port değiştir
"applicationUrl": "http://localhost:5091"
```

### Process kalmış

**Windows:**
```bash
taskkill /F /IM dotnet.exe
```

**Linux/Mac:**
```bash
pkill -f dotnet
```

---

## 💡 Öneriler

**Geliştirme için:**
- ✅ Scriptleri kullanın (`start.bat` veya `start.sh`)
- ✅ Ayrı terminallerde görebilirsiniz
- ✅ Debug kolay

**Visual Studio kullanıyorsanız:**
- ✅ Multiple Startup Projects
- ✅ F5 ile her ikisi de başlar
- ✅ Debug support tam

**Production için:**
- ✅ Docker Compose
- ✅ Her servis ayrı container
- ✅ Kubernetes

---

## 📦 Dosyalar

| Dosya | Açıklama |
|-------|----------|
| `/app/start.bat` | Windows batch script |
| `/app/start.ps1` | PowerShell script (modern) |
| `/app/start.sh` | Linux/Mac bash script |
| `/app/BlazorAutoStartService.cs` | IHostedService (otomatik) |
| `/app/BLAZOR_AUTOSTART.md` | Detaylı döküman |

---

## 🎯 Hızlı Komutlar

```bash
# Tek komut (script)
start.bat                    # Windows
./start.sh                   # Linux/Mac

# Manuel
cd AppointmentManagementSystem.API && dotnet run
cd AppointmentManagementSystem.BlazorUI && dotnet run

# Temizlik
dotnet clean && dotnet restore && dotnet build

# Port kontrol
netstat -ano | findstr :5089    # Windows
lsof -i :5089                   # Mac/Linux
```

---

**Oluşturma:** 2025-01-08  
**Durum:** ✅ Çözüldü ve Test Edildi
