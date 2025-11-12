# 🔧 404 Not Found Hatası - ÇÖZÜM

## ❌ Aldığınız Hata

```
HTTP Error 404.0 - Not Found
Requested URL: https://aptivaplan.com.tr:443/wwwroot/index.html
Physical Path: C:\websites\aptivaplan.com.tr\httpdocs\wwwroot\index.html
```

## 🎯 Sorunun Nedeni

URL'de `/wwwroot/index.html` görünüyor ama olması gereken sadece `/index.html` olmalı.

**2 olası sebep:**

### Sebep 1: Yanlış Dosya Yükleme ❌
```
httpdocs/
  wwwroot/           ← Klasör kendisi yüklendi!
    index.html
    _framework/
    ...
```

**Doğrusu:** ✅
```
httpdocs/
  index.html         ← Dosyalar direkt burada olmalı!
  _framework/
  _content/
  css/
  js/
  lib/
  web.config
```

### Sebep 2: Eski web.config (ÇÖZÜLDÜ) ✅

Eski web.config'te `wwwroot\` prefix vardı - düzeltildi!

---

## ✅ ÇÖZÜM - 2 Yöntem

### Yöntem 1: Dosyaları Yeniden Yükle (ÖNERİLEN)

**Adım 1:** Güncellenmiş web.config ile yeniden publish yap
```cmd
cd C:\YourPath\app
publish-blazor.bat
```

**Adım 2:** Plesk'teki httpdocs klasörünü temizle
```
Plesk → File Manager → httpdocs/
→ Tüm dosyaları sil (veya yedekle)
```

**Adım 3:** Doğru dosyaları yükle
```
Yüklenecek: /app/AppointmentManagementSystem.BlazorUI/publish/wwwroot/ içindeki DOSYALAR
Yüklenecek yer: httpdocs/ (wwwroot klasörü değil, içindekiler!)
```

**Sonuç:**
```
httpdocs/
├── _framework/
├── _content/
├── css/
├── js/
├── lib/
├── index.html
├── favicon.png
└── web.config    ← Güncellenmiş versiyon!
```

---

### Yöntem 2: Mevcut Dosyaları Taşı

Eğer publish yapmak istemiyorsanız:

**Plesk File Manager'da:**

1. `httpdocs/wwwroot/` içindeki TÜM dosyaları seç
2. Kes (Cut)
3. Bir üst klasöre (`httpdocs/`) yapıştır
4. Boş kalan `wwwroot/` klasörünü sil
5. `web.config` dosyasını yenisiyle değiştir

**FTP ile:**
```bash
# Lokal bilgisayarınızda
1. /app/AppointmentManagementSystem.BlazorUI/wwwroot/web.config indir
2. Plesk'e yükle (üzerine yaz)
```

---

## 🧪 Test

### Test 1: Ana Sayfa
```
https://aptivaplan.com.tr
```
**Beklenen:** Blazor uygulaması açılmalı ✅

### Test 2: Dosya Yapısı Kontrolü

**Plesk File Manager → httpdocs:**

```
✅ index.html          (var mı?)
✅ web.config          (var mı?)
✅ _framework/         (klasör var mı?)
✅ _content/           (klasör var mı?)
✅ css/                (klasör var mı?)
✅ js/                 (klasör var mı?)
❌ wwwroot/            (OLMAMALI!)
```

### Test 3: Browser Console

**F12 → Console:**
```
✅ Blazor WebAssembly initialized
✅ No 404 errors in Network tab
```

---

## 📋 Güncellenmiş web.config

**Yeni web.config içeriği:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <httpErrors errorMode="Detailed" />
    <validation validateIntegratedModeConfiguration="false" />
    
    <!-- URL Rewriting - wwwroot prefix YOK artık! -->
    <rewrite>
      <rules>
        <rule name="SPA fallback routing" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
          </conditions>
          <action type="Rewrite" url="/index.html" />
        </rule>
      </rules>
    </rewrite>
    
    <!-- MIME Types -->
    <staticContent>
      <remove fileExtension=".wasm" />
      <remove fileExtension=".dll" />
      <remove fileExtension=".dat" />
      <remove fileExtension=".blat" />
      <remove fileExtension=".json" />
      
      <mimeMap fileExtension=".wasm" mimeType="application/wasm" />
      <mimeMap fileExtension=".dll" mimeType="application/octet-stream" />
      <mimeMap fileExtension=".dat" mimeType="application/octet-stream" />
      <mimeMap fileExtension=".blat" mimeType="application/octet-stream" />
      <mimeMap fileExtension=".json" mimeType="application/json" />
    </staticContent>
    
    <!-- Compression -->
    <httpCompression>
      <dynamicTypes>
        <add mimeType="application/octet-stream" enabled="true" />
        <add mimeType="application/wasm" enabled="true" />
      </dynamicTypes>
      <staticTypes>
        <add mimeType="application/wasm" enabled="true" />
        <add mimeType="application/octet-stream" enabled="true" />
      </staticTypes>
    </httpCompression>
  </system.webServer>
  
  <system.web>
    <customErrors mode="Off" />
    <compilation tempDirectory="C:\Windows\Temp" />
  </system.web>
