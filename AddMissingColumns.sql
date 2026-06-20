-- تشغيلي هذا السكريبت على قاعدة البيانات: MedicalInsuranceApp1
USE MedicalInsuranceApp1;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblMarketerCommissionRecords') AND name = 'SupplyOrderNumbers')
    ALTER TABLE TblMarketerCommissionRecords ADD SupplyOrderNumbers NVARCHAR(500) NULL;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblMarketerCommissionRecords') AND name = 'ReceiptNumbers')
    ALTER TABLE TblMarketerCommissionRecords ADD ReceiptNumbers NVARCHAR(500) NULL;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblMarketerCommissionRecords') AND name = 'PreparedBy')
    ALTER TABLE TblMarketerCommissionRecords ADD PreparedBy NVARCHAR(200) NULL;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblMarketerCommissionRecords') AND name = 'ReviewedBy')
    ALTER TABLE TblMarketerCommissionRecords ADD ReviewedBy NVARCHAR(200) NULL;

-- تسجيل الـ migration في جدول التاريخ لتجنب إعادة التطبيق
IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20260617000000_AddCommissionRecordExtraFields')
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20260617000000_AddCommissionRecordExtraFields', '8.0.0');

PRINT 'تم إضافة الأعمدة بنجاح';
