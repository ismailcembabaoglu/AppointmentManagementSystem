# IIS HTTP 405 - Method Not Allowed Hatası Çözümü

## 🔴 Sorun
PayTR webhook'a POST isteği gönderirken IIS **HTTP 405 - Method Not Allowed** hatası veriyor.

```
HTTP Hata Kodu: 405
IIS 10.0 Detailed Error - 405.0 - Method Not Allowed
```

## 🔍 Olası Sebepler

### 1. WebDAV Modülü Aktif
**En yaygın sebep!** WebDAV (Web Distributed Authoring and Versioning) modülü PUT, DELETE ve POST isteklerini engelleyebilir.

### 2. Static File Handler
IIS'in static file handler'ı API endpoint'lerini yakalıyor olabilir.

### 3. Handler Mappings Sırası Yanlış
aspNetCore handler'ı doğru sırada değilse istekler yanlış handler'a gidebilir.

### 4. Request Filtering
HTTP method'lar blocked listesinde olabilir.

## ✅ Çözüm

### 1. web.config Güncellemesi (YENİ)

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <!-- WebDAV modülünü kaldır -->
    <modules>
      <remove name="WebDAVModule" />
    </modules>
    
    <!-- Handler'ları doğru sırada ayarla -->
    <handlers>
      <remove name="WebDAV" />
      <remove name="ExtensionlessUrlHandler-Integrated-4.0" />
      <remove name="OPTIONSVerbHandler" />
      <remove name="TRACEVerbHandler" />
      <add name="ExtensionlessUrlHandler-Integrated-4.0" path="*." verb="*" type="System.Web.Handlers.TransferRequestHandler" preCondition="integratedMode,runtimeVersionv4.0" />
      <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
    </handlers>
    
    <aspNetCore processPath="dotnet" 
                arguments=".\AppointmentManagementSystem.API.dll" 
                stdoutLogEnabled="false" 
                stdoutLogFile=".\logs\stdout" 
                hostingModel="inprocess" />
    
    <!-- HTTP Methods izinleri -->
    <security>
      <requestFiltering>
        <verbs>
          <add verb="POST" allowed="true" />
          <add verb="GET" allowed="true" />
          <add verb="PUT" allowed="true" />
          <add verb="DELETE" allowed="true" />
          <add verb="OPTIONS" allowed="true" />
          <add verb="PATCH" allowed="true" />
        </verbs>
      </requestFiltering>
    </security>
  </system.webServer>
</configuration>
```

### 2. IIS Manager'dan WebDAV Kontrolü

#### Adım 1: WebDAV Yayıncılık Kuralları (Publishing Rules)
```
IIS Manager → Sites → [Siteniz] → WebDAV Authoring Rules
```

**Kontrol:**
- WebDAV feature yüklü mü kontrol edin
- Eğer yüklüyse, "Disable WebDAV" seçin

#### Adım 2: Modules
```
IIS Manager → Sites → [Siteniz] → Modules
```

**Kontrol:**
- `WebDAVModule` listede var mı?
- Varsa: Sağ tık → Remove

#### Adım 3: Handler Mappings
```
IIS Manager → Sites → [Siteniz] → Handler Mappings
```

**Sıralama (yukarıdan aşağıya):**
1. `aspNetCore` (path: *, verb: *)
2. `ExtensionlessUrlHandler-Integrated-4.0`
3. Diğer handler'lar

**Kontrol:**
- `WebDAV` handler var mı? Varsa sil.
- `aspNetCore` handler EN ÜSTTE olmalı

### 3. Application Pool Ayarları

```
IIS Manager → Application Pools → [Pool İsminiz]
```

**Gerekli Ayarlar:**
```
.NET CLR Version: No Managed Code ✅
Managed Pipeline Mode: Integrated ✅
Start Mode: AlwaysRunning ✅
Identity: ApplicationPoolIdentity veya özel hesap ✅
```

### 4. Request Filtering

```
IIS Manager → Sites → [Siteniz] → Request Filtering → HTTP Verbs
```

**Kontrol:**
- POST, PUT, DELETE, OPTIONS allowed olmalı
- Blocked listesinde OLMAMALI

## 🧪 Test Komutları

### Test 1: Webhook Endpoint'i Test Et
```powershell
# PowerShell
Invoke-WebRequest -Uri "https://hub.aptivaplan.com.tr/api/payments/webhook" `
  -Method POST `
  -Body "test=1" `
  -ContentType "application/x-www-form-urlencoded" `
  -Verbose
```

**Beklenen Sonuç:** HTTP 200, Body: "OK"

### Test 2: CURL ile Test
```bash
curl -X POST https://hub.aptivaplan.com.tr/api/payments/webhook \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "merchant_oid=TEST123&status=success&total_amount=100&hash=test" \
  -v
```

### Test 3: OPTIONS Request (CORS Preflight)
```powershell
Invoke-WebRequest -Uri "https://hub.aptivaplan.com.tr/api/payments/webhook" `
  -Method OPTIONS `
  -Headers @{
    "Origin" = "https://aptivaplan.com.tr"
    "Access-Control-Request-Method" = "POST"
    "Access-Control-Request-Headers" = "Content-Type"
  } `
  -Verbose
```

