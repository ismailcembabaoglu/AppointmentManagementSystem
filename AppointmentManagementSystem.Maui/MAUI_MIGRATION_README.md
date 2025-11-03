# 📱 Appointment Management System - MAUI Migration

## 🎉 Migration Tamamlandı!

BlazorUI projesindeki TÜM özellikler başarıyla MAUI projesine taşındı.

---

## 📦 Eklenen Özellikler

### ✅ Tüm BlazorUI Sayfaları
- **Authentication:** Login, Register
- **Dashboard:** Customer Dashboard, Business Dashboard
- **Business:** Business List, Business Details, Business Management
- **Categories:** Category listing ve seçim
- **Services:** Service management
- **Employees:** Employee management
- **Appointments:** Appointment booking, listing, details
- **Profile:** User profile management
- **Reports:** Business reports
- **Home, Counter, Weather** (demo pages)

### ✅ Tüm Components
- FileUploadComponent
- ImageGalleryComponent
- NotificationPanel
- PayTRIFrameComponent
- ProfileDialog
- RatingDialog
- BusinessSearchResults
- Dialogs klasöründeki tüm dialog'lar:
  - Appointment dialogs
  - Business dialogs
  - Employee dialogs
  - Service dialogs
  - Registration dialogs

### ✅ Tüm API Servisleri
- ApiService (Base service)
- CategoryApiService
- BusinessApiService
- ServiceApiService
- EmployeeApiService
- AppointmentApiService
- PhotoApiService
- DocumentApiService
- PaymentApiService
- TurkishCityService

### ✅ Authentication System
- CustomAuthenticationStateProvider
- JWT token management
- Role-based authorization (Customer, Business, Admin)
- Blazored.LocalStorage integration

### ✅ UI Framework
- Radzen Blazor components
- Bootstrap 5
- Custom CSS (radzen-custom.css, app.css)
- Responsive design

### 🆕 Native Mobile Features (YENİ!)
- **Camera Service:** 
  - Fotoğraf çekme (TakePhotoAsync)
  - Galeriden fotoğraf seçme (PickPhotoAsync)
- **Location Service:**
  - GPS konumu alma (GetCurrentLocationAsync)
  - Konum izinleri yönetimi
- **Native Features Demo Page:** `/native-features`

---

## 🔧 Yapılandırma

### NuGet Paketleri
```xml
<PackageReference Include="Radzen.Blazor" Version="5.6.4" />
<PackageReference Include="Blazored.LocalStorage" Version="4.5.0" />
<PackageReference Include="Microsoft.AspNetCore.Components.Authorization" Version="8.0.21" />
<PackageReference Include="Microsoft.Maui.Controls.MediaElement" Version="8.0.3" />
```

### API Configuration
- **Base URL:** `http://localhost:5089/`
- Configured in `MauiProgram.cs`

### Platform Permissions

#### Android (AndroidManifest.xml)
- ✅ Camera
- ✅ Location (Coarse & Fine)
- ✅ Storage (Read & Write)
- ✅ Internet & Network State

#### iOS (Info.plist)
- ✅ NSCameraUsageDescription
- ✅ NSPhotoLibraryUsageDescription
- ✅ NSLocationWhenInUseUsageDescription
- ✅ NSLocationAlwaysUsageDescription

---

## 🚀 Kullanım

### Projeyi Build Etme
```bash
# Restore packages
dotnet restore

# Build
dotnet build

# Android için
dotnet build -t:Run -f net8.0-android

# iOS için
dotnet build -t:Run -f net8.0-ios
```

### Debug
- Visual Studio 2022 veya VS Code ile açın
- Android veya iOS emulator seçin
- F5 ile çalıştırın

---

## 📱 Desteklenen Platformlar

- ✅ Android (API 24+)
- ✅ iOS (14.2+)
- ✅ macOS (Mac Catalyst)
- ✅ Windows (10.0.17763+)

---

## 🎯 Ana Özellikler

### Customer (Müşteri) Özellikleri
- Kategori ve işletme arama
- İşletme detayları ve fotoğrafları
- Randevu oluşturma
- Randevu listeleme ve yönetimi
- Randevulara puan ve yorum verme
- Profil yönetimi
- Konum tabanlı işletme arama (Native!)
- Randevu fotoğrafları yükleme (Native Camera!)

