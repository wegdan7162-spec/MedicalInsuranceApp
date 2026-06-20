using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCustomFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblCourtFiles");

            migrationBuilder.DropTable(
                name: "TblCourtsN");

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "TblPlainitiffName",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeleteReason",
                table: "TblPlainitiffName",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "TblOutters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeleteReason",
                table: "TblOutters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "TblFriendly",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeleteReason",
                table: "TblFriendly",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "TblCourt1",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeleteReason",
                table: "TblCourt1",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "TblBranch",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeleteReason",
                table: "TblBranch",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Approval",
                table: "AppUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AppUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessTime",
                table: "AppUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "AppUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WorkPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    Specialization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id");
                });

            //migrationBuilder.CreateTable(
            //    name: "TblRolePermissions",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        RoleId = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Module = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        CanView = table.Column<bool>(type: "bit", nullable: false),
            //        CanAdd = table.Column<bool>(type: "bit", nullable: false),
            //        CanEdit = table.Column<bool>(type: "bit", nullable: false),
            //        CanDelete = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TblRolePermissions", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "TblUserActivity",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Controller = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        PageTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        VisitedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TblUserActivity", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "TblUserSession",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        LoginAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        LogoutAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        DurationMinutes = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TblUserSession", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserProfile",
            //    columns: table => new
            //    {
            //        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Gender = table.Column<bool>(type: "bit", nullable: false),
            //        UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
            //        Created = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserProfile", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_UserProfile_AppUsers_UserId",
            //            column: x => x.UserId,
            //            principalTable: "AppUsers",
            //            principalColumn: "Id");
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Employee_UserId",
            //    table: "Employee",
            //    column: "UserId",
            //    unique: true,
            //    filter: "[UserId] IS NOT NULL");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserProfile_UserId",
            //    table: "UserProfile",
            //    column: "UserId",
            //    unique: true,
            //    filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "TblRolePermissions");

            migrationBuilder.DropTable(
                name: "TblUserActivity");

            migrationBuilder.DropTable(
                name: "TblUserSession");

            migrationBuilder.DropTable(
                name: "UserProfile");

            migrationBuilder.DropColumn(
                name: "Approval",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "LastAccessTime",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "AppUsers");

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "TblPlainitiffName",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeleteReason",
                table: "TblPlainitiffName",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "TblOutters",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeleteReason",
                table: "TblOutters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "TblFriendly",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeleteReason",
                table: "TblFriendly",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "TblCourt1",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeleteReason",
                table: "TblCourt1",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "TblBranch",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeleteReason",
                table: "TblBranch",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "TblCourtsN",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    CourtId = table.Column<int>(type: "int", nullable: true),
                    ClaimStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileIcon = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncidentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Num = table.Column<long>(type: "bigint", nullable: false),
                    Place = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlaintiffName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reserve = table.Column<double>(type: "float", nullable: false),
                    Setteld = table.Column<double>(type: "float", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblCourtsN", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblCourtsN_TblBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "TblBranch",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TblCourtsN_TblCourt1_CourtId",
                        column: x => x.CourtId,
                        principalTable: "TblCourt1",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TblCourtFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourtsNId = table.Column<int>(type: "int", nullable: false),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblCourtFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblCourtFiles_TblCourtsN_CourtsNId",
                        column: x => x.CourtsNId,
                        principalTable: "TblCourtsN",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblCourtFiles_CourtsNId",
                table: "TblCourtFiles",
                column: "CourtsNId");

            migrationBuilder.CreateIndex(
                name: "IX_CourtCases_Year_Num",
                table: "TblCourtsN",
                columns: new[] { "Year", "Num" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TblCourtsN_BranchId",
                table: "TblCourtsN",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TblCourtsN_CourtId",
                table: "TblCourtsN",
                column: "CourtId");
        }
    }
}
