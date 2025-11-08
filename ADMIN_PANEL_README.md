# 🔐 Admin Paneli - Kurulum ve Kullanım Kılavuzu

## 📋 Genel Bakış

Admin Paneli, sistem yöneticilerinin tüm işletmeleri, randevuları, çalışanları, ödemeleri ve sistem raporlarını yönetmesine olanak tanır.

## ✨ Özellikler

### 1️⃣ Admin Dashboard
- Toplam işletme sayısı (Aktif/Pasif)
- Toplam müşteri sayısı
- Randevu istatistikleri (Toplam, Bugün, Bekleyen)
- Gelir raporları (Toplam, Aylık)
- Abonelik durumları

### 2️⃣ İşletme Yönetimi
- **Listeleme**: Tüm işletmeleri görüntüleme
- **Filtreleme**: İsim, durum, kategori ile filtreleme
- **Detay Görüntüleme**: İşletme bilgileri, istatistikler
- **Durum Yönetimi**: İşletmeleri aktif/pasif yapma
- **Abonelik Yönetimi**: Otomatik yenileme ayarları

### 3️⃣ Randevu Yönetimi
- İşletmeye ait tüm randevuları görüntüleme
- Tarih ve durum filtreleme
- Randevu durumu güncelleme (Pending, Confirmed, Completed, Cancelled)
- Randevu silme
- Müşteri bilgilerini görüntüleme

### 4️⃣ Çalışan Yönetimi
- İşletmeye ait çalışanları listeleme
- Çalışan silme
- Çalışan durumu görüntüleme

### 5️⃣ Yorum ve Rating Yönetimi
- İşletmeye yapılan tüm yorumları görüntüleme
- Puan ve yorumları listeleme
- Müşteri bilgilerini görüntüleme

### 6️⃣ Ödeme Yönetimi
- İşletme ödemelerini görüntüleme
- Tarih filtreleme
- Ödeme detayları (Kart bilgileri, tutar, durum)
- Ödeme iadesi yapma

### 7️⃣ Raporlama ve İstatistikler
- **Aylık Gelir Trendi**: Grafik ve tablo
- **Randevu Durum Dağılımı**: Pasta grafik
- **En Başarılı İşletmeler**: Bar chart (gelir bazlı)
- **En Çok Tercih Edilen Hizmetler**: Liste ve detaylar
- **Kategori Dağılımı**: Donut chart

## 🚀 Kurulum

### Adım 1: Admin Kullanıcısı Oluşturma

SQL Server Management Studio'da aşağıdaki scripti çalıştırın:

```bash
# SQL script çalıştırma
sqlcmd -S localhost -d AppointmentTestDbss -i /app/AdminUser_Setup.sql
```

**VEYA**

SQL dosyasını (`/app/AdminUser_Setup.sql`) açıp manuel olarak çalıştırın.

**Admin Giriş Bilgileri:**
- Email: `admin@appointmentsystem.com`
- Şifre: `Admin123!`

> ⚠️ **GÜVENLİK UYARISI**: İlk girişten sonra mutlaka şifrenizi değiştirin!

### Adım 2: Projeyi Derleme ve Çalıştırma

```bash
# Backend
cd AppointmentManagementSystem.API
dotnet build
dotnet run

# Blazor UI (başka terminalde)
cd AppointmentManagementSystem.BlazorUI
dotnet build
dotnet run
```

## 📱 Kullanım

### Giriş Yapma

1. Blazor uygulamasını açın: `http://localhost:5090`
2. Login sayfasına gidin
3. Admin bilgileriyle giriş yapın:
   - Email: `admin@appointmentsystem.com`
   - Şifre: `Admin123!`

### Sayfa Navigasyonu

**Ana Dashboard:**
```
/admin/dashboard
```

**İşletme Listesi:**
```
/admin/businesses
```

**İşletme Detayı:**
```
/admin/businesses/{businessId}
```

**Raporlar:**
```
/admin/reports
```

## 🔧 API Endpoints

### Dashboard
```http
GET /api/Admin/dashboard/stats
```

### İşletmeler
```http
GET  /api/Admin/businesses?searchTerm=&isActive=&categoryId=
GET  /api/Admin/businesses/{businessId}
PUT  /api/Admin/businesses/{businessId}/status
```

### Randevular
```http
GET    /api/Admin/businesses/{businessId}/appointments?startDate=&endDate=&status=
DELETE /api/Admin/appointments/{appointmentId}
PUT    /api/Admin/appointments/{appointmentId}/status
```

### Çalışanlar
```http
DELETE /api/Admin/employees/{employeeId}
```

### Ödemeler
```http
GET  /api/Admin/businesses/{businessId}/payments?startDate=&endDate=
PUT  /api/Admin/businesses/{businessId}/subscription/auto-renewal
POST /api/Admin/payments/{paymentId}/refund
```

### Raporlar
```http
GET /api/Admin/reports?startDate=&endDate=
```

## 🎨 UI Bileşenleri

Admin paneli Radzen Blazor componentlerini kullanır:

