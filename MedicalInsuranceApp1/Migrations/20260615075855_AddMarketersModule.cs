using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketersModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ========== إنشاء جداول المسوقين ==========
            migrationBuilder.CreateTable(
                name: "TblMarketers",
                columns: table => new
                {
                    Id           = table.Column<int>(type: "int", nullable: false)
                                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name         = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Phone        = table.Column<string>(type: "nvarchar(50)",  maxLength: 50,  nullable: true),
                    Notes        = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive     = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    UserId       = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt    = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Del          = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt    = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy    = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblMarketers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblMarketerCommissionRecords",
                columns: table => new
                {
                    Id             = table.Column<int>(type: "int", nullable: false)
                                         .Annotation("SqlServer:Identity", "1, 1"),
                    MarketerId     = table.Column<int>(type: "int", nullable: false),
                    SettlementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckNumber    = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes          = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserId         = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt      = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Del            = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt      = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy      = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteReason   = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblMarketerCommissionRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblMarketerCommissionRecords_TblMarketers_MarketerId",
                        column: x => x.MarketerId,
                        principalTable: "TblMarketers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblMarketerCommissionItems",
                columns: table => new
                {
                    Id               = table.Column<int>(type: "int", nullable: false)
                                           .Annotation("SqlServer:Identity", "1, 1"),
                    RecordId         = table.Column<int>(type: "int", nullable: false),
                    FacilityName     = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Period           = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TotalPremiums    = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ReturnedPremiums = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    NetPremiums      = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    CommissionRate   = table.Column<decimal>(type: "decimal(5,4)",  nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblMarketerCommissionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblMarketerCommissionItems_TblMarketerCommissionRecords_RecordId",
                        column: x => x.RecordId,
                        principalTable: "TblMarketerCommissionRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblMarketerCommissionRecords_MarketerId",
                table: "TblMarketerCommissionRecords",
                column: "MarketerId");

            migrationBuilder.CreateIndex(
                name: "IX_TblMarketerCommissionItems_RecordId",
                table: "TblMarketerCommissionItems",
                column: "RecordId");

            // ========== تصحيح Foreign Keys للجداول الأخرى ==========
            migrationBuilder.DropForeignKey(
                name: "FK_TblPaymentAuthorizations_TblBankTransactions_LinkedTransactionId",
                table: "TblPaymentAuthorizations");

            migrationBuilder.DropForeignKey(
                name: "FK_TblReceiptVouchers_TblBankTransactions_LinkedTransactionId",
                table: "TblReceiptVouchers");

            migrationBuilder.AddForeignKey(
                name: "FK_TblPaymentAuthorizations_TblBankTransactions_LinkedTransactionId",
                table: "TblPaymentAuthorizations",
                column: "LinkedTransactionId",
                principalTable: "TblBankTransactions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TblReceiptVouchers_TblBankTransactions_LinkedTransactionId",
                table: "TblReceiptVouchers",
                column: "LinkedTransactionId",
                principalTable: "TblBankTransactions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblPaymentAuthorizations_TblBankTransactions_LinkedTransactionId",
                table: "TblPaymentAuthorizations");

            migrationBuilder.DropForeignKey(
                name: "FK_TblReceiptVouchers_TblBankTransactions_LinkedTransactionId",
                table: "TblReceiptVouchers");

            migrationBuilder.AddForeignKey(
                name: "FK_TblPaymentAuthorizations_TblBankTransactions_LinkedTransactionId",
                table: "TblPaymentAuthorizations",
                column: "LinkedTransactionId",
                principalTable: "TblBankTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TblReceiptVouchers_TblBankTransactions_LinkedTransactionId",
                table: "TblReceiptVouchers",
                column: "LinkedTransactionId",
                principalTable: "TblBankTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.DropTable(name: "TblMarketerCommissionItems");
            migrationBuilder.DropTable(name: "TblMarketerCommissionRecords");
            migrationBuilder.DropTable(name: "TblMarketers");
        }
    }
}
