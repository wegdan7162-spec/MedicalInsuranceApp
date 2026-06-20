using System;
using MedicalInsuranceApp1.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260615000000_AddUnderwritingModule")]
    public partial class AddUnderwritingModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ===== TblInsuranceContracts =====
            migrationBuilder.CreateTable(
                name: "TblInsuranceContracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractNumber         = table.Column<string>(type: "nvarchar(50)",  maxLength: 50,  nullable: true),
                    FacilityName           = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    FacilityType           = table.Column<int>   (type: "int",                          nullable: false),
                    InsuranceStartDate     = table.Column<DateTime>(type: "datetime2",                  nullable: false),
                    InsuranceEndDate       = table.Column<DateTime>(type: "datetime2",                  nullable: false),
                    PrivatePremium         = table.Column<decimal>(type: "decimal(18,3)",               nullable: false),
                    PublicPremium          = table.Column<decimal>(type: "decimal(18,3)",               nullable: false),
                    SupervisionFee         = table.Column<decimal>(type: "decimal(18,3)",               nullable: false),
                    PrepaidAmount          = table.Column<decimal>(type: "decimal(18,3)",               nullable: false),
                    UnderCollectionAmount  = table.Column<decimal>(type: "decimal(18,3)",               nullable: false),
                    TreasuryAmount         = table.Column<decimal>(type: "decimal(18,3)",               nullable: false),
                    Status                 = table.Column<string>(type: "nvarchar(30)",  maxLength: 30,  nullable: false, defaultValue: "نشط"),
                    Notes                  = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserId                 = table.Column<string>(type: "nvarchar(max)",                nullable: true),
                    CreatedAt              = table.Column<DateTime>(type: "datetime2",                  nullable: false),
                    Del                    = table.Column<bool>   (type: "bit",                         nullable: true),
                    DeletedAt              = table.Column<DateTime>(type: "datetime2",                  nullable: true),
                    DeletedBy              = table.Column<string>(type: "nvarchar(max)",                nullable: true),
                    DeleteReason           = table.Column<string>(type: "nvarchar(max)",                nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblInsuranceContracts", x => x.Id);
                });

            // ===== TblSupplyOrders =====
            migrationBuilder.CreateTable(
                name: "TblSupplyOrders",
                columns: table => new
                {
                    Id                 = table.Column<int>    (type: "int",                          nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber        = table.Column<string> (type: "nvarchar(50)",  maxLength: 50,  nullable: true),
                    OrderDate          = table.Column<DateTime>(type: "datetime2",                   nullable: false),
                    ContractId         = table.Column<int>    (type: "int",                          nullable: true),
                    FacilityName       = table.Column<string> (type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CoverageFrom       = table.Column<DateTime>(type: "datetime2",                   nullable: false),
                    CoverageTo         = table.Column<DateTime>(type: "datetime2",                   nullable: false),
                    QuarterDescription = table.Column<string> (type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Amount             = table.Column<decimal> (type: "decimal(18,3)",               nullable: false),
                    Status             = table.Column<string> (type: "nvarchar(30)",  maxLength: 30,  nullable: false, defaultValue: "قيد التحصيل"),
                    Notes              = table.Column<string> (type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserId             = table.Column<string> (type: "nvarchar(max)",                nullable: true),
                    CreatedAt          = table.Column<DateTime>(type: "datetime2",                   nullable: false),
                    Del                = table.Column<bool>   (type: "bit",                          nullable: true),
                    DeletedAt          = table.Column<DateTime>(type: "datetime2",                   nullable: true),
                    DeletedBy          = table.Column<string> (type: "nvarchar(max)",                nullable: true),
                    DeleteReason       = table.Column<string> (type: "nvarchar(max)",                nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblSupplyOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblSupplyOrders_TblInsuranceContracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "TblInsuranceContracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            // ===== TblReceiptVouchers =====
            migrationBuilder.CreateTable(
                name: "TblReceiptVouchers",
                columns: table => new
                {
                    Id                   = table.Column<int>    (type: "int",                          nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptNumber        = table.Column<string> (type: "nvarchar(50)",  maxLength: 50,  nullable: true),
                    ReceiptDate          = table.Column<DateTime>(type: "datetime2",                   nullable: false),
                    ContractId           = table.Column<int>    (type: "int",                          nullable: true),
                    SupplyOrderId        = table.Column<int>    (type: "int",                          nullable: true),
                    FacilityName         = table.Column<string> (type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DinarAmount          = table.Column<decimal> (type: "decimal(18,3)",               nullable: false),
                    DirhamAmount         = table.Column<decimal> (type: "decimal(5,3)",                nullable: false),
                    TotalAmount          = table.Column<decimal> (type: "decimal(18,3)",               nullable: false),
                    PaymentMethod        = table.Column<string> (type: "nvarchar(20)",  maxLength: 20,  nullable: false, defaultValue: "نقدي"),
                    CheckNumber          = table.Column<string> (type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BankName             = table.Column<string> (type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BankAccountId        = table.Column<int>    (type: "int",                          nullable: true),
                    CollectionType       = table.Column<int>    (type: "int",                          nullable: false),
                    SupervisionFee       = table.Column<decimal> (type: "decimal(18,3)",               nullable: false),
                    TreasuryAmount       = table.Column<decimal> (type: "decimal(18,3)",               nullable: false),
                    LinkedTransactionId  = table.Column<int>    (type: "int",                          nullable: true),
                    Notes                = table.Column<string> (type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserId               = table.Column<string> (type: "nvarchar(max)",                nullable: true),
                    CreatedAt            = table.Column<DateTime>(type: "datetime2",                   nullable: false),
                    Del                  = table.Column<bool>   (type: "bit",                          nullable: true),
                    DeletedAt            = table.Column<DateTime>(type: "datetime2",                   nullable: true),
                    DeletedBy            = table.Column<string> (type: "nvarchar(max)",                nullable: true),
                    DeleteReason         = table.Column<string> (type: "nvarchar(max)",                nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblReceiptVouchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblReceiptVouchers_TblInsuranceContracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "TblInsuranceContracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TblReceiptVouchers_TblSupplyOrders_SupplyOrderId",
                        column: x => x.SupplyOrderId,
                        principalTable: "TblSupplyOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TblReceiptVouchers_TblBankAccounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "TblBankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TblReceiptVouchers_TblBankTransactions_LinkedTransactionId",
                        column: x => x.LinkedTransactionId,
                        principalTable: "TblBankTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            // ===== TblPaymentAuthorizations =====
            migrationBuilder.CreateTable(
                name: "TblPaymentAuthorizations",
                columns: table => new
                {
                    Id                  = table.Column<int>    (type: "int",                          nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorizationNumber = table.Column<string> (type: "nvarchar(50)",  maxLength: 50,  nullable: true),
                    AuthorizationDate   = table.Column<DateTime>(type: "datetime2",                   nullable: false),
                    BeneficiaryName     = table.Column<string> (type: "nvarchar(300)", maxLength: 300, nullable: false),
                    BeneficiaryType     = table.Column<string> (type: "nvarchar(50)",  maxLength: 50,  nullable: false, defaultValue: "مسوق"),
                    Amount              = table.Column<decimal> (type: "decimal(18,3)",               nullable: false),
                    PaymentMethod       = table.Column<string> (type: "nvarchar(30)",  maxLength: 30,  nullable: false, defaultValue: "تحويل"),
                    CheckNumber         = table.Column<string> (type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BankAccountId       = table.Column<int>    (type: "int",                          nullable: true),
                    CommissionId        = table.Column<int>    (type: "int",                          nullable: true),
                    Description         = table.Column<string> (type: "nvarchar(1000)",maxLength: 1000,nullable: true),
                    Status              = table.Column<string> (type: "nvarchar(30)",  maxLength: 30,  nullable: false, defaultValue: "قيد التنفيذ"),
                    LinkedTransactionId = table.Column<int>    (type: "int",                          nullable: true),
                    Notes               = table.Column<string> (type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserId              = table.Column<string> (type: "nvarchar(max)",                nullable: true),
                    CreatedAt           = table.Column<DateTime>(type: "datetime2",                   nullable: false),
                    Del                 = table.Column<bool>   (type: "bit",                          nullable: true),
                    DeletedAt           = table.Column<DateTime>(type: "datetime2",                   nullable: true),
                    DeletedBy           = table.Column<string> (type: "nvarchar(max)",                nullable: true),
                    DeleteReason        = table.Column<string> (type: "nvarchar(max)",                nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblPaymentAuthorizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblPaymentAuthorizations_TblBankAccounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "TblBankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TblPaymentAuthorizations_TblCommissions_CommissionId",
                        column: x => x.CommissionId,
                        principalTable: "TblCommissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TblPaymentAuthorizations_TblBankTransactions_LinkedTransactionId",
                        column: x => x.LinkedTransactionId,
                        principalTable: "TblBankTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            // ===== Indexes =====
            migrationBuilder.CreateIndex(
                name: "IX_TblSupplyOrders_ContractId",
                table: "TblSupplyOrders",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_TblReceiptVouchers_Date",
                table: "TblReceiptVouchers",
                column: "ReceiptDate");

            migrationBuilder.CreateIndex(
                name: "IX_TblReceiptVouchers_ContractId",
                table: "TblReceiptVouchers",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_TblReceiptVouchers_SupplyOrderId",
                table: "TblReceiptVouchers",
                column: "SupplyOrderId",
                unique: true,
                filter: "[SupplyOrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TblReceiptVouchers_BankAccountId",
                table: "TblReceiptVouchers",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TblReceiptVouchers_LinkedTransactionId",
                table: "TblReceiptVouchers",
                column: "LinkedTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TblPaymentAuthorizations_BankAccountId",
                table: "TblPaymentAuthorizations",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TblPaymentAuthorizations_CommissionId",
                table: "TblPaymentAuthorizations",
                column: "CommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_TblPaymentAuthorizations_LinkedTransactionId",
                table: "TblPaymentAuthorizations",
                column: "LinkedTransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "TblPaymentAuthorizations");
            migrationBuilder.DropTable(name: "TblReceiptVouchers");
            migrationBuilder.DropTable(name: "TblSupplyOrders");
            migrationBuilder.DropTable(name: "TblInsuranceContracts");
        }
    }
}
