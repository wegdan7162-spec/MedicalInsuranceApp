-- شغّلي هذا السكريبت في SSMS على قاعدة MedicalInsuranceApp1
-- يسجّل الـ migrations المطبقة مسبقاً حتى لا يعيد EF Core تطبيقها
USE MedicalInsuranceApp1;

-- تأكدي من وجود الجدول
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE __EFMigrationsHistory (
        MigrationId    NVARCHAR(150) NOT NULL PRIMARY KEY,
        ProductVersion NVARCHAR(32)  NOT NULL
    );
END

-- سجّل كل migration موجود في الكود وتم تطبيقه يدوياً
INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
SELECT MigrationId, '8.0.0' FROM (VALUES
    ('20260616110000_SeedMarketerCommissions'),
    ('20260616120000_FixMarketerCommissions'),
    ('20260616130000_AddBanksModule'),
    ('20260616200000_AddCodeToBanksAndBranches'),
    ('20260617000000_AddCommissionRecordExtraFields')
) AS T(MigrationId)
WHERE NOT EXISTS (
    SELECT 1 FROM __EFMigrationsHistory h WHERE h.MigrationId = T.MigrationId
);

SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId;
