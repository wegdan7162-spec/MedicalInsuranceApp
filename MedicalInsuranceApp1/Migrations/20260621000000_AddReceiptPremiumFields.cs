using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    public partial class AddReceiptPremiumFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrivatePremium",
                table: "TblReceiptVouchers",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PublicPremium",
                table: "TblReceiptVouchers",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AdvancePremium",
                table: "TblReceiptVouchers",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PendingPremium",
                table: "TblReceiptVouchers",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "PrivatePremium",  table: "TblReceiptVouchers");
            migrationBuilder.DropColumn(name: "PublicPremium",   table: "TblReceiptVouchers");
            migrationBuilder.DropColumn(name: "AdvancePremium",  table: "TblReceiptVouchers");
            migrationBuilder.DropColumn(name: "PendingPremium",  table: "TblReceiptVouchers");
        }
    }
}
