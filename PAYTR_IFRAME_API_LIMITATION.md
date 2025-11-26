# ⚠️ PayTR iFrame API - Kart Saklama Kısıtlaması

## 🔴 SORUN: iFrame API Kart Tokenization Desteklemiyor

PayTR'nin **iFrame API**'si kart saklama (card tokenization) özelliğini **DESTEKLEMIYOR**.

### Neden Kart Bilgileri NULL Geliyor?

iFrame API dokümantasyonunda (`https://dev.paytr.com/iframe-api/iframe-api-1-adim`) şu parametreler **YOK**:
- ❌ `store_card` 
- ❌ `utoken` (user token)
- ❌ `ctoken` (card token)

Bu parametreler sadece **Direct API**'de (`https://dev.paytr.com/direkt-api/kart-saklama-api`) mevcut.

### iFrame API Webhook'ta Ne Geliyor?

iFrame API webhook'unda sadece şunlar gelir:
- ✅ `merchant_oid`
- ✅ `status`
- ✅ `total_amount`
- ✅ `card_type` veya `card_association` (Visa, Mastercard, vb.)
- ✅ `card_number_last_four` (Son 4 hane)
- ❌ `utoken` - **GELMİYOR**
- ❌ `ctoken` - **GELMİYOR**

### Şu An Ne Yapıyor?

Mevcut kod şunları kaydediyor:
```csharp
BusinessSubscription
├── CardType: "Visa" ✅ (display için)
├── MaskedCardNumber: "**** **** **** 1234" ✅ (display için)
├── CardLastFourDigits: "1234" ✅ (display için)
├── PayTRUserToken: NULL ❌ (recurring payment için gerekli)
└── PayTRCardToken: NULL ❌ (recurring payment için gerekli)
```

**Sonuç:** Kartın hangi kart olduğunu görebilirsiniz ama **recurring payment YAPAMAZSINIZ**.

---

## ✅ ÇÖZÜM SEÇENEKLERİ

### Seçenek 1: Direct API'ye Geç (ÖNERİLEN)

PayTR'nin **Direct API**'sini kullanarak kart saklama yapabilirsiniz.

**Avantajları:**
- ✅ Kart tokenization çalışır
- ✅ Recurring payment yapabilirsiniz
- ✅ Kart güncelleme yapabilirsiniz

**Dezavantajları:**
- ❌ Kendi ödeme formunuzu hazırlamanız gerekir
- ❌ PCI-DSS uyumu gerekir (ama PayTR'nin hosted form'unu kullanırsanız gerekmez)
- ❌ Daha fazla development effort

**Nasıl Yapılır:**
1. PayTR'nin Direct API dokümantasyonunu inceleyin: https://dev.paytr.com/direkt-api/kart-saklama-api
2. İlk kayıtta `store_card=1` parametresi ile ödeme yapın
3. Webhook'ta gelen `utoken` ve `ctoken`'ı kaydedin
4. Sonraki ödemelerde bu token'ları kullanın

### Seçenek 2: PayTR Destek ile İletişime Geçin

PayTR destek ekibine şu soruyu sorun:

> "Merhaba,
> 
> iFrame API kullanıyorum ve ilk kayıt sırasında karttan ödeme alıyorum. Bu kartı gelecekteki aylık tahsilatlar için kaydetmek istiyorum.
> 
> iFrame API ile kart saklama (tokenization) mümkün mü? Webhook'ta `utoken` ve `ctoken` almak için özel bir parametre göndermem gerekiyor mu?
> 
> Yoksa Direkt API'ye geçmem mi gerekiyor?
> 
> Teşekkürler."

### Seçenek 3: Hybrid Yaklaşım (GEÇİCİ)

İlk kayıt için iFrame API kullan, sonraki işlemler için Direct API kullan.

**Akış:**
1. İlk kayıt → iFrame API (basit ve hızlı)
2. Ödeme başarılı → Direct API ile kart kayd payment isteği
3. Sonraki aylar → Direct API recurring payment

Bu yaklaşım karmaşık ve önerilmez.

---

## 🛠️ Şu Anki Durum

### Kod Değişiklikleri Yapıldı ✅

1. **Webhook Controller**:
   - `card_number_last_four` parametresi eklendi
   - `card_association` parametresi eklendi
   - Kart bilgileri log edildi

2. **Webhook Handler**:
   - utoken/ctoken NULL kontrolü eklendi
   - Kart bilgileri (card_type, masked_pan) kaydediliyor
   - AutoRenewal token yoksa FALSE

3. **Log Uyarıları**:
   ```
   ⚠️ Card tokens (utoken/ctoken) are NULL. iFrame API does not support card tokenization.
   ⚠️ Saving only available card info (card_type and masked_pan) for display purposes.
   ⚠️ Recurring payments will NOT work without card tokens!
   ⚠️ Consider switching to Direct API: https://dev.paytr.com/direkt-api/kart-saklama-api
   ```

### Veritabanında Ne Var?

```sql
SELECT 
    BusinessId,
    CardType,           -- "Visa" ✅
    MaskedCardNumber,   -- "**** **** **** 1234" ✅
    CardLastFourDigits, -- "1234" ✅
    PayTRUserToken,     -- NULL ❌
    PayTRCardToken,     -- NULL ❌
    AutoRenewal         -- false (token olmadığı için)
FROM BusinessSubscriptions
```

### Kullanıcı Arayüzünde Ne Görünür?

**Kart Yönetimi Sayfası:**
```
┌─────────────────────────────────────┐
│ Kayıtlı Kartlarınız                 │
├─────────────────────────────────────┤
│ 💳 Visa **** 1234                   │
│ ⚠️ Bu kart ile otomatik ödeme       │
│    yapılamıyor. Lütfen kartınızı    │
│    güncelleyin.                     │
│                                     │
│ [Kart Güncelle]                     │
└─────────────────────────────────────┘
```

---

## 📝 SONRAKİ ADIMLAR

1. **PayTR destek ile iletişime geçin** ve iFrame API'de tokenization mümkün mü sorun
2. Eğer mümkün değilse, **Direct API'ye geçiş planı** yapın
3. Mevcut kullanıcılar için **kart güncelleme flow'u** oluşturun

---

## 📚 KAYNAKLAR

- iFrame API Dokümantasyon: https://dev.paytr.com/iframe-api
- Direct API Dokümantasyon: https://dev.paytr.com/direkt-api
- Kart Saklama API: https://dev.paytr.com/direkt-api/kart-saklama-api
- PayTR Destek: https://www.paytr.com/destek-merkezi

---

**Oluşturma Tarihi:** 2025-01-08  
**Durum:** ⚠️ iFrame API Kısıtlaması - Çözüm Bekleniyor
