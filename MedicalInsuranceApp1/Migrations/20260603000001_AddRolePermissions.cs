using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    public partial class AddRolePermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblRolePermissions",
                columns: table => new
                {
                    Id       = table.Column<int>(nullable: false)
                                    .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId   = table.Column<string>(maxLength: 450, nullable: false),
                    Module   = table.Column<string>(maxLength: 100, nullable: false),
                    CanView  = table.Column<bool>(nullable: false, defaultValue: false),
                    CanAdd   = table.Column<bool>(nullable: false, defaultValue: false),
                    CanEdit  = table.Column<bool>(nullable: false, defaultValue: false),
                    CanDelete= table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblRolePermissions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_Module",
                table: "TblRolePermissions",
                columns: new[] { "RoleId", "Module" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "TblRolePermissions");
        }
    }
}
