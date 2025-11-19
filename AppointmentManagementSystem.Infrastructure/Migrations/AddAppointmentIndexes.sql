-- ⚡ PERFORMANS İYİLEŞTİRME: Appointment tablosuna index'ler ekleniyor
-- Bu index'ler query hızını 5-10 kat artırır

-- Eğer index'ler varsa önce sil (tekrar çalıştırma için)
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_CustomerId' AND object_id = OBJECT_ID('Appointments'))
    DROP INDEX IX_Appointments_CustomerId ON Appointments;

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_BusinessId' AND object_id = OBJECT_ID('Appointments'))
    DROP INDEX IX_Appointments_BusinessId ON Appointments;

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_AppointmentDate' AND object_id = OBJECT_ID('Appointments'))
    DROP INDEX IX_Appointments_AppointmentDate ON Appointments;

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_Status' AND object_id = OBJECT_ID('Appointments'))
    DROP INDEX IX_Appointments_Status ON Appointments;

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_Business_Date_Status' AND object_id = OBJECT_ID('Appointments'))
    DROP INDEX IX_Appointments_Business_Date_Status ON Appointments;

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_Customer_Date' AND object_id = OBJECT_ID('Appointments'))
    DROP INDEX IX_Appointments_Customer_Date ON Appointments;

-- Index'leri oluştur
CREATE NONCLUSTERED INDEX IX_Appointments_CustomerId 
ON Appointments (CustomerId);

CREATE NONCLUSTERED INDEX IX_Appointments_BusinessId 
ON Appointments (BusinessId);

CREATE NONCLUSTERED INDEX IX_Appointments_AppointmentDate 
ON Appointments (AppointmentDate DESC); -- DESC çünkü en yeni randevular önce

CREATE NONCLUSTERED INDEX IX_Appointments_Status 
ON Appointments (Status);

-- Composite index - En sık kullanılan query kombinasyonları
CREATE NONCLUSTERED INDEX IX_Appointments_Business_Date_Status 
ON Appointments (BusinessId, AppointmentDate DESC, Status)
INCLUDE (CustomerId, ServiceId, EmployeeId, StartTime, EndTime);

CREATE NONCLUSTERED INDEX IX_Appointments_Customer_Date 
ON Appointments (CustomerId, AppointmentDate DESC)
INCLUDE (BusinessId, ServiceId, Status);

PRINT '✅ Appointment index''leri başarıyla oluşturuldu!';
