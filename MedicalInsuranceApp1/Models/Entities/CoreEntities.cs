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

    [Table("TblCourtsN")]
    public class CourtCase
    {
        public int Id { get; set; }
        public long Num { get; set; }
        public int Year { get; set; }
        [Required, MaxLength(500)]
        public string PlaintiffName { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        public int? CourtId { get; set; }
        public DateTime RegDate { get; set; }
        public string? FileName { get; set; }
        public string? FileExtension { get; set; }
        public byte[]? FileIcon { get; set; }
        [MaxLength(1000)]
        public string? Note { get; set; }
        public string? UserId { get; set; }   // ← string الآن بدل int
        public double Setteld { get; set; }
        public double Reserve { get; set; }

        public string? Place { get; set; }
        public DateTime? IncidentDate { get; set; }
        public string? FileStatus { get; set; } = "نشط";
        public string? ClaimStatus { get; set; } = "تحت التسوية";
        public string? ClaimType { get; set; } = "مغطى";

        [ForeignKey("BranchId")] public Branch? Branch { get; set; }
        [ForeignKey("CourtId")] public Court? Court { get; set; }
        public ICollection<CourtFile> Files { get; set; } = new List<CourtFile>();
    }

    [Table("TblCourtFiles")]
    public class CourtFile
    {
        public int Id { get; set; }
        public byte[] FileData { get; set; } = Array.Empty<byte>();
        public int CourtsNId { get; set; }
        [ForeignKey("CourtsNId")] public CourtCase? CourtCase { get; set; }
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
        public string? FileStatus { get; set; }
        public string ClaimStatus { get; set; } = "UnderSettlement";
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
        public string FileStatus { get; set; } = "Active";
        public string ClaimStatus { get; set; } = "UnderSettlement";
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
}