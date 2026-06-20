using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplyOrderWorkflowColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WorkflowStatus",
                table: "TblSupplyOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "مسودة");

            migrationBuilder.AddColumn<DateTime>(
                name: "UnderwritingApprovedAt",
                table: "TblSupplyOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnderwritingApprovedBy",
                table: "TblSupplyOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UnderwritingVerifiedAt",
                table: "TblSupplyOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnderwritingVerifiedBy",
                table: "TblSupplyOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "WorkflowStatus",          table: "TblSupplyOrders");
            migrationBuilder.DropColumn(name: "UnderwritingApprovedAt",  table: "TblSupplyOrders");
            migrationBuilder.DropColumn(name: "UnderwritingApprovedBy",  table: "TblSupplyOrders");
            migrationBuilder.DropColumn(name: "UnderwritingVerifiedAt",  table: "TblSupplyOrders");
            migrationBuilder.DropColumn(name: "UnderwritingVerifiedBy",  table: "TblSupplyOrders");
        }
    }
}
