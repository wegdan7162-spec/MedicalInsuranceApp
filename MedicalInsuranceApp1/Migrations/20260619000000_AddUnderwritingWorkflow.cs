using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    /// <inheritdoc />
    public partial class AddUnderwritingWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ─── أعمدة جديدة على TblInsuranceContracts ───────────────────
            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "TblInsuranceContracts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "TblInsuranceContracts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorizedSignatory",
                table: "TblInsuranceContracts",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            // ─── أعمدة جديدة على TblReceiptVouchers ─────────────────────
            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "TblReceiptVouchers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Office",
                table: "TblReceiptVouchers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankBranchName",
                table: "TblReceiptVouchers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            // ─── TblInsuredEmployees ──────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "TblInsuredEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId      = table.Column<int>(type: "int", nullable: false),
                    FullName        = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Specialization  = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone           = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    MonthlySalary   = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    EntityRate      = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    EmployeeRate    = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    EntityShare     = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    EmployeeShare   = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TotalPremium    = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    UserId          = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt       = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Del             = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt       = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy       = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteReason    = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblInsuredEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblInsuredEmployees_TblInsuranceContracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "TblInsuranceContracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InsuredEmployees_ContractId",
                table: "TblInsuredEmployees",
                column: "ContractId");

            // ─── TblBankDepositSlips ───────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "TblBankDepositSlips",
                columns: table => new
                {
                    Id                = table.Column<int>(type: "int", nullable: false)
                                              .Annotation("SqlServer:Identity", "1, 1"),
                    SlipNumber        = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DepositDate       = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClientName        = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    BankAccountId     = table.Column<int>(type: "int", nullable: true),
                    Amount            = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    CheckCount        = table.Column<int>(type: "int", nullable: false),
                    BeneficiaryBranch = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DueDate           = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentMethod     = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ReceiptVoucherId  = table.Column<int>(type: "int", nullable: true),
                    Notes             = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserId            = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt         = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Del               = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt         = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy         = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteReason      = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblBankDepositSlips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblBankDepositSlips_TblBankAccounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "TblBankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TblBankDepositSlips_TblReceiptVouchers_ReceiptVoucherId",
                        column: x => x.ReceiptVoucherId,
                        principalTable: "TblReceiptVouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankDepositSlips_Date",
                table: "TblBankDepositSlips",
                column: "DepositDate");

            // ─── TblAccountingEntries ─────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "TblAccountingEntries",
                columns: table => new
                {
                    Id                = table.Column<int>(type: "int", nullable: false)
                                              .Annotation("SqlServer:Identity", "1, 1"),
                    EntryNumber       = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EntryDate         = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description       = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SupplyOrderId     = table.Column<int>(type: "int", nullable: true),
                    DebitAmount       = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DebitAccount      = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BankDepositSlipId = table.Column<int>(type: "int", nullable: true),
                    BankDebitAmount   = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    BankDebitAccount  = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status            = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    UserId            = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt         = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Del               = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt         = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy         = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteReason      = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblAccountingEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblAccountingEntries_TblSupplyOrders_SupplyOrderId",
                        column: x => x.SupplyOrderId,
                        principalTable: "TblSupplyOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TblAccountingEntries_TblBankDepositSlips_BankDepositSlipId",
                        column: x => x.BankDepositSlipId,
                        principalTable: "TblBankDepositSlips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingEntries_Date",
                table: "TblAccountingEntries",
                column: "EntryDate");

            // ─── TblDailyJournals ─────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "TblDailyJournals",
                columns: table => new
                {
                    Id           = table.Column<int>(type: "int", nullable: false)
                                         .Annotation("SqlServer:Identity", "1, 1"),
                    JournalDate  = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount  = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Status       = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ReviewNotes  = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ApprovedAt   = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy   = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserId       = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt    = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Del          = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt    = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy    = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblDailyJournals", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyJournals_Date",
                table: "TblDailyJournals",
                column: "JournalDate");

            // ─── TblDailyJournalItems ─────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "TblDailyJournalItems",
                columns: table => new
                {
                    Id                = table.Column<int>(type: "int", nullable: false)
                                              .Annotation("SqlServer:Identity", "1, 1"),
                    JournalId         = table.Column<int>(type: "int", nullable: false),
                    SupplyOrderId     = table.Column<int>(type: "int", nullable: true),
                    ReceiptVoucherId  = table.Column<int>(type: "int", nullable: true),
                    BankDepositSlipId = table.Column<int>(type: "int", nullable: true),
                    AccountingEntryId = table.Column<int>(type: "int", nullable: true),
                    Amount            = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblDailyJournalItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblDailyJournalItems_TblDailyJournals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "TblDailyJournals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblDailyJournalItems_TblSupplyOrders_SupplyOrderId",
                        column: x => x.SupplyOrderId,
                        principalTable: "TblSupplyOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TblDailyJournalItems_TblReceiptVouchers_ReceiptVoucherId",
                        column: x => x.ReceiptVoucherId,
                        principalTable: "TblReceiptVouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TblDailyJournalItems_TblBankDepositSlips_BankDepositSlipId",
                        column: x => x.BankDepositSlipId,
                        principalTable: "TblBankDepositSlips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TblDailyJournalItems_TblAccountingEntries_AccountingEntryId",
                        column: x => x.AccountingEntryId,
                        principalTable: "TblAccountingEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "TblDailyJournalItems");
            migrationBuilder.DropTable(name: "TblDailyJournals");
            migrationBuilder.DropTable(name: "TblAccountingEntries");
            migrationBuilder.DropTable(name: "TblBankDepositSlips");
            migrationBuilder.DropTable(name: "TblInsuredEmployees");

            migrationBuilder.DropColumn(name: "Phone",               table: "TblInsuranceContracts");
            migrationBuilder.DropColumn(name: "Address",             table: "TblInsuranceContracts");
            migrationBuilder.DropColumn(name: "AuthorizedSignatory", table: "TblInsuranceContracts");

            migrationBuilder.DropColumn(name: "Department",    table: "TblReceiptVouchers");
            migrationBuilder.DropColumn(name: "Office",        table: "TblReceiptVouchers");
            migrationBuilder.DropColumn(name: "BankBranchName", table: "TblReceiptVouchers");
        }
    }
}
