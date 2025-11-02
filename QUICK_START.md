# PayTR Entegrasyonu - Hızlı Kurulum Kılavuzu

## ⚡ Hızlı Başlangıç (5 Dakika)

### 1️⃣ NuGet Paketlerini Yükle

```bash
# Application Layer
cd AppointmentManagementSystem.Application
dotnet add package Microsoft.Extensions.Configuration.Abstractions --version 8.0.0

# Infrastructure Layer
cd ../AppointmentManagementSystem.Infrastructure
dotnet add package Microsoft.Extensions.Hosting.Abstractions --version 8.0.0
dotnet add package Microsoft.Extensions.Http --version 8.0.0
dotnet add package Microsoft.Extensions.Configuration.Binder --version 8.0.0

# Root'a dön
cd ..
dotnet restore
```

### 2️⃣ Build Et

```bash
dotnet build
```

### 3️⃣ Database Migration

**SQL Server Management Studio'da:**
- `/app/PayTR_Migration.sql` dosyasını aç ve çalıştır

**VEYA Package Manager Console:**
```powershell
Update-Database
```

### 4️⃣ PayTR Ayarları

`appsettings.json` dosyasını güncelle:

```json
{
  "PayTR": {
    "MerchantId": "PAYTR_SANDBOX_MERCHANT_ID",
    "MerchantKey": "PAYTR_SANDBOX_KEY",
    "MerchantSalt": "PAYTR_SANDBOX_SALT",
    "CallbackUrl": "https://YOUR-DOMAIN/api/payments/webhook",
    "TestMode": true
  }
}
```

### 5️⃣ Çalıştır

```bash
# API
cd AppointmentManagementSystem.API
dotnet run

# Blazor (başka terminalde)
cd AppointmentManagementSystem.BlazorUI
dotnet run
```

---

## 🧪 Test Et

### Register Flow
1. Tarayıcıda: `https://localhost:5001/register`
2. **İşletme** rolü seç
3. Tüm bilgileri doldur (işletme, hizmet, çalışan)
4. Özet sayfasında **"Kayıt Ol ve Devam Et"**
5. PayTR ödeme ekranı gelecek

### Test Kart Bilgileri
```
Kart Numarası: 4111 1111 1111 1111
Son Kullanma: 12/25
CVV: 123
```

---

## 📦 Eklenen Paketler

| Proje | Paket | Versiyon | Amaç |
|-------|-------|----------|------|
| Application | Microsoft.Extensions.Configuration.Abstractions | 8.0.0 | IConfiguration support |
| Infrastructure | Microsoft.Extensions.Hosting.Abstractions | 8.0.0 | IHostedService support |
| Infrastructure | Microsoft.Extensions.Http | 8.0.0 | HttpClient factory |
| Infrastructure | Microsoft.Extensions.Configuration.Binder | 8.0.0 | Configuration.GetValue() |

---

## 🔍 Sorun Giderme

### Build Hatası: CS1061 GetValue
```bash
cd AppointmentManagementSystem.Infrastructure
dotnet add package Microsoft.Extensions.Configuration.Binder --version 8.0.0
```

### Build Hatası: CS1061 AddHttpClient
```bash
cd AppointmentManagementSystem.Infrastructure
dotnet add package Microsoft.Extensions.Http --version 8.0.0
```

### Migration Hatası
```bash
# Migration'ı sil ve yeniden oluştur
dotnet ef migrations remove
dotnet ef migrations add AddPaymentAndSubscription
dotnet ef database update
```

### PayTR Credentials
1. https://www.paytr.com → Kayıt ol
2. Sandbox hesabı oluştur
3. API Keys kopyala
4. appsettings.json'a ekle

---

## ✅ Doğrulama

### Database Kontrol
```sql
-- Tablolar oluştu mu?
SELECT * FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('BusinessSubscriptions', 'Payments')

-- Business pasif mi?
SELECT Id, Name, IsActive FROM Businesses WHERE IsActive = 0

-- Subscription kayıtları
SELECT * FROM BusinessSubscriptions

-- Payment kayıtları
SELECT * FROM Payments
```

### API Endpoint Kontrol
```bash
# Swagger UI
https://localhost:5001/swagger

# Endpoints:
POST /api/payments/initiate-card-registration
POST /api/payments/webhook
GET /api/payments/subscription/{businessId}
GET /api/payments/history/{businessId}
```

---

## 📊 Özellikler

✅ PayTR kart tokenization (güvenli kart saklama)  
✅ Aylık 700 TL otomatik tahsilat  
✅ İlk 30 gün ücretsiz  
✅ Ödeme başarısız → Business pasif  
✅ 5 retry attempt (exponential backoff)  
✅ Webhook idempotency koruması  
✅ HMAC-SHA256 imza doğrulama  
✅ IHostedService ile otomatik ödeme servisi  

---

## 📞 Yardım

**Detaylı Döküman:** `/app/PAYTR_SETUP.md`  
**Migration SQL:** `/app/PayTR_Migration.sql`  
**PayTR Docs:** https://dev.paytr.com  

---

**Son Güncelleme:** 2025-01-08  
**Durum:** ✅ Hazır
