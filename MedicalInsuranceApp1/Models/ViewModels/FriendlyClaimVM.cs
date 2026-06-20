using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalInsuranceApp1.Models.ViewModels
{
    public class FriendlyClaimListVM
    {
        public int Id { get; set; }
        public string ClaimNumber { get; set; } = string.Empty;
        public int Year { get; set; }
        public string PlaintiffName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string Place { get; set; } = string.Empty;
        public string AgentName { get; set; } = string.Empty;
        public double Reserve { get; set; }
        public double Setteld { get; set; }
        public string ClaimStatus { get; set; } = string.Empty;
        public string FileStatus { get; set; } = string.Empty;
        public DateTime IncidentDate { get; set; }
        public DateTime RegDate { get; set; }
        public int FilesCount { get; set; }
    }

    public class FriendlyClaimCreateVM
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

        [Required(ErrorMessage = "مكان الواقعة مطلوب")]
        public string Place { get; set; } = string.Empty;

        [Required(ErrorMessage = "تاريخ الواقعة مطلوب")]
        public DateTime IncidentDate { get; set; } = DateTime.Now;

        public DateTime RegDate { get; set; } = DateTime.Now;

        public string? Subject { get; set; }
        public string? AgentName { get; set; }
        public double Reserve { get; set; }
        public double Setteld { get; set; }

        [MaxLength(1000)]
        public string? Note { get; set; }

        public string ClaimStatus { get; set; } = " م.مغطاء";
        public string FileStatus { get; set; } = "تحت التسوية";
        public int? CourtId { get; set; }

        // Dropdowns
        public List<SelectListItem> Plaintiffs { get; set; } = new();
        public List<SelectListItem> Branches { get; set; } = new();
        public List<SelectListItem> Courts { get; set; } = new();
        public List<string> StatusList { get; set; } = new();
        public List<string> ClaimStatusList { get; set; } = new();
    }

    public class FriendlyClaimEditVM : FriendlyClaimCreateVM
    {
        public int Id { get; set; }
    }

    public class FriendlyClaimDetailsVM
    {
        public int Id { get; set; }
        public long Num { get; set; }
        public int Year { get; set; }
        public string PlaintiffName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string Place { get; set; } = string.Empty;
        public string? Subject { get; set; }
        public string? AgentName { get; set; }
        public string? CourtName { get; set; }
        public double Reserve { get; set; }
        public double Setteld { get; set; }
        public string? Note { get; set; }
        public string ClaimStatus { get; set; } = string.Empty;
        public string FileStatus { get; set; } = string.Empty;
        public DateTime IncidentDate { get; set; }
        public DateTime RegDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public List<FriendlyFileVM> Files { get; set; } = new();
    }

    public class FriendlyFileVM
    {
        public int Id { get; set; }
        public int FriendlyId { get; set; }
        public string FileName { get; set; } = string.Empty;
    }
}