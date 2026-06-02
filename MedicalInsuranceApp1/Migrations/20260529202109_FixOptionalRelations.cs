using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    /// <inheritdoc />
    public partial class FixOptionalRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblCourtsN_TblBranch_BranchId",
                table: "TblCourtsN");

            migrationBuilder.DropForeignKey(
                name: "FK_TblCourtsN_TblCourt1_CourtId",
                table: "TblCourtsN");

            migrationBuilder.DropForeignKey(
                name: "FK_TblFriendly_TblBranch_BranchId",
                table: "TblFriendly");

            migrationBuilder.DropForeignKey(
                name: "FK_TblOutters_TblBranch_BranchId",
                table: "TblOutters");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "TblOutters",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "TblFriendly",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CourtId",
                table: "TblCourtsN",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "TblCourtsN",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_TblCourtsN_TblBranch_BranchId",
                table: "TblCourtsN",
                column: "BranchId",
                principalTable: "TblBranch",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TblCourtsN_TblCourt1_CourtId",
                table: "TblCourtsN",
                column: "CourtId",
                principalTable: "TblCourt1",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TblFriendly_TblBranch_BranchId",
                table: "TblFriendly",
                column: "BranchId",
                principalTable: "TblBranch",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TblOutters_TblBranch_BranchId",
                table: "TblOutters",
                column: "BranchId",
                principalTable: "TblBranch",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblCourtsN_TblBranch_BranchId",
                table: "TblCourtsN");

            migrationBuilder.DropForeignKey(
                name: "FK_TblCourtsN_TblCourt1_CourtId",
                table: "TblCourtsN");

            migrationBuilder.DropForeignKey(
                name: "FK_TblFriendly_TblBranch_BranchId",
                table: "TblFriendly");

            migrationBuilder.DropForeignKey(
                name: "FK_TblOutters_TblBranch_BranchId",
                table: "TblOutters");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "TblOutters",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "TblFriendly",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CourtId",
                table: "TblCourtsN",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "TblCourtsN",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TblCourtsN_TblBranch_BranchId",
                table: "TblCourtsN",
                column: "BranchId",
                principalTable: "TblBranch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblCourtsN_TblCourt1_CourtId",
                table: "TblCourtsN",
                column: "CourtId",
                principalTable: "TblCourt1",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblFriendly_TblBranch_BranchId",
                table: "TblFriendly",
                column: "BranchId",
                principalTable: "TblBranch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TblOutters_TblBranch_BranchId",
                table: "TblOutters",
                column: "BranchId",
                principalTable: "TblBranch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
