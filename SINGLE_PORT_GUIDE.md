# 🎯 Tek Port Üzerinden Çalıştırma (Single Port Deployment)

API ve Blazor artık **aynı port** üzerinden çalışıyor!

## 🌟 Özellikler

✅ **Tek Port:** http://localhost:5089  
✅ **CORS Problemi Yok:** Aynı origin  
✅ **Production Ready:** Static file serving  
✅ **Kolay Deploy:** Tek dizin, tek process  

---

## 🚀 Hızlı Başlangıç

### Yöntem 1: Tek Komut (ÖNERİLEN)

**Windows:**
```cmd
start-single-port.bat
```

**Linux/Mac:**
```bash
./start-single-port.sh
```

Bu script:
1. Paketleri restore eder
2. Solution'ı build eder
3. Blazor'u publish eder
4. API'yi başlatır

---

### Yöntem 2: Manuel Adımlar

**1. Blazor'u Build Et:**
```bash
# Windows
build-blazor.bat

# Linux/Mac
./build-blazor.sh
```

**2. API'yi Başlat:**
```bash
cd AppointmentManagementSystem.API
dotnet run
```

---

## 🔗 URL'ler

Tüm servislere **tek porttan** erişin:

| Servis | URL |
|--------|-----|
| **Blazor UI** | http://localhost:5089 |
| **API** | http://localhost:5089/api/* |
| **Swagger** | http://localhost:5089/swagger |

---

## 📂 Nasıl Çalışır?

### 1. Blazor Build
```bash
dotnet publish AppointmentManagementSystem.BlazorUI \
  -c Release \
  -o AppointmentManagementSystem.API/wwwroot/blazor
```

**Çıktı:**
```
AppointmentManagementSystem.API/
└── wwwroot/
    └── blazor/
        ├── index.html
        ├── _framework/
        ├── css/
        └── js/
```

### 2. API Serving

Program.cs'de:
```csharp
// Static files middleware
app.UseStaticFiles();
app.UseBlazorFrameworkFiles();

// API routes
app.MapControllers();

// SPA fallback - Blazor
app.MapFallbackToFile("blazor/index.html");
```

### 3. Routing

| İstek | Handler |
|-------|---------|
| `/` | Blazor index.html |
| `/login` | Blazor (SPA routing) |
| `/api/auth/login` | API Controller |
| `/swagger` | Swagger UI |
| `/css/app.css` | Static file |

---

## 🛠️ Build Scriptleri

### build-blazor.bat / .sh

Sadece Blazor'u build eder:
```bash
# Kullanım
build-blazor.bat

# Ne yapar?
1. Blazor projesini restore eder
2. Release modda publish eder
3. API/wwwroot/blazor'a kopyalar
```

### start-single-port.bat / .sh

Her şeyi yapar:
```bash
# Kullanım
start-single-port.bat

# Ne yapar?
1. Solution restore
2. Solution build
3. Blazor publish
4. API çalıştır
```

---

## 🔄 Geliştirme Workflow

### İlk Kurulum
```bash
# 1. Blazor'u build et
build-blazor.bat

# 2. API'yi çalıştır
cd AppointmentManagementSystem.API
dotnet run

# 3. Tarayıcıda aç
http://localhost:5089
```

### Blazor Değişikliklerinde
```bash
# 1. Blazor'u yeniden build et
build-blazor.bat

# 2. API'yi yeniden başlat (Ctrl+C sonra dotnet run)
# veya hot reload için F5
```

### API Değişikliklerinde
```bash
# API otomatik hot reload yapacak
# Değişiklik yaptıktan sonra sadece kaydet
```

---

## 🎯 Avantajlar

**CORS Problemi Yok:**
```javascript
// Önce (Farklı portlar)
fetch('http://localhost:5089/api/users') // CORS hatası!

// Şimdi (Aynı port)
fetch('/api/users') // ✅ Çalışır!
```

**Tek Deployment:**
```bash
# Önce
docker run api:latest -p 5089:80
docker run blazor:latest -p 5090:80

# Şimdi
docker run app:latest -p 80:80
```

