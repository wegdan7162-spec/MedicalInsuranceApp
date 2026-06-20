-- ============================================================
-- تطبيق التغييرات الناقصة على قاعدة البيانات
-- شغّل هذا السكريبت في SQL Server Management Studio
-- ============================================================

-- ── 1. أعمدة جديدة على TblInsuranceContracts ─────────────
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblInsuranceContracts') AND name = 'Phone')
    ALTER TABLE TblInsuranceContracts ADD Phone nvarchar(50) NULL;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblInsuranceContracts') AND name = 'Address')
    ALTER TABLE TblInsuranceContracts ADD Address nvarchar(500) NULL;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblInsuranceContracts') AND name = 'AuthorizedSignatory')
    ALTER TABLE TblInsuranceContracts ADD AuthorizedSignatory nvarchar(300) NULL;

-- ── 2. أعمدة جديدة على TblReceiptVouchers ────────────────
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblReceiptVouchers') AND name = 'Department')
    ALTER TABLE TblReceiptVouchers ADD Department nvarchar(200) NULL;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblReceiptVouchers') AND name = 'Office')
    ALTER TABLE TblReceiptVouchers ADD Office nvarchar(200) NULL;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblReceiptVouchers') AND name = 'BankBranchName')
    ALTER TABLE TblReceiptVouchers ADD BankBranchName nvarchar(200) NULL;

-- ── 3. جدول المؤمن عليهم ─────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID('TblInsuredEmployees') AND type = 'U')
BEGIN
    CREATE TABLE TblInsuredEmployees (
        Id                INT IDENTITY(1,1) PRIMARY KEY,
        ContractId        INT NOT NULL,
        FullName          NVARCHAR(400) NOT NULL,
        Specialization    NVARCHAR(200) NULL,
        Phone             NVARCHAR(30)  NULL,
        MonthlySalary     DECIMAL(18,3) NOT NULL DEFAULT 0,
        EntityRate        DECIMAL(5,4)  NOT NULL DEFAULT 0.03,
        EmployeeRate      DECIMAL(5,4)  NOT NULL DEFAULT 0.02,
        EntityShare       DECIMAL(18,3) NOT NULL DEFAULT 0,
        EmployeeShare     DECIMAL(18,3) NOT NULL DEFAULT 0,
        TotalPremium      DECIMAL(18,3) NOT NULL DEFAULT 0,
        UserId            NVARCHAR(450) NULL,
        CreatedAt         DATETIME2     NOT NULL DEFAULT GETDATE(),
        Del               BIT           NULL DEFAULT 0,
        DeletedAt         DATETIME2     NULL,
        DeletedBy         NVARCHAR(450) NULL,
        DeleteReason      NVARCHAR(500) NULL,
        CONSTRAINT FK_TblInsuredEmployees_Contract
            FOREIGN KEY (ContractId) REFERENCES TblInsuranceContracts(Id) ON DELETE CASCADE
    );
END

-- ── 4. جدول قسائم الإيداع ────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID('TblBankDepositSlips') AND type = 'U')
BEGIN
    CREATE TABLE TblBankDepositSlips (
        Id                INT IDENTITY(1,1) PRIMARY KEY,
        SlipNumber        NVARCHAR(100) NULL,
        DepositDate       DATETIME2     NOT NULL,
        ClientName        NVARCHAR(300) NOT NULL,
        BankAccountId     INT           NULL,
        Amount            DECIMAL(18,3) NOT NULL DEFAULT 0,
        CheckCount        INT           NOT NULL DEFAULT 0,
        BeneficiaryBranch NVARCHAR(200) NULL,
        DueDate           DATETIME2     NULL,
        PaymentMethod     NVARCHAR(30)  NOT NULL DEFAULT N'صك',
        ReceiptVoucherId  INT           NULL,
        Notes             NVARCHAR(500) NULL,
        UserId            NVARCHAR(450) NULL,
        CreatedAt         DATETIME2     NOT NULL DEFAULT GETDATE(),
        Del               BIT           NULL DEFAULT 0,
        DeletedAt         DATETIME2     NULL,
        DeletedBy         NVARCHAR(450) NULL,
        DeleteReason      NVARCHAR(500) NULL,
        CONSTRAINT FK_TblBankDepositSlips_BankAccount
            FOREIGN KEY (BankAccountId) REFERENCES TblBankAccounts(Id) ON DELETE SET NULL,
        CONSTRAINT FK_TblBankDepositSlips_ReceiptVoucher
            FOREIGN KEY (ReceiptVoucherId) REFERENCES TblReceiptVouchers(Id) ON DELETE SET NULL
    );
END

