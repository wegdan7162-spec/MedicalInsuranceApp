using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    /// <summary>
    /// إضافة بيانات المسوقين الثلاثة الذين لم تُلتقط بياناتهم في المرحلة الأولى
    /// (رجب المسلاتي — عبدالمطلب أبوبكر ساسي — على محمود على السريتي)
    /// </summary>
    public partial class FixMarketerCommissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ========== رجب المسلاتي ==========
            // المختبر الطبي المرجعي طرابلس | 7-12/2020 | نسبة 5%
            migrationBuilder.Sql(@"
                DECLARE @mid INT = (SELECT TOP 1 Id FROM TblMarketers WHERE Name = N'رجب المسلاتي');
                IF @mid IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TblMarketerCommissionRecords WHERE MarketerId = @mid)
                BEGIN
                    INSERT INTO TblMarketerCommissionRecords (MarketerId, SettlementDate, CreatedAt, Del)
                    VALUES (@mid, '2021-01-18', GETDATE(), 0);
                    DECLARE @rid1 INT = SCOPE_IDENTITY();
                    INSERT INTO TblMarketerCommissionItems (RecordId, FacilityName, Period, TotalPremiums, ReturnedPremiums, NetPremiums, CommissionRate, CommissionAmount)
                    VALUES (@rid1,
                        N'المختبر الطبي المرجعي طرابلس',
                        N'7ـ 8 ـ 9 ـ 10 ـ 11 ـ 12 /2020',
                        44538.999, 15588.650, 28950.349, 0.05, 1447.517);
                END
            ");

            // ========== عبدالمطلب أبوبكر ساسي ==========
            // شركة الزويتينة للنفط | 2025/03/13 حتى 2025/12/31 | نسبة 10%
            migrationBuilder.Sql(@"
                DECLARE @mid INT = (SELECT TOP 1 Id FROM TblMarketers WHERE Name = N'عبدالمطلب أبوبكر ساسي');
                IF @mid IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TblMarketerCommissionRecords WHERE MarketerId = @mid)
                BEGIN
                    INSERT INTO TblMarketerCommissionRecords (MarketerId, SettlementDate, CreatedAt, Del)
                    VALUES (@mid, '2025-08-24', GETDATE(), 0);
                    DECLARE @rid2 INT = SCOPE_IDENTITY();
                    INSERT INTO TblMarketerCommissionItems (RecordId, FacilityName, Period, TotalPremiums, ReturnedPremiums, NetPremiums, CommissionRate, CommissionAmount)
                    VALUES (@rid2,
                        N'شركة الزويتينة للنفط',
                        N'مـن 2025/03/13م حتى 2025/12/31م',
                        103827.136, 0, 103827.136, 0.10, 10382.714);
                END
            ");

            // ========== على محمود على السريتي ==========
            // العيادة المركزية للاسنان تاجوراء | 2025/01/01 حتى 2025/06/30 | نسبة 7%
            migrationBuilder.Sql(@"
                DECLARE @mid INT = (SELECT TOP 1 Id FROM TblMarketers WHERE Name = N'على محمود على السريتي');
                IF @mid IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TblMarketerCommissionRecords WHERE MarketerId = @mid)
                BEGIN
                    INSERT INTO TblMarketerCommissionRecords (MarketerId, SettlementDate, CreatedAt, Del)
                    VALUES (@mid, '2025-09-02', GETDATE(), 0);
                    DECLARE @rid3 INT = SCOPE_IDENTITY();
                    INSERT INTO TblMarketerCommissionItems (RecordId, FacilityName, Period, TotalPremiums, ReturnedPremiums, NetPremiums, CommissionRate, CommissionAmount)
                    VALUES (@rid3,
                        N'العيادة المركزية للاسنان تاجوراء',
                        N'عن الفترة 2025/01/01م حتى 2025/06/30م',
                        76538.050, 30615.220, 45922.830, 0.07, 3214.598);
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE i FROM TblMarketerCommissionItems i
                INNER JOIN TblMarketerCommissionRecords r ON i.RecordId = r.Id
                INNER JOIN TblMarketers m ON r.MarketerId = m.Id
                WHERE m.Name IN (
                    N'رجب المسلاتي',
                    N'عبدالمطلب أبوبكر ساسي',
                    N'على محمود على السريتي'
                );
                DELETE r FROM TblMarketerCommissionRecords r
                INNER JOIN TblMarketers m ON r.MarketerId = m.Id
                WHERE m.Name IN (
                    N'رجب المسلاتي',
                    N'عبدالمطلب أبوبكر ساسي',
                    N'على محمود على السريتي'
                );
            ");
        }
    }
}
