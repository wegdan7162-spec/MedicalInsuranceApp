//using System.ComponentModel.DataAnnotations;

//namespace MedicalInsuranceApp1.Models.ViewModels
//{
//    public class CourtCaseListVM
//    {
//        public int Id { get; set; }
//        public string ClaimNumber { get; set; } = string.Empty;
//        public int Year { get; set; }
//        public string PlaintiffName { get; set; } = string.Empty;
//        public string BranchName { get; set; } = string.Empty;
//        public string CourtName { get; set; } = string.Empty;
//        public double Reserve { get; set; }
//        public double Setteld { get; set; }
//        public string Place { get; set; } = string.Empty;
//        public DateTime? IncidentDate { get; set; }
//        public string FileStatus { get; set; } = string.Empty;
//        public string ClaimStatus { get; set; } = string.Empty;
//        public string ClaimType { get; set; } = string.Empty;
//        public DateTime RegDate { get; set; }
//        public int FilesCount { get; set; }
//    }

//    public class CourtCaseCreateVM
//    {
//        [Required(ErrorMessage = "اسم المدعي مطلوب")]
//        public string PlaintiffName { get; set; } = string.Empty;

//        [Required(ErrorMessage = "الفرع مطلوب")]
//        public int BranchId { get; set; }

//        [Required(ErrorMessage = "المحكمة مطلوبة")]
//        public int CourtId { get; set; }

//        [Required(ErrorMessage = "تاريخ التسجيل مطلوب")]
//        public DateTime RegDate { get; set; } = DateTime.Now;

//        public double Reserve { get; set; }
//        public double Setteld { get; set; }

//        [MaxLength(1000)]
//        public string? Note { get; set; }

//        public string? Place { get; set; }
//        public DateTime? IncidentDate { get; set; }
//        public string FileStatus { get; set; } = "نشط";
//        public string ClaimStatus { get; set; } = "تحت التسوية";
//        public string ClaimType { get; set; } = "مغطى";

//        // Dropdowns
//        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Branches { get; set; } = new();
//        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Courts { get; set; } = new();
//        public List<string> StatusList { get; set; } = new();
//        public List<string> ClaimTypeList { get; set; } = new();
//        public List<string> FileStatusList { get; set; } = new();
//    }

//    public class CourtCaseEditVM : CourtCaseCreateVM
//    {
//        public int Id { get; set; }
//    }

//    public class CourtCaseDetailsVM
//    {
//        public int Id { get; set; }
//        public long Num { get; set; }
//        public int Year { get; set; }
//        public string PlaintiffName { get; set; } = string.Empty;
//        public string BranchName { get; set; } = string.Empty;
//        public string CourtName { get; set; } = string.Empty;
//        public double Reserve { get; set; }
//        public double Setteld { get; set; }
//        public string? Note { get; set; }
//        public string? Place { get; set; }
//        public DateTime? IncidentDate { get; set; }
//        public string FileStatus { get; set; } = string.Empty;
//        public string ClaimStatus { get; set; } = string.Empty;
//        public string ClaimType { get; set; } = string.Empty;
//        public DateTime RegDate { get; set; }
//        public string CreatedBy { get; set; } = string.Empty;
//        public List<CourtFileVM> Files { get; set; } = new();
//    }

//    public class CourtFileVM
//    {
//        public int Id { get; set; }
//        public string FileName { get; set; } = string.Empty;
//        public string FileExtension { get; set; } = string.Empty;
//        public int CourtsNId { get; set; }
//    }
//}