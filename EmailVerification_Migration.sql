-- Email Verification Migration
-- Her komut satırını ayrı ayrı çalıştırın

-- 1. Users tablosuna yeni kolonlar ekle
ALTER TABLE Users ADD IsEmailVerified BIT NOT NULL DEFAULT 0;
ALTER TABLE Users ADD EmailVerificationToken NVARCHAR(255) NULL;
ALTER TABLE Users ADD EmailVerificationTokenExpiry DATETIME2 NULL;

-- 2. Mevcut kullanıcıları aktif ve email doğrulanmış olarak işaretle
UPDATE Users SET IsEmailVerified = 1, IsActive = 1 WHERE EmailVerificationToken IS NULL;
