-- Payment Tablosuna Yeni Alanlar Ekleme
-- Admin Panel için gerekli field'ları ekler

USE [AppointmentTestDbss]
GO

-- CardType field'ı ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND name = 'CardType')
BEGIN
    ALTER TABLE [dbo].[Payments]
    ADD [CardType] NVARCHAR(50) NULL
    PRINT 'CardType column added successfully'
END
ELSE
BEGIN
    PRINT 'CardType column already exists'
END
GO

-- MaskedCardNumber field'ı ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND name = 'MaskedCardNumber')
BEGIN
    ALTER TABLE [dbo].[Payments]
    ADD [MaskedCardNumber] NVARCHAR(20) NULL
    PRINT 'MaskedCardNumber column added successfully'
END
ELSE
BEGIN
    PRINT 'MaskedCardNumber column already exists'
END
GO

-- PaymentType field'ı ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND name = 'PaymentType')
BEGIN
    ALTER TABLE [dbo].[Payments]
    ADD [PaymentType] NVARCHAR(50) NULL
    PRINT 'PaymentType column added successfully'
END
ELSE
BEGIN
    PRINT 'PaymentType column already exists'
END
GO

-- TransactionId field'ı ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND name = 'TransactionId')
BEGIN
    ALTER TABLE [dbo].[Payments]
    ADD [TransactionId] NVARCHAR(100) NULL
    PRINT 'TransactionId column added successfully'
END
ELSE
BEGIN
    PRINT 'TransactionId column already exists'
END
GO

-- RetryAttempt field'ı ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND name = 'RetryAttempt')
BEGIN
    ALTER TABLE [dbo].[Payments]
    ADD [RetryAttempt] INT NOT NULL DEFAULT 0
    PRINT 'RetryAttempt column added successfully'
END
ELSE
BEGIN
    PRINT 'RetryAttempt column already exists'
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
    
    PRINT 'PaymentDate column updated to NOT NULL'
END
ELSE
BEGIN
    PRINT 'PaymentDate column is already NOT NULL'
END
GO

-- Mevcut kayıtları güncelle
UPDATE [dbo].[Payments]
SET 
    [TransactionId] = COALESCE([TransactionId], [PayTRTransactionId]),
    [RetryAttempt] = COALESCE([RetryAttempt], [RetryCount], 0),
    [PaymentType] = COALESCE([PaymentType], 'Subscription')
WHERE [TransactionId] IS NULL OR [RetryAttempt] IS NULL OR [PaymentType] IS NULL
GO

PRINT 'Payments table updated successfully!'
GO

-- Tabloyu kontrol et
SELECT TOP 5 
    Id,
    BusinessId,
    Amount,
    Status,
    TransactionId,
    PayTRTransactionId,
    PaymentDate,
    CardType,
    MaskedCardNumber,
    PaymentType,
    RetryAttempt,
    RetryCount,
    ErrorMessage
FROM [dbo].[Payments]
ORDER BY CreatedAt DESC
GO
