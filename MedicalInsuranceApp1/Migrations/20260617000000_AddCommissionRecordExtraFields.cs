using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    /// <inheritdoc />
    public partial class AddCommissionRecordExtraFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupplyOrderNumbers",
                table: "TblMarketerCommissionRecords",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiptNumbers",
                table: "TblMarketerCommissionRecords",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreparedBy",
                table: "TblMarketerCommissionRecords",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedBy",
                table: "TblMarketerCommissionRecords",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplyOrderNumbers",
                table: "TblMarketerCommissionRecords");

            migrationBuilder.DropColumn(
                name: "ReceiptNumbers",
                table: "TblMarketerCommissionRecords");

            migrationBuilder.DropColumn(
                name: "PreparedBy",
                table: "TblMarketerCommissionRecords");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                table: "TblMarketerCommissionRecords");
        }
    }
}
