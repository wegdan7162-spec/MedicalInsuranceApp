using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    public partial class AddDeleteFieldsToAllTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // TblBranch
            migrationBuilder.AddColumn<DateTime>("DeletedAt", "TblBranch", nullable: true);
            migrationBuilder.AddColumn<string>("DeletedBy", "TblBranch", maxLength: 256, nullable: true);
            migrationBuilder.AddColumn<string>("DeleteReason", "TblBranch", maxLength: 500, nullable: true);

            // TblCourt1
            migrationBuilder.AddColumn<bool>("Del", "TblCourt1", nullable: true);
            migrationBuilder.AddColumn<DateTime>("DeletedAt", "TblCourt1", nullable: true);
            migrationBuilder.AddColumn<string>("DeletedBy", "TblCourt1", maxLength: 256, nullable: true);
            migrationBuilder.AddColumn<string>("DeleteReason", "TblCourt1", maxLength: 500, nullable: true);

            // TblPlainitiffName
            migrationBuilder.AddColumn<DateTime>("DeletedAt", "TblPlainitiffName", nullable: true);
            migrationBuilder.AddColumn<string>("DeletedBy", "TblPlainitiffName", maxLength: 256, nullable: true);
            migrationBuilder.AddColumn<string>("DeleteReason", "TblPlainitiffName", maxLength: 500, nullable: true);

            // TblFriendly
            migrationBuilder.AddColumn<bool>("Del", "TblFriendly", nullable: true);
            migrationBuilder.AddColumn<DateTime>("DeletedAt", "TblFriendly", nullable: true);
            migrationBuilder.AddColumn<string>("DeletedBy", "TblFriendly", maxLength: 256, nullable: true);
            migrationBuilder.AddColumn<string>("DeleteReason", "TblFriendly", maxLength: 500, nullable: true);

            // TblOutters
            migrationBuilder.AddColumn<bool>("Del", "TblOutters", nullable: true);
            migrationBuilder.AddColumn<DateTime>("DeletedAt", "TblOutters", nullable: true);
            migrationBuilder.AddColumn<string>("DeletedBy", "TblOutters", maxLength: 256, nullable: true);
            migrationBuilder.AddColumn<string>("DeleteReason", "TblOutters", maxLength: 500, nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("DeletedAt", "TblBranch");
            migrationBuilder.DropColumn("DeletedBy", "TblBranch");
            migrationBuilder.DropColumn("DeleteReason", "TblBranch");

            migrationBuilder.DropColumn("Del", "TblCourt1");
            migrationBuilder.DropColumn("DeletedAt", "TblCourt1");
            migrationBuilder.DropColumn("DeletedBy", "TblCourt1");
            migrationBuilder.DropColumn("DeleteReason", "TblCourt1");

            migrationBuilder.DropColumn("DeletedAt", "TblPlainitiffName");
            migrationBuilder.DropColumn("DeletedBy", "TblPlainitiffName");
            migrationBuilder.DropColumn("DeleteReason", "TblPlainitiffName");

            migrationBuilder.DropColumn("Del", "TblFriendly");
            migrationBuilder.DropColumn("DeletedAt", "TblFriendly");
            migrationBuilder.DropColumn("DeletedBy", "TblFriendly");
            migrationBuilder.DropColumn("DeleteReason", "TblFriendly");

            migrationBuilder.DropColumn("Del", "TblOutters");
            migrationBuilder.DropColumn("DeletedAt", "TblOutters");
            migrationBuilder.DropColumn("DeletedBy", "TblOutters");
            migrationBuilder.DropColumn("DeleteReason", "TblOutters");
        }
    }
}
