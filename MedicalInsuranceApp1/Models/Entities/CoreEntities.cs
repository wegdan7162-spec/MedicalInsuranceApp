using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalInsuranceApp1.Models.Entities
{
    [Table("TblBranch")]
    public class Branch
    {
        public int Id { get; set; }
        [Required, MaxLength(500)]
        public string BranchName { get; set; } = string.Empty;
        public bool? Del { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }
    }

    [Table("TblCourt1")]
    public class Court
    {
        public int Id { get; set; }
        [Required, MaxLength(500)]
        public string CourtName { get; set; } = string.Empty;
        public bool? Del { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }
    }

    [Table("TblExpenseTypes")]
    public class ExpenseType
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string ExpenseName { get; set; } = string.Empty;
    }

    [Table("TblPlainitiffName")]
    public class PlaintiffName
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string PlainitiffName { get; set; } = string.Empty;
        public string? Job { get; set; }
        public long? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }
    }

    [Table("TblIncidentPlace")]
    public class IncidentPlace
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string IncidentPlaceName { get; set; } = string.Empty;
        public long Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public bool Del { get; set; } = false;
        public string? QRCPic { get; set; }
        public int BranchId { get; set; }
        public int CourtsId { get; set; }
    }

    

    [Table("TblFriendly")]
    public class FriendlyClaim
    {
        public  int Id { get; set; }
        public long Num { get; set; }
        public int Year { get; set; }
        public int? PlaintiffNameId { get; set; }
        public int? BranchId { get; set; }
        public double Setteld { get; set; }
        public double Reserve { get; set; }
        public string? Place { get; set; }
        [MaxLength(1000)] public string? Note { get; set; }
        public string? Subject { get; set; }
        public DateTime RegDate { get; set; }
        public string? FileName { get; set; }
        public string? FileExtension { get; set; }
        public byte[]? FileIcon { get; set; }
        public string? UserId { get; set; }  // ← string
        public string? AgentName { get; set; }
        public DateTime IncidentDate { get; set; }
        public string? FileStatus { get; set; }= "UnderSettlement";
        public string ClaimStatus { get; set; } = "م مغطاء";
        public int? CourtId { get; set; }
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }

        [ForeignKey("PlaintiffNameId")] public PlaintiffName? Plaintiff { get; set; }
        [ForeignKey("BranchId")] public Branch? Branch { get; set; }
        public ICollection<FriendlyFile> Files { get; set; } = new List<FriendlyFile>();
    }

    [Table("TblFriendlyFiles")]
    public class FriendlyFile
    {
        public int Id { get; set; }
        public byte[] FileData { get; set; } = Array.Empty<byte>();
        public int FriendlyId { get; set; }
        [ForeignKey("FriendlyId")] public FriendlyClaim? FriendlyClaim { get; set; }
    }

    [Table("TblOutters")]
    public class OutterClaim
    {
        public int Id { get; set; }
        public long Num { get; set; }
        public int Year { get; set; }
        public int? PlaintiffNameId { get; set; }
        public int? BranchId { get; set; }
        public int CourtId { get; set; }
        public double Setteld { get; set; }
        public double Reserve { get; set; }
        public string? Place { get; set; }
        [MaxLength(1000)] public string? Note { get; set; }
        public DateTime RegDate { get; set; }
        public string? FileName { get; set; }
        public string? FileExtension { get; set; }
        public byte[]? FileIcon { get; set; }
        public string? UserId { get; set; }  // ← string
        public string FileStatus { get; set; } = "UnderSettlement";
        public string ClaimStatus { get; set; } = "م مغطاء";
        public DateTime? IncidentDate { get; set; }
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }

        [ForeignKey("PlaintiffNameId")] public PlaintiffName? Plaintiff { get; set; }
        [ForeignKey("BranchId")] public Branch? Branch { get; set; }
        [ForeignKey("CourtId")] public Court? Court { get; set; }
        public ICollection<OutterFile> Files { get; set; } = new List<OutterFile>();
    }

    [Table("TblOutterFiles")]
    public class OutterFile
    {
        public int Id { get; set; }
        public byte[] FileData { get; set; } = Array.Empty<byte>();
        public int OutterId { get; set; }
        [ForeignKey("OutterId")] public OutterClaim? OutterClaim { get; set; }
    }

    [Table("TblExpenses")]
    public class Expense
    {
        public int Id { get; set; }
        [MaxLength(20)] public string ClaimType { get; set; } = string.Empty;
        public int ClaimNumber { get; set; }
        public int Year { get; set; }
        public int ExpenseTypeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? Notes { get; set; }
        public string? UserId { get; set; }  // ← string
        public int BranchId { get; set; }
        [ForeignKey("ExpenseTypeId")] public ExpenseType? ExpenseType { get; set; }
    }

    [Table("TblMessages")]
    public class Message
    {
        public int Id { get; set; }
        public string SenderId { get; set; } = string.Empty;   // ← string
        public string ReceiverId { get; set; } = string.Empty; // ← string
        public DateTime SendDate { get; set; }
        public string MessageText { get; set; } = string.Empty;
        public DateTime? ReadDate { get; set; }
    }

    [Table("TblEvents")]
    public class AuditEvent
    {
        public int Id { get; set; }
        public string EventType { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string? UserId { get; set; }  // ← string
        public string? TableName { get; set; }
        public int RowId { get; set; }
        public string? OldData { get; set; }
    }

    [Table("TblUserActivity")]
    public class UserActivity
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string PageTitle { get; set; } = string.Empty;
        public string? IPAddress { get; set; }
        public DateTime VisitedAt { get; set; } = DateTime.Now;
    }

    [Table("TblRolePermissions")]
    public class RolePermission
    {
        public int Id { get; set; }
        [Required] public string RoleId { get; set; } = string.Empty;
        [Required, MaxLength(100)] public string Module { get; set; } = string.Empty;
        public bool CanView   { get; set; } = false;
        public bool CanAdd    { get; set; } = false;
        public bool CanEdit   { get; set; } = false;
        public bool CanDelete { get; set; } = false;
    }

    // ====================== التسويات ======================

    /// <summary>تسوية حجوزات</summary>
    [Table("TblReservationSettlement")]
    public class ReservationSettlement
    {
        public int Id { get; set; }
        public long ClaimNum { get; set; }
        public int Year { get; set; }
        public int? PlaintiffNameId { get; set; }
        public int? BranchId { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal ReserveAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal SettledAmount { get; set; }
        public DateTime? ReservationDate { get; set; }
        [MaxLength(50)] public string Status { get; set; } = "قيد التسوية";
        [MaxLength(1000)] public string? Notes { get; set; }
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }

        [ForeignKey("PlaintiffNameId")] public PlaintiffName? Plaintiff { get; set; }
        [ForeignKey("BranchId")] public Branch? Branch { get; set; }
    }

    /// <summary>تسوية أتعاب القضايا</summary>
    [Table("TblCaseFeeSettlement")]
    public class CaseFeeSettlement
    {
        public int Id { get; set; }
        [MaxLength(100)] public string CaseNumber { get; set; } = string.Empty;
        public int Year { get; set; }
        public int? CourtId { get; set; }
        public int? BranchId { get; set; }
        [MaxLength(200)] public string LawyerName { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")] public decimal AgreeAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal PaidAmount { get; set; }
        public DateTime? ContractDate { get; set; }
        [MaxLength(50)] public string Status { get; set; } = "قيد الدفع";
        [MaxLength(1000)] public string? Notes { get; set; }
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }

        [ForeignKey("CourtId")] public Court? Court { get; set; }
        [ForeignKey("BranchId")] public Branch? Branch { get; set; }
    }

    /// <summary>تسوية أتعاب المحاماة الخواص</summary>
    [Table("TblPrivateLawyerSettlement")]
    public class PrivateLawyerSettlement
    {
        public int Id { get; set; }
        [MaxLength(100)] public string CaseNumber { get; set; } = string.Empty;
        public int Year { get; set; }
        public int? CourtId { get; set; }
        public int? BranchId { get; set; }
        [Required, MaxLength(200)] public string LawyerName { get; set; } = string.Empty;
        [MaxLength(20)] public string? LawyerPhone { get; set; }
        [MaxLength(100)] public string? LawyerIBAN { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal AgreeAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal PaidAmount { get; set; }
        [MaxLength(50)] public string PaymentMethod { get; set; } = "تحويل بنكي";
        public DateTime? ContractDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        [MaxLength(50)] public string Status { get; set; } = "قيد الدفع";
        [MaxLength(1000)] public string? Notes { get; set; }
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }

        [ForeignKey("CourtId")] public Court? Court { get; set; }
        [ForeignKey("BranchId")] public Branch? Branch { get; set; }
    }

    [Table("TblUserSession")]
    public class UserSession
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime LoginAt { get; set; }
        public DateTime? LogoutAt { get; set; }
        public string? IPAddress { get; set; }
        public int? DurationMinutes { get; set; }
    }

    // ====================== المصارف وفروعها ======================

    /// <summary>المصارف — مصرف الوحدة، مصرف الصحارى...</summary>
    [Table("TblBanks")]
    public class Bank
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string? BankCode { get; set; }

        [Required, MaxLength(200)]
        public string BankName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public ICollection<BankBranch> Branches { get; set; } = new List<BankBranch>();
    }

    /// <summary>فروع المصرف — فرع الخمس، فرع الزاوية...</summary>
    [Table("TblBankBranches")]
    public class BankBranch
    {
        public int Id { get; set; }

        public int BankId { get; set; }
        [ForeignKey("BankId")] public Bank? Bank { get; set; }

        [MaxLength(50)]
        public string? BranchCode { get; set; }

        [Required, MaxLength(200)]
        public string BranchName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }

    // ====================== الجزء المالي ======================

    /// <summary>الحسابات المصرفية— مصرف الصحارى، مصرف الوحدة...</summary>
    [Table("TblBankAccounts")]
    public class BankAccount
    {
        public int Id { get; set; }

        /// <summary>اسم المصرف — مثال: مصرف الصحارى الرئيسي</summary>
        [Required, MaxLength(200)]
        public string BankName { get; set; } = string.Empty;

        /// <summary>رقم الحساب — مثال: 2000002375</summary>
        [Required, MaxLength(50)]
        public string AccountNumber { get; set; } = string.Empty;

        /// <summary>الفرع الجغرافي للمصرف — مثال: باب المدينة</summary>
        [MaxLength(100)]
        public string? BankBranch { get; set; }

        /// <summary>الرصيد الافتتاحي</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal OpeningBalance { get; set; } = 0;

        /// <summary>الرصيد الحالي (يُحسب تراكمياً)</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal CurrentBalance { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public ICollection<BankTransaction> Transactions { get; set; } = new List<BankTransaction>();
    }

    /// <summary>نوع المستند في القيد</summary>
    public enum DocumentType
    {
        صك,
        قسيمة_إيداع,
        حوالة,
        نقدي,
        خصم_تلقائي,
        أخرى
    }

    /// <summary>نوع الحركة المالية</summary>
    public enum TransactionCategory
    {
        /// <summary>دفع مطالبة تأمين</summary>
        دفع_مطالبة,
        /// <summary>استلام قسط من جهة موردة</summary>
        إيداع_قسط,
        /// <summary>صرف عمولة مسوق</summary>
        صرف_عمولة,
        /// <summary>حجز قضائي مرتبط بملف مطالبة</summary>
        حجز_قضائي,
        /// <summary>أتعاب محاماة</summary>
        أتعاب_محاماة,
        /// <summary>مصاريف إدارية</summary>
        مصاريف_إدارية,
        /// <summary>غير ذلك</summary>
        أخرى
    }

    /// <summary>حركات الحساب المصرفي— مدين/دائن مع ربط اختياري بملف مطالبة</summary>
    [Table("TblBankTransactions")]
    public class BankTransaction
    {
        public int Id { get; set; }

        public int BankAccountId { get; set; }
        [ForeignKey("BankAccountId")] public BankAccount? BankAccount { get; set; }

        /// <summary>تاريخ الحركة</summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>رقم القيد المحاسبي</summary>
        [MaxLength(50)]
        public string? EntryNumber { get; set; }

        /// <summary>نوع المستند</summary>
        public DocumentType? DocType { get; set; }

        /// <summary>رقم الصك أو قسيمة الإيداع</summary>
        [MaxLength(100)]
        public string? DocumentNumber { get; set; }

        /// <summary>البيان (الوصف)</summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>مبلغ مدين (خروج من الحساب)</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal DebitAmount { get; set; } = 0;

        /// <summary>مبلغ دائن (دخول للحساب)</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal CreditAmount { get; set; } = 0;

        /// <summary>الرصيد بعد الحركة</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal RunningBalance { get; set; } = 0;

        /// <summary>تصنيف الحركة</summary>
        public TransactionCategory Category { get; set; } = TransactionCategory.أخرى;

        // ---- ربط بملف مطالبة ودية (اختياري) ----
        public int? FriendlyClaimId { get; set; }
        [ForeignKey("FriendlyClaimId")] public FriendlyClaim? FriendlyClaim { get; set; }

        // ---- ربط بملف مطالبة خارجية (اختياري) ----
        public int? OutterClaimId { get; set; }
        [ForeignKey("OutterClaimId")] public OutterClaim? OutterClaim { get; set; }

        // ---- ربط بعمولة (اختياري) ----
        public int? CommissionId { get; set; }
        [ForeignKey("CommissionId")] public Commission? Commission { get; set; }

        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }
    }

    /// <summary>العمولات المباشرة — مدفوعة للمسوقين على الأقساط الواردة</summary>
    [Table("TblCommissions")]
    public class Commission
    {
        public int Id { get; set; }

        /// <summary>اسم الجهة الموردة (المصحة / المستشفى / الشركة)</summary>
        [Required, MaxLength(300)]
        public string SupplierName { get; set; } = string.Empty;

        /// <summary>قيمة القسط المورد</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal PremiumAmount { get; set; }

        /// <summary>نسبة العمولة %</summary>
        [Column(TypeName = "decimal(5,2)")]
        public decimal CommissionRate { get; set; } = 5;

        /// <summary>قيمة العمولة (= PremiumAmount * CommissionRate / 100)</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal CommissionAmount { get; set; }

        /// <summary>اسم المسوق المستفيد</summary>
        [MaxLength(300)]
        public string? BeneficiaryName { get; set; }

        /// <summary>رقم إذن الصرف</summary>
        [MaxLength(50)]
        public string? AuthorizationNumber { get; set; }

        /// <summary>رقم القيد المحاسبي</summary>
        [MaxLength(50)]
        public string? EntryNumber { get; set; }

        /// <summary>تاريخ الصرف</summary>
        public DateTime? PaymentDate { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "قيد الصرف";  // قيد الصرف | مصروفة

        [MaxLength(500)]
        public string? Notes { get; set; }

        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }

        // حركة الصرف في الحساب المصرفي (تُنشأ عند تنفيذ صرف العمولة)
        public ICollection<BankTransaction> Transactions { get; set; } = new List<BankTransaction>();
    }

    // ==================== وحدة المسوقين ====================

    /// <summary>المسوق — الشخص المستفيد من العمولة</summary>
    [Table("TblMarketers")]
    public class Marketer
    {
        public int Id { get; set; }

        [Required, MaxLength(300)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }

        public ICollection<MarketerCommissionRecord> CommissionRecords { get; set; } = new List<MarketerCommissionRecord>();
    }

    /// <summary>كشف تسوية عمولة — كل كشف يمثل دفعة واحدة لمسوق</summary>
    [Table("TblMarketerCommissionRecords")]
    public class MarketerCommissionRecord
    {
        public int Id { get; set; }

        public int MarketerId { get; set; }
        [ForeignKey("MarketerId")] public Marketer? Marketer { get; set; }

        /// <summary>تاريخ التسوية</summary>
        public DateTime SettlementDate { get; set; } = DateTime.Today;

        /// <summary>رقم الصك</summary>
        [MaxLength(200)]
        public string? CheckNumber { get; set; }

        /// <summary>أوامر توريد أرقم</summary>
        [MaxLength(500)]
        public string? SupplyOrderNumbers { get; set; }

        /// <summary>إيصالات قبض أرقم</summary>
        [MaxLength(500)]
        public string? ReceiptNumbers { get; set; }

        /// <summary>إعداد</summary>
        [MaxLength(200)]
        public string? PreparedBy { get; set; }

        /// <summary>مراجعة</summary>
        [MaxLength(200)]
        public string? ReviewedBy { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }

        public ICollection<MarketerCommissionItem> Items { get; set; } = new List<MarketerCommissionItem>();
    }

    /// <summary>بند من بنود كشف العمولة — سطر واحد في الجدول</summary>
    [Table("TblMarketerCommissionItems")]
    public class MarketerCommissionItem
    {
        public int Id { get; set; }

        public int RecordId { get; set; }
        [ForeignKey("RecordId")] public MarketerCommissionRecord? Record { get; set; }

        /// <summary>الجهة المورِدة للأقساط</summary>
        [Required, MaxLength(300)]
        public string FacilityName { get; set; } = string.Empty;

        /// <summary>الفترة المسدد عنها</summary>
        [MaxLength(200)]
        public string? Period { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal TotalPremiums { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal ReturnedPremiums { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal NetPremiums { get; set; }

        [Column(TypeName = "decimal(5,4)")]
        public decimal CommissionRate { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal CommissionAmount { get; set; }
    }

    // ==================== وحدة الموردين ====================

    /// <summary>الموردون — الجهات التي تورد الأقساط</summary>
    [Table("TblSuppliers")]
    public class Supplier
    {
        public int Id { get; set; }

        /// <summary>اسم المورد / الجهة</summary>
        [Required, MaxLength(300)]
        public string Name { get; set; } = string.Empty;

        /// <summary>رقم الهاتف</summary>
        [MaxLength(50)]
        public string? Phone { get; set; }

        /// <summary>البريد الإلكتروني</summary>
        [MaxLength(200)]
        public string? Email { get; set; }

        /// <summary>العنوان</summary>
        [MaxLength(500)]
        public string? Address { get; set; }

        /// <summary>رقم العقد / الوثيقة</summary>
        [MaxLength(100)]
        public string? ContractNumber { get; set; }

        /// <summary>نوع الجهة: خاصة / حكومية / أخرى</summary>
        [MaxLength(50)]
        public string? EntityType { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }
    }

    // ==================== وحدة الاكتتاب ====================

    /// <summary>نوع الجهة المؤمن عليها</summary>
    public enum FacilityType { Private = 0, Public = 1 }

    /// <summary>نوع التحصيل في إيصال القبض</summary>
    public enum CollectionType
    {
        Mandatory      = 0,  // إجباري
        MedicalCard    = 1,  // بطاقة طبية
        Supplemental   = 2,  // تكميلي
        Marine         = 3,  // بحري
        Fires          = 4,  // حريق
        Miscellaneous  = 5,  // حوادث متنوعة
        Various        = 6   // متنوعة
    }

    /// <summary>عقود التأمين الطبي — قسم الاكتتاب</summary>
    [Table("TblInsuranceContracts")]
    public class InsuranceContract
    {
        public int Id { get; set; }

        /// <summary>رقم العقد/الوثيقة (مثال: 1951)</summary>
        [MaxLength(50)]
        public string? ContractNumber { get; set; }

        /// <summary>اسم الجهة المؤمن عليها (المستشفى/المؤسسة)</summary>
        [Required, MaxLength(300)]
        public string FacilityName { get; set; } = string.Empty;

        /// <summary>نوع الجهة: خاص / عام</summary>
        public FacilityType FacilityType { get; set; } = FacilityType.Private;

        /// <summary>تاريخ بداية التأمين</summary>
        public DateTime InsuranceStartDate { get; set; }

        /// <summary>تاريخ نهاية التأمين</summary>
        public DateTime InsuranceEndDate { get; set; }

        /// <summary>القسط الخاص</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal PrivatePremium { get; set; }

        /// <summary>القسط العام</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal PublicPremium { get; set; }

        /// <summary>رسوم الإشراف</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal SupervisionFee { get; set; }

        /// <summary>المبلغ المدفوع مقدماً</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal PrepaidAmount { get; set; }

        /// <summary>المبلغ تحت التحصيل</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal UnderCollectionAmount { get; set; }

        /// <summary>رصيد الخزينة</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal TreasuryAmount { get; set; }

        /// <summary>رقم هاتف الجهة</summary>
        [MaxLength(50)]
        public string? Phone { get; set; }

        /// <summary>العنوان الفعلي للجهة</summary>
        [MaxLength(500)]
        public string? Address { get; set; }

        /// <summary>اسم الشخص المخول بالتوقيع القانوني</summary>
        [MaxLength(300)]
        public string? AuthorizedSignatory { get; set; }

        /// <summary>حالة العقد: نشط / منتهي / ملغي</summary>
        [MaxLength(30)]
        public string Status { get; set; } = "نشط";

        [MaxLength(500)]
        public string? Notes { get; set; }
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }

        public ICollection<SupplyOrder>        SupplyOrders      { get; set; } = new List<SupplyOrder>();
        public ICollection<ReceiptVoucher>     Receipts          { get; set; } = new List<ReceiptVoucher>();
        public ICollection<InsuredEmployee>    InsuredEmployees  { get; set; } = new List<InsuredEmployee>();
    }

    /// <summary>أوامر التوريد — طلب تحصيل القسط من الجهة</summary>
    [Table("TblSupplyOrders")]
    public class SupplyOrder
    {
        public int Id { get; set; }

        /// <summary>رقم أمر التوريد (مثال: H00085N، 0000515)</summary>
        [MaxLength(50)]
        public string? OrderNumber { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Today;

        public int? ContractId { get; set; }
        public InsuranceContract? Contract { get; set; }

        /// <summary>ربط بالمورد من جدول الموردين</summary>
        public int? SupplierId { get; set; }
        [ForeignKey("SupplierId")] public Supplier? Supplier { get; set; }

        [MaxLength(300)]
        public string? FacilityName { get; set; }

        /// <summary>بداية فترة الغطاء</summary>
        public DateTime CoverageFrom { get; set; }

        /// <summary>نهاية فترة الغطاء</summary>
        public DateTime CoverageTo { get; set; }

        /// <summary>وصف الربع — مثال: الربع الثاني 2025</summary>
        [MaxLength(100)]
        public string? QuarterDescription { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal Amount { get; set; }

        /// <summary>حالة الأمر: قيد التحصيل / مُوَرَّد / ملغي</summary>
        [MaxLength(30)]
        public string Status { get; set; } = "قيد التحصيل";

        // ── حالة سير العمل (Four-Eyes Principle) ──
        /// <summary>
        /// مسودة → معتمد_اكتتاب → في_انتظار_تأكيد → مؤكد → مغلق
        /// </summary>
        [MaxLength(50)]
        public string WorkflowStatus { get; set; } = "مسودة";

        /// <summary>المرحلة 1: اعتماد الاكتتاب</summary>
        public DateTime? UnderwritingApprovedAt { get; set; }
        [MaxLength(200)]
        public string? UnderwritingApprovedBy { get; set; }

        /// <summary>المرحلة 3: تأكيد التحصيل من قِبَل الاكتتاب</summary>
        public DateTime? UnderwritingVerifiedAt { get; set; }
        [MaxLength(200)]
        public string? UnderwritingVerifiedBy { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }

        public ReceiptVoucher? Receipt { get; set; }
    }

    /// <summary>إيصالات القبض — استلام الأقساط من الجهات</summary>
    [Table("TblReceiptVouchers")]
    public class ReceiptVoucher
    {
        public int Id { get; set; }

        /// <summary>رقم إيصال القبض (مثال: 1001951)</summary>
        [MaxLength(50)]
        public string? ReceiptNumber { get; set; }

        public DateTime ReceiptDate { get; set; } = DateTime.Today;

        public int? ContractId { get; set; }
        public InsuranceContract? Contract { get; set; }

        public int? SupplyOrderId { get; set; }
        public SupplyOrder? SupplyOrder { get; set; }

        [MaxLength(300)]
        public string? FacilityName { get; set; }

        /// <summary>المبلغ بالدينار</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal DinarAmount { get; set; }

        /// <summary>المبلغ بالدرهم</summary>
        [Column(TypeName = "decimal(5,3)")]
        public decimal DirhamAmount { get; set; }

        /// <summary>الإجمالي = DinarAmount + DirhamAmount/1000</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal TotalAmount { get; set; }

        /// <summary>طريقة الدفع: نقدي / صك / تحويل</summary>
        [MaxLength(20)]
        public string PaymentMethod { get; set; } = "نقدي";

        [MaxLength(100)]
        public string? CheckNumber { get; set; }

        [MaxLength(200)]
        public string? BankName { get; set; }

        /// <summary>الحساب المصرفي الذي أُودع فيه المبلغ</summary>
        public int? BankAccountId { get; set; }
        public BankAccount? BankAccount { get; set; }

        /// <summary>نوع التحصيل</summary>
        public CollectionType CollectionType { get; set; } = CollectionType.Mandatory;

        /// <summary>الأقساط المباشرة — القطاع الخاص</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal PrivatePremium { get; set; }

        /// <summary>الأقساط المباشرة — القطاع العام</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal PublicPremium { get; set; }

        /// <summary>الأقساط المقدمة</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal AdvancePremium { get; set; }

        /// <summary>أقساط تحت الدفع (القيمة المسجلة في الإيصال)</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal PendingPremium { get; set; }

        /// <summary>مدة التأمين — من</summary>
        public DateTime? CoverageFrom { get; set; }

        /// <summary>مدة التأمين — إلى</summary>
        public DateTime? CoverageTo { get; set; }

        /// <summary>رسوم الإشراف = الإجمالي × 0.005</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal SupervisionFee { get; set; }

        /// <summary>صافي الخزينة</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal TreasuryAmount { get; set; }

        /// <summary>مبلغ الإيداع المصرفي الفعلي (يُدخله موظف الخزينة يدوياً)</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal? BankDepositAmount { get; set; }

        /// <summary>الإدارة المُصدِرة للإيصال</summary>
        [MaxLength(200)]
        public string? Department { get; set; }

        /// <summary>المكتب / الفرع المُصدِر</summary>
        [MaxLength(200)]
        public string? Office { get; set; }

        /// <summary>فرع المصرف (للصك)</summary>
        [MaxLength(200)]
        public string? BankBranchName { get; set; }

        /// <summary>الحركة المصرفية المرتبطة (تُنشأ تلقائياً)</summary>
        public int? LinkedTransactionId { get; set; }
        public BankTransaction? LinkedTransaction { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }
    }

    // ==================== كشف المؤمن عليهم ====================

    /// <summary>المؤمن عليهم — الكوادر الطبية والطبية المساعدة المرتبطة بعقد تأمين</summary>
    [Table("TblInsuredEmployees")]
    public class InsuredEmployee
    {
        public int Id { get; set; }

        public int ContractId { get; set; }
        [ForeignKey("ContractId")] public InsuranceContract? Contract { get; set; }

        /// <summary>الاسم الرباعي</summary>
        [Required, MaxLength(400)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>التخصص الدقيق (مثال: جراح باطنة، طبيب عيون)</summary>
        [MaxLength(200)]
        public string? Specialization { get; set; }

        /// <summary>رقم الهاتف</summary>
        [MaxLength(30)]
        public string? Phone { get; set; }

        /// <summary>الراتب الشهري — أساس الحسبة</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal MonthlySalary { get; set; }

        /// <summary>نسبة تحمل الجهة — افتراضياً 3%</summary>
        [Column(TypeName = "decimal(5,4)")]
        public decimal EntityRate { get; set; } = 0.03m;

        /// <summary>نسبة خصم المرتب — افتراضياً 2%</summary>
        [Column(TypeName = "decimal(5,4)")]
        public decimal EmployeeRate { get; set; } = 0.02m;

        /// <summary>حصة الجهة = MonthlySalary × EntityRate</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal EntityShare { get; set; }

        /// <summary>حصة الموظف = MonthlySalary × EmployeeRate</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal EmployeeShare { get; set; }

        /// <summary>الإجمالي = EntityShare + EmployeeShare</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal TotalPremium { get; set; }

        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }
    }

    // ==================== قسيمة الإيداع المصرفي ====================

    /// <summary>قسيمة إيداع مصرفي — الإيصال الفعلي الصادر من المصرف</summary>
    [Table("TblBankDepositSlips")]
    public class BankDepositSlip
    {
        public int Id { get; set; }

        /// <summary>رقم قسيمة الإيداع</summary>
        [MaxLength(100)]
        public string? SlipNumber { get; set; }

        public DateTime DepositDate { get; set; } = DateTime.Today;

        /// <summary>اسم الشركة / العميل</summary>
        [Required, MaxLength(300)]
        public string ClientName { get; set; } = string.Empty;

        /// <summary>الحساب المصرفي المودع فيه</summary>
        public int? BankAccountId { get; set; }
        [ForeignKey("BankAccountId")] public BankAccount? BankAccount { get; set; }

        /// <summary>القيمة بالدينار الليبي</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal Amount { get; set; }

        /// <summary>عدد الصكوك المودعة</summary>
        public int CheckCount { get; set; } = 0;

        /// <summary>الفرع المستفيد (فرع المصرف)</summary>
        [MaxLength(200)]
        public string? BeneficiaryBranch { get; set; }

        /// <summary>تاريخ الاستحقاق</summary>
        public DateTime? DueDate { get; set; }

        /// <summary>طريقة الدفع: نقدي / صك / تحويل</summary>
        [MaxLength(30)]
        public string PaymentMethod { get; set; } = "صك";

        /// <summary>مرتبط بإيصال القبض</summary>
        public int? ReceiptVoucherId { get; set; }
        [ForeignKey("ReceiptVoucherId")] public ReceiptVoucher? ReceiptVoucher { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }
    }

    // ==================== القيود المحاسبية ====================

    /// <summary>
    /// القيد المحاسبي — يُنشأ تلقائياً بمرحلتين:
    ///   المرحلة 1: عند إدخال أمر التوريد → يُسجَّل الجانب المدين (خزينة/أقساط تحت التحصيل)
    ///   المرحلة 2: عند إدخال قسيمة الإيداع → يُسجَّل الجانب المدين لحساب المصرف
    /// </summary>
    [Table("TblAccountingEntries")]
    public class AccountingEntry
    {
        public int Id { get; set; }

        /// <summary>رقم القيد (مسلسل تلقائي)</summary>
        [MaxLength(50)]
        public string? EntryNumber { get; set; }

        public DateTime EntryDate { get; set; } = DateTime.Today;

        /// <summary>البيان: اسم الجهة + الفترة التأمينية</summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        // ── المرحلة الأولى: أمر التوريد ──
        public int? SupplyOrderId { get; set; }
        [ForeignKey("SupplyOrderId")] public SupplyOrder? SupplyOrder { get; set; }

        /// <summary>قيمة الجانب المدين من أمر التوريد (خزينة)</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal DebitAmount { get; set; }

        /// <summary>مسمى الحساب المدين — مثال: خزينة / أقساط تحت التحصيل</summary>
        [MaxLength(200)]
        public string? DebitAccount { get; set; } = "أقساط تحت التحصيل";

        // ── المرحلة الثانية: الإيداع المصرفي ──
        public int? BankDepositSlipId { get; set; }
        [ForeignKey("BankDepositSlipId")] public BankDepositSlip? BankDepositSlip { get; set; }

        /// <summary>قيمة الإيداع في حساب المصرف (مدين)</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal BankDebitAmount { get; set; }

        /// <summary>مسمى حساب المصرف — يُملأ آلياً من اسم الحساب</summary>
        [MaxLength(200)]
        public string? BankDebitAccount { get; set; }

        /// <summary>حالة القيد: مبدئي (بعد أمر التوريد) / مكتمل (بعد الإيداع) / معتمد</summary>
        [MaxLength(30)]
        public string Status { get; set; } = "مبدئي";

        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }

        public ICollection<AccountingEntryLine> Lines { get; set; } = new List<AccountingEntryLine>();
    }

    /// <summary>
    /// بند في القيد المحاسبي (مدين أو دائن) — القيد المزدوج
    /// </summary>
    [Table("TblAccountingEntryLines")]
    public class AccountingEntryLine
    {
        public int Id { get; set; }

        public int EntryId { get; set; }
        [ForeignKey("EntryId")] public AccountingEntry? Entry { get; set; }

        /// <summary>ترتيب السطر في القيد</summary>
        public int LineOrder { get; set; }

        /// <summary>اسم الحساب — مثال: خزانة التأمين، مصرف الصحارى ح/2000002375</summary>
        [Required, MaxLength(300)]
        public string AccountName { get; set; } = string.Empty;

        /// <summary>البيان التفصيلي للسطر</summary>
        [MaxLength(500)]
        public string? LineDescription { get; set; }

        /// <summary>مدين</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal Debit { get; set; } = 0;

        /// <summary>دائن</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal Credit { get; set; } = 0;
    }

    // ==================== اليومية ====================

    /// <summary>اليومية — ملف يجمع كل مستندات اليوم ويُحوَّل لقسم المراجعة الداخلية</summary>
    [Table("TblDailyJournals")]
    public class DailyJournal
    {
        public int Id { get; set; }

        public DateTime JournalDate { get; set; } = DateTime.Today;

        /// <summary>المجموع الكلي لإيصالات اليوم</summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal TotalAmount { get; set; }

        /// <summary>حالة اليومية: قيد المراجعة / معتمدة / مردودة</summary>
        [MaxLength(30)]
        public string Status { get; set; } = "قيد المراجعة";

        /// <summary>ملاحظات المراجع الداخلي</summary>
        [MaxLength(1000)]
        public string? ReviewNotes { get; set; }

        public DateTime? ApprovedAt { get; set; }

        [MaxLength(200)]
        public string? ApprovedBy { get; set; }

        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }

        public ICollection<DailyJournalItem> Items { get; set; } = new List<DailyJournalItem>();
    }

    /// <summary>بند في اليومية — يربط أمر التوريد + إيصال القبض + قسيمة الإيداع + القيد المحاسبي</summary>
    [Table("TblDailyJournalItems")]
    public class DailyJournalItem
    {
        public int Id { get; set; }

        public int JournalId { get; set; }
        [ForeignKey("JournalId")] public DailyJournal? Journal { get; set; }

        public int? SupplyOrderId { get; set; }
        [ForeignKey("SupplyOrderId")] public SupplyOrder? SupplyOrder { get; set; }

        public int? ReceiptVoucherId { get; set; }
        [ForeignKey("ReceiptVoucherId")] public ReceiptVoucher? ReceiptVoucher { get; set; }

        public int? BankDepositSlipId { get; set; }
        [ForeignKey("BankDepositSlipId")] public BankDepositSlip? BankDepositSlip { get; set; }

        public int? AccountingEntryId { get; set; }
        [ForeignKey("AccountingEntryId")] public AccountingEntry? AccountingEntry { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal Amount { get; set; }
    }

    /// <summary>إذون الصرف — تفويض صرف المبالغ للمسوقين والموردين</summary>
    [Table("TblPaymentAuthorizations")]
    public class PaymentAuthorization
    {
        public int Id { get; set; }

        /// <summary>رقم إذن الصرف (مثال: 0003704)</summary>
        [MaxLength(50)]
        public string? AuthorizationNumber { get; set; }

        public DateTime AuthorizationDate { get; set; } = DateTime.Today;

        /// <summary>اسم المستفيد</summary>
        [Required, MaxLength(300)]
        public string BeneficiaryName { get; set; } = string.Empty;

        /// <summary>نوع المستفيد: مسوق / مورد / موظف / جهة خارجية</summary>
        [MaxLength(50)]
        public string BeneficiaryType { get; set; } = "مسوق";

        [Column(TypeName = "decimal(18,3)")]
        public decimal Amount { get; set; }

        /// <summary>طريقة الدفع: صبات / تحويل / نقدي</summary>
        [MaxLength(30)]
        public string PaymentMethod { get; set; } = "تحويل";

        [MaxLength(100)]
        public string? CheckNumber { get; set; }

        public int? BankAccountId { get; set; }
        public BankAccount? BankAccount { get; set; }

        /// <summary>ربط بعمولة مسوق إن وجد</summary>
        public int? CommissionId { get; set; }
        public Commission? Commission { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>حالة الإذن: قيد التنفيذ / منفذ / ملغي</summary>
        [MaxLength(30)]
        public string Status { get; set; } = "قيد التنفيذ";

        /// <summary>الحركة المصرفية المرتبطة (تُنشأ عند التنفيذ)</summary>
        public int? LinkedTransactionId { get; set; }
        public BankTransaction? LinkedTransaction { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? Del { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeleteReason { get; set; }
    }
}