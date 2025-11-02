# Blazor Otomatik Başlatma Sistemi

API projesi başlatıldığında Blazor UI otomatik olarak başlatılacaktır.

## 🚀 Nasıl Çalışır?

### Yöntem 1: IHostedService (Otomatik - Önerilen)

API'de `BlazorAutoStartService` adında bir background service ekledik. Bu servis:

- ✅ API başladığında otomatik çalışır
- ✅ 2 saniye bekleyip Blazor'u başlatır
- ✅ Yeni terminal/pencere açar
- ✅ API kapanınca Blazor'u da kapatır
- ✅ Sadece Development ortamında çalışır

**Kullanım:**
```bash
# Sadece API'yi çalıştır
cd AppointmentManagementSystem.API
dotnet run

# Blazor otomatik olarak başlayacak!
```

**Kontrol:**
```json
// appsettings.json
{
  "BlazorUI": {
    "AutoStart": true  // false yaparak devre dışı bırakabilirsiniz
  }
}
```

---

### Yöntem 2: Visual Studio Multiple Startup Projects

Visual Studio kullanıyorsanız, birden fazla projeyi aynı anda başlatabilirsiniz:

**Adımlar:**
1. Solution'a sağ tık
2. **"Set Startup Projects..."** seçin
3. **"Multiple startup projects"** seçin
4. Her iki proje için de **"Start"** seçin:
   - ✅ AppointmentManagementSystem.API → **Start**
   - ✅ AppointmentManagementSystem.BlazorUI → **Start**
5. **OK** ve **F5** ile çalıştır

**Avantajlar:**
- Her iki proje de Visual Studio debugger'a bağlı
- Aynı anda debug yapabilirsiniz
- Output window'da her ikisini de görebilirsiniz

---

### Yöntem 3: Docker Compose (Production)

Production/deployment için Docker Compose kullanılabilir:

```yaml
version: '3.8'
services:
  api:
    build: ./AppointmentManagementSystem.API
    ports:
      - "5089:80"
    
  blazor:
    build: ./AppointmentManagementSystem.BlazorUI
    ports:
      - "5002:80"
    depends_on:
      - api
```

---

## ⚙️ Ayarlar

### appsettings.json

```json
{
  "BlazorUI": {
    "AutoStart": true  // Otomatik başlatmayı etkinleştir/devre dışı bırak
  }
}
```

### BlazorAutoStartService Özellikleri

| Özellik | Değer | Açıklama |
|---------|-------|----------|
| Ortam | Development | Sadece development'ta çalışır |
| Bekleme | 2 saniye | API'nin tam başlaması için bekler |
| Yeni Pencere | Evet | Blazor ayrı terminalde açılır |
| Auto-Close | Evet | API kapanınca Blazor da kapanır |

---

## 🧪 Test Etme

### 1. Tek Komut ile Başlatma
```bash
cd AppointmentManagementSystem.API
dotnet run
```

**Çıktı:**
```
info: AppointmentManagementSystem.API.Services.BlazorAutoStartService[0]
      🚀 Starting Blazor UI automatically...
info: AppointmentManagementSystem.API.Services.BlazorAutoStartService[0]
      ✅ Blazor UI started successfully!
info: AppointmentManagementSystem.API.Services.BlazorAutoStartService[0]
      📁 Blazor Path: /path/to/AppointmentManagementSystem.BlazorUI
info: AppointmentManagementSystem.API.Services.BlazorAutoStartService[0]
      🌐 Blazor should be available at: https://localhost:5002
```

### 2. URL'ler
- **API:** https://localhost:5089
- **Swagger:** https://localhost:5089/swagger
- **Blazor:** https://localhost:5002 (veya Blazor console'da gösterilen port)

---

## 🛑 Otomatik Başlatmayı Devre Dışı Bırakma

### Geçici (Bu çalıştırma için)
```bash
# Manuel olarak başlatın
cd AppointmentManagementSystem.API
dotnet run

# Blazor'u başka terminalde manuel başlatın
cd AppointmentManagementSystem.BlazorUI
dotnet run
```

### Kalıcı (appsettings.json)
```json
{
  "BlazorUI": {
    "AutoStart": false
  }
}
```

---

## 📊 Avantajlar

| Özellik | Manuel | IHostedService | VS Multiple |
|---------|--------|----------------|-------------|
| Tek komut | ❌ | ✅ | ✅ |
| Auto-close | ❌ | ✅ | ✅ |
| Ayrı terminal | ❌ | ✅ | ❌ |
| Debug support | ✅ | ⚠️ | ✅ |
| Production | ✅ | ❌ | ❌ |

---

## 🔧 Sorun Giderme

### Blazor başlamıyor
```bash
# Proje path'ini kontrol et
cd AppointmentManagementSystem.API
cd ../AppointmentManagementSystem.BlazorUI
ls -la  # Dosyalar görünüyor mu?
```

### Port conflict
```bash
# Blazor'un portunu değiştir
cd AppointmentManagementSystem.BlazorUI
# Properties/launchSettings.json'da port değiştir
```

### Process kill edilmiyor
```bash
# Manuel kill
taskkill /F /IM dotnet.exe  # Windows
pkill -f dotnet              # Linux/Mac
```

---

## 💡 Öneriler

**Development:**
- ✅ IHostedService kullanın (otomatik)
- ✅ Veya Visual Studio Multiple Startup

**Production:**
- ✅ Docker Compose
- ✅ Kubernetes
- ✅ Manuel deployment

**Debug:**
- ✅ Visual Studio Multiple Startup (debugger için)

---

## 📝 Not

- Otomatik başlatma sadece **Development** ortamında çalışır
- Production'da her servis bağımsız deploy edilmelidir
- Visual Studio kullanıyorsanız Multiple Startup Projects daha iyi debug deneyimi sunar
- Command line kullanıyorsanız IHostedService yöntemi idealdir

---

**Oluşturulma:** 2025-01-08  
**Durum:** ✅ Aktif
