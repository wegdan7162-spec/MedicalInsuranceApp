using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    /// <inheritdoc />
    public partial class AddClaimStatusToCourtCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClaimStatus",
                table: "TblCourtsN",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClaimStatus",
                table: "TblCourtsN");
        }
    }
}
