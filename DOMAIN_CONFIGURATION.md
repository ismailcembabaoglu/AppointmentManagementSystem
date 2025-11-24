# Domain Yapılandırması

## 🌐 Sistem Mimarisi

### Frontend
- **URL:** https://aptivaplan.com.tr
- **Teknoloji:** Blazor WebAssembly
- **Görev:** Kullanıcı arayüzü

### Backend API
- **URL:** https://hub.aptivaplan.com.tr/api
- **Teknoloji:** ASP.NET Core Web API
- **Görev:** API endpoints, PayTR webhook işleme

## 🔄 PayTR Ödeme Akışı

### 1. Ödeme Başlatma
```
Kullanıcı (Frontend)
  ↓
https://aptivaplan.com.tr/register
  ↓
[Backend API] POST https://hub.aptivaplan.com.tr/api/payments/initiate-card-registration
  ↓
PayTR iframe URL alınır
  ↓
Kullanıcı PayTR iframe'de ödeme yapar
```

### 2. PayTR → Backend Webhook (SERVER-TO-SERVER)
```
PayTR Sunucuları
  ↓
POST https://hub.aptivaplan.com.tr/api/payments/webhook
  ↓
Backend işler, "OK" döner
  ↓
PayTR işlemi başarılı sayar
```

### 3. Kullanıcı Yönlendirme
```
PayTR
  ↓
[Backend] https://hub.aptivaplan.com.tr/api/payments/success-redirect
  ↓
302 Redirect
  ↓
[Frontend] https://aptivaplan.com.tr/payment/success
```

## ⚙️ Yapılandırma Dosyaları

### appsettings.json
```json
{
  "PayTR": {
    "CallbackUrl": "https://hub.aptivaplan.com.tr/api/payments/webhook",
    "OkRedirectUrl": "https://hub.aptivaplan.com.tr/api/payments/success-redirect",
    "FailRedirectUrl": "https://hub.aptivaplan.com.tr/api/payments/fail-redirect",
    "SuccessUrl": "https://aptivaplan.com.tr/payment/success",
    "FailUrl": "https://aptivaplan.com.tr/payment/failed"
  }
}
```

### PayTR Merchant Panel Ayarları
```
Bildirim URL: https://hub.aptivaplan.com.tr/api/payments/webhook
```

## 🔒 Güvenlik

### CORS Ayarları (Backend)
Frontend'den API'ye erişim için:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://aptivaplan.com.tr")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

### SSL/TLS
- ✅ Frontend: HTTPS zorunlu
- ✅ Backend: HTTPS zorunlu (PayTR webhook için)
- ✅ SSL sertifikaları geçerli olmalı

## 🧪 Test Komutları

### Backend API Test
```powershell
# Webhook endpoint test
Invoke-WebRequest -Uri "https://hub.aptivaplan.com.tr/api/payments/webhook" -Method POST

# Health check (eğer varsa)
Invoke-WebRequest -Uri "https://hub.aptivaplan.com.tr/api/health"
```

### Frontend Test
```powershell
# Ana sayfa
Invoke-WebRequest -Uri "https://aptivaplan.com.tr"

# Ödeme success sayfası
Invoke-WebRequest -Uri "https://aptivaplan.com.tr/payment/success"
```

## 📋 Deployment Checklist

### hub.aptivaplan.com.tr (Backend)
- [ ] IIS site binding: Port 443, SSL sertifikası
- [ ] web.config dosyası mevcut
- [ ] Application Pool: .NET CLR = No Managed Code
- [ ] Handler Mappings: aspNetCore handler
- [ ] Request Filtering: POST allowed
- [ ] appsettings.json doğru URL'lerle güncellendi

### aptivaplan.com.tr (Frontend)
- [ ] Blazor WASM publish edildi
- [ ] HTTPS redirect aktif
- [ ] wwwroot dosyaları doğru yerde

### PayTR Panel
- [ ] Bildirim URL: https://hub.aptivaplan.com.tr/api/payments/webhook
- [ ] Test ödeme başarılı
- [ ] Bildirim Durumu: Başarılı

## 🔍 Sorun Giderme

### Webhook Gelmiyor
1. **URL kontrol:** hub.aptivaplan.com.tr erişilebilir mi?
2. **Firewall:** PayTR IP'lerine açık mı?
3. **SSL:** Sertifika geçerli mi?
4. **IIS:** POST metodu allowed mı?

### CORS Hatası
```
Frontend'den API'ye istek atarken CORS hatası:
→ Backend CORS policy'sini kontrol et
→ aptivaplan.com.tr origin'e izin verilmiş mi?
```

### 405 Method Not Allowed
```
Webhook'da 405 hatası:
→ web.config dosyası var mı?
→ IIS Handler Mappings doğru mu?
→ Request Filtering'de POST allowed mı?
```

## 📞 Önemli Notlar

1. **İki farklı domain kullanılıyor:**
   - aptivaplan.com.tr: Frontend (Blazor WASM)
   - hub.aptivaplan.com.tr: Backend (API)

2. **PayTR webhook'u Backend'e gider:**
   - Server-to-server iletişim
   - Frontend ile ilgisi YOK

3. **Kullanıcı redirect'leri:**
   - Backend'den Frontend'e yönlendirme yapılır
   - success-redirect ve fail-redirect endpoint'leri bu işi yapar

4. **Hash validation:**
   - Her webhook'ta hash doğrulanmalı
   - MerchantKey ve MerchantSalt doğru olmalı

---

**Güncelleme:** 24.11.2025
**Hazırlayan:** E1 AI Agent
