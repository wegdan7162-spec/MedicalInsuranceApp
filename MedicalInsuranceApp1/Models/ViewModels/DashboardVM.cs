namespace MedicalInsuranceApp1.Models.ViewModels
{
    public class DashboardVM
    {
        // ===== إحصائيات المطالبات =====
        public int TotalCourtCases { get; set; }
        public int TotalFriendly { get; set; }
        public int TotalOutter { get; set; }
        public int TotalClaims => TotalCourtCases + TotalFriendly;

        // ===== إحصائيات مالية =====
        public double TotalReserve { get; set; }
        public double TotalSettled { get; set; }

        // ===== إحصائيات المستخدمين =====
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }

        // ===== آخر المطالبات =====
        public List<RecentClaimVM> RecentClaims { get; set; } = new();

        // ===== رسم بياني شهري =====
        public List<MonthlyStatVM> MonthlyStats { get; set; } = new();
    }

    public class RecentClaimVM
    {
        public int Id { get; set; }
        public string ClaimNumber { get; set; } = string.Empty;
        public string ClaimType { get; set; } = string.Empty;
        public string Plaintiff { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public double Reserve { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RegDate { get; set; }
    }

    public class MonthlyStatVM
    {
        public string Month { get; set; } = string.Empty;
        public int Courts { get; set; }
        public int Friendly { get; set; }
        public int Outter { get; set; }
    }
}