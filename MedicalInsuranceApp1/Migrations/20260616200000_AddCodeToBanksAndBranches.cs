using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    public partial class AddCodeToBanksAndBranches : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'TblBanks' AND COLUMN_NAME = 'BankCode'
                )
                BEGIN
                    ALTER TABLE TblBanks ADD BankCode nvarchar(50) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'TblBankBranches' AND COLUMN_NAME = 'BranchCode'
                )
                BEGIN
                    ALTER TABLE TblBankBranches ADD BranchCode nvarchar(50) NULL;
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'TblBanks' AND COLUMN_NAME = 'BankCode'
                )
                BEGIN
                    ALTER TABLE TblBanks DROP COLUMN BankCode;
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'TblBankBranches' AND COLUMN_NAME = 'BranchCode'
                )
                BEGIN
                    ALTER TABLE TblBankBranches DROP COLUMN BranchCode;
                END
            ");
        }
    }
}
