-- Admin Panel için Tüm Database Migration'ları
-- Bu dosya tüm gerekli field'ları ekler
-- Kullanım: sqlcmd -S sadik -d AppointmentTestDbss -i AdminPanel_Complete_Migration.sql

USE [AppointmentTestDbss]
GO

PRINT '=========================================='
PRINT 'ADMIN PANEL MIGRATION BAŞLIYOR...'
PRINT '=========================================='
GO

-- ==========================================
-- BÖLÜM 1: BusinessSubscriptions Tablosu
-- ==========================================
PRINT ''
PRINT 'BÖLÜM 1: BusinessSubscriptions Güncelleniyor...'
GO

-- Status field'ı ekle (SubscriptionStatus'a ek olarak)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BusinessSubscriptions]') AND name = 'Status')
BEGIN
    ALTER TABLE [dbo].[BusinessSubscriptions]
    ADD [Status] NVARCHAR(50) NOT NULL DEFAULT 'Active'
    PRINT '✓ Status column eklendi'
END
ELSE
BEGIN
    PRINT '○ Status column zaten mevcut'
END
GO

-- CardType field'ı ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BusinessSubscriptions]') AND name = 'CardType')
BEGIN
    ALTER TABLE [dbo].[BusinessSubscriptions]
    ADD [CardType] NVARCHAR(50) NULL
    PRINT '✓ CardType column eklendi'
END
ELSE
BEGIN
    PRINT '○ CardType column zaten mevcut'
END
GO

-- CardLastFourDigits field'ı ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BusinessSubscriptions]') AND name = 'CardLastFourDigits')
BEGIN
    ALTER TABLE [dbo].[BusinessSubscriptions]
    ADD [CardLastFourDigits] NVARCHAR(10) NULL
    PRINT '✓ CardLastFourDigits column eklendi'
END
ELSE
BEGIN
    PRINT '○ CardLastFourDigits column zaten mevcut'
END
GO

-- StartDate field'ı ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BusinessSubscriptions]') AND name = 'StartDate')
BEGIN
    ALTER TABLE [dbo].[BusinessSubscriptions]
    ADD [StartDate] DATETIME2 NULL
    PRINT '✓ StartDate column eklendi'
END
ELSE
BEGIN
    PRINT '○ StartDate column zaten mevcut'
END
GO

-- AutoRenewal field'ı ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BusinessSubscriptions]') AND name = 'AutoRenewal')
BEGIN
    ALTER TABLE [dbo].[BusinessSubscriptions]
    ADD [AutoRenewal] BIT NOT NULL DEFAULT 1
    PRINT '✓ AutoRenewal column eklendi'
END
ELSE
BEGIN
    PRINT '○ AutoRenewal column zaten mevcut'
END
GO

-- BusinessSubscriptions mevcut kayıtlarını güncelle
UPDATE [dbo].[BusinessSubscriptions]
SET 
    [Status] = [SubscriptionStatus],
    [CardType] = [CardBrand],
    [CardLastFourDigits] = RIGHT([MaskedCardNumber], 4),
    [StartDate] = [SubscriptionStartDate],
    [AutoRenewal] = 1
WHERE [Status] IS NULL OR [CardType] IS NULL
GO

PRINT '✓ BusinessSubscriptions tablosu güncellendi!'
GO

-- ==========================================
-- BÖLÜM 2: Payments Tablosu
-- ==========================================
PRINT ''
PRINT 'BÖLÜM 2: Payments Güncelleniyor...'
GO

-- CardType field'ı ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND name = 'CardType')
BEGIN
    ALTER TABLE [dbo].[Payments]
    ADD [CardType] NVARCHAR(50) NULL
    PRINT '✓ CardType column eklendi'
END
ELSE
BEGIN
    PRINT '○ CardType column zaten mevcut'
END
GO

-- MaskedCardNumber field'ı ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND name = 'MaskedCardNumber')
BEGIN
    ALTER TABLE [dbo].[Payments]
    ADD [MaskedCardNumber] NVARCHAR(20) NULL
    PRINT '✓ MaskedCardNumber column eklendi'
