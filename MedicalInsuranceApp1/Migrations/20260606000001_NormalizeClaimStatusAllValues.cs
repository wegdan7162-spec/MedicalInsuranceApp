using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    /// <summary>
    /// توحيد جميع قيم ClaimStatus القديمة أو المختلفة في أشكالها إلى القيم العربية المعتمدة
    /// يعالج: مسدد → مسددة ، مغلق → مغلقة ، وأي قيم إنجليزية متبقية
    /// </summary>
    public partial class NormalizeClaimStatusAllValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // تصحيح "مسدد" (بدون تاء التأنيث) → "مسددة"
            migrationBuilder.Sql("UPDATE TblOutters  SET ClaimStatus = N'مسددة' WHERE ClaimStatus = N'مسدد'");
            migrationBuilder.Sql("UPDATE TblFriendly SET ClaimStatus = N'مسددة' WHERE ClaimStatus = N'مسدد'");

            // تصحيح "مغلق" → "مغلقة"
            migrationBuilder.Sql("UPDATE TblOutters  SET ClaimStatus = N'مغلقة' WHERE ClaimStatus = N'مغلق'");
            migrationBuilder.Sql("UPDATE TblFriendly SET ClaimStatus = N'مغلقة' WHERE ClaimStatus = N'مغلق'");

            // تصحيح أي قيم إنجليزية متبقية
            migrationBuilder.Sql("UPDATE TblOutters  SET ClaimStatus = N'تحت التسوية' WHERE ClaimStatus IS NULL OR ClaimStatus = '' OR ClaimStatus = 'UnderSettlement'");
            migrationBuilder.Sql("UPDATE TblFriendly SET ClaimStatus = N'تحت التسوية' WHERE ClaimStatus IS NULL OR ClaimStatus = '' OR ClaimStatus = 'UnderSettlement'");
            migrationBuilder.Sql("UPDATE TblOutters  SET ClaimStatus = N'مسددة'       WHERE ClaimStatus = 'Paid'");
            migrationBuilder.Sql("UPDATE TblFriendly SET ClaimStatus = N'مسددة'       WHERE ClaimStatus = 'Paid'");
            migrationBuilder.Sql("UPDATE TblOutters  SET ClaimStatus = N'مغلقة'       WHERE ClaimStatus = 'Closed'");
            migrationBuilder.Sql("UPDATE TblFriendly SET ClaimStatus = N'مغلقة'       WHERE ClaimStatus = 'Closed'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // لا يوجد rollback منطقي لتوحيد البيانات
        }
    }
}