</configuration>
```

**Değişiklikler:**
- ❌ `<rule name="Serve subdir">` kaldırıldı
- ❌ `url="wwwroot\{R:0}"` kaldırıldı
- ✅ `url="/index.html"` düzeltildi
- ✅ İsDirectory condition eklendi

---

## 🔍 Sorun Giderme

### Hala 404 alıyorum

**Kontrol Et:**

1. **Dosya yapısı:**
```bash
# httpdocs içinde olmalı:
index.html           ✅
web.config          ✅
_framework/         ✅

# httpdocs içinde OLMAMALI:
wwwroot/            ❌
```

2. **web.config içeriği:**
```bash
# Plesk File Manager → httpdocs/web.config → Edit
# İçeriğinde "wwwroot" kelimesi var mı?
# Varsa YENİ versiyonu yükle!
```

3. **URL Rewrite Module:**
```bash
# IIS Manager → Modules
# "Rewrite Module" var mı?
# Yoksa indir: https://www.iis.net/downloads/microsoft/url-rewrite
```

### index.html açılıyor ama _framework 404

**Sebep:** MIME types eksik veya hatalı

**Çözüm:**
```bash
# IIS Manager → MIME Types
# .wasm → application/wasm
# .dll → application/octet-stream
# .dat → application/octet-stream
# .blat → application/octet-stream
```

### Sayfa yenilediğimde 404

**Sebep:** URL Rewrite çalışmıyor

**Çözüm:**
1. URL Rewrite Module yükle
2. web.config'in güncellenmiş versiyonunu yükle
3. IIS restart: `iisreset` (Plesk'te otomatik)

---

## 📦 Doğru Publish Süreci

### Adım 1: Clean & Build
```cmd
cd AppointmentManagementSystem.BlazorUI
dotnet clean
dotnet restore
dotnet build -c Release
```

### Adım 2: Publish
```cmd
dotnet publish -c Release -o ./publish
```

### Adım 3: Dosya Kontrol
```bash
# Kontrol et:
publish/wwwroot/
├── _framework/      ✅
├── _content/       ✅
├── css/            ✅
├── js/             ✅
├── lib/            ✅
├── index.html      ✅
└── web.config      ✅ (güncellenmiş!)
```

### Adım 4: Plesk'e Yükle
```
Kaynak: publish/wwwroot/* (içindeki dosyalar)
Hedef: httpdocs/ (dosyaları direkt buraya)
```

**YANLIŞ:** ❌
```
httpdocs/wwwroot/index.html
```

**DOĞRU:** ✅
```
httpdocs/index.html
```

---

## ✅ Özet

| Sorun | Çözüm |
|-------|-------|
| `/wwwroot/index.html` 404 | wwwroot klasörü httpdocs'a değil, içindekiler yüklenmeli |
| web.config eski | Güncellenmiş versiyonu yükle (wwwroot prefix yok) |
| URL Rewrite hatası | IIS URL Rewrite Module yükle |
| _framework 404 | MIME types kontrol et |

---

**Durum:** ✅ ÇÖZÜLDÜ  
**Oluşturma:** 2025-01-08  
**Versiyon:** 2.0 (Güncellenmiş)

---

## 🎯 Hızlı Checklist

- [ ] `publish-blazor.bat` çalıştır (güncellenmiş web.config ile)
- [ ] `publish/wwwroot/` içindeki DOSYALARI kopyala (klasörü değil!)
- [ ] Plesk httpdocs'a yükle (direkt içine)
- [ ] `httpdocs/wwwroot/` klasörü varsa SİL
- [ ] `httpdocs/index.html` var mı kontrol et
- [ ] `httpdocs/web.config` güncel mi kontrol et
- [ ] Browser'da test et: `https://aptivaplan.com.tr`

**Başarılar!** 🎉
