-- PayTR Payment and Subscription Tables Migration
-- Run this script on your SQL Server database

-- Create BusinessSubscriptions Table
CREATE TABLE [dbo].[BusinessSubscriptions](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [BusinessId] [int] NOT NULL,
    [PayTRUserToken] [nvarchar](200) NULL,
    [PayTRCardToken] [nvarchar](200) NULL,
    [CardBrand] [nvarchar](50) NULL,
    [MaskedCardNumber] [nvarchar](20) NULL,
    [MonthlyAmount] [decimal](18, 2) NOT NULL,
    [Currency] [nvarchar](10) NOT NULL,
    [SubscriptionStatus] [nvarchar](50) NOT NULL,
    [NextBillingDate] [datetime2](7) NULL,
    [LastBillingDate] [datetime2](7) NULL,
    [SubscriptionStartDate] [datetime2](7) NULL,
    [SubscriptionEndDate] [datetime2](7) NULL,
    [IsActive] [bit] NOT NULL,
    [CreatedAt] [datetime2](7) NOT NULL,
    [UpdatedAt] [datetime2](7) NOT NULL,
    CONSTRAINT [PK_BusinessSubscriptions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BusinessSubscriptions_Businesses_BusinessId] FOREIGN KEY([BusinessId])
        REFERENCES [dbo].[Businesses] ([Id])
        ON DELETE CASCADE
)
GO

-- Create Payments Table
CREATE TABLE [dbo].[Payments](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [BusinessId] [int] NOT NULL,
    [MerchantOid] [nvarchar](100) NOT NULL,
    [Amount] [decimal](18, 2) NOT NULL,
    [Currency] [nvarchar](10) NOT NULL,
    [Status] [nvarchar](50) NOT NULL,
    [PaymentDate] [datetime2](7) NULL,
    [RetryCount] [int] NOT NULL,
    [NextRetryDate] [datetime2](7) NULL,
    [MaxRetries] [int] NOT NULL,
    [ErrorMessage] [nvarchar](1000) NULL,
    [PayTRResponse] [nvarchar](max) NULL,
    [PayTRTransactionId] [nvarchar](50) NULL,
    [CreatedAt] [datetime2](7) NOT NULL,
    [UpdatedAt] [datetime2](7) NOT NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Payments_Businesses_BusinessId] FOREIGN KEY([BusinessId])
        REFERENCES [dbo].[Businesses] ([Id])
)
GO

-- Create Indexes
CREATE UNIQUE NONCLUSTERED INDEX [IX_BusinessSubscriptions_BusinessId] 
    ON [dbo].[BusinessSubscriptions]([BusinessId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Payments_BusinessId] 
    ON [dbo].[Payments]([BusinessId] ASC)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Payments_MerchantOid] 
    ON [dbo].[Payments]([MerchantOid] ASC)
GO

PRINT 'PayTR Payment and Subscription tables created successfully!'
