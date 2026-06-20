using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    public partial class RemoveClaimTypeFromOutterClaim : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Fix NULL values in ClaimType column
            migrationBuilder.Sql("UPDATE TblOutters SET ClaimType = N'مغطى' WHERE ClaimType IS NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
