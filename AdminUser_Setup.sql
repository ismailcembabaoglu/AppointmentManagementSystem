-- Admin User Ekleme SQL Script
-- Not: Bu scripti SQL Server Management Studio veya benzeri bir araçla çalıştırın

USE [AppointmentTestDbss]
GO

-- Admin kullanıcısı oluştur
-- Şifre: Admin123! (BCrypt hash: $2a$11$K9Z...)
-- Not: Şifreyi application üzerinden değiştirmeniz önerilir

DECLARE @AdminUserId INT;

-- Önce admin kullanıcısının var olup olmadığını kontrol et
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@appointmentsystem.com')
BEGIN
    -- Admin kullanıcısı ekle
    INSERT INTO Users (
        Name, 
        Email, 
        PasswordHash, 
        Role, 
        PhoneNumber, 
        IsActive, 
        CreatedAt, 
        UpdatedAt
    )
    VALUES (
        'System Admin',
        'admin@appointmentsystem.com',
        '$2a$11$K9ZwVfLrx6ZqnGvVcbQmNe3rZvQqC1fPfCm9VFKCMXGMxjDKJFQgO', -- Admin123!
        'Admin',
        '5555555555',
        1,
        GETDATE(),
        GETDATE()
    );

    SET @AdminUserId = SCOPE_IDENTITY();

    PRINT 'Admin kullanıcısı başarıyla oluşturuldu!';
    PRINT 'Email: admin@appointmentsystem.com';
    PRINT 'Şifre: Admin123!';
    PRINT '';
    PRINT 'GÜVENLİK UYARISI: Lütfen ilk girişten sonra şifrenizi değiştirin!';
END
ELSE
BEGIN
    PRINT 'Admin kullanıcısı zaten mevcut!';
END
GO

-- Admin kullanıcısını görüntüle
SELECT 
    Id,
    Name,
    Email,
    Role,
    PhoneNumber,
    IsActive,
    CreatedAt
FROM Users
WHERE Email = 'admin@appointmentsystem.com';
GO
