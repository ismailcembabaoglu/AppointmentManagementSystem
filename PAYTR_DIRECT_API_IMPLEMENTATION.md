# ✅ PayTR Direct API Entegrasyonu Tamamlandı!

## 🎯 Yapılan Değişiklikler

### 1. Yeni Servisler Oluşturuldu

**`IPayTRDirectAPIService`** - Direct API interface
- ✅ `InitiateCardRegistrationPayment()` - İlk kayıt + kart saklama
- ✅ `ChargeStoredCard()` - Kayıtlı karttan ödeme (recurring)
- ✅ `GetStoredCards()` - Kullanıcının kayıtlı kartlarını listele
- ✅ `DeleteStoredCard()` - Kayıtlı kartı sil

**`PayTRDirectAPIService`** - Direct API implementation
- POST: `https://www.paytr.com/odeme` - Ödeme başlatma
- POST: `https://www.paytr.com/odeme/capi/list` - Kart listesi
- POST: `https://www.paytr.com/odeme/capi/delete` - Kart silme

### 2. Yeni Command & Handler

**`InitiateDirectAPICardRegistrationCommand`**
- Kart bilgilerini (CardNumber, CVV, vb.) alır
- Direct API ile PayTR'ye gönderir
- Webhook'ta utoken/ctoken gelir

**`InitiateDirectAPICardRegistrationHandler`**
- İşletme kontrolü yapar
- Mevcut utoken varsa (ikinci kart için) kullanır
- PayTR Direct API'ye istek gönderir
- Webhook'u bekler

### 3. Yeni Endpoint

```http
POST /api/payments/initiate-direct-card-registration
```

**Request Body:**
```json
{
  "businessId": 6,
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
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "success": true,
    "merchantOid": "REG6_A1B2C3D4",
    "message": "Payment processing. Webhook will confirm card storage.",
    "redirectUrl": null
  }
}
```

### 4. DTOs Eklendi

- `PayTRDirectPaymentResponse`
- `PayTRCardListResponse`
- `PayTRStoredCard`

### 5. DependencyInjection Güncellendi

```csharp
services.AddScoped<IPayTRDirectAPIService, PayTRDirectAPIService>();
services.AddHttpClient<IPayTRDirectAPIService, PayTRDirectAPIService>();
```

---

## 🔄 İşlem Akışı

### İlk Kayıt (Yeni Kart Ekleme)

```
1. Frontend → POST /api/payments/initiate-direct-card-registration
   - Kart bilgileri gönderilir
   
2. Backend → PayTR Direct API
   - merchant_id, card_number, cvv, etc.
   - store_card=1 (kartı kaydet)
   - utoken="" (yeni kullanıcı için)
   
3. PayTR → İşlem yapar
   - Ödeme başarılı
   - utoken oluşturur (USER_REG6_xxx)
   - ctoken oluşturur (kart token)
   
4. PayTR → Webhook'a bildirim gönderir
   POST /api/payments/webhook
   - merchant_oid=REG6_A1B2C3D4
   - utoken=USER_REG6_A1B2C3D4_F8E7D6C5
   - ctoken=abc123def456
   - card_type=Visa
   - masked_pan=**** **** **** 1234
   
5. Backend → ProcessPaymentWebhookHandler
   - utoken ve ctoken'ı BusinessSubscription'a kaydeder
   - AutoRenewal = true (artık recurring çalışır)
   - Business aktif edilir
   
6. ✅ BAŞARILI: Kart kayıtlı, recurring payment hazır!
```

### İkinci Kart Ekleme (Aynı Kullanıcı)

```
1. Frontend → POST /api/payments/initiate-direct-card-registration
   - Yeni kart bilgileri
   
2. Backend → Mevcut utoken'ı bulur
   - BusinessSubscription'dan PayTRUserToken alır
   
3. Backend → PayTR Direct API
   - utoken=USER_REG6_xxx (MEVCUT)
   - store_card=1
   - Yeni kart bilgileri
   
4. PayTR → İşlem yapar
   - AYNI utoken altında yeni ctoken oluşturur
   - Kartlar gruplanır
   
5. Webhook → Yeni ctoken kaydedilir
```

### Recurring Payment (Aylık Otomatik Ödeme)

```
1. MonthlyBillingService (Background Service)
   - Her gün çalışır
   - NextBillingDate kontrolü
   
2. Ödeme zamanı geldi
   - utoken ve ctoken bulunur
   
3. PayTRDirectAPIService.ChargeStoredCard()
   - Kayıtlı karttan ödeme
   - CVV gerekmez (recurring için)
   
4. PayTR → Ödeme yapar
   
5. Webhook → Sonuç bildirimi
   - Başarılı: NextBillingDate +30 gün
   - Başarısız: Retry mekanizması
```

---

## 🧪 Test Adımları

### 1. Backend'i Başlat

```bash
cd /app/AppointmentManagementSystem.API
dotnet build
dotnet run
```

### 2. Swagger'da Test Et

1. Swagger UI'ya git: `http://localhost:5089/swagger`
2. `/api/payments/initiate-direct-card-registration` endpoint'ini bul
3. "Try it out" tıkla
4. Request body'yi doldur:

```json
{
  "businessId": 6,
  "email": "test@example.com",
  "businessName": "Test Kuaförü",
  "ownerName": "Ahmet Yılmaz",
  "phoneNumber": "5551234567",
  "address": "İstanbul Kadıköy",
  "cardOwner": "AHMET YILMAZ",
  "cardNumber": "4111111111111111",
  "expiryMonth": "12",
  "expiryYear": "25",
  "cvv": "123"
}
```

5. "Execute" tıkla

