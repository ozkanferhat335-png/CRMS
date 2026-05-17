# CRMS - Gelişmiş CRM Uygulaması

Küçük ve orta ölçekli işletmeler için Windows masaüstü CRM uygulaması.

## 🎯 Proje Özellikleri

- **Teknoloji**: C# 7.3 + Windows Forms + SQLite
- **Mimari**: Katmanlı Mimari (UI/Business/Data Access/Entity)
- **Veritabanı**: SQLite
- **Platform**: Windows Desktop

## 📋 Ana Modüller

- ✅ Giriş Sistemi (Login/Rol Yönetimi)
- ✅ Dashboard (Gerçek zamanlı veri, grafikler)
- ✅ Müşteri Yönetimi
- ✅ İletişim ve Görüşme Yönetimi
- ✅ Satış Yönetimi
- ✅ Teklif Yönetimi
- ✅ Görev Yönetimi
- ✅ Takvim Sistemi
- ✅ Destek Talep Sistemi
- ✅ Bildirim Sistemi
- ✅ Raporlama Sistemi
- ✅ Log ve Audit Sistemi

## 🏗️ Proje Yapısı

```
CRMS/
├── CRMS.Entity/              # Model sınıfları
├── CRMS.DataAccess/          # Veri erişim katmanı (Repository)
├── CRMS.Business/            # İş mantığı katmanı
├── CRMS.Common/              # Ortak sınıflar ve utilities
├── CRMS.UI/                  # Windows Forms arayüzü
└── CRMS.Database/            # SQLite şemaları
```

## 🔐 Güvenlik

- SHA256 parola şifreleme
- Rol bazlı yetkilendirme (RBAC)
- SQL Injection koruması
- Input validasyonları
- Audit sistemi

## 📦 Kullanılan NuGet Paketleri

- SQLite
- Guna.UI2 (Modern UI)
- FontAwesome.Sharp (İkonlar)
- Serilog (Logging)
- Newtonsoft.Json

## 👥 Kullanıcı Rolleri

1. **Sistem Yöneticisi** - Tam erişim
2. **Satış Temsilcisi** - Satış ve müşteri yönetimi
3. **Destek Personeli** - Destek talepleri
4. **Yönetici** - Raporlama ve analiz

## 📝 Lisans

MIT License

## 👨‍💻 Geliştirici

ozkanferhat335-png