- **RadzenCard**: Kart container'ları
- **RadzenDataGrid**: Tablo görünümleri
- **RadzenChart**: Grafikler (Bar, Line, Pie, Donut)
- **RadzenButton**: Butonlar
- **RadzenDropDown**: Dropdown'lar
- **RadzenDatePicker**: Tarih seçiciler
- **RadzenBadge**: Durum göstergeleri
- **RadzenDialog**: Modal dialog'lar
- **RadzenNotification**: Bildirimler

## 🔐 Yetkilendirme

Tüm admin sayfaları ve endpoint'ler `[Authorize(Roles = "Admin")]` attribute'u ile korunmaktadır. Sadece `Role = "Admin"` olan kullanıcılar erişebilir.

## 📊 Raporlar

### Aylık Gelir Raporu
- Son 12 ay veya seçilen tarih aralığı
- Column chart ve tablo görünümü
- Toplam gelir ve ödeme sayısı

### Randevu İstatistikleri
- Durum bazında dağılım (Pending, Confirmed, Completed, Cancelled)
- Pie chart ve yüzdelik dilimler

### Top 10 İşletmeler
- Gelir bazlı sıralama
- Bar chart ve detaylı tablo
- Ortalama puan görüntüleme

### En Çok Tercih Edilen Hizmetler
- Rezervasyon sayısına göre sıralama
- İşletme bilgileri
- Toplam gelir

### Kategori Dağılımı
- İşletme ve randevu sayıları
- Donut chart görünümü

## 🛠️ Yapı

### Backend
```
/AppointmentManagementSystem.Application/Features/Admin/
├── Queries/
│   ├── GetAdminDashboardStatsQuery.cs
│   ├── GetAllBusinessesAdminQuery.cs
│   ├── GetBusinessDetailAdminQuery.cs
│   ├── GetBusinessAppointmentsAdminQuery.cs
│   ├── GetBusinessPaymentsAdminQuery.cs
│   └── GetReportsDataQuery.cs
├── Commands/
│   ├── UpdateBusinessStatusCommand.cs
│   ├── UpdateSubscriptionAutoRenewalCommand.cs
│   ├── DeleteAppointmentAdminCommand.cs
│   ├── DeleteEmployeeAdminCommand.cs
│   ├── UpdateAppointmentStatusAdminCommand.cs
│   └── RefundPaymentCommand.cs
└── Handlers/
    └── [Query ve Command Handler'ları]

/AppointmentManagementSystem.API/Controllers/
└── AdminController.cs
```

### Frontend
```
/AppointmentManagementSystem.BlazorUI/
├── Services/ApiServices/
│   ├── IAdminApiService.cs
│   └── AdminApiService.cs
├── Models/
│   ├── AdminDashboardStats.cs
│   ├── BusinessAdminModel.cs
│   ├── BusinessDetailAdminModel.cs
│   ├── AppointmentAdminModel.cs
│   ├── PaymentAdminModel.cs
│   └── ReportsDataModel.cs
└── Pages/Admin/
    ├── AdminDashboard.razor
    ├── BusinessManagement.razor
    ├── BusinessDetail.razor
    ├── Reports.razor
    └── Components/
        ├── AppointmentsTab.razor
        ├── EmployeesTab.razor
        ├── ReviewsTab.razor
        ├── PaymentsTab.razor
        └── StatusUpdateDialog.razor
```

## 📝 Notlar

- Admin paneli tamamen mobil uyumlu Radzen componentleri ile tasarlanmıştır
- Tüm işlemler için onay dialog'ları mevcuttur
- Başarılı/Başarısız işlemler için notification'lar gösterilir
- Tüm veriler sayfalama destekli olarak gösterilir
- Filtreleme ve arama özellikleri mevcuttur

## 🔒 Güvenlik

- Admin endpoint'leri JWT token ile korunmaktadır
- Sadece "Admin" rolüne sahip kullanıcılar erişebilir
- Tüm kritik işlemler için onay gereklidir
- SQL injection koruması (Entity Framework)
- XSS koruması (Blazor otomatik encoding)

## 🐛 Sorun Giderme

### Admin kullanıcısı giriş yapamıyor
1. SQL script'in doğru çalıştığından emin olun
2. Email ve şifrenin doğru olduğunu kontrol edin
3. User tablosunda Role = "Admin" olduğundan emin olun

### API endpoint'leri 401 Unauthorized hatası veriyor
1. JWT token'ın geçerli olduğunu kontrol edin
2. Token'da Role claim'inin "Admin" olduğunu doğrulayın
3. Browser console'da token'ı kontrol edin

### Sayfa yüklenmiyorsa
1. Backend API'nin çalıştığından emin olun
2. Blazor uygulamasının doğru API URL'ini kullandığını kontrol edin
3. Browser console'da hataları kontrol edin

## 📞 Destek

Herhangi bir sorun için lütfen geliştirici ile iletişime geçin.

---

**Son Güncelleme**: 2025-01-08  
**Versiyon**: 1.0  
**Geliştirici**: E1 AI Agent
