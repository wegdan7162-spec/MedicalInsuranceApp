-- ============================================================
-- الجزء المالي — فرع التأمين الطبي
-- شركة ليبيا للتأمين
-- ============================================================
-- شغّلي هذا السكريبت مرة واحدة على SQL Server
-- بعدين شغّلي: add-migration AddFinancialModule  ثم  update-database
-- ============================================================

-- 1. جدول الحسابات البنكية
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TblBankAccounts')
BEGIN
    CREATE TABLE TblBankAccounts (
        Id               INT IDENTITY(1,1) PRIMARY KEY,
        BankName         NVARCHAR(200)     NOT NULL,
        AccountNumber    NVARCHAR(50)      NOT NULL,
        BankBranch       NVARCHAR(100)     NULL,
        OpeningBalance   DECIMAL(18,3)     NOT NULL DEFAULT 0,
        CurrentBalance   DECIMAL(18,3)     NOT NULL DEFAULT 0,
        IsActive         BIT               NOT NULL DEFAULT 1,
        Notes            NVARCHAR(MAX)     NULL,
        Del              BIT               NULL DEFAULT 0,
        DeletedAt        DATETIME2         NULL,
        DeletedBy        NVARCHAR(450)     NULL
    );
    PRINT 'تم إنشاء TblBankAccounts';
END
ELSE
    PRINT 'TblBankAccounts موجود مسبقاً';

-- 2. جدول العمولات (قبل BankTransactions لأنها مرجع)
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TblCommissions')
BEGIN
    CREATE TABLE TblCommissions (
        Id                  INT IDENTITY(1,1) PRIMARY KEY,
        SupplierName        NVARCHAR(300)  NOT NULL,
        PremiumAmount       DECIMAL(18,3)  NOT NULL DEFAULT 0,
        CommissionRate      DECIMAL(5,2)   NOT NULL DEFAULT 5,
        CommissionAmount    DECIMAL(18,3)  NOT NULL DEFAULT 0,
        BeneficiaryName     NVARCHAR(300)  NULL,
        AuthorizationNumber NVARCHAR(50)   NULL,
        EntryNumber         NVARCHAR(50)   NULL,
        PaymentDate         DATETIME2      NULL,
        Status              NVARCHAR(50)   NOT NULL DEFAULT N'قيد الصرف',
        Notes               NVARCHAR(500)  NULL,
        UserId              NVARCHAR(450)  NULL,
        CreatedAt           DATETIME2      NOT NULL DEFAULT GETDATE(),
        Del                 BIT            NULL DEFAULT 0,
        DeletedAt           DATETIME2      NULL,
        DeletedBy           NVARCHAR(450)  NULL,
        DeleteReason        NVARCHAR(500)  NULL
    );
    PRINT 'تم إنشاء TblCommissions';
END
ELSE
    PRINT 'TblCommissions موجود مسبقاً';

-- 3. جدول حركات الحساب البنكي
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TblBankTransactions')
BEGIN
    CREATE TABLE TblBankTransactions (
        Id                INT IDENTITY(1,1) PRIMARY KEY,
        BankAccountId     INT             NOT NULL,
        TransactionDate   DATETIME2       NOT NULL,
        EntryNumber       NVARCHAR(50)    NULL,
        DocType           INT             NULL,   -- enum: 0=صك 1=قسيمة_إيداع 2=حوالة 3=نقدي 4=خصم_تلقائي 5=أخرى
        DocumentNumber    NVARCHAR(100)   NULL,
        Description       NVARCHAR(500)   NULL,
        DebitAmount       DECIMAL(18,3)   NOT NULL DEFAULT 0,
        CreditAmount      DECIMAL(18,3)   NOT NULL DEFAULT 0,
        RunningBalance    DECIMAL(18,3)   NOT NULL DEFAULT 0,
        Category          INT             NOT NULL DEFAULT 6, -- 6=أخرى
        FriendlyClaimId   INT             NULL,
        OutterClaimId     INT             NULL,
        CommissionId      INT             NULL,
        UserId            NVARCHAR(450)   NULL,
        CreatedAt         DATETIME2       NOT NULL DEFAULT GETDATE(),
        Del               BIT             NULL DEFAULT 0,
        DeletedAt         DATETIME2       NULL,
        DeletedBy         NVARCHAR(450)   NULL,
        DeleteReason      NVARCHAR(500)   NULL,

        CONSTRAINT FK_BankTx_Account
            FOREIGN KEY (BankAccountId) REFERENCES TblBankAccounts(Id),

        CONSTRAINT FK_BankTx_FriendlyClaim
            FOREIGN KEY (FriendlyClaimId) REFERENCES TblFriendly(Id)
            ON DELETE SET NULL,

        CONSTRAINT FK_BankTx_OutterClaim
            FOREIGN KEY (OutterClaimId) REFERENCES TblOutters(Id)
            ON DELETE SET NULL,

        CONSTRAINT FK_BankTx_Commission
            FOREIGN KEY (CommissionId) REFERENCES TblCommissions(Id)
            ON DELETE SET NULL
    );

    CREATE INDEX IX_BankTransactions_Date      ON TblBankTransactions(TransactionDate);
    CREATE INDEX IX_BankTransactions_AccountId ON TblBankTransactions(BankAccountId);

    PRINT 'تم إنشاء TblBankTransactions';
END
ELSE
    PRINT 'TblBankTransactions موجود مسبقاً';

-- 4. بيانات أولية: الحسابات البنكية الموجودة من ملفات الاكسل
IF NOT EXISTS (SELECT 1 FROM TblBankAccounts WHERE AccountNumber = '2000002375')
    INSERT INTO TblBankAccounts (BankName, AccountNumber, BankBranch, Notes)
    VALUES (N'مصرف الصحارى', N'2000002375', N'الفرع الرئيسي', N'حساب فرع التأمين الطبي');

IF NOT EXISTS (SELECT 1 FROM TblBankAccounts WHERE AccountNumber = '2000095853')
    INSERT INTO TblBankAccounts (BankName, AccountNumber, BankBranch, Notes)
    VALUES (N'مصرف الصحارى', N'2000095853', N'فرع باب المدينة', N'حساب فرع التأمين الطبي');

IF NOT EXISTS (SELECT 1 FROM TblBankAccounts WHERE AccountNumber = '011001343000016')
    INSERT INTO TblBankAccounts (BankName, AccountNumber, BankBranch, Notes)
    VALUES (N'مصرف الوحدة', N'011001343000016', N'الفرع الرئيسي طرابلس', N'حساب فرع التأمين الطبي');

PRINT '==============================';
PRINT 'اكتملت العملية بنجاح';
PRINT '==============================';

-- ============================================================
-- بعد تشغيل هذا السكريبت شغّلي في Package Manager Console:
--   Add-Migration AddFinancialModule
--   Update-Database
-- ============================================================
