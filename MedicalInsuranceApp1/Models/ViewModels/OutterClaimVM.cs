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

        public string ClaimStatus { get; set; } = "تحت التسوية";
        public string FileStatus { get; set; } = "نشط";

        [MaxLength(1000)]
        public string? Note { get; set; }

        // Dropdowns
        public List<SelectListItem> Plaintiffs { get; set; } = new();
        public List<SelectListItem> Branches { get; set; } = new();
        public List<SelectListItem> Courts { get; set; } = new();
        public List<string> StatusList { get; set; } = new();
        public List<string> FileStatusList { get; set; } = new();
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
        public DateTime RegDate { get; set; }
        public DateTime? IncidentDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public List<OutterFileVM> Files { get; set; } = new();
    }

    public class OutterFileVM
    {
        public int Id { get; set; }
        public int OutterId { get; set; }
        public string FileName { get; set; } = string.Empty;
    }
}
