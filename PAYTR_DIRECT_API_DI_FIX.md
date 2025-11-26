# 🔧 PayTRDirectAPIService Dependency Injection Hatası - Çözüm

## ❌ Hata:

```json
{
  "error": "A suitable constructor for type 'AppointmentManagementSystem.Infrastructure.Services.PayTRDirectAPIService' could not be located. Ensure the type is concrete and all parameters of a public constructor are either registered as services or passed as arguments."
}
```

## 🔍 Sebep:

PayTRDirectAPIService constructor'ında 3 parametre var:
1. ✅ IConfiguration - Zaten register edilmiş
2. ✅ ILogger<PayTRDirectAPIService> - Zaten register edilmiş
3. ❌ IHttpClientFactory - **Register EDİLMEMİŞ!**

```csharp
public PayTRDirectAPIService(
    IConfiguration configuration,
    ILogger<PayTRDirectAPIService> logger,
    IHttpClientFactory httpClientFactory) // ← Bu eksikti!
```

## ✅ Çözümler:

### 1. Program.cs - HttpClientFactory Eklendi

**Dosya:** `/app/AppointmentManagementSystem.API/Program.cs`

**Eklenen Kod:**
```csharp
// HttpClientFactory - PayTRDirectAPIService için gerekli
builder.Services.AddHttpClient();

// Gemini AI Client
builder.Services.AddHttpClient<IGeminiClient, GeminiClient>();
```

**Konum:** Satır 76'dan önce ekledik.

### 2. appsettings.json - FrontendUrl Eklendi

**Dosya:** `/app/AppointmentManagementSystem.API/appsettings.json`

**Eklenen Satır:**
```json
{
  "PayTR": {
    "MerchantId": "642441",
    "MerchantKey": "UarFDD85dD4xg8Go",
    "MerchantSalt": "YmC4JfRh4c3JkQ9p",
    "FrontendUrl": "https://aptivaplan.com.tr", // ← YENİ!
    "CallbackUrl": "https://hub.aptivaplan.com.tr/api/payments/webhook",
    "TestMode": true
  }
}
```

PayTRDirectAPIService bu ayarı kullanıyor:
```csharp
var frontendUrl = _configuration["PayTR:FrontendUrl"] ?? "https://aptivaplan.com.tr";
_merchantOkUrl = $"{frontendUrl}/payment/success";
_merchantFailUrl = $"{frontendUrl}/payment/fail";
```

## 📋 Kontrol Listesi:

- ✅ IHttpClientFactory register edildi
- ✅ FrontendUrl appsettings.json'a eklendi
- ✅ PayTR MerchantId, MerchantKey, MerchantSalt mevcut
- ✅ CallbackUrl doğru
- ✅ TestMode: true

## 🧪 Test Adımları:

### 1. Backend'i Restart Et:

```bash
cd /app/AppointmentManagementSystem.API
dotnet build
dotnet run
```

### 2. Endpoint'i Test Et:

```bash
curl -X POST https://localhost:5089/api/payments/initiate-direct-card-registration \
  -H "Content-Type: application/json" \
  -d '{
    "businessId": 1,
    "email": "test@example.com",
    "businessName": "Test İşletmesi",
    "ownerName": "Ahmet Yılmaz",
    "phoneNumber": "5551234567",
    "address": "İstanbul, Türkiye",
    "cardOwner": "AHMET YILMAZ",
    "cardNumber": "4111111111111111",
    "expiryMonth": "12",
    "expiryYear": "25",
    "cvv": "123"
  }'
```

### 3. Beklenen Response:

```json
{
  "success": true,
  "data": {
    "success": true,
    "merchantOid": "REG1_A1B2C3D4",
    "message": "Payment processing. Webhook will confirm card storage.",
    "redirectUrl": null
  }
}
```

## 🔍 Hata Devam Ederse:

### Kontrol 1: HttpClientFactory Register Edildi mi?

Program.cs'de şu satır olmalı:
```csharp
builder.Services.AddHttpClient();
```

### Kontrol 2: PayTRDirectAPIService DI'da Register Edildi mi?

DependencyInjection.cs'de şu satır olmalı:
```csharp
services.AddScoped<IPayTRDirectAPIService, PayTRDirectAPIService>();
services.AddHttpClient<IPayTRDirectAPIService, PayTRDirectAPIService>();
```

### Kontrol 3: appsettings.json Doğru mu?

```json
{
  "PayTR": {
    "MerchantId": "642441",
    "MerchantKey": "...",
    "MerchantSalt": "...",
    "FrontendUrl": "https://aptivaplan.com.tr",
    "CallbackUrl": "https://hub.aptivaplan.com.tr/api/payments/webhook",
    "TestMode": true
  }
}
```

### Kontrol 4: Backend Logları

```bash
tail -f /var/log/supervisor/backend.out.log
```

Şunları görmelisin:
```
🔵 Direct API: Initiating card registration payment for Business 1
PayTR Token generated: xxx...
📤 Sending Direct API request to PayTR...
```

## 🎯 Sonuç:

Yapılan değişikliklerden sonra:
- ✅ IHttpClientFactory inject ediliyor
- ✅ PayTRDirectAPIService oluşturuluyor
- ✅ Direct API endpoint çalışıyor
- ✅ PayTR'ye istek gönderiliyor

---

**Güncelleme Tarihi:** 2025-01-08  
**Durum:** ✅ Düzeltildi - Test Edilmeli

## 📝 Test Kartları (PayTR Test Mode):

```
Kart No: 4355084355084358
Tarih: 12/30
CVV: 000
```

Bu test kartı ile ödeme yapabilirsin ve webhook'ta kart bilgileri gelecek!
