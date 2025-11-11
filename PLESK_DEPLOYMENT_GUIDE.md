# 🚀 Blazor WebAssembly - Plesk Deployment Rehberi

## ⚠️ Önemli Notlar

Bu Blazor WebAssembly uygulaması .NET 9.0 ile geliştirilmiştir. Plesk'e deploy etmek için:

1. ✅ **Statik hosting kullanılır** (ASP.NET Core runtime gerekmez)
2. ✅ **IIS üzerinde çalışır** (web.config ile)
3. ✅ **API ayrı çalışmalı** (AppointmentManagementSystem.API)

---

## 📋 Gereksinimler

### Sunucu Tarafında
- Windows Server (2016 veya üzeri)
- IIS 8.0 veya üzeri
- URL Rewrite Module (IIS için)

### Geliştirici Tarafında
- .NET 9.0 SDK
- Visual Studio 2022 veya dotnet CLI

---

## 🔧 Adım 1: Publish Hazırlığı

### 1.1 web.config Kontrolü

`/app/AppointmentManagementSystem.BlazorUI/wwwroot/web.config` dosyası oluşturuldu. ✅

**İçeriği:**
- URL Rewriting (SPA routing için)
- MIME Types (.wasm, .dll, .json)
- Compression ayarları
- Error handling

### 1.2 API URL Güncelleme

**ÖNEMLİ:** Publish öncesi API URL'ini güncelle!

`/app/AppointmentManagementSystem.BlazorUI/Program.cs` - Satır 21:
```csharp
// Geliştirme
BaseAddress = new Uri("https://localhost:5089/")

// Production (Plesk'teki API adresi)
BaseAddress = new Uri("https://YOUR-DOMAIN.com/api/")
```

veya appsettings kullan:

`/app/AppointmentManagementSystem.BlazorUI/wwwroot/appsettings.json`:
```json
{
  "ApiBaseUrl": "https://YOUR-DOMAIN.com/api/"
}
```

---

## 🏗️ Adım 2: Publish İşlemi

### Yöntem 1: dotnet CLI (Önerilen)

```bash
cd /app/AppointmentManagementSystem.BlazorUI

# Release build
dotnet publish -c Release -o ./publish

# Publish klasörü: /app/AppointmentManagementSystem.BlazorUI/publish/wwwroot
```

### Yöntem 2: Visual Studio

1. Solution Explorer'da `AppointmentManagementSystem.BlazorUI` sağ tıkla
2. **"Publish"** seç
3. **"Folder"** seç
4. Target Location: `publish`
5. **"Publish"** butonuna tıkla

---

## 📦 Adım 3: Dosyaları Hazırlama

Publish sonrası şu klasör oluşur:
```
/publish/wwwroot/
├── _framework/          (Blazor dosyaları - 30-50 MB)
├── _content/           (Component libraries)
├── css/                (Stil dosyaları)
├── js/                 (JavaScript dosyaları)
├── lib/                (Kütüphaneler)
├── index.html          (Ana sayfa)
├── favicon.png
└── web.config          (IIS yapılandırma - ÖNEMLİ!)
```

**Yüklenecek dosyalar:** `wwwroot` klasörünün **TÜM İÇERİĞİ**

---

## 🌐 Adım 4: Plesk'e Yükleme

### 4.1 FTP/Dosya Yöneticisi ile

1. Plesk'e giriş yap
2. **Websites & Domains** → Domain seç
3. **File Manager** aç
4. **httpdocs** veya **wwwroot** klasörüne git
5. Tüm eski dosyaları sil (yedekle önce!)
6. `publish/wwwroot/` içindeki **TÜM DOSYALARI** yükle

### 4.2 Yüklenen Dosyalar

```
httpdocs/
├── _framework/          ✅
├── _content/           ✅
├── css/                ✅
├── js/                 ✅
├── lib/                ✅
├── index.html          ✅
├── favicon.png         ✅
└── web.config          ✅ (ÇOK ÖNEMLİ!)
```

---

## ⚙️ Adım 5: IIS Ayarları (Plesk'te)

### 5.1 URL Rewrite Module Kontrolü

**Websites & Domains** → **Apache & nginx Settings** (veya IIS Settings)

**Gerekli Module:**
- ✅ URL Rewrite Module 2.0+

Yoksa indir: https://www.iis.net/downloads/microsoft/url-rewrite

### 5.2 Application Pool Ayarları

**IIS Manager** → **Application Pools** → Sitenizi seç:

```
.NET CLR Version: No Managed Code  ✅ (Önemli!)
Managed Pipeline Mode: Integrated
Identity: ApplicationPoolIdentity
```

**Not:** Blazor WASM statik dosyalar olduğu için .NET runtime gerekmez!

### 5.3 MIME Types Kontrolü

`web.config` dosyamız MIME types içeriyor, ama sunucu seviyesinde de kontrol et:

**IIS Manager** → **MIME Types**:

| Extension | MIME Type |
|-----------|-----------|
| .wasm | application/wasm |
| .dll | application/octet-stream |
| .dat | application/octet-stream |
| .blat | application/octet-stream |
| .json | application/json |

