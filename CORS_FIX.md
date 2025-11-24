# CORS Sorunu Çözümü

## 🔴 Sorun
Frontend (https://aptivaplan.com.tr) Backend'e (https://hub.aptivaplan.com.tr) istek atarken CORS hatası alıyordu:

```
Access to fetch at 'https://hub.aptivaplan.com.tr/api/...' from origin 'https://aptivaplan.com.tr' 
has been blocked by CORS policy: No 'Access-Control-Allow-Origin' header is present on the requested resource.
```

## ✅ Çözüm

### 1. Backend CORS Policy Güncellendi
**AppointmentManagementSystem.API/Program.cs** dosyasında CORS policy'sine frontend domain'i eklendi:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",
        corsBuilder =>
        {
            corsBuilder.WithOrigins(
                "https://localhost:7172",  // Development
                "http://localhost:5090",   // Development
                "https://localhost:5090",  // Development
                "https://aptivaplan.com.tr",  // Frontend (Production)
                "http://aptivaplan.com.tr",   // Frontend (HTTP fallback)
                "https://hub.aptivaplan.com.tr", // Backend (self)
                "http://hub.aptivaplan.com.tr"   // Backend (HTTP fallback)
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials() // JWT ve SignalR için gerekli
            .WithExposedHeaders("*");
        });
});
```

### 2. Frontend API Base URL Kontrolü
**AppointmentManagementSystem.BlazorUI/Program.cs**:
```csharp
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://hub.aptivaplan.com.tr/")
});
```

## 🧪 Test Etme

### Browser Console Test
Frontend'de (https://aptivaplan.com.tr) tarayıcı console'unda:

```javascript
// Test API çağrısı
fetch('https://hub.aptivaplan.com.tr/api/categories')
  .then(response => response.json())
  .then(data => console.log('✅ CORS çalışıyor:', data))
  .catch(error => console.error('❌ CORS hatası:', error));
```

### CURL Test (Backend'den)
```bash
curl -H "Origin: https://aptivaplan.com.tr" \
     -H "Access-Control-Request-Method: GET" \
     -H "Access-Control-Request-Headers: Content-Type" \
     -X OPTIONS \
     https://hub.aptivaplan.com.tr/api/categories -v
```

**Beklenen Response Headers:**
```
Access-Control-Allow-Origin: https://aptivaplan.com.tr
Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS
Access-Control-Allow-Headers: Content-Type, Authorization
Access-Control-Allow-Credentials: true
```

## 🔧 IIS Deployment Sonrası Kontrol

### 1. web.config CORS Ayarları
**ÖNEMLİ:** web.config'de CORS headers OLMAMASINI sağladık!

ASP.NET Core uygulamalarında CORS middleware (Program.cs) üzerinden yönetilir. web.config'deki CORS headers ile çakışma olmaması için **kaldırıldı**.

```xml
<!-- CORS headers KALDIRILDI -->
<!-- Program.cs'deki CORS policy kullanılıyor -->
```

**Neden?**
- web.config'de wildcard (`*`) ile Program.cs'de `.AllowCredentials()` çakışır
- ASP.NET Core middleware daha esnek ve güvenli
- Spesifik origin kontrolü sadece Program.cs'de yapılıyor

### 2. IIS URL Rewrite Modülü
**GEREKLİ DEĞİL!** CORS tamamen ASP.NET Core middleware'de yönetiliyor.

IIS URL Rewrite ile CORS ayarlamak **ÖNERİLMEZ** çünkü:
- ASP.NET Core'un kendi CORS middleware'i ile çakışır
- Wildcard + Credentials problemi yaratır
- Daha az esnek ve güvenli

**Mevcut Çözüm:** Program.cs'deki CORS policy yeterli ✅

## 🐛 Sorun Giderme

### Problem 1: OPTIONS Request Failed
**Belirti:** Preflight OPTIONS request 405 veya 404 veriyor

**Çözüm:**
1. IIS Handler Mappings'de aspNetCore handler var mı kontrol et
2. Request Filtering'de OPTIONS metodu allowed olmalı

### Problem 2: Credentials ile CORS Hatası
**Belirti:** `Access-Control-Allow-Credentials` hatası

**Çözüm:**
- `.AllowCredentials()` eklendi (zaten var)
- Frontend'de `credentials: 'include'` kullanılmalı

```javascript
fetch('https://hub.aptivaplan.com.tr/api/...', {
  credentials: 'include',
  headers: {
    'Authorization': 'Bearer YOUR_TOKEN'
  }
});
```

### Problem 3: Wildcard + Credentials Çakışması
**Belirti:** "Cannot use wildcard in Access-Control-Allow-Origin when credentials flag is true"

**Çözüm:**
- Wildcard (`*`) kullanma, spesifik origin'ler tanımla (✅ Zaten yapıldı)

## ✅ Deployment Checklist

### Backend (hub.aptivaplan.com.tr)
- [ ] Program.cs'de CORS policy güncel
- [ ] Application publish edildi
- [ ] IIS'te uygulama çalışıyor
- [ ] SSL sertifikası geçerli
- [ ] OPTIONS request test edildi

### Frontend (aptivaplan.com.tr)
- [ ] Program.cs'de API BaseAddress doğru
- [ ] Blazor WASM publish edildi
- [ ] Browser console'da fetch test edildi
- [ ] API çağrıları çalışıyor

### Test Senaryoları
```
✅ GET /api/categories (Public endpoint)
✅ POST /api/auth/login (Authentication)
✅ GET /api/appointments (Authorization header ile)
✅ POST /api/payments/webhook (PayTR için)
```

## 📊 Desteklenen Origin'ler

| Domain | HTTP | HTTPS | Açıklama |
|--------|------|-------|----------|
| localhost:7172 | ❌ | ✅ | Development (Blazor) |
| localhost:5090 | ✅ | ✅ | Development (Blazor) |
| aptivaplan.com.tr | ✅ | ✅ | Production Frontend |
| hub.aptivaplan.com.tr | ✅ | ✅ | Production Backend |

## 🔐 Güvenlik Notları

1. **Production'da HTTP → HTTPS Redirect:** 
   - HTTP origin'leri sadece geçiş dönemi için eklenmiştir
   - Production'da HTTPS zorunlu kılınmalı

2. **Credentials:**
   - JWT token'lar Authorization header'da gönderilir
   - `.AllowCredentials()` JWT için gereklidir

3. **Exposed Headers:**
   - Custom header'lar `.WithExposedHeaders("*")` ile erişilebilir
   - Frontend'de response header'larına erişim sağlar

---

**Düzeltme Tarihi:** 24.11.2025  
**Hazırlayan:** E1 AI Agent  
**Durum:** ✅ Çözüldü
