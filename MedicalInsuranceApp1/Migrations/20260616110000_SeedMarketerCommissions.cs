using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    /// <inheritdoc />
    public partial class SeedMarketerCommissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ========== فرع الزاوية ==========
            migrationBuilder.Sql(@"
                DECLARE @mid INT = (SELECT TOP 1 Id FROM TblMarketers WHERE Name = N'فرع الزاوية');
                IF @mid IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TblMarketerCommissionRecords WHERE MarketerId = @mid)
                BEGIN
                    INSERT INTO TblMarketerCommissionRecords (MarketerId, SettlementDate, CreatedAt, Del)
                    VALUES (@mid, '2020-02-12', GETDATE(), 0);
                    DECLARE @rid1 INT = SCOPE_IDENTITY();
                    INSERT INTO TblMarketerCommissionItems (RecordId, FacilityName, Period, TotalPremiums, ReturnedPremiums, NetPremiums, CommissionRate, CommissionAmount)
                    VALUES (@rid1, N'إدارة الخدمات الصحية رقدالين', N'7ـ 8 ـ 9 ـ 10 ـ 11ـ12 /2019م', 145302.12, 72651.06, 72651.06, 0.05, 3632.55);
                END
            ");

            // ========== محمد على الدردار ==========
            migrationBuilder.Sql(@"
                DECLARE @mid INT = (SELECT TOP 1 Id FROM TblMarketers WHERE Name = N'محمد على الدردار');
                IF @mid IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TblMarketerCommissionRecords WHERE MarketerId = @mid)
                BEGIN
                    INSERT INTO TblMarketerCommissionRecords (MarketerId, SettlementDate, CreatedAt, Del)
                    VALUES (@mid, '2025-09-23', GETDATE(), 0);
                    DECLARE @rid2 INT = SCOPE_IDENTITY();
                    INSERT INTO TblMarketerCommissionItems (RecordId, FacilityName, Period, TotalPremiums, ReturnedPremiums, NetPremiums, CommissionRate, CommissionAmount)
                    VALUES
                        (@rid2, N'مركز الافق للتصوير الطبي',    N'من 2024/09/01م حتى 2024/12/31م', 3115.0,  0, 3115.0,  0.05, 155.75),
                        (@rid2, N'مصحة العرب (معمل التحاليل)',  N'من 2024/01/01م حتى 2024/12/31م', 5195.0,  0, 5195.0,  0.05, 259.75),
                        (@rid2, N'مركز الفؤاد للخدمات الطبية', N'من 2025/01/01م حتى 2025/03/30م', 12050.0, 0, 12050.0, 0.05, 602.5),
                        (@rid2, N'مركز المسار للتصوير الطبي',  N'من 2025/01/01م حتى 2025/06/30م', 2540.0,  0, 2540.0,  0.05, 127.0);
                END
            ");

            // ========== ضياء الدين الخطابي ==========
            migrationBuilder.Sql(@"
                DECLARE @mid INT = (SELECT TOP 1 Id FROM TblMarketers WHERE Name = N'ضياء الدين الخطابي');
                IF @mid IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TblMarketerCommissionRecords WHERE MarketerId = @mid)
                BEGIN
                    INSERT INTO TblMarketerCommissionRecords (MarketerId, SettlementDate, CreatedAt, Del)
                    VALUES (@mid, '2026-01-14', GETDATE(), 0);
                    DECLARE @rid3 INT = SCOPE_IDENTITY();
                    INSERT INTO TblMarketerCommissionItems (RecordId, FacilityName, Period, TotalPremiums, ReturnedPremiums, NetPremiums, CommissionRate, CommissionAmount)
                    VALUES
                        (@rid3, N'شركة رواد النخبة لطب العيون', N'من 2024/01/01م حتى 2024/12/31م', 22322.7,  0, 22322.7,  0.05, 1116.13),
                        (@rid3, N'شركة رواد النخبة لطب العيون', N'من 2025/01/01م حتى 2025/12/31م', 24749.26, 0, 24749.26, 0.05, 1237.46);
                END
            ");

            // ========== ابراهيم محمد الحداد ==========
            migrationBuilder.Sql(@"
                DECLARE @mid INT = (SELECT TOP 1 Id FROM TblMarketers WHERE Name = N'ابراهيم محمد الحداد');
                IF @mid IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TblMarketerCommissionRecords WHERE MarketerId = @mid)
                BEGIN
                    INSERT INTO TblMarketerCommissionRecords (MarketerId, SettlementDate, CreatedAt, Del)
                    VALUES (@mid, '2018-08-01', GETDATE(), 0);
                    DECLARE @rid4 INT = SCOPE_IDENTITY();
                    INSERT INTO TblMarketerCommissionItems (RecordId, FacilityName, Period, TotalPremiums, ReturnedPremiums, NetPremiums, CommissionRate, CommissionAmount)
                    VALUES
                        (@rid4, N'إدارة الخدمات الصحية بني وليد',   N'عن أشهري / يونيو ـ يوليو ـ أغسطس / 2018م', 36824.16, 0, 36824.16, 0.05, 1841.21),
                        (@rid4, N'مستشفى ابن سيناء سرت التعليمي',  N'عن شهري/ يوليو ـ أغسطس / 2018م',            6142.5,   0, 6142.5,   0.05, 307.13);
                END
            ");

            // ========== عبدالسلام على حسن العنتوت ==========
            migrationBuilder.Sql(@"
                DECLARE @mid INT = (SELECT TOP 1 Id FROM TblMarketers WHERE Name = N'عبدالسلام على حسن العنتوت');
                IF @mid IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TblMarketerCommissionRecords WHERE MarketerId = @mid)
                BEGIN
                    INSERT INTO TblMarketerCommissionRecords (MarketerId, SettlementDate, CreatedAt, Del)
                    VALUES (@mid, '2021-12-22', GETDATE(), 0);
                    DECLARE @rid5 INT = SCOPE_IDENTITY();
                    INSERT INTO TblMarketerCommissionItems (RecordId, FacilityName, Period, TotalPremiums, ReturnedPremiums, NetPremiums, CommissionRate, CommissionAmount)
                    VALUES (@rid5, N'مركز خدمات الكلى طرابلس', N'1ـ 2 ـ 3 ـ 4 ـ 5ـ 6 ـ 7 ـ 8 / 2021م', 51667.88, 0, 51667.88, 0.05, 2583.39);
                END
            ");

            // ========== عزام أسماعيل الطبولي ==========
            migrationBuilder.Sql(@"
                DECLARE @mid INT = (SELECT TOP 1 Id FROM TblMarketers WHERE Name = N'عزام أسماعيل الطبولي');
                IF @mid IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TblMarketerCommissionRecords WHERE MarketerId = @mid)
                BEGIN
                    INSERT INTO TblMarketerCommissionRecords (MarketerId, SettlementDate, CreatedAt, Del)
                    VALUES (@mid, '2025-08-17', GETDATE(), 0);
                    DECLARE @rid6 INT = SCOPE_IDENTITY();
                    INSERT INTO TblMarketerCommissionItems (RecordId, FacilityName, Period, TotalPremiums, ReturnedPremiums, NetPremiums, CommissionRate, CommissionAmount)
                    VALUES (@rid6, N'مستشفى العاصمة التخصصية', N'ـ 01 ـ 02 ـ 03 ـ 04 ـ 05 ـ 06 ـ 07 ـ 08/ 2025م', 60932.0, 0, 60932.0, 0.05, 3046.60);
                END
            ");

            // ========== فتحي الرعيض ==========
            migrationBuilder.Sql(@"
                DECLARE @mid INT = (SELECT TOP 1 Id FROM TblMarketers WHERE Name = N'فتحي الرعيض');
                IF @mid IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TblMarketerCommissionRecords WHERE MarketerId = @mid)
                BEGIN
                    INSERT INTO TblMarketerCommissionRecords (MarketerId, SettlementDate, CreatedAt, Del)
                    VALUES (@mid, '2020-03-04', GETDATE(), 0);
                    DECLARE @rid7 INT = SCOPE_IDENTITY();
                    INSERT INTO TblMarketerCommissionItems (RecordId, FacilityName, Period, TotalPremiums, ReturnedPremiums, NetPremiums, CommissionRate, CommissionAmount)
                    VALUES (@rid7, N'مستشفى زليتن التعليمي', N'عن الفترة من 2019/01م حتى 2019/12م بنسبة 5%', 110881.91, 0, 110881.91, 0.05, 5544.10);
                END
            ");

            // ========== مفتاح على مخلوف ==========
            migrationBuilder.Sql(@"
                DECLARE @mid INT = (SELECT TOP 1 Id FROM TblMarketers WHERE Name = N'مفتاح على مخلوف');
                IF @mid IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TblMarketerCommissionRecords WHERE MarketerId = @mid)
                BEGIN
                    INSERT INTO TblMarketerCommissionRecords (MarketerId, SettlementDate, CreatedAt, Del)
                    VALUES (@mid, '2022-03-01', GETDATE(), 0);
                    DECLARE @rid8 INT = SCOPE_IDENTITY();
                    INSERT INTO TblMarketerCommissionItems (RecordId, FacilityName, Period, TotalPremiums, ReturnedPremiums, NetPremiums, CommissionRate, CommissionAmount)
                    VALUES (@rid8, N'مستشفى الجلاء لامراض النساء والولادة', N'عن مارس / 2022م', 14559.84, 0, 14559.84, 0.05, 727.99);
                END
            ");

            // ========== نسرين بشير اعبيد ==========
            migrationBuilder.Sql(@"
                DECLARE @mid INT = (SELECT TOP 1 Id FROM TblMarketers WHERE Name = N'نسرين بشير اعبيد');
                IF @mid IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TblMarketerCommissionRecords WHERE MarketerId = @mid)
                BEGIN
                    INSERT INTO TblMarketerCommissionRecords (MarketerId, SettlementDate, CreatedAt, Del)
                    VALUES (@mid, '2020-06-22', GETDATE(), 0);
                    DECLARE @rid9 INT = SCOPE_IDENTITY();
                    INSERT INTO TblMarketerCommissionItems (RecordId, FacilityName, Period, TotalPremiums, ReturnedPremiums, NetPremiums, CommissionRate, CommissionAmount)
                    VALUES (@rid9, N'إدارة الخدمات الصحية جنزور', N'2020 / 03', 29900.15, 0, 29900.15, 0.05, 1495.07);
                END
            ");

            // ========== محمود أحمد البيباص ==========
            migrationBuilder.Sql(@"
                DECLARE @mid INT = (SELECT TOP 1 Id FROM TblMarketers WHERE Name = N'محمود أحمد البيباص');
                IF @mid IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TblMarketerCommissionRecords WHERE MarketerId = @mid)
                BEGIN
                    INSERT INTO TblMarketerCommissionRecords (MarketerId, SettlementDate, CreatedAt, Del)
                    VALUES (@mid, '2026-04-12', GETDATE(), 0);
                    DECLARE @rid10 INT = SCOPE_IDENTITY();
                    INSERT INTO TblMarketerCommissionItems (RecordId, FacilityName, Period, TotalPremiums, ReturnedPremiums, NetPremiums, CommissionRate, CommissionAmount)
                    VALUES (@rid10, N'مصحة الشيماء للخدمات الطبية', N'ـ10 ـ 11 ـ 12 /2025م', 41626.66, 0, 41626.66, 0.05, 2081.33);
                END
            ");

            // ========== عادل سالم اعدال ==========
            migrationBuilder.Sql(@"
                DECLARE @mid INT = (SELECT TOP 1 Id FROM TblMarketers WHERE Name = N'عادل سالم اعدال');
                IF @mid IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TblMarketerCommissionRecords WHERE MarketerId = @mid)
                BEGIN
                    INSERT INTO TblMarketerCommissionRecords (MarketerId, SettlementDate, CreatedAt, Del)
                    VALUES (@mid, '2026-06-03', GETDATE(), 0);
                    DECLARE @rid11 INT = SCOPE_IDENTITY();
                    INSERT INTO TblMarketerCommissionItems (RecordId, FacilityName, Period, TotalPremiums, ReturnedPremiums, NetPremiums, CommissionRate, CommissionAmount)
                    VALUES (@rid11, N'شركة دار الخليل للخدمات الطبية', N'من 2026/01/01م حتى 2026/03/31', 57991.57, 0, 57991.57, 0.05, 2899.58);
                END
            ");

            // ========== عبدالله محمد القنين ==========
            migrationBuilder.Sql(@"
                DECLARE @mid INT = (SELECT TOP 1 Id FROM TblMarketers WHERE Name = N'عبدالله محمد القنين');
                IF @mid IS NOT NULL AND NOT EXISTS (SELECT 1 FROM TblMarketerCommissionRecords WHERE MarketerId = @mid)
                BEGIN
                    INSERT INTO TblMarketerCommissionRecords (MarketerId, SettlementDate, CreatedAt, Del)
                    VALUES (@mid, '2026-02-19', GETDATE(), 0);
                    DECLARE @rid12 INT = SCOPE_IDENTITY();
                    INSERT INTO TblMarketerCommissionItems (RecordId, FacilityName, Period, TotalPremiums, ReturnedPremiums, NetPremiums, CommissionRate, CommissionAmount)
                    VALUES (@rid12, N'شركة زناتة للخدمات الطبية', N'من 2025/04/01م حتى 2025/12/31م', 44445.0, 0, 44445.0, 0.05, 2222.25);
                END
            ");

            // رجب المسلاتي، عبدالمطلب أبوبكر ساسي، على محمود على السريتي — لا توجد بيانات في الملفات
            // عبدالمطلب أبوبكر ساسي — لا توجد بيانات
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE i FROM TblMarketerCommissionItems i
                INNER JOIN TblMarketerCommissionRecords r ON i.RecordId = r.Id
                INNER JOIN TblMarketers m ON r.MarketerId = m.Id
                WHERE m.Name IN (
                    N'فرع الزاوية', N'محمد على الدردار', N'ضياء الدين الخطابي',
                    N'ابراهيم محمد الحداد', N'عبدالسلام على حسن العنتوت',
                    N'عزام أسماعيل الطبولي', N'فتحي الرعيض', N'مفتاح على مخلوف',
                    N'نسرين بشير اعبيد', N'محمود أحمد البيباص',
                    N'عادل سالم اعدال', N'عبدالله محمد القنين'
                );
                DELETE r FROM TblMarketerCommissionRecords r
                INNER JOIN TblMarketers m ON r.MarketerId = m.Id
                WHERE m.Name IN (
                    N'فرع الزاوية', N'محمد على الدردار', N'ضياء الدين الخطابي',
                    N'ابراهيم محمد الحداد', N'عبدالسلام على حسن العنتوت',
                    N'عزام أسماعيل الطبولي', N'فتحي الرعيض', N'مفتاح على مخلوف',
                    N'نسرين بشير اعبيد', N'محمود أحمد البيباص',
                    N'عادل سالم اعدال', N'عبدالله محمد القنين'
                );
            ");
        }
    }
}
