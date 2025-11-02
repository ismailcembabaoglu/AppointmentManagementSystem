# 🔧 Tek Port Kurulum - Hızlı Düzeltme

## ✅ Sorun Çözüldü

### Hata:
```
CS1061: 'WebApplication' bir 'UseBlazorFrameworkFiles' tanımı içermiyor
```

### Çözüm:
API projesine gerekli NuGet paketi eklendi.

---

## 📦 Kurulum Adımları

### 1. NuGet Paketini Yükle

**Manuel (Command Line):**
```bash
cd AppointmentManagementSystem.API
dotnet add package Microsoft.AspNetCore.Components.WebAssembly.Server --version 8.0.21
```

**Veya Visual Studio:**
- Paket zaten .csproj'a eklendi
- Sadece restore yapın:
```bash
dotnet restore
```

### 2. Restore & Build

```bash
# Root dizinde
dotnet restore
dotnet build
```

---

## 🚀 Çalıştırma

### Tek Komut (Her Şey Dahil)

```bash
# Windows
start-single-port.bat

# Linux/Mac
./start-single-port.sh
```

Bu komut:
1. ✅ Paketleri restore eder
2. ✅ Solution'ı build eder
3. ✅ Blazor'u publish eder
4. ✅ API'yi başlatır

### Manuel Adımlar

```bash
# 1. Blazor build
build-blazor.bat

# 2. API çalıştır
cd AppointmentManagementSystem.API
dotnet run

# 3. Tarayıcıda aç
http://localhost:5089
```

---

## 📋 Eklenen Paket

| Paket | Versiyon | Amaç |
|-------|----------|------|
| Microsoft.AspNetCore.Components.WebAssembly.Server | 8.0.21 | Blazor static file serving |

**Ne İşe Yarar?**
- `UseBlazorFrameworkFiles()` extension method
- Blazor framework dosyalarını serve eder
- SPA fallback routing

---

## ✅ Doğrulama

### Test 1: Build
```bash
dotnet build AppointmentManagementSystem.API
# Hata olmamalı
```

### Test 2: Çalıştır
```bash
start-single-port.bat
# API başlamalı
```

### Test 3: Erişim
```
http://localhost:5089
# Blazor açılmalı (build olduysa)
```

---

## 🎯 Sonraki Adımlar

1. ✅ `dotnet restore` (paketleri yükle)
2. ✅ `dotnet build` (build et)
3. ✅ `start-single-port.bat` (çalıştır)
4. ✅ http://localhost:5089 (test et)

---

## 📝 Not

Bu paket sadece **tek port çalışma** için gerekli.

Eğer **ayrı portlar** kullanıyorsanız (`start.bat`), bu paket gerekmez.

---

**Durum:** ✅ Çözüldü  
**Tarih:** 2025-01-08
