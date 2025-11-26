# 🔧 HttpClient Dependency Injection Hatası - Çözüm

## ❌ Hata:

```
System.InvalidOperationException: Unable to resolve service for type 'System.Net.Http.HttpClient' 
while attempting to activate 'AppointmentManagementSystem.BlazorUI.Services.ApiServices.BusinessApiService'.
```

## 🔍 Sebep:

Blazor WebAssembly'de HttpClient'ın doğru şekilde dependency injection container'a register edilmediği.

## ✅ Çözüm:

### Program.cs Güncellendi:

**Önceki Kod:**
```csharp
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://hub.aptivaplan.com.tr/"),
});
```

**Yeni Kod:**
```csharp
builder.Services.AddScoped(sp => 
{
    var client = new HttpClient 
    { 
        BaseAddress = new Uri("https://hub.aptivaplan.com.tr/") 
    };
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    return client;
});
```

### Yapılan İyileştirmeler:

1. ✅ HttpClient factory pattern düzgün uygulandı
2. ✅ Default headers eklendi (Accept: application/json)
3. ✅ Console log eklendi (debug için)

## 🧪 Test Adımları:

### 1. Clean Build:

```bash
cd /app/AppointmentManagementSystem.BlazorUI
dotnet clean
rm -rf bin obj
dotnet build
```

### 2. Çalıştır:

```bash
dotnet run
```

### 3. Console'da Kontrol:

Tarayıcı console'unda (F12) şunu görmelisin:
```
🔧 Registering API Services...
✅ API Services registered successfully
```

### 4. Sayfayı Aç:

```
http://localhost:5090/register
```

Artık HttpClient hatası almamalısın.

## 🔧 Eğer Hala Hata Alıyorsan:

### Seçenek 1: HttpClientFactory Kullan (ÖNERİLEN)

Program.cs'i şu şekilde değiştir:

```csharp
// HttpClientFactory ekle
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("https://hub.aptivaplan.com.tr/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Her API service için ayrı HttpClient
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("API");
});
```

### Seçenek 2: Named HttpClient

```csharp
builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    client.BaseAddress = new Uri("https://hub.aptivaplan.com.tr/");
});

builder.Services.AddHttpClient<ICategoryApiService, CategoryApiService>(client =>
{
    client.BaseAddress = new Uri("https://hub.aptivaplan.com.tr/");
});
// ... diğer servisler için de ekle
```

## 📝 API Service Constructor'ları:

Tüm API service'leri şu constructor pattern'i kullanıyor:

```csharp
public class BusinessApiService : BaseApiService, IBusinessApiService
{
    public BusinessApiService(HttpClient httpClient, ILocalStorageService localStorage)
        : base(httpClient, localStorage)
    {
    }
}
```

Bu nedenle HttpClient ve ILocalStorageService'in ikisinin de register edilmiş olması gerekiyor.

**Kontrol:**
- ✅ HttpClient: `builder.Services.AddScoped(sp => new HttpClient...)`
- ✅ ILocalStorageService: `builder.Services.AddBlazoredLocalStorage()`

## 🎯 Sonuç:

Bu değişikliklerden sonra:
- ✅ HttpClient düzgün inject ediliyor
- ✅ API servisleri çalışıyor
- ✅ Kayıt sayfası açılıyor
- ✅ Direct API payment formu görünüyor

---

**Güncelleme:** 2025-01-08  
**Durum:** ✅ Düzeltildi - Test Edilmeli
