using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Infrastrcture;
using MedicalInsuranceApp1.Models.Interfaces;
using MedicalInsuranceApp1.Models.Settings;
using MedicalInsuranceApp1.Services.Implementations;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using MedicalInsuranceApp1.Models.ViewModels.Identity;

var builder = WebApplication.CreateBuilder(args);

// ===== Database =====
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MedicalClaimsConnection")));

// ===== Identity =====
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;
    options.User.RequireUniqueEmail = false;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ===== Cookie =====
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
});

// ===== Services =====
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
    options.Filters.AddService<ActivityLogFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddScoped<UserMigrationService>();
builder.Services.AddScoped<ActivityLogFilter>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

// ===== Mail =====
var mailSettings = builder.Configuration.GetSection("MailSettings").Get<MailSettings>() ?? new MailSettings();
builder.Services.AddSingleton(mailSettings);
builder.Services.AddScoped<IEmailSender, EmailSender>();

var app = builder.Build();



// ===== Migrate + Seed =====
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // تطبيق أي migrations لم تُطبّق بعد
    var db = services.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    // إضافة أعمدة جديدة إن لم تكن موجودة (safe fallback)
    await db.Database.ExecuteSqlRawAsync(@"
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                       WHERE TABLE_NAME='TblBanks' AND COLUMN_NAME='BankCode')
            ALTER TABLE TblBanks ADD BankCode nvarchar(50) NULL;

        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                       WHERE TABLE_NAME='TblBankBranches' AND COLUMN_NAME='BranchCode')
            ALTER TABLE TblBankBranches ADD BranchCode nvarchar(50) NULL;
    ");

    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    await SeedData.InitializeAsync(roleManager, userManager, db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ===== Localization (لحل مشكلة parsing الأرقام العشرية) =====
var invariantCulture = CultureInfo.InvariantCulture;
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(invariantCulture),
    SupportedCultures = new[] { invariantCulture },
    SupportedUICultures = new[] { CultureInfo.GetCultureInfo("ar-SA") }
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// ===== Routes =====
app.MapControllerRoute(
    name: "migration",
    pattern: "Migration/{action=Index}/{id?}",
    defaults: new { controller = "Migration" });

app.MapControllerRoute(
    name: "users",
    pattern: "Users/{action=Index}/{id?}",
    defaults: new { controller = "Users" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "roles",
    pattern: "Roles/{action=Index}/{id?}",
    defaults: new { controller = "Roles" });

//app.MapControllerRoute(
//    name: "courtcases",
//    pattern: "CourtCases/{action=Index}/{id?}",
//    defaults: new { controller = "CourtCases" });

app.MapControllerRoute(
    name: "friendlyclaims",
    pattern: "FriendlyClaims/{action=Index}/{id?}",
    defaults: new { controller = "FriendlyClaims" });

app.MapControllerRoute(
    name: "outterClaims",
    pattern: "OutterClaims/{action=Index}/{id?}",
    defaults: new { controller = "OutterClaims" });

app.MapControllerRoute(
    name: "branches",
    pattern: "Branches/{action=Index}/{id?}",
    defaults: new { controller = "Branches" });

app.MapControllerRoute(
    name: "courts",
    pattern: "Courts/{action=Index}/{id?}",
    defaults: new { controller = "Courts" });


app.MapControllerRoute(
    name: "plaintiffs",
    pattern: "Plaintiffs/{action=Index}/{id?}",
    defaults: new { controller = "Plaintiffs" });



app.MapControllerRoute(
    name: "trash",
    pattern: "Trash/{action=OutterClaims}/{id?}",
    defaults: new { controller = "Trash" });

app.MapControllerRoute(
    name: "activity",
    pattern: "Activity/{action=Index}/{id?}",
    defaults: new { controller = "Activity" });

app.MapControllerRoute(
    name: "reports",
    pattern: "Reports/{action=Index}/{id?}",
    defaults: new { controller = "Reports" });

// ===== المصارف =====
app.MapControllerRoute(
    name: "banks",
    pattern: "Banks/{action=Index}/{id?}",
    defaults: new { controller = "Banks" });

// ===== الجزء المالي =====
app.MapControllerRoute(
    name: "bankaccounts",
    pattern: "BankAccounts/{action=Index}/{id?}",
    defaults: new { controller = "BankAccounts" });

app.MapControllerRoute(
    name: "banktransactions",
    pattern: "BankTransactions/{action=Index}/{id?}",
    defaults: new { controller = "BankTransactions" });

app.MapControllerRoute(
    name: "commissions",
    pattern: "Commissions/{action=Index}/{id?}",
    defaults: new { controller = "Commissions" });

app.MapControllerRoute(
    name: "import",
    pattern: "Import/{action=Index}/{id?}",
    defaults: new { controller = "Import" });

// ===== الاكتتاب =====
app.MapControllerRoute(
    name: "insurancecontracts",
    pattern: "InsuranceContracts/{action=Index}/{id?}",
    defaults: new { controller = "InsuranceContracts" });

app.MapControllerRoute(
    name: "supplyorders",
    pattern: "SupplyOrders/{action=Index}/{id?}",
    defaults: new { controller = "SupplyOrders" });

app.MapControllerRoute(
    name: "receiptvouchers",
    pattern: "ReceiptVouchers/{action=Index}/{id?}",
    defaults: new { controller = "ReceiptVouchers" });

app.MapControllerRoute(
    name: "paymentauthorizations",
    pattern: "PaymentAuthorizations/{action=Index}/{id?}",
    defaults: new { controller = "PaymentAuthorizations" });

app.MapControllerRoute(
    name: "issuancestats",
    pattern: "IssuanceStats/{action=Index}/{id?}",
    defaults: new { controller = "IssuanceStats" });

// ===== المسوقون =====
app.MapControllerRoute(
    name: "marketers",
    pattern: "Marketers/{action=Index}/{id?}",
    defaults: new { controller = "Marketers" });

// ===== الموردون =====
app.MapControllerRoute(
    name: "suppliers",
    pattern: "Suppliers/{action=Index}/{id?}",
    defaults: new { controller = "Suppliers" });

// ===== القيود المحاسبية =====
app.MapControllerRoute(
    name: "accountingentries",
    pattern: "AccountingEntries/{action=Index}/{id?}",
    defaults: new { controller = "AccountingEntries" });

// ===== قسائم الإيداع =====
app.MapControllerRoute(
    name: "bankdepositslips",
    pattern: "BankDepositSlips/{action=Index}/{id?}",
    defaults: new { controller = "BankDepositSlips" });

// ===== اليومية =====
app.MapControllerRoute(
    name: "dailyjournal",
    pattern: "DailyJournal/{action=Index}/{id?}",
    defaults: new { controller = "DailyJournal" });

app.Run();
