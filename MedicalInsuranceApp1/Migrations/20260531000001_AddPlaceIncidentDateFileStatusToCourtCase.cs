using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaceIncidentDateFileStatusToCourtCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Place",
                table: "TblCourtsN",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IncidentDate",
                table: "TblCourtsN",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileStatus",
                table: "TblCourtsN",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "نشط");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Place", table: "TblCourtsN");
            migrationBuilder.DropColumn(name: "IncidentDate", table: "TblCourtsN");
            migrationBuilder.DropColumn(name: "FileStatus", table: "TblCourtsN");
        }
    }
}