**Basit nginx config:**
```nginx
# Önce
location /api { proxy_pass api:5089; }
location / { proxy_pass blazor:5090; }

# Şimdi
location / { proxy_pass app:80; }
```

---

## 📦 Production Deployment

### Option 1: Docker

**Dockerfile:**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy API with Blazor
COPY AppointmentManagementSystem.API/bin/Release/net9.0/publish/ .

EXPOSE 80
ENTRYPOINT ["dotnet", "AppointmentManagementSystem.API.dll"]
```

**Build:**
```bash
# 1. Blazor build
build-blazor.bat

# 2. API publish
dotnet publish AppointmentManagementSystem.API -c Release

# 3. Docker build
docker build -t appointment-system .

# 4. Run
docker run -p 80:80 appointment-system
```

### Option 2: IIS

1. Blazor'u build et: `build-blazor.bat`
2. API'yi publish et
3. IIS'e deploy et
4. Application Pool → .NET Core
5. Site binding → Port 80

### Option 3: Linux Server

```bash
# 1. Build
build-blazor.sh
dotnet publish AppointmentManagementSystem.API -c Release

# 2. Copy to server
scp -r bin/Release/net9.0/publish/ user@server:/var/www/app

# 3. Systemd service
sudo systemctl start appointment-system
```

---

## 🐛 Sorun Giderme

### Blazor yüklenmiyor

**Kontrol:**
```bash
# wwwroot/blazor klasörü var mı?
ls AppointmentManagementSystem.API/wwwroot/blazor/

# index.html var mı?
ls AppointmentManagementSystem.API/wwwroot/blazor/index.html
```

**Çözüm:**
```bash
build-blazor.bat
```

### 404 Not Found hatası

**Blazor routes için:**
- SPA fallback aktif mi kontrol et
- MapFallbackToFile çalışıyor mu?

**API routes için:**
- `/api/` prefix var mı?
- Controller route doğru mu?

### Static files yüklenmiyor

**Kontrol Program.cs:**
```csharp
app.UseStaticFiles();
app.UseBlazorFrameworkFiles();
```

---

## 🔄 Eski Yöntemden Geçiş

### Önce (İki Port)
```bash
# Terminal 1
cd API && dotnet run  # Port 5089

# Terminal 2  
cd BlazorUI && dotnet run  # Port 5090
```

### Şimdi (Tek Port)
```bash
# Tek terminal
start-single-port.bat  # Port 5089
```

---

## ⚙️ Konfigürasyon

### appsettings.json

```json
{
  "BlazorUI": {
    "AutoStart": false  // Artık gerekli değil
  }
}
```

### Program.cs

```csharp
// Blazor serving etkin
var blazorDistPath = Path.Combine(
    Directory.GetCurrentDirectory(), 
    "wwwroot", 
    "blazor"
);

if (Directory.Exists(blazorDistPath))
{
    app.UseStaticFiles();
    app.UseBlazorFrameworkFiles();
    app.MapFallbackToFile("blazor/index.html");
}
```

---

## 📊 Karşılaştırma

| Özellik | İki Port | Tek Port |
|---------|----------|----------|
| Kurulum | Kolay | Orta |
| CORS | Problem | Yok |
| Development | Kolay | Rebuild gerekli |
| Production | Karmaşık | Basit |
| Deploy | İki servis | Tek servis |
| URL | İki domain | Tek domain |
| Önerilen | Development | Production ⭐ |

---

## 💡 İpuçları

**Development:**
```bash
# Hızlı test için hot reload
dotnet watch run --project AppointmentManagementSystem.API

# Blazor değişikliği sonrası
build-blazor.bat
```

**Production:**
```bash
# Release build
dotnet publish -c Release

# Size optimize
dotnet publish -c Release /p:PublishTrimmed=true
```

**Debug:**
```bash
# API logs
dotnet run --project AppointmentManagementSystem.API

# Browser console
F12 → Console → Blazor errors
```

---

## 🎯 Sonuç

✅ **Tek komut:** `start-single-port.bat`  
✅ **Tek port:** http://localhost:5089  
✅ **Production ready:** Static serving  
✅ **CORS yok:** Same origin  

**Başarılar! 🚀**