-- ── 5. جدول القيود المحاسبية ──────────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID('TblAccountingEntries') AND type = 'U')
BEGIN
    CREATE TABLE TblAccountingEntries (
        Id                 INT IDENTITY(1,1) PRIMARY KEY,
        EntryNumber        NVARCHAR(50)  NULL,
        EntryDate          DATETIME2     NOT NULL,
        Description        NVARCHAR(500) NULL,
        SupplyOrderId      INT           NULL,
        DebitAmount        DECIMAL(18,3) NOT NULL DEFAULT 0,
        DebitAccount       NVARCHAR(200) NULL,
        BankDepositSlipId  INT           NULL,
        BankDebitAmount    DECIMAL(18,3) NOT NULL DEFAULT 0,
        BankDebitAccount   NVARCHAR(200) NULL,
        Status             NVARCHAR(30)  NOT NULL DEFAULT N'مبدئي',
        UserId             NVARCHAR(450) NULL,
        CreatedAt          DATETIME2     NOT NULL DEFAULT GETDATE(),
        Del                BIT           NULL DEFAULT 0,
        DeletedAt          DATETIME2     NULL,
        DeletedBy          NVARCHAR(450) NULL,
        DeleteReason       NVARCHAR(500) NULL,
        CONSTRAINT FK_TblAccountingEntries_SupplyOrder
            FOREIGN KEY (SupplyOrderId) REFERENCES TblSupplyOrders(Id) ON DELETE SET NULL,
        CONSTRAINT FK_TblAccountingEntries_BankDepositSlip
            FOREIGN KEY (BankDepositSlipId) REFERENCES TblBankDepositSlips(Id) ON DELETE SET NULL
    );
END

-- ── 6. جدول بنود القيود المحاسبية ────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID('TblAccountingEntryLines') AND type = 'U')
BEGIN
    CREATE TABLE TblAccountingEntryLines (
        Id              INT IDENTITY(1,1) PRIMARY KEY,
        EntryId         INT           NOT NULL,
        LineOrder       INT           NOT NULL DEFAULT 0,
        AccountName     NVARCHAR(300) NOT NULL,
        LineDescription NVARCHAR(500) NULL,
        Debit           DECIMAL(18,3) NOT NULL DEFAULT 0,
        Credit          DECIMAL(18,3) NOT NULL DEFAULT 0,
        CONSTRAINT FK_TblAccountingEntryLines_Entry
            FOREIGN KEY (EntryId) REFERENCES TblAccountingEntries(Id) ON DELETE CASCADE
    );
    CREATE INDEX IX_TblAccountingEntryLines_EntryId ON TblAccountingEntryLines(EntryId);
END

-- ── 7. جدول اليومية ──────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID('TblDailyJournals') AND type = 'U')
BEGIN
    CREATE TABLE TblDailyJournals (
        Id           INT IDENTITY(1,1) PRIMARY KEY,
        JournalDate  DATETIME2      NOT NULL,
        TotalAmount  DECIMAL(18,3)  NOT NULL DEFAULT 0,
        Status       NVARCHAR(30)   NOT NULL DEFAULT N'قيد المراجعة',
        ReviewNotes  NVARCHAR(1000) NULL,
        ApprovedAt   DATETIME2      NULL,
        ApprovedBy   NVARCHAR(200)  NULL,
        UserId       NVARCHAR(450)  NULL,
        CreatedAt    DATETIME2      NOT NULL DEFAULT GETDATE(),
        Del          BIT            NULL DEFAULT 0,
        DeletedAt    DATETIME2      NULL,
        DeletedBy    NVARCHAR(450)  NULL,
        DeleteReason NVARCHAR(500)  NULL
    );
END

-- ── 8. جدول بنود اليومية ─────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID('TblDailyJournalItems') AND type = 'U')
BEGIN
    CREATE TABLE TblDailyJournalItems (
        Id                 INT IDENTITY(1,1) PRIMARY KEY,
        JournalId          INT           NOT NULL,
        SupplyOrderId      INT           NULL,
        ReceiptVoucherId   INT           NULL,
        BankDepositSlipId  INT           NULL,
        AccountingEntryId  INT           NULL,
        Amount             DECIMAL(18,3) NOT NULL DEFAULT 0,
        CONSTRAINT FK_TblDailyJournalItems_Journal
            FOREIGN KEY (JournalId) REFERENCES TblDailyJournals(Id) ON DELETE CASCADE,
        CONSTRAINT FK_TblDailyJournalItems_SupplyOrder
            FOREIGN KEY (SupplyOrderId) REFERENCES TblSupplyOrders(Id) ON DELETE SET NULL,
        CONSTRAINT FK_TblDailyJournalItems_ReceiptVoucher
            FOREIGN KEY (ReceiptVoucherId) REFERENCES TblReceiptVouchers(Id) ON DELETE SET NULL,
        CONSTRAINT FK_TblDailyJournalItems_BankDepositSlip
            FOREIGN KEY (BankDepositSlipId) REFERENCES TblBankDepositSlips(Id) ON DELETE SET NULL,
        CONSTRAINT FK_TblDailyJournalItems_AccountingEntry
            FOREIGN KEY (AccountingEntryId) REFERENCES TblAccountingEntries(Id) ON DELETE SET NULL
    );
END

-- ── 9. عمود SupplierId على TblSupplyOrders ───────────────
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblSupplyOrders') AND name = 'SupplierId')
BEGIN
    ALTER TABLE TblSupplyOrders ADD SupplierId INT NULL;
    ALTER TABLE TblSupplyOrders ADD CONSTRAINT FK_TblSupplyOrders_Supplier
        FOREIGN KEY (SupplierId) REFERENCES TblSuppliers(Id) ON DELETE SET NULL;
END

-- ── حقل مبلغ الإيداع المصرفي في إيصالات القبض ─────────────
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblReceiptVouchers') AND name = 'BankDepositAmount')
    ALTER TABLE TblReceiptVouchers ADD BankDepositAmount DECIMAL(18,3) NULL;

PRINT N'تم تطبيق جميع التغييرات بنجاح ✓';
