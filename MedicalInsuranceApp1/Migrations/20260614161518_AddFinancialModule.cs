using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancialModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblBankAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BankBranch = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OpeningBalance = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Del = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblBankAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblCaseFeeSettlement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    CourtId = table.Column<int>(type: "int", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    LawyerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AgreeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ContractDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Del = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblCaseFeeSettlement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblCaseFeeSettlement_TblBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "TblBranch",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TblCaseFeeSettlement_TblCourt1_CourtId",
                        column: x => x.CourtId,
                        principalTable: "TblCourt1",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TblCommissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    PremiumAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    BeneficiaryName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    AuthorizationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EntryNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Del = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblCommissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblPrivateLawyerSettlement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    CourtId = table.Column<int>(type: "int", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    LawyerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LawyerPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LawyerIBAN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AgreeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContractDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContractEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Del = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblPrivateLawyerSettlement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblPrivateLawyerSettlement_TblBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "TblBranch",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TblPrivateLawyerSettlement_TblCourt1_CourtId",
                        column: x => x.CourtId,
                        principalTable: "TblCourt1",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TblReservationSettlement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimNum = table.Column<long>(type: "bigint", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    PlaintiffNameId = table.Column<int>(type: "int", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    ReserveAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SettledAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReservationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Del = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "TblBankTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankAccountId = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntryNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DocType = table.Column<int>(type: "int", nullable: true),
                    DocumentNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    RunningBalance = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    FriendlyClaimId = table.Column<int>(type: "int", nullable: true),
                    OutterClaimId = table.Column<int>(type: "int", nullable: true),
                    CommissionId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Del = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblBankTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblBankTransactions_TblBankAccounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "TblBankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblBankTransactions_TblCommissions_CommissionId",
                        column: x => x.CommissionId,
                        principalTable: "TblCommissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TblBankTransactions_TblFriendly_FriendlyClaimId",
                        column: x => x.FriendlyClaimId,
                        principalTable: "TblFriendly",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TblBankTransactions_TblOutters_OutterClaimId",
                        column: x => x.OutterClaimId,
                        principalTable: "TblOutters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_AccountId",
                table: "TblBankTransactions",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_Date",
                table: "TblBankTransactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_TblBankTransactions_CommissionId",
                table: "TblBankTransactions",
                column: "CommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_TblBankTransactions_FriendlyClaimId",
                table: "TblBankTransactions",
                column: "FriendlyClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_TblBankTransactions_OutterClaimId",
                table: "TblBankTransactions",
                column: "OutterClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_TblCaseFeeSettlement_BranchId",
                table: "TblCaseFeeSettlement",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TblCaseFeeSettlement_CourtId",
                table: "TblCaseFeeSettlement",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_TblPrivateLawyerSettlement_BranchId",
                table: "TblPrivateLawyerSettlement",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TblPrivateLawyerSettlement_CourtId",
                table: "TblPrivateLawyerSettlement",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_TblReservationSettlement_BranchId",
                table: "TblReservationSettlement",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TblReservationSettlement_PlaintiffNameId",
                table: "TblReservationSettlement",
                column: "PlaintiffNameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblBankTransactions");

            migrationBuilder.DropTable(
                name: "TblCaseFeeSettlement");

            migrationBuilder.DropTable(
                name: "TblPrivateLawyerSettlement");

            migrationBuilder.DropTable(
                name: "TblReservationSettlement");

            migrationBuilder.DropTable(
                name: "TblBankAccounts");

            migrationBuilder.DropTable(
                name: "TblCommissions");
        }
    }
}
