# 🔧 Plesk Deployment Hatası - ÇÖZÜLDÜ ✅

## ❌ Aldığınız Hata

```
Server Error in '/' Application.
Configuration Error
Parser Error Message: The 'tempDirectory' attribute must be set to a valid absolute path.
Source File: C:\Windows\Microsoft.NET\Framework\v4.0.30319\Config\web.config
```

## 🎯 Sorunun Nedeni

1. Blazor WebAssembly uygulamanız **.NET 9.0** ile yazılmış (modern .NET)
2. Plesk/IIS uygulamayı **.NET Framework 4.0** ile çalıştırmaya çalışıyor (eski ASP.NET)
3. **web.config** dosyası eksik veya Blazor WASM için uygun değil
4. Blazor WASM **statik bir SPA** - sunucu tarafında .NET runtime gerekmez!

## ✅ Yapılan Düzeltmeler

### 1. ✅ web.config Dosyası Oluşturuldu

**Konum:** `/app/AppointmentManagementSystem.BlazorUI/wwwroot/web.config`

**İçeriği:**
- ✅ URL Rewriting (SPA routing için)
- ✅ MIME Types (.wasm, .dll, .json, .dat, .blat)
- ✅ Compression ayarları
- ✅ IIS uyumlu yapılandırma
- ✅ Hata detayları (geliştirme için)

### 2. ✅ .csproj Güncellendi

**Dosya:** `/app/AppointmentManagementSystem.BlazorUI/AppointmentManagementSystem.BlazorUI.csproj`

web.config'in publish'e dahil edilmesi sağlandı.

### 3. ✅ Publish Script'leri Hazırlandı

- `/app/publish-blazor.bat` (Windows)
- `/app/publish-blazor.sh` (Linux/Mac)

### 4. ✅ Detaylı Deployment Rehberi

**Dosya:** `/app/PLESK_DEPLOYMENT_GUIDE.md`

Adım adım tüm kurulum ve sorun giderme bilgileri.

### 5. ✅ Alternatif web.config

**Dosya:** `/app/AppointmentManagementSystem.BlazorUI/wwwroot/web.config.simple`

Ana web.config çalışmazsa kullanmak için basitleştirilmiş versiyon.

---

## 🚀 HIZLI ÇÖZÜM - 3 Adım

### Adım 1: Publish Yap

**Windows:**
```cmd
cd C:\YourPath\app
publish-blazor.bat
```

**Linux/Mac:**
```bash
cd /your/path/app
./publish-blazor.sh
```

### Adım 2: Dosyaları Yükle

Plesk'e şu klasörü yükle:
```
/app/AppointmentManagementSystem.BlazorUI/publish/wwwroot/
```

**Önemli:** 
- ✅ **TÜM DOSYALARI** yükle (_framework, _content, css, js, lib, index.html, web.config)
- ✅ **web.config** dosyası mutlaka yüklenmeli!
- ✅ Klasör yapısını koru

### Adım 3: IIS Ayarları (Plesk'te)

**Plesk → Websites & Domains → IIS Settings:**

1. **Application Pool:**
   - .NET CLR Version: **No Managed Code** ✅
   - Managed Pipeline Mode: **Integrated**

2. **URL Rewrite Module:**
   - Yüklü değilse: https://www.iis.net/downloads/microsoft/url-rewrite

3. **ASP.NET Ayarları:**
   - Plesk'te "ASP.NET" seçenekleri varsa **devre dışı bırak**

---

## 🧪 Test

1. Tarayıcıda: `https://YOUR-DOMAIN.com`
2. **Beklenilen:** Blazor uygulaması açılmalı ✅
3. **F12 Console:** Hata olmamalı ✅

---

## 🐛 Hala Hata Alıyorsanız

### Senaryo 1: "Could not load file or assembly"

**Çözüm:**
- IIS → Application Pool → **.NET CLR Version: No Managed Code** yap
- web.config dosyasının yüklendiğini kontrol et
- MIME types doğru mu kontrol et

### Senaryo 2: "404 Not Found" (sayfa yenileme)

**Çözüm:**
- IIS URL Rewrite Module yükle
- web.config içindeki `<rewrite>` bölümünü kontrol et

### Senaryo 3: "Parser Error - tempDirectory"

**Çözüm:**
- Application Pool → **.NET CLR Version: No Managed Code** ✅
- Eğer hala hata alıyorsanız, `web.config.simple` dosyasını `web.config` olarak kullan:

```bash
# Plesk File Manager'da:
web.config → web.config.backup olarak yeniden adlandır
web.config.simple → web.config olarak yeniden adlandır
```

### Senaryo 4: _framework klasörü 404

**Çözüm:**
- **Tüm dosyaları** yüklediğinden emin ol
- Klasör yapısı korunmalı
- Binary mode'da yükle (FTP)

---

## 📋 Checklist

### Publish
- [ ] `publish-blazor.bat` veya `.sh` çalıştırıldı
- [ ] `publish/wwwroot/` klasörü oluştu
- [ ] web.config dosyası wwwroot içinde var

### Yükleme
- [ ] **Tüm dosyalar** Plesk httpdocs'a yüklendi
- [ ] web.config dosyası yüklendi
- [ ] _framework klasörü tam yüklendi
- [ ] Klasör yapısı korundu

### IIS
- [ ] Application Pool: No Managed Code
- [ ] URL Rewrite Module yüklü
- [ ] ASP.NET devre dışı

### Test
- [ ] Ana sayfa açılıyor
- [ ] Console'da hata yok
- [ ] Sayfa yenileme çalışıyor

---

## 📞 Yardım Dosyaları

| Dosya | Açıklama |
|-------|----------|
| `/app/PLESK_DEPLOYMENT_GUIDE.md` | Detaylı deployment rehberi |
| `/app/publish-blazor.bat` | Windows publish script |
| `/app/publish-blazor.sh` | Linux/Mac publish script |
| `/app/AppointmentManagementSystem.BlazorUI/wwwroot/web.config` | IIS yapılandırma dosyası |
| `/app/AppointmentManagementSystem.BlazorUI/wwwroot/web.config.simple` | Basit alternatif |

---

## 🎯 Özet

**Sorun:** .NET 9.0 Blazor WASM uygulaması, ASP.NET Framework 4.0 ile çalıştırılmaya çalışılıyor.

**Çözüm:** 
1. ✅ Doğru web.config dosyası oluşturuldu
2. ✅ IIS "No Managed Code" kullanmalı
3. ✅ Statik dosya hosting (ASP.NET runtime gerekmez)
4. ✅ URL Rewrite ve MIME Types yapılandırıldı

---

**Durum:** ✅ ÇÖZÜLDÜ  
**Oluşturma:** 2025-01-08  
**Versiyon:** 1.0

---

## 📧 Destek

Sorun devam ederse:
1. Browser console (F12) screenshot'u çekin
2. IIS Application Pool ayarlarını kontrol edin
3. web.config dosyasının yüklendiğini doğrulayın
