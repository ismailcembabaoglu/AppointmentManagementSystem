# PayTR Abonelik Sistemi - Kurulum Talimatları

## 🔧 Gerekli Paketleri Yükleme

Projenin derlenebilmesi için aşağıdaki NuGet paketlerini yüklemeniz gerekmektedir:

### 1. Application Layer Paketleri

```bash
cd AppointmentManagementSystem.Application
dotnet add package Microsoft.Extensions.Configuration.Abstractions --version 8.0.0
```

### 2. Infrastructure Layer Paketleri

```bash
cd ../AppointmentManagementSystem.Infrastructure
dotnet add package Microsoft.Extensions.Hosting.Abstractions --version 8.0.0
dotnet add package Microsoft.Extensions.Http --version 8.0.0
dotnet add package Microsoft.Extensions.Configuration.Binder --version 8.0.0
```

### 3. Tüm Paketleri Restore Etme

```bash
cd ..
dotnet restore
```

## 📦 Eklenen Dosyalar

### Domain Layer
- `Payment.cs` - Ödeme entity
- `BusinessSubscription.cs` - Abonelik entity
- `IPaymentRepository.cs` - Payment repository interface
- `IBusinessSubscriptionRepository.cs` - Subscription repository interface
- `IPayTRService.cs` - PayTR service interface

### Infrastructure Layer
- `PaymentRepository.cs` - Payment repository implementation
- `BusinessSubscriptionRepository.cs` - Subscription repository implementation
- `PayTRService.cs` - PayTR API entegrasyonu
- `MonthlyBillingService.cs` - IHostedService (otomatik ödeme)
- Migration: `20250108_AddPaymentAndSubscription.cs`

### Application Layer
- `PaymentDtos.cs` - Payment DTOs
- `Result.cs` - Result helper class
- `InitiateCardRegistrationCommand.cs`
- `ProcessPaymentWebhookCommand.cs`
- `PaymentQueries.cs`
- Command & Query Handlers

### API Layer
- `PaymentsController.cs` - Payment endpoints

### Blazor UI
- `IPaymentApiService.cs` & `PaymentApiService.cs`
- `PayTRIFrameComponent.razor` - PayTR iframe component
- `Register.razor` - Güncellenmiş (5 adımlı kayıt)

## 🗄️ Database Migration

```bash
# Migration oluştur (dotnet-ef kurulu değilse)
dotnet tool install --global dotnet-ef

# Database'i güncelle
cd AppointmentManagementSystem.API
dotnet ef database update --project ../AppointmentManagementSystem.Infrastructure

# Veya SQL Server Management Studio'da manuel olarak:
# Migration dosyasını (20250108_AddPaymentAndSubscription.cs) Up() metodunu çalıştır
```

## ⚙️ PayTR Konfigürasyonu

### appsettings.json

```json
{
  "PayTR": {
    "MerchantId": "PAYTR_SANDBOX_MERCHANT_ID",
    "MerchantKey": "PAYTR_SANDBOX_KEY", 
    "MerchantSalt": "PAYTR_SANDBOX_SALT",
    "ApiUrl": "https://www.paytr.com/odeme",
    "StatusUrl": "https://www.paytr.com/odeme/durum-sorgu",
    "CallbackUrl": "https://YOUR-DOMAIN/api/payments/webhook",
    "TestMode": true
  }
}
```

### Sandbox Credentials Alma

1. [PayTR Merchant Panel](https://www.paytr.com) - Kayıt ol
2. Sandbox hesabı oluştur
3. Merchant ID, Key ve Salt değerlerini al
4. appsettings.json'a ekle

### Webhook URL Ayarlama

PayTR Merchant Panel → Ayarlar → Webhook URL:
```
https://YOUR-DOMAIN/api/payments/webhook
```

## 🚀 Uygulamayı Çalıştırma

```bash
# Backend (API)
cd AppointmentManagementSystem.API
dotnet run

# Frontend (Blazor)
cd ../AppointmentManagementSystem.BlazorUI
dotnet run
```

## 🧪 Test Etme

### 1. Business Kaydı
1. `/register` sayfasına git
2. "İşletme" rolünü seç
3. Tüm adımları doldur
4. Özet sayfasında "Kayıt Ol ve Devam Et"
5. Ödeme ekranında test kartı kullan

### 2. Test Kart Bilgileri (Sandbox)

```
Kart Numarası: 4111 1111 1111 1111
Son Kullanma: Gelecekteki herhangi bir tarih (örn: 12/25)
CVV: 123
```

### 3. Webhook Test

Webhook'u test etmek için Postman veya curl:

```bash
curl -X POST https://localhost:5001/api/payments/webhook \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "merchant_oid=REG-1-abc123" \
  -d "status=success" \
  -d "total_amount=0" \
  -d "hash=CALCULATED_HASH" \
  -d "utoken=test_user_token" \
  -d "ctoken=test_card_token" \
  -d "card_type=Visa" \
  -d "masked_pan=411111******1111"
```

## 📊 Database Tabloları

Eklenen tablolar:
- `Payments` - Ödeme kayıtları
- `BusinessSubscriptions` - Abonelik bilgileri

Güncellenen tablolar:
- `Businesses` - `IsActive` default false

## 🔄 Otomatik Ödeme Akışı

1. **MonthlyBillingService** (IHostedService) otomatik çalışır
2. Her gün saat 02:00'da NextBillingDate kontrolü
3. Süre dolan aboneliklere ödeme çekimi
4. Başarısız ödemeler 6 saatte bir yeniden denenir
5. 5 deneme sonrası başarısız olursa Business pasif olur

## 📝 Önemli Notlar

- ✅ İlk 30 gün ücretsiz
- ✅ Kart bilgileri PayTR'de tokenize ediliyor
- ✅ Webhook imza doğrulama (HMAC-SHA256)
- ✅ Idempotency koruması
- ✅ Exponential backoff retry stratejisi
- ⚠️ Production'da HTTPS zorunlu
- ⚠️ CallbackUrl production domain'e güncellenmeli

## 🐛 Sorun Giderme

### Derleme Hataları
```bash
# Tüm paketleri temizle ve yeniden yükle
dotnet clean
dotnet restore
dotnet build
```

### Migration Hataları
```bash
# Migration'ı sil ve yeniden oluştur
dotnet ef migrations remove --project AppointmentManagementSystem.Infrastructure
dotnet ef migrations add AddPaymentAndSubscription --project AppointmentManagementSystem.Infrastructure
dotnet ef database update --project AppointmentManagementSystem.Infrastructure
```

### PayTR Webhook Çalışmıyor
1. CallbackUrl doğru mu?
2. HTTPS kullanılıyor mu?
3. Hash doğru hesaplanıyor mu?
4. Merchant credentials doğru mu?

## 📞 Destek

Herhangi bir sorun için:
- PayTR Dokümantasyonu: https://dev.paytr.com
- Proje README: Bu dosya

---

**Hazırlayan:** E1 AI Agent  
**Tarih:** 2025  
**Versiyon:** 1.0
