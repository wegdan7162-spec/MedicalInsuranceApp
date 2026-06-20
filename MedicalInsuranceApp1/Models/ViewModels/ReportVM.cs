using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalInsuranceApp1.Models.ViewModels
{
    // ===== فلاتر التقرير =====
    public class ReportFilterVM
    {
        // نوع التقرير
        public string ReportType { get; set; } = "OutterClaims";

        // فلتر الفترة
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo   { get; set; }

        // فلاتر
        public int?    BranchId    { get; set; }
        public string? Place       { get; set; }
        public string? FileStatus { get; set; }
        public string? ClaimStatus { get; set; }

        // ===== اختيار الأعمدة =====
        public bool ColClaimSource   { get; set; } = true;
        public bool ColNum           { get; set; } = true;
        public bool ColPlaintiff     { get; set; } = true;
        public bool ColBranch        { get; set; } = true;
        public bool ColPlace         { get; set; } = true;
        public bool ColCourt         { get; set; } = true;
        public bool ColIncidentDate  { get; set; } = true;
        public bool ColRegDate       { get; set; } = false;
        public bool ColFileStatus { get; set; } = true;
        public bool ColClaimStatus   { get; set; } = true;
        public bool ColReserve       { get; set; } = true;
        public bool ColSettled       { get; set; } = true;
        public bool ColTotalExpenses { get; set; } = true;
        public bool ColExpenseDetail { get; set; } = true;  // أعمدة كل نوع مصروف

        // ===== Dropdowns =====
        public List<SelectListItem> Branches   { get; set; } = new();
        public List<string> ClaimStatuses { get; set; } = new() { "م.مغطاء", "غير مغطاء" };
        public List<string> StatusList  { get; set; } = new()
        {
            "تحت التسوية", "قيد المراجعة", "محالة للقانوني",
            "جلسات المحكمة", "حكم لصالح الشركة", "حكم ضد الشركة",
            "محالة للمالية", "تسوية ودية", "مسدد", "مغلقة"
        };

        // ===== نتائج =====
        public bool              HasResults       { get; set; } = false;
        public List<ReportRowVM> Rows             { get; set; } = new();
        public List<string>      ExpenseTypeNames { get; set; } = new();

        // ===== حقول الطباعة =====
        public string? UserFullName       { get; set; }
        public string  FilterDescription  { get; set; } = "كل المطالبات";
        public string  ReportTypeTitle    => ReportType switch {
            "OutterClaims"    => "مطالبات فرع التأمين الطبي - القضايا",
            "FriendlyClaims"  => "مطالبات فرع التأمين الطبي - الصلح الودي",
            "Both"            => "مطالبات فرع التأمين الطبي - القضايا والصلح الودي",
            _                 => "مطالبات فرع التأمين الطبي"
        };

        // ===== إحصائيات =====
        public double  TotalReserve  => Rows.Sum(r => r.Reserve);
        public double  TotalSettled  => Rows.Sum(r => r.Settled);
        public decimal TotalExpenses => Rows.Sum(r => r.TotalExpenses);
        public int     TotalCount    => Rows.Count;
    }

    // ===== صف واحد في التقرير =====
    public class ReportRowVM
    {
        public string ClaimSource  { get; set; } = ""; // "قضية" أو "صلح ودي"
        public long   Num          { get; set; }
        public int    Year         { get; set; }
        public string PlaintiffName { get; set; } = "";
        public string AgentName    { get; set; } = "";
        public string BranchName   { get; set; } = "";
        public string Place        { get; set; } = "";
        public string CourtName    { get; set; } = ""; // للقضايا فقط
        public DateTime? IncidentDate { get; set; }
        public DateTime  RegDate     { get; set; }
        public double  Reserve     { get; set; }
        public double  Settled     { get; set; }
        public decimal TotalExpenses { get; set; }
        public string  ClaimStatus { get; set; } = "م.مغطاء";
      /// <summary>
      /// / public string  ClaimType   { get; set; } = ""; // مغطى / غير مغطى
      /// </summary>

        // مصروفات حسب النوع (اسم النوع → المبلغ)
        public Dictionary<string, decimal> ExpensesByType { get; set; } = new();
    }
}
