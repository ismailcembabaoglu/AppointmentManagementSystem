# PayTR Webhook HTTP 405 Hatası - Çözüm Kılavuzu

## 🔴 Sorun
PayTR ödeme sonrası webhook'a bildirim gönderirken **HTTP 405 - Method Not Allowed** hatası alıyordu.
Bu nedenle PayTR ödemeyi "Başarısız" olarak işaretliyordu.

## ✅ Yapılan Düzeltmeler

### 1. PaymentsController.cs
- Webhook endpoint'i sadece **POST** metodunu kabul edecek şekilde güncellendi
- Gereksiz HTTP metodları (GET, OPTIONS, HEAD) kaldırıldı
- PayTR'ye her durumda "OK" yanıtı döndürülüyor (hata durumunda bile)
- Detaylı logging eklendi

### 2. ProcessPaymentWebhookHandler.cs
- Hash doğrulama bypass'ı kaldırıldı
- Artık test modunda bile hash doğrulaması yapılıyor
- Geçersiz hash durumunda işlem reddediliyor

### 3. PaymentSuccess.razor
- Frontend'den manuel webhook tetikleme kodu tamamen kaldırıldı
- Artık sadece PayTR'nin gerçek webhook'u işleniyor

### 4. web.config (YENİ)
- IIS için POST metoduna açıkça izin verildi
- CORS ayarları yapıldı
- AspNetCore modülü yapılandırıldı

## 🚀 IIS Deployment Adımları

### 1. web.config Dosyasını Deploy Edin
```
AppointmentManagementSystem.API/web.config dosyası
IIS'te publish edilen klasöre kopyalanmalı
```

### 2. IIS Application Pool Ayarları
```
1. IIS Manager'ı açın
2. Application Pools → [Sizin Pool İsminiz]
3. .NET CLR Version: "No Managed Code" seçin
4. Managed Pipeline Mode: "Integrated"
5. Identity: ApplicationPoolIdentity veya özel bir hesap
```

### 3. IIS Site Bindings
```
1. Sites → [Siteniz] → Bindings
2. HTTPS binding ekleyin (Port 443)
3. SSL sertifikası seçin
4. HTTP to HTTPS redirect aktif olmalı
```

### 4. Handler Mappings Kontrolü
```
1. Sites → [Siteniz] → Handler Mappings
2. "aspNetCore" handler'ın olduğundan emin olun
3. Yoksa, web.config'den otomatik eklenecektir
```

### 5. Request Filtering
```
1. Sites → [Siteniz] → Request Filtering
2. HTTP Verbs sekmesinde "POST" allowed olmalı
3. Eğer blocked ise, remove edin
```

## 🔧 PayTR Merchant Panel Ayarları

### Webhook URL Ayarlama
```
1. PayTR Merchant Panel → Ayarlar
2. Bildirim URL'si: https://hub.sellerdoping.com.tr/api/payments/webhook
3. Kaydet
```

**ÖNEMLİ:** 
- Frontend: `https://aptivaplan.com.tr` (Kullanıcı arayüzü)
- Backend API: `https://hub.sellerdoping.com.tr/api` (PayTR webhook'u buraya gelmeli)

### Test Etme
PayTR'de test ödeme yapın ve aşağıdaki logları kontrol edin:

**IIS Logs (hub.sellerdoping.com.tr):** `C:\inetpub\logs\LogFiles\W3SVC1\`
```
Başarılı webhook:
POST /api/payments/webhook - 200 0 0
```

**URL Yapısı:**
- Frontend: https://aptivaplan.com.tr
- Backend: https://hub.sellerdoping.com.tr/api
- Webhook: https://hub.sellerdoping.com.tr/api/payments/webhook

**Application Logs:** Backend loglarında göreceksiniz:
```
=== PayTR Webhook Received ===
Content-Type: application/x-www-form-urlencoded
Method: POST
MerchantOid: CARD1AFF4467A
Status: success
...
Webhook response: OK
```

## 📊 PayTR Panel - Bildirim Durumu

Başarılı webhook sonrası PayTR panelinde şunları görmelisiniz:
```
Bildirim Durumu: Başarılı ✅
Son Deneme: [tarih]
HTTP Yanıt Kodu: 200
Yanıt İçeriği: OK
```

## ⚠️ Önemli Notlar

1. **HTTPS Zorunlu**: PayTR webhook'ları sadece HTTPS adreslerine gönderir
2. **Firewall**: Sunucunuzun firewall'u PayTR IP'lerinden gelen isteklere açık olmalı
3. **Authentication**: Webhook endpoint'i [AllowAnonymous] olmalı (zaten öyle)
4. **Response**: Her durumda "OK" text döndürülmeli
5. **Hash Validation**: PayTR'nin gönderdiği hash mutlaka doğrulanmalı

## 🐛 Hata Ayıklama

### HTTP 405 Hatası Hala Alıyorsanız

1. **web.config kontrol:**
```bash
# web.config dosyasının publish klasöründe olduğundan emin olun
ls /path/to/published/app/web.config
```

2. **IIS Handler Mappings:**
```
IIS Manager → Handler Mappings → aspNetCore var mı?
```

3. **Application Pool:**
```
.NET CLR Version = "No Managed Code" olmalı
```

4. **Request Filtering:**
```
POST metodu blocked değil mi?
```

### Hash Validation Hatası

Eğer "Invalid webhook signature" alıyorsanız:

1. **appsettings.json kontrolü:**
```json
{
  "PayTR": {
    "MerchantKey": "DOĞRU_KEY",
    "MerchantSalt": "DOĞRU_SALT"
  }
}
```

2. **PayTR'den gelen verileri logla:**
```
Backend loglarında şunları görmelisiniz:
Expected Hash (Base64): [hash]
Received Hash: [hash]
İkisi aynı olmalı!
```

3. **Test etme:**
```bash
# PowerShell ile test
$body = @{
    merchant_oid = "TEST123"
    status = "success"
    total_amount = "100"
    hash = "CALCULATED_HASH"
}
Invoke-WebRequest -Uri "https://aptivaplan.com.tr/api/payments/webhook" -Method POST -Body $body
```

## 📞 Destek

Sorun devam ederse:
1. IIS loglarını kontrol edin
2. Application loglarını kontrol edin
3. PayTR teknik desteğe başvurun: destek@paytr.com

---

**Düzeltme Tarihi:** 24.11.2025  
**Düzelten:** E1 AI Agent
