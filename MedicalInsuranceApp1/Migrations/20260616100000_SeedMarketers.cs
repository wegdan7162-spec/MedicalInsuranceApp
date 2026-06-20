using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalInsuranceApp1.Migrations
{
    /// <inheritdoc />
    public partial class SeedMarketers : Migration
    {
        private static readonly string[] _marketers = new[]
        {
            "ابراهيم محمد الحداد",
            "رجب المسلاتي",
            "ضياء الدين الخطابي",
            "عادل سالم اعدال",
            "عبدالسلام على حسن العنتوت",
            "عبدالله محمد القنين",
            "عبدالمطلب أبوبكر ساسي",
            "عزام أسماعيل الطبولي",
            "على محمود على السريتي",
            "فتحي الرعيض",
            "فرع الزاوية",
            "محمد على الدردار",
            "محمود أحمد البيباص",
            "مفتاح على مخلوف",
            "نسرين بشير اعبيد",
        };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var name in _marketers)
            {
                var safeName = name.Replace("'", "''");
                migrationBuilder.Sql($@"
                    IF NOT EXISTS (SELECT 1 FROM TblMarketers WHERE Name = N'{safeName}')
                    INSERT INTO TblMarketers (Name, IsActive, CreatedAt, Del)
                    VALUES (N'{safeName}', 1, GETDATE(), 0);
                ");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var name in _marketers)
            {
                var safeName = name.Replace("'", "''");
                migrationBuilder.Sql($@"
                    DELETE FROM TblMarketers WHERE Name = N'{safeName}' AND Del = 0;
                ");
            }
        }
    }
}
