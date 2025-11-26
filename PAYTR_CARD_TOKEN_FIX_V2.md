# PayTR Kart Tokenization Sorunu - Çözüm

## 🔴 Sorun

PayTR'den webhook'ta kart bilgileri (`utoken`, `ctoken`, `card_type`, `masked_pan`) **NULL** geliyordu ve BusinessSubscriptions tablosuna kaydedilmiyordu.

## ✅ Çözüm

PayTR kart tokenization için **`store_card=1` parametresine ek olarak `utoken` parametresini de göndermemiz gerekiyor.**

### Yapılan Değişiklik

**Dosya:** `/app/AppointmentManagementSystem.Infrastructure/Services/PayTRService.cs`

**Eklenen Kod:**

```csharp
// utoken oluştur - PayTR kart tokenization için gerekli
// Format: USER_{BusinessId/Email}_{UniqueId}
var utokenUnique = Guid.NewGuid().ToString("N").Substring(0, 16).ToUpperInvariant();
var utoken = $"USER_{merchantOid}_{utokenUnique}";
```

**FormData'ya Eklenen:**

```csharp
{ "store_card", "1" }, // Kart bilgilerini kaydet
{ "utoken", utoken },  // ZORUNLU: PayTR'ye göndereceğimiz unique user token
```

## 📋 Nasıl Çalışır?

### 1. Ödeme Başlatılırken:
- Uygulama unique bir `utoken` generate eder
- Bu token'ı PayTR'ye `store_card=1` ile birlikte gönderir
- Format: `USER_REG6A1B2C3D_F8E7D6C5B4A3`

### 2. PayTR İşlemi Tamamladığında:
- PayTR webhook'ta bize şu bilgileri döner:
  - `utoken`: Bizim gönderdiğimiz token (USER_REG6...)
  - `ctoken`: PayTR'nin oluşturduğu kart token'ı (unique)
  - `card_type`: Visa, Mastercard, vb.
  - `masked_pan`: **** **** **** 1234

### 3. Webhook Handler:
- Bu bilgiler `ProcessPaymentWebhookHandler` tarafından işlenir
- `BusinessSubscription` tablosuna kaydedilir:
  ```sql
  UPDATE BusinessSubscriptions SET
    PayTRUserToken = 'USER_REG6A1B2C3D_F8E7D6C5B4A3',
    PayTRCardToken = 'ctoken_from_paytr',
    CardType = 'Visa',
    MaskedCardNumber = '**** **** **** 1234',
    CardLastFourDigits = '1234'
  WHERE BusinessId = 6
  ```

## 🧪 Test Etme

### 1. Backend'i Restart Edin

```bash
cd /app/AppointmentManagementSystem.API
dotnet build
dotnet run
```

### 2. Yeni Bir Kayıt Yapın

1. Blazor UI'da `/register` sayfasına gidin
2. İşletme bilgilerini doldurun
3. Ödeme sayfasına ilerleyin
4. Test kartı kullanın:
   ```
   Kart No: 4111 1111 1111 1111
   CVV: 123
   Tarih: 12/25
   ```

### 3. Logları Kontrol Edin

Backend loglarında şunu görmelisiniz:

```
PayTR Request - UToken: USER_REG6A1B2C3D_F8E7D6C5B4A3, StoreCard: 1
...
Webhook Received - Utoken: USER_REG6A1B2C3D_F8E7D6C5B4A3
Webhook Received - Ctoken: xxxxxxxxxxxxx
Webhook Received - CardType: Visa
Webhook Received - MaskedPan: **** **** **** 1234
```

### 4. Veritabanını Kontrol Edin

```sql
SELECT 
    BusinessId,
    PayTRUserToken,
    PayTRCardToken,
    CardType,
    MaskedCardNumber,
    CardLastFourDigits
FROM BusinessSubscriptions
WHERE BusinessId = [YourBusinessId]
```

**Beklenen Sonuç:**
- `PayTRUserToken`: Dolu (USER_... formatında)
- `PayTRCardToken`: Dolu
- `CardType`: Visa/Mastercard/vb.
- `MaskedCardNumber`: **** **** **** 1234
- `CardLastFourDigits`: 1234

## 🔍 Sorun Giderme

### Hala NULL Geliyorsa:

**1. PayTR Test Mode Kontrolü:**
```json
// appsettings.json
{
  "PayTR": {
    "TestMode": true  // Test modda olmalı
  }
}
```

**2. Merchant Panel Ayarları:**
- PayTR Merchant Panel → Ayarlar
- "Kart Saklama" özelliği AÇIK olmalı
- Webhook URL doğru set edilmeli

**3. Log Kontrolü:**
```bash
# Backend logları
cd /app/AppointmentManagementSystem.API
dotnet run

# Webhook geldiğinde console'da:
# "Utoken: USER_..." görünmeli
```

**4. PayTR Sandbox Hesabı:**
- Gerçek PayTR hesabı olmalı (test merchant değil)
- Card tokenization özelliği aktif olmalı

## 📊 İşlem Akışı

```
1. User → Register sayfası
2. Form doldurulur
3. "Kayıt Ol" tıklanır
   ↓
4. Backend: InitiateCardRegistrationCommand
   - utoken generate edilir: "USER_REG6_ABC123"
   - PayTR'ye gönderilir: store_card=1 & utoken=USER_REG6_ABC123
   ↓
5. PayTR: Ödeme sayfası gösterir
6. User: Kart bilgilerini girer
   ↓
7. PayTR: İşlemi tamamlar
   - utoken: USER_REG6_ABC123 (bizim gönderdiğimiz)
   - ctoken: XYZ789ABC (PayTR'nin generate ettiği)
   - card_type: Visa
   - masked_pan: **** **** **** 1234
   ↓
8. PayTR → Webhook: /api/payments/webhook
   ↓
9. Backend: ProcessPaymentWebhookHandler
   - Kart bilgileri BusinessSubscription'a kaydedilir
   - Business aktif edilir
   ↓
10. ✅ BAŞARILI: Kart bilgileri veritabanında!
```

## 🎯 Sonuç

Bu düzeltme ile:
- ✅ Kart bilgileri webhook'ta gelecek
- ✅ BusinessSubscriptions tablosuna kaydedilecek
- ✅ Recurring payment'lar bu kart bilgileri ile çalışacak
- ✅ MonthlyBillingService otomatik ödeme yapabilecek

## 📝 Notlar

- **ÖNEMLI:** Bu değişiklik sadece YENİ kayıtlarda çalışır
- Mevcut kayıtlar için kart güncelleme (CARD prefix) kullanılmalı
- Test modda çalıştığınızdan emin olun
- Production'a geçmeden önce mutlaka test edin!

---

**Güncelleme Tarihi:** 2025-01-08
**Durum:** ✅ Uygulandı ve Test Edilmeli
