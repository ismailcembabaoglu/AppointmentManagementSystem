# 🔧 iFrame Problemi Çözümü

## ❌ Sorun:

Kayıt sayfasında hala iFrame ödeme formu görünüyor, Direct API formu gelmiyor.

## ✅ Yapılan Değişiklikler:

### 1. Debug Alert Eklendi

Register.razor Step 4'e debug alert eklendi:
```razor
<RadzenAlert AlertStyle="AlertStyle.Success">
    🆕 Direct API Ödeme Formu Aktif
    Kart bilgilerinizi direkt olarak gireceksiniz (iFrame kullanılmıyor)
</RadzenAlert>
```

### 2. Console Log Eklendi

```csharp
Console.WriteLine($"✅ Moving to payment step. BusinessId: {registeredBusinessId}");
Console.WriteLine($"🔵 Direct API Payment Form will be rendered");
```

## 🔍 Sorun Giderme Adımları:

### Adım 1: Tarayıcı Cache'ini Temizle

**Chrome/Edge:**
```
1. F12 (Developer Tools)
2. Network sekmesi
3. "Disable cache" işaretle
4. Ctrl + Shift + R (Hard Reload)
```

**Firefox:**
```
1. Ctrl + Shift + Delete
2. "Cached Web Content" seç
3. Temizle
4. Sayfayı yenile
```

### Adım 2: Blazor Rebuild

```bash
cd /app/AppointmentManagementSystem.BlazorUI
dotnet clean
dotnet build
dotnet run
```

### Adım 3: Browser Storage Temizle

**F12 Console'da:**
```javascript
localStorage.clear();
sessionStorage.clear();
location.reload(true);
```

### Adım 4: Doğru Sayfaya Gittiğinizden Emin Olun

```
URL: http://localhost:5090/register
```

### Adım 5: Console Logları Kontrol Et

F12 → Console sekmesinde şunları aramalısınız:
```
✅ Moving to payment step. BusinessId: 6
🔵 Direct API Payment Form will be rendered
```

Eğer bu mesajları görmüyorsanız:
- Step 4'e geçiş olmamış olabilir
- currentBusinessStep değeri 4 değil
- registeredBusinessId null olabilir

## 🐛 Olası Sorunlar:

### 1. Eski Build Kullanılıyor

**Çözüm:**
```bash
cd /app/AppointmentManagementSystem.BlazorUI
dotnet clean
rm -rf bin obj
dotnet build
```

### 2. Browser Service Worker Cache

**Çözüm:**
```javascript
// F12 Console
navigator.serviceWorker.getRegistrations().then(function(registrations) {
 for(let registration of registrations) {
  registration.unregister();
 }
 location.reload(true);
});
```

### 3. Component Bulunamıyor

**Kontrol Et:**
```bash
ls -la /app/AppointmentManagementSystem.BlazorUI/Components/DirectAPIPaymentForm.razor
```

Dosya olmalı: `-rw-r--r-- 12343 DirectAPIPaymentForm.razor`

### 4. Using Statement Eksik

Register.razor başında olmalı:
```razor
@using AppointmentManagementSystem.BlazorUI.Components
```

## ✅ Başarı Kontrolü:

### Doğru Görünüm:

```
┌─────────────────────────────────────────┐
│ 🆕 Direct API Ödeme Formu Aktif        │
│ Kart bilgilerinizi direkt gireceksiniz│
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│          💳 Kart Bilgileriniz           │
│  Güvenli ödeme için kart bilgilerinizi │
│                 girin                   │
│                                         │
│ Kart Sahibi: [_______________]         │
│ Kart No: [_______________]             │
│ Tarih: [__] [__]  CVV: [___]          │
│                                         │
│ [Ödemeyi Tamamla]                      │
└─────────────────────────────────────────┘
```

### Yanlış Görünüm (iFrame):

```
┌─────────────────────────────────────────┐
│     PayTR iFrame içinde form var       │
│     (Eski entegrasyon)                 │
└─────────────────────────────────────────┘
```

## 📞 Hala Çalışmıyorsa:

1. **Backend'i kontrol et:**
   ```bash
   curl -X POST http://localhost:5089/api/payments/initiate-direct-card-registration \
     -H "Content-Type: application/json" \
     -d '{"businessId":6,"email":"test@test.com",...}'
   ```

2. **Console hatalarını kontrol et:**
   - F12 → Console
   - Kırmızı hatalar var mı?

3. **Network trafiğini kontrol et:**
   - F12 → Network
   - `/register` sayfası yüklenirken hangi dosyalar gelmiyor?

## 🎯 Kesin Çözüm:

Eğer yukarıdaki hiçbiri işe yaramazsa:

```bash
# 1. Tüm process'leri durdur
pkill -f dotnet

# 2. Temizlik yap
cd /app/AppointmentManagementSystem.BlazorUI
dotnet clean
rm -rf bin obj
rm -rf wwwroot/_framework

# 3. Rebuild
dotnet build

# 4. Başlat
dotnet run

# 5. Tarayıcıyı incognito modda aç
# Chrome: Ctrl + Shift + N
# Firefox: Ctrl + Shift + P

# 6. http://localhost:5090/register git
```

---

**Son Güncelleme:** 2025-01-08  
**Durum:** Debug mesajları eklendi, tarayıcı cache temizliği önerildi
