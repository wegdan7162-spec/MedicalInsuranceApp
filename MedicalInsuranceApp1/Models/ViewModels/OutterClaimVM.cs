using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalInsuranceApp1.Models.ViewModels
{
    public class OutterClaimListVM
    {
        public int Id { get; set; }
        public string ClaimNumber { get; set; } = string.Empty;
        public int Year { get; set; }
        public string PlaintiffName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string CourtName { get; set; } = string.Empty;
        public string Place { get; set; } = string.Empty;
        public double Reserve { get; set; }
        public double Setteld { get; set; }
        public string ClaimStatus { get; set; } = string.Empty;
        public string FileStatus { get; set; } = string.Empty;
        public DateTime RegDate { get; set; }
        public DateTime? IncidentDate { get; set; }
        public int FilesCount { get; set; }
    }

    public class OutterClaimCreateVM
    {
        [Required(ErrorMessage = "رقم المطالبة مطلوب")]
        [Range(1, long.MaxValue, ErrorMessage = "رقم المطالبة يجب أن يكون أكبر من صفر")]
        public long Num { get; set; }

        [Required(ErrorMessage = "سنة المطالبة مطلوبة")]
        [Range(1900, 2100, ErrorMessage = "السنة غير صحيحة")]
        public int ClaimYear { get; set; } = DateTime.Now.Year;

        [Required(ErrorMessage = "المدعي مطلوب")]
        public int PlaintiffNameId { get; set; }

        [Required(ErrorMessage = "الفرع مطلوب")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "المحكمة مطلوبة")]
        public int CourtId { get; set; }

        [Required(ErrorMessage = "مكان الواقعة مطلوب")]
        public string Place { get; set; } = string.Empty;

        public DateTime? IncidentDate { get; set; }

        [Required(ErrorMessage = "تاريخ التسجيل مطلوب")]
        public DateTime RegDate { get; set; } = DateTime.Now;

        public double Reserve { get; set; }
        public double Setteld { get; set; }

        public string ClaimStatus { get; set; } = "م.مغطاء";
        public string FileStatus { get; set; } = "تحت التسوية";   
        //

        [MaxLength(1000)]
        public string? Note { get; set; }

        // حقول المصروف الأولي (اختيارية)
        public int? ExpenseTypeId { get; set; }
        public decimal? ExpenseAmount { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public string? ExpenseNotes { get; set; }

        // Dropdowns
        public List<SelectListItem> Plaintiffs { get; set; } = new();
        public List<SelectListItem> Branches { get; set; } = new();
        public List<SelectListItem> Courts { get; set; } = new();
        public List<string> StatusList { get; set; } = new();
        public List<string> ClaimStatusList { get; set; } = new();
        public List<SelectListItem> ExpenseTypes { get; set; } = new();
    }

    public class OutterClaimEditVM : OutterClaimCreateVM
    {
        public int Id { get; set; }
    }

    public class OutterClaimDetailsVM
    {
        public int Id { get; set; }
        public long Num { get; set; }
        public int Year { get; set; }
        public string PlaintiffName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string CourtName { get; set; } = string.Empty;
        public string Place { get; set; } = string.Empty;
        public double Reserve { get; set; }
        public double Setteld { get; set; }
        public string? Note { get; set; }
        public string ClaimStatus { get; set; } = string.Empty;
        public string FileStatus { get; set; } = string.Empty;
        //
        public DateTime RegDate { get; set; }
        public DateTime? IncidentDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public List<OutterFileVM> Files { get; set; } = new();
        public List<ExpenseListVM> Expenses { get; set; } = new();
        public decimal TotalExpenses => Expenses.Sum(e => e.Amount);
    }

    public class OutterFileVM
    {
        public int Id { get; set; }
        public int OutterId { get; set; }
        public string FileName { get; set; } = string.Empty;
    }

    // ===== ViewModels المصروفات =====

    public class ExpenseListVM
    {
        public int Id { get; set; }
        public string ExpenseTypeName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? Notes { get; set; }
        public string AddedBy { get; set; } = string.Empty;
    }

    public class AddExpenseVM
    {
        // بيانات المطالبة (للعرض فقط)
        public int ClaimId { get; set; }
        public long ClaimNum { get; set; }
        public int ClaimYear { get; set; }
        public string PlaintiffName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public int BranchId { get; set; }

        // بيانات المصروف
        [Required(ErrorMessage = "نوع المصروف مطلوب")]
        public int ExpenseTypeId { get; set; }

        [Required(ErrorMessage = "المبلغ مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "المبلغ يجب أن يكون أكبر من صفر")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "التاريخ مطلوب")]
        public DateTime ExpenseDate { get; set; } = DateTime.Now;

        public string? Notes { get; set; }

        // Dropdown
        public List<SelectListItem> ExpenseTypes { get; set; } = new();
    }

    public class EditExpenseVM : AddExpenseVM
    {
        public int ExpenseId { get; set; }
    }
}
