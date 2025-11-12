# ⚡ HIZLI ÇÖZÜM - 3 Adım

## Sizin Hatanız:
```
❌ https://aptivaplan.com.tr/wwwroot/index.html → 404 Not Found
```

## Sebep:
Dosya yapısı yanlış! `/wwwroot/` URL'de olmamalı.

---

## ✅ ÇÖZÜM

### 📋 Adım 1: Yeniden Publish (1 dakika)

**Windows:**
```cmd
cd C:\YourProjectPath\app
publish-blazor.bat
```

**Not:** Güncellenmiş web.config ile publish yapılacak (wwwroot prefix kaldırıldı)

---

### 📂 Adım 2: Plesk'e Doğru Yükle (2 dakika)

#### YANLIŞ YÜKLEME: ❌
```
httpdocs/
  └── wwwroot/           ← KLasörü kendisi
        ├── index.html
        ├── _framework/
        └── ...
```

#### DOĞRU YÜKLEME: ✅
```
httpdocs/
  ├── index.html         ← Dosyalar direkt burada!
  ├── _framework/
  ├── _content/
  ├── css/
  ├── js/
  ├── lib/
  └── web.config
```

#### Nasıl Yapılır?

**Plesk File Manager:**
1. `httpdocs/` klasörüne git
2. Tüm eski dosyaları sil
3. Lokal bilgisayarınızda: `/app/AppointmentManagementSystem.BlazorUI/publish/wwwroot/` aç
4. İçindeki **TÜM DOSYA ve KLASÖRLERI** seç (wwwroot klasörünü değil!)
5. Plesk'e yükle → `httpdocs/` içine

**FTP:**
```
Lokal: /publish/wwwroot/*
Uzak:  /httpdocs/
```

---

### 🧪 Adım 3: Test (10 saniye)

```
https://aptivaplan.com.tr
```

**Beklenen:**
- ✅ Blazor uygulaması açılmalı
- ✅ URL'de `/wwwroot/` olmamalı
- ✅ Ana sayfa görünmeli

---

## 🔍 Hala Çalışmıyor mu?

### Kontrol 1: Dosya Yapısı

**Plesk → File Manager → httpdocs:**

```
✅ httpdocs/index.html        (DOĞRU)
✅ httpdocs/_framework/       (DOĞRU)
✅ httpdocs/web.config        (DOĞRU)

❌ httpdocs/wwwroot/          (YANLIŞ - OLMAMALI!)
```

### Kontrol 2: web.config İçeriği

**Plesk → File Manager → httpdocs/web.config → Edit**

Arama yap (Ctrl+F): `wwwroot`

- **Buldu:** ❌ Eski versiyon! Yeni web.config'i yükle
- **Bulamadı:** ✅ Doğru versiyon

### Kontrol 3: IIS Application Pool

**Plesk → IIS Settings:**

```
.NET CLR Version: No Managed Code   ✅
Managed Pipeline: Integrated        ✅
```

---

## 📥 Manuel Çözüm (Yeniden publish istemiyorsanız)

### Mevcut dosyaları taşıyın:

1. **Plesk File Manager:**
   - `httpdocs/wwwroot/` klasörüne git
   - **Tüm dosyaları** seç (Ctrl+A)
   - **Cut** (Kes)
   - Üst klasöre git: `httpdocs/`
   - **Paste** (Yapıştır)
   - Boş `wwwroot/` klasörünü sil

2. **web.config güncelle:**
   - Lokal: `/app/AppointmentManagementSystem.BlazorUI/wwwroot/web.config` indir
   - Plesk: `httpdocs/web.config` üzerine yaz

3. **Test et:**
   ```
   https://aptivaplan.com.tr
   ```

---

## 🎯 Özet

| Ne Yaptık | Neden |
|-----------|-------|
| web.config güncelledik | `wwwroot\` prefix kaldırıldı |
| Dosyaları taşıdık | httpdocs'un içinde direkt olmalı |
| wwwroot klasörü sildik | URL'de görünmemeli |

---

## ✅ Checklist

- [ ] `publish-blazor.bat` çalıştırdım
- [ ] `publish/wwwroot/` içindeki DOSYALARI yükledim (klasörü değil)
- [ ] Plesk `httpdocs/` direkt içine yükledim
- [ ] `httpdocs/index.html` var
- [ ] `httpdocs/web.config` güncel
- [ ] `httpdocs/wwwroot/` klasörü YOK
- [ ] `https://aptivaplan.com.tr` açılıyor

---

**Durum:** ✅ ÇÖZÜLDÜ  
**Süre:** ~3 dakika  
**Sonuç:** Blazor uygulamanız çalışıyor! 🎉

---

## 📞 Hala Sorun mu Var?

1. Browser Console (F12) screenshot'u çekin
2. Plesk File Manager'da `httpdocs/` klasör yapısının screenshot'unu alın
3. `/app/PLESK_DOSYA_YAPISI_COZUMU.md` dosyasını okuyun (detaylı rehber)
