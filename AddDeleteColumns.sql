-- تشغيل هذا السكريبت في SQL Server Management Studio أو View > SQL Server Object Explorer في Visual Studio
-- قاعدة البيانات: MedicalInsuranceApp1

USE MedicalInsuranceApp1;

-- TblBranch
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblBranch') AND name = 'DeletedAt')
    ALTER TABLE TblBranch ADD DeletedAt DATETIME NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblBranch') AND name = 'DeletedBy')
    ALTER TABLE TblBranch ADD DeletedBy NVARCHAR(256) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblBranch') AND name = 'DeleteReason')
    ALTER TABLE TblBranch ADD DeleteReason NVARCHAR(500) NULL;

-- TblCourt1
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblCourt1') AND name = 'Del')
    ALTER TABLE TblCourt1 ADD Del BIT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblCourt1') AND name = 'DeletedAt')
    ALTER TABLE TblCourt1 ADD DeletedAt DATETIME NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblCourt1') AND name = 'DeletedBy')
    ALTER TABLE TblCourt1 ADD DeletedBy NVARCHAR(256) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblCourt1') AND name = 'DeleteReason')
    ALTER TABLE TblCourt1 ADD DeleteReason NVARCHAR(500) NULL;

-- TblPlainitiffName
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblPlainitiffName') AND name = 'DeletedAt')
    ALTER TABLE TblPlainitiffName ADD DeletedAt DATETIME NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblPlainitiffName') AND name = 'DeletedBy')
    ALTER TABLE TblPlainitiffName ADD DeletedBy NVARCHAR(256) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblPlainitiffName') AND name = 'DeleteReason')
    ALTER TABLE TblPlainitiffName ADD DeleteReason NVARCHAR(500) NULL;

-- TblFriendly
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblFriendly') AND name = 'Del')
    ALTER TABLE TblFriendly ADD Del BIT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblFriendly') AND name = 'DeletedAt')
    ALTER TABLE TblFriendly ADD DeletedAt DATETIME NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblFriendly') AND name = 'DeletedBy')
    ALTER TABLE TblFriendly ADD DeletedBy NVARCHAR(256) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblFriendly') AND name = 'DeleteReason')
    ALTER TABLE TblFriendly ADD DeleteReason NVARCHAR(500) NULL;

-- TblOutters
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblOutters') AND name = 'Del')
    ALTER TABLE TblOutters ADD Del BIT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblOutters') AND name = 'DeletedAt')
    ALTER TABLE TblOutters ADD DeletedAt DATETIME NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblOutters') AND name = 'DeletedBy')
    ALTER TABLE TblOutters ADD DeletedBy NVARCHAR(256) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TblOutters') AND name = 'DeleteReason')
    ALTER TABLE TblOutters ADD DeleteReason NVARCHAR(500) NULL;

PRINT 'تم إضافة الأعمدة بنجاح';

-- جداول حركة المستخدمين
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TblUserActivity')
CREATE TABLE TblUserActivity (
    Id          INT IDENTITY PRIMARY KEY,
    UserId      NVARCHAR(450) NOT NULL,
    UserName    NVARCHAR(256) NOT NULL,
    FullName    NVARCHAR(256) NOT NULL,
    Controller  NVARCHAR(100) NOT NULL,
    Action      NVARCHAR(100) NOT NULL,
    PageTitle   NVARCHAR(200) NOT NULL,
    IPAddress   NVARCHAR(50)  NULL,
    VisitedAt   DATETIME      NOT NULL DEFAULT GETDATE()
);

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TblUserSession')
CREATE TABLE TblUserSession (
    Id              INT IDENTITY PRIMARY KEY,
    UserId          NVARCHAR(450) NOT NULL,
    UserName        NVARCHAR(256) NOT NULL,
    FullName        NVARCHAR(256) NOT NULL,
    LoginAt         DATETIME      NOT NULL,
    LogoutAt        DATETIME      NULL,
    IPAddress       NVARCHAR(50)  NULL,
    DurationMinutes INT           NULL
);

PRINT 'تم إنشاء جداول حركة المستخدمين';
