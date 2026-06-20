-- تشغيلي هذا السكريبت على قاعدة البيانات: MedicalInsuranceApp1
USE MedicalInsuranceApp1;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TblSuppliers')
BEGIN
    CREATE TABLE TblSuppliers (
        Id             INT IDENTITY(1,1) PRIMARY KEY,
        Name           NVARCHAR(300) NOT NULL,
        Phone          NVARCHAR(50)  NULL,
        Email          NVARCHAR(200) NULL,
        Address        NVARCHAR(500) NULL,
        ContractNumber NVARCHAR(100) NULL,
        EntityType     NVARCHAR(50)  NULL,
        Notes          NVARCHAR(500) NULL,
        IsActive       BIT           NOT NULL DEFAULT 1,
        UserId         NVARCHAR(450) NULL,
        CreatedAt      DATETIME2     NOT NULL DEFAULT GETDATE(),
        Del            BIT           NULL,
        DeletedAt      DATETIME2     NULL,
        DeletedBy      NVARCHAR(256) NULL,
        DeleteReason   NVARCHAR(MAX) NULL
    );
    PRINT 'تم إنشاء جدول TblSuppliers بنجاح';
END
ELSE
    PRINT 'الجدول موجود مسبقاً';

-- تسجيل الـ migration
IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20260618000000_AddSuppliersTable')
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20260618000000_AddSuppliersTable', '8.0.0');
