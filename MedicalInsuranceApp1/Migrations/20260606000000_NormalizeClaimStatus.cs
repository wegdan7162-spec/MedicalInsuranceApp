using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    public partial class NormalizeClaimStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // تحويل كل القيم القديمة (NULL, فارغ, إنجليزي) إلى عربية
            migrationBuilder.Sql("UPDATE TblOutters  SET ClaimStatus = N'تحت التسوية' WHERE ClaimStatus IS NULL OR ClaimStatus = '' OR ClaimStatus = 'UnderSettlement'");
            migrationBuilder.Sql("UPDATE TblFriendly SET ClaimStatus = N'تحت التسوية' WHERE ClaimStatus IS NULL OR ClaimStatus = '' OR ClaimStatus = 'UnderSettlement'");
            // تأكد من عدم وجود NULL بعد الآن
            migrationBuilder.Sql("UPDATE TblOutters  SET ClaimStatus = N'تحت التسوية' WHERE ClaimStatus IS NULL");
            migrationBuilder.Sql("UPDATE TblFriendly SET ClaimStatus = N'تحت التسوية' WHERE ClaimStatus IS NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE TblOutters  SET ClaimStatus = 'UnderSettlement' WHERE ClaimStatus = N'تحت التسوية'");
            migrationBuilder.Sql("UPDATE TblFriendly SET ClaimStatus = 'UnderSettlement' WHERE ClaimStatus = N'تحت التسوية'");
        }
    }
}