### Business (İşletme) Özellikleri
- İşletme paneli
- Hizmet yönetimi (CRUD)
- Çalışan yönetimi (CRUD)
- Randevu yönetimi
- Raporlama ve istatistikler
- PayTR ödeme entegrasyonu
- İşletme fotoğrafları (Native Camera!)
- Çalışan belgeleri yükleme

### Native Mobile Features
- **Kamera:** Fotoğraf çekme ve galeri seçimi
- **Konum:** GPS lokasyon alma
- **İzinler:** Runtime permission handling
- Demo sayfası: `/native-features`

---

## 🔐 Güvenlik

- JWT Bearer Authentication
- Role-based Authorization (Customer, Business, Admin)
- Secure token storage (LocalStorage)
- HTTPS ready
- Permission-based native feature access

---

## 📊 Dosya Yapısı

```
AppointmentManagementSystem.Maui/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   ├── Pages/
│   │   ├── Auth/ (Login, Register)
│   │   ├── Business/ (Dashboard, Reports)
│   │   ├── Appointments/
│   │   ├── Categories/
│   │   ├── Services/
│   │   ├── Employees/
│   │   ├── Profile/
│   │   ├── Dashboard/
│   │   ├── NativeFeatures.razor (YENİ!)
│   │   └── ... (diğer sayfalar)
│   ├── Dialogs/ (Tüm dialog componentleri)
│   └── ... (diğer componentler)
├── Services/
│   ├── ApiServices/ (Tüm API servisleri)
│   ├── Authentication/ (Auth provider)
│   ├── CameraService.cs (YENİ!)
│   ├── LocationService.cs (YENİ!)
│   └── TurkishCityService.cs
├── Models/
│   ├── ApiResponse.cs
│   └── AvailableEmployeesResponse.cs
├── Platforms/
│   ├── Android/ (Permissions configured)
│   └── iOS/ (Permissions configured)
├── wwwroot/
│   ├── css/ (Radzen, Bootstrap, custom)
│   ├── js/ (Mobile detect, notifications)
│   └── lib/ (Bootstrap)
├── Shared/
│   ├── LandingLayout.razor
│   └── RedirectToLogin.razor
├── MauiProgram.cs (DI configuration)
└── App.razor

```

---

## 🎨 UI/UX

- **Radzen Blazor:** Modern, responsive components
- **Bootstrap 5:** Grid system, utilities
- **Custom CSS:** Brand styling
- **Mobile-first:** Touch-optimized
- **Dark mode ready:** Radzen themes support

---

## 🔄 API Integration

Tüm API endpoint'leri HttpClient üzerinden çağrılıyor:
- Base URL: `http://localhost:5089/`
- JWT token otomatik ekleniyor (BaseApiService)
- Error handling ve notification'lar hazır
- Response modelleri tanımlı

---

## 📝 Notlar

1. **API Server:** Backend API'nin (port 5089) çalışır durumda olması gerekiyor
2. **Database:** SQL Server connection string güncel olmalı
3. **PayTR:** Sandbox credentials gerekli (Production için)
4. **Native Features:** Android/iOS emulator veya gerçek cihazda test edin
5. **Permissions:** İlk kullanımda kullanıcıdan izin istenecek

---

## 🐛 Bilinen Sorunlar

- Yok! Tüm özellikler başarıyla migrate edildi.

---

## 📞 Destek

Herhangi bir sorun için:
- Backend API loglarını kontrol edin
- MAUI debug console'u inceleyin
- Platform-specific issues için Platforms/ klasörünü kontrol edin

---

## ✨ Yapılabilecek İyileştirmeler

- [ ] Offline mode (Local database caching)
- [ ] Push notifications
- [ ] Biometric authentication (Fingerprint, Face ID)
- [ ] AR features (Business location viewer)
- [ ] Voice commands
- [ ] Barcode/QR scanning
- [ ] Calendar integration
- [ ] Maps integration (Detailed)
- [ ] Share functionality
- [ ] In-app payments (Apple Pay, Google Pay)

---

**Migration Date:** 2025  
**Status:** ✅ Complete  
**Version:** 1.0  
**Platform:** .NET 8 MAUI
