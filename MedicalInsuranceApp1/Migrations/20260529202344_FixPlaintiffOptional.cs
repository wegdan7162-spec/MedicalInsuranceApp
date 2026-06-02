using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    /// <inheritdoc />
    public partial class FixPlaintiffOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblFriendly_TblPlainitiffName_PlaintiffNameId",
                table: "TblFriendly");

            migrationBuilder.DropForeignKey(
                name: "FK_TblOutters_TblPlainitiffName_PlaintiffNameId",
                table: "TblOutters");

            migrationBuilder.AlterColumn<int>(
                name: "PlaintiffNameId",
                table: "TblOutters",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PlaintiffNameId",
                table: "TblFriendly",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_TblFriendly_TblPlainitiffName_PlaintiffNameId",
                table: "TblFriendly",
                column: "PlaintiffNameId",
                principalTable: "TblPlainitiffName",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TblOutters_TblPlainitiffName_PlaintiffNameId",
                table: "TblOutters",
                column: "PlaintiffNameId",
                principalTable: "TblPlainitiffName",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblFriendly_TblPlainitiffName_PlaintiffNameId",
                table: "TblFriendly");

            migrationBuilder.DropForeignKey(
                name: "FK_TblOutters_TblPlainitiffName_PlaintiffNameId",
                table: "TblOutters");

            migrationBuilder.AlterColumn<int>(
                name: "PlaintiffNameId",
                table: "TblOutters",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlaintiffNameId",
                table: "TblFriendly",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TblFriendly_TblPlainitiffName_PlaintiffNameId",
                table: "TblFriendly",
                column: "PlaintiffNameId",
                principalTable: "TblPlainitiffName",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblOutters_TblPlainitiffName_PlaintiffNameId",
                table: "TblOutters",
                column: "PlaintiffNameId",
                principalTable: "TblPlainitiffName",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