---

## 🧪 Adım 6: Test

### 6.1 Tarayıcı Testi

```
https://YOUR-DOMAIN.com
```

**Beklenen:**
- ✅ Sayfa yüklenmeli
- ✅ Console'da hata olmamalı (F12)
- ✅ Network'te 200 OK görülmeli

### 6.2 Console Kontrolü (F12)

**Başarılı yüklenme:**
```
Blazor WebAssembly initialized
```

**Hata varsa:**
```
Failed to load resource: _framework/blazor.boot.json
Could not load file or assembly...
```

**Çözüm:** MIME types veya dosya yolu hatalı

### 6.3 API Bağlantı Testi

Console'da:
```javascript
fetch('https://YOUR-DOMAIN.com/api/categories')
  .then(r => r.json())
  .then(console.log)
```

**Başarılı:** API response almalısınız  
**Hatalı:** CORS veya API URL hatalı

---

## 🐛 Sorun Giderme

### Hata 1: "Could not load file or assembly"

**Sebep:** MIME types eksik veya hatalı

**Çözüm:**
1. `web.config` dosyasının yüklendiğinden emin ol
2. IIS'te MIME types kontrol et
3. Application Pool → `.NET CLR Version: No Managed Code` olmalı

### Hata 2: "404 Not Found" (sayfa yenilediğinde)

**Sebep:** URL Rewrite çalışmıyor

**Çözüm:**
1. IIS URL Rewrite Module yükle
2. `web.config` içindeki `<rewrite>` bölümünü kontrol et
3. IIS Manager → URL Rewrite → Rules kontrol et

### Hata 3: "Server Error in '/' Application"

**Sebep:** ASP.NET Framework ile çalıştırılmaya çalışılıyor

**Çözüm:**
1. Application Pool → `.NET CLR Version: No Managed Code`
2. `web.config` güncel mi kontrol et
3. Plesk'te "ASP.NET" ayarları varsa **devre dışı bırak**

### Hata 4: "Parser Error - tempDirectory"

**Sebep:** ASP.NET Framework 4.0 kullanılmaya çalışılıyor

**Çözüm:**
1. Application Pool → `.NET CLR Version: No Managed Code` ✅
2. Plesk → Hosting Settings → ASP.NET devre dışı
3. `web.config` dosyamızdaki `tempDirectory` satırını kaldır

### Hata 5: API CORS Hatası

**Sebep:** API CORS ayarları eksik

**Çözüm (API tarafında):**
```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",
        policy => policy
            .WithOrigins("https://YOUR-DOMAIN.com")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

app.UseCors("AllowBlazor");
```

### Hata 6: _framework klasörü 404

**Sebep:** Dosya yolları hatalı veya eksik yükleme

**Çözüm:**
1. `publish/wwwroot/` içindeki **TÜM DOSYALARI** yükle
2. Klasör yapısı korunmalı
3. FTP binary mode'da yükle (ASCII değil!)

---

## 📝 Checklist

### Publish Öncesi
- [ ] API URL güncellendi (Program.cs)
- [ ] web.config wwwroot'ta mevcut
- [ ] dotnet publish çalıştırıldı
- [ ] publish/wwwroot klasörü oluştu

### Yükleme
- [ ] Tüm dosyalar httpdocs'a yüklendi
- [ ] web.config dosyası var
- [ ] _framework klasörü tam yüklendi
- [ ] Klasör yapısı korundu

### Sunucu Ayarları
- [ ] IIS URL Rewrite Module yüklü
- [ ] Application Pool: No Managed Code
- [ ] MIME Types doğru
- [ ] ASP.NET devre dışı (Plesk)

### Test
- [ ] Ana sayfa açılıyor
- [ ] Console'da hata yok
- [ ] API bağlantısı çalışıyor
- [ ] Sayfa yenileme (F5) çalışıyor
- [ ] Direct URL navigation çalışıyor

---

## 🎯 Özet Komutlar

```bash
# 1. Publish
cd /app/AppointmentManagementSystem.BlazorUI
dotnet publish -c Release -o ./publish

# 2. Dosyalar
# publish/wwwroot/* → Plesk httpdocs/

# 3. IIS Ayarları (Plesk'te)
# Application Pool → .NET CLR Version: No Managed Code
# URL Rewrite Module yükle
# MIME Types kontrol et

# 4. Test
# https://YOUR-DOMAIN.com
```

---

## 📞 Yardım

### API Ayrı Deploy Etme

API'yi **ayrı bir subdomain**'de host et:

**API:** https://api.YOUR-DOMAIN.com  
**Blazor:** https://YOUR-DOMAIN.com

**Blazor Program.cs:**
```csharp
BaseAddress = new Uri("https://api.YOUR-DOMAIN.com/")
```

### URL Rewrite Module İndir

https://www.iis.net/downloads/microsoft/url-rewrite

---

**Oluşturma:** 2025-01-08  
**Durum:** ✅ Hazır  
**Versiyon:** 1.0
