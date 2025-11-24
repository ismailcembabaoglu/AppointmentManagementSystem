# PayTR Webhook Düzeltmesi - Deployment Checklist

## ✅ Yapılacaklar Listesi

### 1. Kodu Publish Et
```bash
cd AppointmentManagementSystem.API
dotnet publish -c Release -o ../publish
```

### 2. web.config Dosyasını Kontrol Et
```
✓ AppointmentManagementSystem.API/web.config dosyası oluşturuldu
✓ Publish klasörüne kopyalanmalı
✓ IIS'te doğru yerde olmalı
```

### 3. IIS Ayarları

#### a. Application Pool
- [ ] .NET CLR Version: **No Managed Code**
- [ ] Managed Pipeline Mode: **Integrated**
- [ ] Start Mode: **AlwaysRunning**

#### b. Site Bindings
- [ ] HTTPS Binding var mı? (Port 443)
- [ ] SSL Sertifikası geçerli mi?
- [ ] HTTP'den HTTPS'e redirect aktif mi?

#### c. Handler Mappings
- [ ] aspNetCore handler var mı?
- [ ] Path: `*`, Verb: `*`

#### d. Request Filtering
- [ ] POST metodu allowed
- [ ] PUT metodu allowed
- [ ] DELETE metodu allowed

### 4. PayTR Merchant Panel Ayarları

- [ ] Bildirim URL: `https://hub.sellerdoping.com.tr/api/payments/webhook`
- [ ] Test ödeme yap
- [ ] PayTR panelinde "Bildirim Durumu: Başarılı" görünüyor mu?

**ÖNEMLİ NOT:**
- Frontend: `https://aptivaplan.com.tr` (Blazor UI)
- Backend API: `https://hub.sellerdoping.com.tr/api` (PayTR webhook buraya gelmeli!)

### 5. Test Senaryoları

#### Test 1: Webhook Erişilebilirlik
```powershell
# PowerShell'de test et
$body = @{
    merchant_oid = "TEST123"
    status = "success"
    total_amount = "100"
    hash = "test"
}
Invoke-WebRequest -Uri "https://hub.sellerdoping.com.tr/api/payments/webhook" -Method POST -Body $body
```

**Beklenen Sonuç:** HTTP 200, Body: "OK"

#### Test 2: Gerçek Ödeme
```
1. PayTR test ortamında ödeme yap
2. Test kartı: 4111 1111 1111 1111
3. PayTR panelinde işlemi kontrol et
4. Bildirim Durumu: Başarılı olmalı
5. Backend loglarında webhook geldiğini gör
```

#### Test 3: Hash Validation
```
Backend loglarında şunu gör:
✅ Expected Hash (Base64): ABC123...
✅ Received Hash: ABC123...
✅ Hash match successful
```

### 6. Log Kontrolü

#### IIS Logs
```
Konum: C:\inetpub\logs\LogFiles\W3SVC1\
Ara: POST /api/payments/webhook
Beklenen: 200 0 0
```

#### Application Logs
```
Backend loglarında görmek istediğimiz:
=== PayTR Webhook Received ===
Content-Type: application/x-www-form-urlencoded
Method: POST
MerchantOid: [değer]
Status: success
...
Webhook response: OK
```

### 7. PayTR Panel Kontrolü

Başarılı webhook sonrası PayTR panelinde:
```
✅ Ödeme Durumu: Başarılı
✅ Bildirim Durumu: Başarılı
✅ HTTP Yanıt Kodu: 200
✅ Yanıt İçeriği: OK
```

## 🔴 Hata Durumları ve Çözümleri

### HTTP 405 - Method Not Allowed
**Çözüm:**
1. web.config dosyasını kontrol et
2. IIS Handler Mappings'i kontrol et
3. Request Filtering'de POST allowed olmalı

### HTTP 404 - Not Found
**Çözüm:**
1. Routing doğru mu kontrol et
2. API base path: `/api/payments/webhook`
3. Application published correctly

### HTTP 500 - Internal Server Error
**Çözüm:**
1. Application loglarını kontrol et
2. appsettings.json doğru mu?
3. Database bağlantısı çalışıyor mu?

### "Invalid webhook signature"
**Çözüm:**
1. PayTR MerchantKey ve MerchantSalt doğru mu?
2. Hash calculation doğru yapılıyor mu?
3. Backend loglarında hash'leri karşılaştır

## 📝 Değişen Dosyalar

1. **AppointmentManagementSystem.API/Controllers/PaymentsController.cs**
   - Webhook endpoint sadece POST kabul ediyor
   - Her durumda "OK" dönüyor
   - Detaylı logging eklendi

2. **AppointmentManagementSystem.Application/Features/Payments/Handlers/ProcessPaymentWebhookHandler.cs**
   - Hash validation bypass kaldırıldı
   - Test modunda bile hash doğrulaması yapılıyor

3. **AppointmentManagementSystem.BlazorUI/Pages/Payment/PaymentSuccess.razor**
   - Manuel webhook tetikleme kodu kaldırıldı
   - Artık sadece PayTR'nin gerçek webhook'u kullanılıyor

4. **AppointmentManagementSystem.API/web.config** (YENİ)
   - IIS için POST metoduna izin verildi
   - AspNetCore modülü yapılandırıldı

5. **AppointmentManagementSystem.API/appsettings.json**
   - CallbackUrl güncellendi: `https://hub.sellerdoping.com.tr/api/payments/webhook`
   - OkRedirectUrl güncellendi: `https://hub.sellerdoping.com.tr/api/payments/success-redirect`
   - FailRedirectUrl güncellendi: `https://hub.sellerdoping.com.tr/api/payments/fail-redirect`

6. **AppointmentManagementSystem.Infrastructure/Services/PayTRService.cs**
   - Fallback URL'ler hub.sellerdoping.com.tr'ye güncellendi

## 🎯 Başarı Kriterleri

- [ ] PayTR test ödemesi yapılabildi
- [ ] Webhook başarıyla geldi (loglar)
- [ ] PayTR panelinde "Bildirim Durumu: Başarılı"
- [ ] HTTP 200 yanıt kodu alındı
- [ ] "OK" yanıtı döndü
- [ ] Hash validation başarılı
- [ ] Business account aktifleşti
- [ ] Subscription kaydedildi

## 📞 İletişim

Sorun devam ederse:
- IIS loglarını paylaş
- Backend loglarını paylaş
- PayTR işlem detaylarını paylaş

---

**Hazırlayan:** E1 AI Agent  
**Tarih:** 24.11.2025