## 🔧 Manuel IIS Konfigürasyonu

Eğer web.config yeterli olmazsa, IIS Manager'dan manuel yapılandırma:

### 1. WebDAV'ı Tamamen Kaldır (Windows Features)

```
Control Panel → Programs → Turn Windows features on or off
→ Internet Information Services
→ World Wide Web Services
→ Common HTTP Features
→ ❌ WebDAV Publishing (İşareti kaldır)
```

**Sistem Restart Gerekebilir**

### 2. Application Pool'u Yeniden Oluştur

```powershell
# PowerShell (Admin olarak çalıştır)

# Eski pool'u durdur
Stop-WebAppPool -Name "YourAppPool"

# Yeni pool oluştur
New-WebAppPool -Name "YourAppPool_New"

# Ayarları yap
Set-ItemProperty IIS:\AppPools\YourAppPool_New -Name "managedRuntimeVersion" -Value ""
Set-ItemProperty IIS:\AppPools\YourAppPool_New -Name "startMode" -Value "AlwaysRunning"

# Site'ı yeni pool'a ata
Set-ItemProperty IIS:\Sites\YourSite -Name "applicationPool" -Value "YourAppPool_New"
```

### 3. Failed Request Tracing Aktifleştir

```
IIS Manager → Sites → [Siteniz] → Failed Request Tracing...
```

**Ayarlar:**
1. Enable: ✅
2. Status Code: 405
3. Provider: ASPNET, WWW Server

**Log Konumu:** `C:\inetpub\logs\FailedReqLogFiles\`

Failed request oluştuğunda log dosyası incelenebilir.

## 📊 Yaygın 405 Hata Senaryoları

### Senaryo 1: WebDAV Modülü Aktif
**Belirti:**
```
Handler: WebDAVModule
Error: 405.0 - Method Not Allowed
```

**Çözüm:** web.config'e `<remove name="WebDAVModule" />` ekle ✅

### Senaryo 2: Static File Handler
**Belirti:**
```
Handler: StaticFile
Error: 405.0 - Method Not Allowed
```

**Çözüm:** Handler mappings'de `aspNetCore` en üste taşı

### Senaryo 3: Request Filtering
**Belirti:**
```
Error: 405.0 - Method Not Allowed
FailedRequestTracing: "Verb Blocked"
```

**Çözüm:** Request Filtering → HTTP Verbs → POST'u allow et

## 🐛 Troubleshooting Adımları

### Adım 1: IIS Loglarını İncele
```powershell
# En son 10 log entry'sini göster
Get-Content "C:\inetpub\logs\LogFiles\W3SVC1\u_ex*.log" -Tail 10 | Select-String "405"
```

### Adım 2: Event Viewer
```
eventvwr.msc
→ Windows Logs → Application
→ Source: "IIS AspNetCore Module V2"
```

**Ara:** Hata mesajları, 405, webhook

### Adım 3: Failed Request Tracing
```
C:\inetpub\logs\FailedReqLogFiles\W3SVC1\
```

En son XML dosyasını browser'da aç, hangi module'de fail olduğunu gör.

### Adım 4: ASP.NET Core Logs
```
# Application klasöründe
C:\inetpub\wwwroot\YourApp\logs\stdout*.log
```

Backend tarafında hata var mı kontrol et.

## ✅ Deployment Checklist

- [ ] web.config güncel (WebDAV removed)
- [ ] IIS'te WebDAV modülü kaldırıldı
- [ ] Handler mappings doğru sırada
- [ ] Request Filtering'de POST allowed
- [ ] Application Pool "No Managed Code"
- [ ] aspNetCore handler en üstte
- [ ] web.config publish klasöründe
- [ ] IIS restart yapıldı
- [ ] Test webhook çalıştı (HTTP 200)

## 🎯 Başarı Kriterleri

### PayTR Panel
```
✅ Ödeme Durumu: Başarılı
✅ Bildirim Durumu: Başarılı
✅ HTTP Yanıt Kodu: 200
✅ Yanıt İçeriği: OK
```

### IIS Logs
```
POST /api/payments/webhook - 200 0 0
```

### Backend Logs
```
=== PayTR Webhook Received ===
Method: POST
MerchantOid: CARD1AFF4467A
Status: success
Webhook response: OK
```

## 📞 Hala Çalışmıyorsa

1. **Windows Features'den WebDAV'ı kaldırın** (sistem restart gerekebilir)
2. **IIS'i tamamen restart edin:** `iisreset /restart`
3. **Application Pool'u recycle edin**
4. **Failed Request Tracing loglarını inceleyin**
5. **Başka bir IIS site oluşturup test edin**

---

**Güncelleme:** 24.11.2025  
**Hazırlayan:** E1 AI Agent  
**Durum:** ✅ Gelişmiş çözüm eklendi
