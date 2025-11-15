-- Email Verification Migration
-- Bu script'i veritabanınızda çalıştırın

USE aptivaplDb;
GO

-- Users tablosuna yeni kolonlar ekle
ALTER TABLE Users
ADD IsEmailVerified BIT NOT NULL DEFAULT 0,
    EmailVerificationToken NVARCHAR(255) NULL,
    EmailVerificationTokenExpiry DATETIME2 NULL;
GO

-- IsActive kolonunun default değerini false yap (yeni kayıtlar için)
ALTER TABLE Users
ADD CONSTRAINT DF_Users_IsActive DEFAULT 0 FOR IsActive;
GO

-- Mevcut kullanıcıları aktif ve email doğrulanmış olarak işaretle (geriye dönük uyumluluk)
UPDATE Users
SET IsEmailVerified = 1,
    IsActive = 1
WHERE EmailVerificationToken IS NULL;
GO

PRINT 'Email verification migration completed successfully!';
GO
