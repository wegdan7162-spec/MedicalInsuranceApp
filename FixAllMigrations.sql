-- =====================================================
-- شغّلي هذا السكريبت في SSMS على قاعدة MedicalInsuranceApp1
-- يسجّل جميع الـ migrations الموجودة في الكود حتى لا يعيد EF Core تطبيقها
-- =====================================================
USE MedicalInsuranceApp1;

-- تأكد من وجود جدول التاريخ
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE __EFMigrationsHistory (
        MigrationId    NVARCHAR(150) NOT NULL PRIMARY KEY,
        ProductVersion NVARCHAR(32)  NOT NULL
    );
    PRINT 'تم إنشاء جدول __EFMigrationsHistory';
END

-- سجّل جميع الـ migrations (INSERT WHERE NOT EXISTS لتجنب التكرار)
INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
SELECT MigrationId, '8.0.0' FROM (VALUES
    ('20260529200715_InitialIdentitySetup'),
    ('20260529202109_FixOptionalRelations'),
    ('20260529202344_FixPlaintiffOptional'),
    ('20260529214111_ResetAdmin'),
    ('20260530144434_AddClaimStatusToCourtCase'),
    ('20260531000000_AddClaimTypeToCourtCase'),
    ('20260531000001_AddPlaceIncidentDateFileStatusToCourtCase'),
    ('20260601000000_AddDeleteFieldsToAllTables'),
    ('20260603000000_AddClaimTypeToOutterClaim'),
    ('20260603000001_AddRolePermissions'),
    ('20260605120000_RemoveClaimTypeFromOutterClaim'),
    ('20260606000000_NormalizeClaimStatus'),
    ('20260606000001_NormalizeClaimStatusAllValues'),
    ('20260606000002_AddClaimTypeToFriendly'),
    ('20260607224314_AddUserCustomFields'),
    ('20260610000000_AddSettlementTables'),
    ('20260614161518_AddFinancialModule'),
    ('20260615000000_AddUnderwritingModule'),
    ('20260615075855_AddMarketersModule'),
    ('20260615160342_AddBanksModule'),
    ('20260616000000_AddMarketersModule'),
    ('20260616095650_AddSuppliersTable'),
    ('20260616100000_SeedMarketers'),
    ('20260616110000_SeedMarketerCommissions'),
    ('20260616120000_FixMarketerCommissions'),
    ('20260616130000_AddBanksModule'),
    ('20260616200000_AddCodeToBanksAndBranches'),
    ('20260617000000_AddCommissionRecordExtraFields')
) AS T(MigrationId)
WHERE NOT EXISTS (
    SELECT 1 FROM __EFMigrationsHistory h WHERE h.MigrationId = T.MigrationId
);

PRINT 'تم تسجيل جميع الـ migrations';

-- تأكدي أن جدول TblSuppliers موجود (إذا لم يكن موجوداً)
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
        UserId         NVARCHAR(MAX) NULL,
        CreatedAt      DATETIME2     NOT NULL DEFAULT GETDATE(),
        Del            BIT           NULL,
        DeletedAt      DATETIME2     NULL,
        DeletedBy      NVARCHAR(MAX) NULL,
        DeleteReason   NVARCHAR(MAX) NULL
    );
    PRINT 'تم إنشاء جدول TblSuppliers';
END
ELSE
    PRINT 'جدول TblSuppliers موجود مسبقاً';

-- تأكدي من وجود أعمدة BankCode و BranchCode
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblBanks') AND name = 'BankCode')
BEGIN
    ALTER TABLE TblBanks ADD BankCode NVARCHAR(50) NULL;
    PRINT 'تم إضافة عمود BankCode';
END
ELSE
    PRINT 'عمود BankCode موجود مسبقاً';

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblBankBranches') AND name = 'BranchCode')
BEGIN
    ALTER TABLE TblBankBranches ADD BranchCode NVARCHAR(50) NULL;
    PRINT 'تم إضافة عمود BranchCode';
END
ELSE
    PRINT 'عمود BranchCode موجود مسبقاً';

-- عرض الحالة النهائية
SELECT MigrationId FROM __EFMigrationsHistory ORDER BY MigrationId;
PRINT 'انتهى السكريبت بنجاح — الآن شغّلي Update-Database في VS';