### 3. Logları Kontrol Et

Console'da şunu görmelisiniz:

```
=== Direct API Card Registration Started ===
BusinessId: 6, Email: test@example.com
🔵 Direct API: Initiating card registration payment for Business 6
PayTR Token generated: xxx...
📤 Sending Direct API request to PayTR...
Store Card: 1, Non-3D: 1
📥 PayTR Response: ...
✅ Direct API payment initiated successfully
MerchantOid: REG6_A1B2C3D4
⏳ Waiting for webhook callback with card tokens (utoken/ctoken)...
```

### 4. Webhook Geldiğinde

```
=== PayTR Webhook Received ===
MerchantOid: REG6_A1B2C3D4
Status: success
Utoken: USER_REG6_A1B2C3D4_F8E7D6C5 ✅
Ctoken: abc123def456 ✅
CardType: Visa ✅
MaskedPan: **** **** **** 1234 ✅
✅ Subscription created with card tokens
```

### 5. Veritabanını Kontrol Et

```sql
SELECT 
    BusinessId,
    PayTRUserToken,
    PayTRCardToken,
    CardType,
    MaskedCardNumber,
    AutoRenewal,
    NextBillingDate
FROM BusinessSubscriptions
WHERE BusinessId = 6
```

**Beklenen Sonuç:**
- ✅ PayTRUserToken: `USER_REG6_A1B2C3D4_F8E7D6C5`
- ✅ PayTRCardToken: `abc123def456`
- ✅ CardType: `Visa`
- ✅ MaskedCardNumber: `**** **** **** 1234`
- ✅ AutoRenewal: `true`
- ✅ NextBillingDate: `2025-02-08` (30 gün sonra)

---

## 📝 Blazor UI Güncellemeleri (Yapılacak)

### Register.razor

**Eski:**
```csharp
// iFrame API kullanılıyor
var response = await Http.PostAsJsonAsync("api/payments/initiate-card-registration", ...);
```

**Yeni:**
```csharp
// Direct API kullan
var request = new
{
    businessId = newBusiness.Id,
    email = businessData.Email,
    businessName = businessData.Name,
    ownerName = businessData.OwnerName,
    phoneNumber = businessData.PhoneNumber,
    address = businessData.Address,
    cardOwner = cardData.CardOwner,
    cardNumber = cardData.CardNumber,
    expiryMonth = cardData.ExpiryMonth,
    expiryYear = cardData.ExpiryYear,
    cvv = cardData.CVV
};

var response = await Http.PostAsJsonAsync("api/payments/initiate-direct-card-registration", request);
```

### KartYonetimi.razor (Yeni Sayfa)

```razor
@page "/business/card-management"

<h3>Kayıtlı Kartlarım</h3>

@if (cards != null && cards.Any())
{
    @foreach (var card in cards)
    {
        <div class="card">
            <h4>@card.CardBrand @card.MaskedPan</h4>
            <p>Son Kullanma: @card.ExpiryMonth/@card.ExpiryYear</p>
            <button @onclick="() => DeleteCard(card.Ctoken)">Sil</button>
        </div>
    }
}

<button @onclick="AddNewCard">Yeni Kart Ekle</button>

@code {
    private List<PayTRStoredCard>? cards;
    
    protected override async Task OnInitializedAsync()
    {
        // Kayıtlı kartları getir
        // GET /api/payments/stored-cards
    }
    
    private async Task AddNewCard()
    {
        // Yeni kart ekleme formu göster
        // POST /api/payments/initiate-direct-card-registration
        // (mevcut utoken ile)
    }
}
```

---

## ⚠️ Önemli Notlar

### 1. Test Mode

```json
// appsettings.json
{
  "PayTR": {
    "TestMode": true  // Test mode aktif
  }
}
```

### 2. Merchant Panel Ayarları

PayTR Merchant Panel'de:
- ✅ **Direkt API** aktif olmalı
- ✅ **Kart Saklama** özelliği açık olmalı
- ✅ **Webhook URL** doğru: `https://hub.aptivaplan.com.tr/api/payments/webhook`

### 3. Non-3D İşlem

- İlk kayıtta **Non-3D** kullanıyoruz (non_3d=1)
- 3D Secure yok, direkt işlem
- Daha hızlı ama biraz daha riskli
- Recurring payment için gerekli

### 4. Legacy iFrame API

- `InitiateCardRegistrationCommand` artık **DEPRECATED**
- Yeni kayıtlar için kullanılmamalı
- Mevcut kod geriye uyumluluk için duruyor

---

## 🚀 Production'a Geçiş

### 1. Test Mode Kapat

```json
{
  "PayTR": {
    "TestMode": false
  }
}
```

### 2. Gerçek Kart Testi

- Test kartları çalışmaz
- Gerçek kart ile test yapın
- Hemen iade edin

### 3. Monitoring

- Webhook loglarını izleyin
- Background service loglarını izleyin
- Başarısız ödemeleri takip edin

---

## 📚 Kaynaklar

- PayTR Direct API: https://dev.paytr.com/direkt-api
- Kart Saklama: https://dev.paytr.com/direkt-api/kart-saklama-api
- Yeni Kart Ekleme: https://dev.paytr.com/direkt-api/kart-saklama-api/yeni-kart-ekleme
- Kayıtlı Karttan Ödeme: https://dev.paytr.com/direkt-api/kart-saklama-api/kayitli-karttan-odeme

---

**Oluşturma Tarihi:** 2025-01-08  
**Durum:** ✅ Backend Tamamlandı - Frontend Güncellemesi Bekleniyor  
**Sonraki Adım:** Blazor UI'da register flow'u güncelle