END
ELSE
BEGIN
    PRINT '○ MaskedCardNumber column zaten mevcut'
END
GO

-- PaymentType field'ı ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND name = 'PaymentType')
BEGIN
    ALTER TABLE [dbo].[Payments]
    ADD [PaymentType] NVARCHAR(50) NULL
    PRINT '✓ PaymentType column eklendi'
END
ELSE
BEGIN
    PRINT '○ PaymentType column zaten mevcut'
END
GO

-- TransactionId field'ı ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND name = 'TransactionId')
BEGIN
    ALTER TABLE [dbo].[Payments]
    ADD [TransactionId] NVARCHAR(100) NULL
    PRINT '✓ TransactionId column eklendi'
END
ELSE
BEGIN
    PRINT '○ TransactionId column zaten mevcut'
END
GO

-- RetryAttempt field'ı ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND name = 'RetryAttempt')
BEGIN
    ALTER TABLE [dbo].[Payments]
    ADD [RetryAttempt] INT NOT NULL DEFAULT 0
    PRINT '✓ RetryAttempt column eklendi'
END
ELSE
BEGIN
    PRINT '○ RetryAttempt column zaten mevcut'
END
GO

-- PaymentDate'i NOT NULL yap (eğer nullable ise)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND name = 'PaymentDate' AND is_nullable = 1)
BEGIN
    -- Önce NULL değerleri güncelle
    UPDATE [dbo].[Payments]
    SET [PaymentDate] = COALESCE([PaymentDate], [CreatedAt], GETDATE())
    WHERE [PaymentDate] IS NULL
    
    -- Sonra NOT NULL constraint ekle
    ALTER TABLE [dbo].[Payments]
    ALTER COLUMN [PaymentDate] DATETIME2 NOT NULL
    
    PRINT '✓ PaymentDate column NOT NULL yapıldı'
END
ELSE
BEGIN
    PRINT '○ PaymentDate column zaten NOT NULL'
END
GO

-- Payments mevcut kayıtlarını güncelle
UPDATE [dbo].[Payments]
SET 
    [TransactionId] = COALESCE([TransactionId], [PayTRTransactionId]),
    [RetryAttempt] = COALESCE([RetryAttempt], [RetryCount], 0),
    [PaymentType] = COALESCE([PaymentType], 'Subscription')
WHERE [TransactionId] IS NULL OR [RetryAttempt] IS NULL OR [PaymentType] IS NULL
GO

PRINT '✓ Payments tablosu güncellendi!'
GO

-- ==========================================
-- KONTROL VE DOĞRULAMA
-- ==========================================
PRINT ''
PRINT '=========================================='
PRINT 'KONTROL VE DOĞRULAMA'
PRINT '=========================================='
GO

PRINT ''
PRINT 'BusinessSubscriptions - Yeni Kolonlar:'
SELECT 
    COL.name AS ColumnName,
    TYP.name AS DataType,
    COL.max_length AS MaxLength,
    COL.is_nullable AS IsNullable
FROM sys.columns COL
INNER JOIN sys.types TYP ON COL.user_type_id = TYP.user_type_id
WHERE COL.object_id = OBJECT_ID(N'[dbo].[BusinessSubscriptions]')
    AND COL.name IN ('Status', 'CardType', 'CardLastFourDigits', 'StartDate', 'AutoRenewal')
ORDER BY COL.name
GO

PRINT ''
PRINT 'Payments - Yeni Kolonlar:'
SELECT 
    COL.name AS ColumnName,
    TYP.name AS DataType,
    COL.max_length AS MaxLength,
    COL.is_nullable AS IsNullable
FROM sys.columns COL
INNER JOIN sys.types TYP ON COL.user_type_id = TYP.user_type_id
WHERE COL.object_id = OBJECT_ID(N'[dbo].[Payments]')
    AND COL.name IN ('CardType', 'MaskedCardNumber', 'PaymentType', 'TransactionId', 'RetryAttempt', 'PaymentDate')
ORDER BY COL.name
GO

PRINT ''
PRINT '=========================================='
PRINT 'MIGRATION BAŞARIYLA TAMAMLANDI! ✓'
PRINT '=========================================='
GO
