-- BusinessSubscription Tablosuna Yeni Alanlar Ekleme
-- Admin Panel için gerekli field'ları ekler

USE [AppointmentTestDbss]
GO

-- Status field'ı ekle (SubscriptionStatus'a ek olarak)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BusinessSubscriptions]') AND name = 'Status')
BEGIN
    ALTER TABLE [dbo].[BusinessSubscriptions]
    ADD [Status] NVARCHAR(50) NOT NULL DEFAULT 'Active'
    PRINT 'Status column added successfully'
END
ELSE
BEGIN
    PRINT 'Status column already exists'
END
GO

-- CardType field'ı ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BusinessSubscriptions]') AND name = 'CardType')
BEGIN
    ALTER TABLE [dbo].[BusinessSubscriptions]
    ADD [CardType] NVARCHAR(50) NULL
    PRINT 'CardType column added successfully'
END
ELSE
BEGIN
    PRINT 'CardType column already exists'
END
GO

-- CardLastFourDigits field'ı ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BusinessSubscriptions]') AND name = 'CardLastFourDigits')
BEGIN
    ALTER TABLE [dbo].[BusinessSubscriptions]
    ADD [CardLastFourDigits] NVARCHAR(10) NULL
    PRINT 'CardLastFourDigits column added successfully'
END
ELSE
BEGIN
    PRINT 'CardLastFourDigits column already exists'
END
GO

-- StartDate field'ı ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BusinessSubscriptions]') AND name = 'StartDate')
BEGIN
    ALTER TABLE [dbo].[BusinessSubscriptions]
    ADD [StartDate] DATETIME2 NULL
    PRINT 'StartDate column added successfully'
END
ELSE
BEGIN
    PRINT 'StartDate column already exists'
END
GO

-- AutoRenewal field'ı ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BusinessSubscriptions]') AND name = 'AutoRenewal')
BEGIN
    ALTER TABLE [dbo].[BusinessSubscriptions]
    ADD [AutoRenewal] BIT NOT NULL DEFAULT 1
    PRINT 'AutoRenewal column added successfully'
END
ELSE
BEGIN
    PRINT 'AutoRenewal column already exists'
END
GO

-- Mevcut kayıtları güncelle
UPDATE [dbo].[BusinessSubscriptions]
SET 
    [Status] = [SubscriptionStatus],
    [CardType] = [CardBrand],
    [CardLastFourDigits] = RIGHT([MaskedCardNumber], 4),
    [StartDate] = [SubscriptionStartDate],
    [AutoRenewal] = 1
WHERE [Status] IS NULL OR [CardType] IS NULL
GO

PRINT 'BusinessSubscriptions table updated successfully!'
GO

-- Tabloyu kontrol et
SELECT TOP 5 
    Id,
    BusinessId,
    Status,
    SubscriptionStatus,
    CardType,
    CardBrand,
    CardLastFourDigits,
    MaskedCardNumber,
    StartDate,
    SubscriptionStartDate,
    AutoRenewal,
    NextBillingDate
FROM [dbo].[BusinessSubscriptions]
GO
