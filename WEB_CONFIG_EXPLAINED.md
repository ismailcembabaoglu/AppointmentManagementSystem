# web.config Dosyası Açıklaması

## 📄 web.config Nedir?

IIS (Internet Information Services) üzerinde çalışan ASP.NET Core uygulamaları için yapılandırma dosyasıdır.

## ⚙️ Mevcut Yapılandırma

### 1. ASP.NET Core Module
```xml
<handlers>
  <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
</handlers>
```

**Açıklama:**
- IIS'e gelen tüm istekleri ASP.NET Core uygulamasına yönlendirir
- `AspNetCoreModuleV2`: .NET Core 3.0+ için gerekli modül

### 2. ASP.NET Core Process Ayarları
```xml
<aspNetCore processPath="dotnet" 
            arguments=".\AppointmentManagementSystem.API.dll" 
            stdoutLogEnabled="false" 
            stdoutLogFile=".\logs\stdout" 
            hostingModel="inprocess" />
```

**Parametreler:**
- `processPath="dotnet"`: .NET runtime'ı çalıştırır
- `arguments`: DLL dosya yolu
- `stdoutLogEnabled`: Console logları (production'da false)
- `stdoutLogFile`: Log dosya konumu
- `hostingModel="inprocess"`: IIS process'i içinde çalışır (daha hızlı)

**Alternatif:** `hostingModel="outofprocess"` (ayrı process, daha yavaş ama izole)

### 3. HTTP Method İzinleri
```xml
<security>
  <requestFiltering>
    <verbs>
      <add verb="POST" allowed="true" />
      <add verb="GET" allowed="true" />
      <add verb="PUT" allowed="true" />
      <add verb="DELETE" allowed="true" />
      <add verb="OPTIONS" allowed="true" />
    </verbs>
  </requestFiltering>
</security>
```

**Açıklama:**
- IIS seviyesinde HTTP method kontrolü
- PayTR webhook için POST gerekli
- CORS preflight için OPTIONS gerekli

## ❌ CORS Headers KALDIRILDI

### Neden CORS Headers web.config'de Yok?

```xml
<!-- KALDIRILDI:
<httpProtocol>
  <customHeaders>
    <add name="Access-Control-Allow-Origin" value="*" />
  </customHeaders>
</httpProtocol>
-->
```

**Sebep 1: Wildcard + Credentials Çakışması**
```
web.config: Access-Control-Allow-Origin: *
Program.cs: .AllowCredentials()

❌ Hata: "Cannot use wildcard in Access-Control-Allow-Origin when credentials flag is true"
```

**Sebep 2: ASP.NET Core Middleware Daha İyi**
- ✅ Spesifik origin kontrolü
- ✅ Dinamik yapılandırma
- ✅ Method ve header kontrolü
- ✅ Preflight cache
- ✅ Güvenlik

**Sebep 3: Çift Header Sorunu**
```
web.config: Access-Control-Allow-Origin: *
Program.cs: Access-Control-Allow-Origin: https://aptivaplan.com.tr

❌ Sonuç: İki header gönderilir, browser hata verir
```

## ✅ Doğru Yapılandırma

### web.config (IIS Seviyesi)
- HTTP method izinleri ✅
- Process ayarları ✅
- Handler mappings ✅
- CORS headers ❌ (kaldırıldı)

### Program.cs (ASP.NET Core Seviyesi)
- CORS policy ✅
- Authentication ✅
- Authorization ✅
- Routing ✅

## 🔧 Deployment Sonrası Kontrol

### 1. web.config Dosyası Yerinde mi?
```powershell
# IIS'te publish edilmiş klasörde kontrol et
Test-Path "C:\inetpub\wwwroot\YourApp\web.config"
```

### 2. AspNetCoreModuleV2 Yüklü mü?
```powershell
# IIS Manager → Modules
# "AspNetCoreModuleV2" listelenmiş olmalı
```

Yoksa [.NET Core Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/8.0) yükleyin.

### 3. Application Pool Ayarları
```
.NET CLR Version: No Managed Code ✅
Managed Pipeline Mode: Integrated ✅
Identity: ApplicationPoolIdentity veya özel hesap ✅
```

## 🐛 Yaygın Hatalar ve Çözümleri

### Hata 1: HTTP 502.5 - Process Failure
**Sebep:**
- .NET runtime yüklü değil
- DLL dosya yolu yanlış
- Application Pool izinleri yetersiz

**Çözüm:**
```powershell
# 1. .NET runtime kontrolü
dotnet --version

# 2. DLL dosya kontrolü
Test-Path "C:\inetpub\wwwroot\YourApp\AppointmentManagementSystem.API.dll"

# 3. Event Viewer logları
eventvwr.msc → Windows Logs → Application
```

### Hata 2: CORS Error (Despite Correct Configuration)
**Sebep:** web.config'de CORS headers var

**Çözüm:** web.config'den CORS headers'ı kaldır (✅ Yapıldı)

### Hata 3: HTTP 405 - Method Not Allowed
**Sebep:** Request filtering'de method blocked

**Çözüm:** web.config'de verbs kısmında method'u ekle (✅ Yapıldı)

## 📊 web.config vs Program.cs

| Özellik | web.config | Program.cs |
|---------|-----------|-----------|
| HTTP Methods | ✅ Request filtering | ❌ |
| CORS | ❌ (kaldırıldı) | ✅ Middleware |
| Authentication | ❌ | ✅ JWT |
| Authorization | ❌ | ✅ Policy |
| Routing | ❌ | ✅ Controller routes |
| Logging | ✅ stdout | ✅ ILogger |

## 🔐 Güvenlik Best Practices

### 1. Production'da stdout Logging Kapalı
```xml
<aspNetCore stdoutLogEnabled="false" />
```

### 2. CORS Sadece Middleware'de
```csharp
// Program.cs
builder.Services.AddCors(options => { ... });
```

### 3. Sensitive Data web.config'de Yok
❌ Connection string
❌ API keys
❌ JWT secret

✅ appsettings.json
✅ Environment variables
✅ Azure Key Vault

## 📝 Örnek Tam web.config

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <!-- Handler Mappings -->
    <handlers>
      <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
    </handlers>
    
    <!-- ASP.NET Core Process -->
    <aspNetCore processPath="dotnet" 
                arguments=".\AppointmentManagementSystem.API.dll" 
                stdoutLogEnabled="false" 
                stdoutLogFile=".\logs\stdout" 
                hostingModel="inprocess" />
    
    <!-- HTTP Methods -->
    <security>
      <requestFiltering>
        <verbs>
          <add verb="POST" allowed="true" />
          <add verb="GET" allowed="true" />
          <add verb="PUT" allowed="true" />
          <add verb="DELETE" allowed="true" />
          <add verb="OPTIONS" allowed="true" />
        </verbs>
      </requestFiltering>
    </security>
    
    <!-- CORS headers YOK - Program.cs'de yönetiliyor -->
  </system.webServer>
</configuration>
```

---

**Güncelleme:** 24.11.2025  
**Hazırlayan:** E1 AI Agent  
**Durum:** ✅ Optimize edildi
