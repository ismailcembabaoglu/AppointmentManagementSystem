# PayTR Webhook Debug Kılavuzu

## 🔴 Problem: Webhook Hiç Çalışmıyor

PayTR'den backend'e webhook isteği hiç gelmiyor veya geliyor ama işlenmiyor.

## 🔍 Debug Adımları

### 1. Webhook URL'si Dışarıdan Erişilebilir Mi?

#### Test 1: Basit GET İsteği
Tarayıcıda aç:
```
https://hub.aptivaplan.com.tr/api/payments/webhook
```

**Beklenen:** 
- ✅ Sayfa açılmalı (boş olabilir ama 404 olmamalı)
- ❌ SSL hatası olmamalı
- ❌ Timeout olmamalı

**Olası Sonuçlar:**
- **404 Not Found**: Routing yanlış veya backend çalışmıyor
- **SSL Certificate Error**: SSL sertifikası geçersiz
- **Connection Timeout**: Firewall engelliyor veya site kapalı
- **200/405**: ✅ Site erişilebilir (405 normal, GET desteklemiyor)

#### Test 2: POST İsteği (Online Tool)
[https://reqbin.com](https://reqbin.com) veya [https://hoppscotch.io](https://hoppscotch.io) kullan:

```
Method: POST
URL: https://hub.aptivaplan.com.tr/api/payments/webhook
Content-Type: application/x-www-form-urlencoded
Body: merchant_oid=TEST&status=success&total_amount=100&hash=test
```

**Beklenen:** HTTP 200, Body: "OK"

### 2. IIS Loglarını Kontrol Et

```powershell
# PowerShell (Admin)
cd C:\inetpub\logs\LogFiles\W3SVC1\

# En son log dosyasını göster
Get-ChildItem | Sort-Object LastWriteTime -Descending | Select-Object -First 1 | Get-Content -Tail 50
```

**Ara:**
```
POST /api/payments/webhook
```

**Olası Sonuçlar:**
- **Hiç POST isteği yok**: PayTR webhook URL'si yanlış veya firewall engelliyor
- **405 hatası var**: IIS/WebDAV sorunu
- **404 hatası var**: Routing yanlış
- **500 hatası var**: Backend kodu hatalı
- **200 başarılı**: ✅ Webhook geliyor ve işleniyor

### 3. Backend Loglarını Kontrol Et

#### Visual Studio Output
Eğer Visual Studio'da Debug modunda çalıştırıyorsan:
```
Output window → "AppointmentManagementSystem.API"
```

**Ara:**
```
=== PayTR Webhook Received ===
```

#### IIS Logs (stdout)
```powershell
cd C:\inetpub\wwwroot\YourApp\logs\
Get-Content stdout_*.log -Tail 100
```

**Olası Sonuçlar:**
- **Log yok**: Backend hiç çalışmıyor
- **"Webhook Received" yok**: İstek gelmiyor
- **Exception var**: Backend kodu hatalı

### 4. PayTR Panel Ayarını Kontrol Et

```
PayTR Merchant Panel → Ayarlar → Entegrasyon Ayarları
```

**Kontrol Et:**
```
Bildirim URL'si: https://hub.aptivaplan.com.tr/api/payments/webhook
```

**Dikkat:**
- ✅ https:// ile başlamalı
- ✅ /api/payments/webhook tam olarak bu şekilde
- ❌ Boşluk olmamalı
- ❌ Ekstra slash olmamalı (örn: /api//payments)

**Kaydet ve test ödeme yap!**

### 5. Firewall ve Network Kontrolü

#### Windows Firewall
```powershell
# Inbound rules kontrol et
Get-NetFirewallRule | Where-Object {$_.Direction -eq "Inbound" -and $_.Enabled -eq "True"} | Select-Object Name, DisplayName
```

**Kontrol:**
- Port 443 (HTTPS) açık mı?
- "World Wide Web Services (HTTPS Traffic-In)" aktif mi?

#### PayTR IP'leri
PayTR'nin webhook'ları şu IP aralığından gelir:
```
185.106.144.0/24
```

**Firewall'da bu IP'lere izin ver!**

### 6. IIS Site ve Application Pool Kontrolü

```powershell
# IIS site durumunu kontrol et
Import-Module WebAdministration
Get-Website | Where-Object {$_.Name -like "*aptiva*"} | Select-Object Name, State, PhysicalPath

# Application Pool durumu
Get-WebAppPoolState -Name "YourAppPool"
```

**Kontrol:**
- Site State: Started ✅
- Application Pool: Started ✅

Eğer Stopped ise:
```powershell
Start-Website -Name "YourSiteName"
Start-WebAppPool -Name "YourAppPool"
```

### 7. SSL Sertifikası Kontrolü

```powershell
# Site bindings kontrol et
Get-WebBinding -Name "YourSiteName"
```

**Kontrol:**
- Protocol: https ✅
- Port: 443 ✅
- SSL Flags: Sni (Server Name Indication) ✅

**SSL Sertifikası Geçerli Mi?**
```
https://www.ssllabs.com/ssltest/analyze.html?d=hub.aptivaplan.com.tr
```

### 8. Manuel Webhook Testi (Postman/PowerShell)

#### PowerShell Test
```powershell
$body = @{
    merchant_oid = "TEST123"
    status = "success"
    total_amount = "100"
    hash = "test_hash"
}

Invoke-WebRequest -Uri "https://hub.aptivaplan.com.tr/api/payments/webhook" `
    -Method POST `
    -Body $body `
    -ContentType "application/x-www-form-urlencoded" `
    -Verbose
```

**Beklenen:**
```
StatusCode: 200
Content: OK
```

### 9. Backend Endpoint'i Kontrol Et

Controller'ın çalışıp çalışmadığını test et:

```powershell
# Basit bir GET endpoint test et
Invoke-WebRequest -Uri "https://hub.aptivaplan.com.tr/api/categories" -Method GET
```

**Beklenen:** HTTP 200 (veya 401 Authentication gerekiyorsa)

**Eğer 404 alıyorsan:** Backend çalışmıyor veya routing yanlış.

## 🛠️ Olası Sorunlar ve Çözümleri

### Sorun 1: "Connection Timeout" veya Site Açılmıyor

**Sebep:**
- IIS site kapalı
- Firewall/network engelliyor
- DNS yanlış yapılandırılmış

**Çözüm:**
```powershell
# IIS'i restart et
iisreset /restart

# Site'ı başlat
Start-Website -Name "YourSiteName"

# Firewall'da port 443'ü aç
New-NetFirewallRule -DisplayName "HTTPS Inbound" -Direction Inbound -Protocol TCP -LocalPort 443 -Action Allow
```

### Sorun 2: "404 Not Found"

**Sebep:**
- Backend çalışmıyor
- Routing yanlış
- Controller attribute yanlış

**Çözüm:**
1. Backend'i publish et
2. IIS'te Application Pool restart
3. Controller'da route'u kontrol et: `[Route("api/[controller]")]`

### Sorun 3: "500 Internal Server Error"

**Sebep:**
- Backend kodu hatalı
- Database bağlantısı yok
- appsettings.json eksik

**Çözüm:**
1. Backend loglarını kontrol et
2. Event Viewer → Application logs
3. appsettings.json doğru mu kontrol et

### Sorun 4: PayTR Panelinde "Timeout"

**Sebep:**
- Webhook endpoint 30 saniyeden uzun sürede yanıt veriyor
- Backend infinite loop'ta

**Çözüm:**
1. Backend loglarında exception var mı kontrol et
2. Database query'leri optimize et
3. Webhook handler'da timeout koy

### Sorun 5: IIS Loglarında Hiç POST İsteği Yok

**Sebep:**
- PayTR panel ayarı yanlış
- PayTR firewall'dan engellenmiş
- Webhook URL yanlış

**Çözüm:**
1. PayTR panel ayarını kontrol et
2. Firewall'da PayTR IP'lerine izin ver
3. Manuel webhook test et

## ✅ Checklist

- [ ] Webhook URL tarayıcıda açılıyor (405 veya 200 alıyorum)
- [ ] Manuel POST test başarılı (PowerShell/Postman)
- [ ] IIS site ve app pool started
- [ ] SSL sertifikası geçerli
- [ ] Firewall port 443 açık
- [ ] PayTR IP'leri whitelisted
- [ ] PayTR panel webhook URL doğru
- [ ] Backend publish edildi ve güncel
- [ ] web.config dosyası publish klasöründe
- [ ] IIS restart yapıldı

## 🎯 Hızlı Test Senaryosu

```powershell
# 1. Site erişilebilir mi?
Invoke-WebRequest -Uri "https://hub.aptivaplan.com.tr" -Method GET

# 2. API çalışıyor mu?
Invoke-WebRequest -Uri "https://hub.aptivaplan.com.tr/api/categories" -Method GET

# 3. Webhook endpoint erişilebilir mi?
Invoke-WebRequest -Uri "https://hub.aptivaplan.com.tr/api/payments/webhook" -Method POST -Body "test=1"

# Hepsi başarılıysa: ✅ Backend çalışıyor
# Herhangi biri başarısızsa: ❌ Backend sorunu var
```

---

**Hazırlayan:** E1 AI Agent  
**Tarih:** 24.11.2025
