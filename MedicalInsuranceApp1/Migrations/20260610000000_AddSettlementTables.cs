using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    /// <inheritdoc />
    public partial class AddSettlementTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ===== تسوية حجوزات =====
            migrationBuilder.CreateTable(
                name: "TblReservationSettlement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimNum         = table.Column<long>(type: "bigint", nullable: false),
                    Year             = table.Column<int>(type: "int", nullable: false),
                    PlaintiffNameId  = table.Column<int>(type: "int", nullable: true),
                    BranchId         = table.Column<int>(type: "int", nullable: true),
                    ReserveAmount    = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SettledAmount    = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReservationDate  = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status           = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "قيد التسوية"),
                    Notes            = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UserId           = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt        = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Del              = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt        = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy        = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteReason     = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblReservationSettlement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblReservationSettlement_TblBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "TblBranch",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TblReservationSettlement_TblPlainitiffName_PlaintiffNameId",
                        column: x => x.PlaintiffNameId,
                        principalTable: "TblPlainitiffName",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblReservationSettlement_BranchId",
                table: "TblReservationSettlement",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TblReservationSettlement_PlaintiffNameId",
                table: "TblReservationSettlement",
                column: "PlaintiffNameId");

            // ===== تسوية أتعاب القضايا =====
            migrationBuilder.CreateTable(
                name: "TblCaseFeeSettlement",
                columns: table => new
                {
                    Id           = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseNumber   = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Year         = table.Column<int>(type: "int", nullable: false),
                    CourtId      = table.Column<int>(type: "int", nullable: true),
                    BranchId     = table.Column<int>(type: "int", nullable: true),
                    LawyerName   = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AgreeAmount  = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAmount   = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ContractDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status       = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "قيد الدفع"),
                    Notes        = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UserId       = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt    = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Del          = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt    = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy    = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblCaseFeeSettlement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblCaseFeeSettlement_TblCourt1_CourtId",
                        column: x => x.CourtId,
                        principalTable: "TblCourt1",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TblCaseFeeSettlement_TblBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "TblBranch",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblCaseFeeSettlement_CourtId",
                table: "TblCaseFeeSettlement",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_TblCaseFeeSettlement_BranchId",
                table: "TblCaseFeeSettlement",
                column: "BranchId");

            // ===== تسوية أتعاب المحاماة الخواص =====
            migrationBuilder.CreateTable(
                name: "TblPrivateLawyerSettlement",
                columns: table => new
                {
                    Id              = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseNumber      = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Year            = table.Column<int>(type: "int", nullable: false),
                    CourtId         = table.Column<int>(type: "int", nullable: true),
                    BranchId        = table.Column<int>(type: "int", nullable: true),
                    LawyerName      = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LawyerPhone     = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LawyerIBAN      = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AgreeAmount     = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAmount      = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod   = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "تحويل بنكي"),
                    ContractDate    = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContractEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status          = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "قيد الدفع"),
                    Notes           = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UserId          = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt       = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Del             = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt       = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy       = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteReason    = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblPrivateLawyerSettlement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblPrivateLawyerSettlement_TblCourt1_CourtId",
                        column: x => x.CourtId,
                        principalTable: "TblCourt1",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TblPrivateLawyerSettlement_TblBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "TblBranch",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblPrivateLawyerSettlement_CourtId",
                table: "TblPrivateLawyerSettlement",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_TblPrivateLawyerSettlement_BranchId",
                table: "TblPrivateLawyerSettlement",
                column: "BranchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "TblReservationSettlement");
            migrationBuilder.DropTable(name: "TblCaseFeeSettlement");
            migrationBuilder.DropTable(name: "TblPrivateLawyerSettlement");
        }
    }
}
