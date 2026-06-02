using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Identity;
using MedicalInsuranceApp1.Infrastrcture;

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

var app = builder.Build();



// ===== Migrate + Seed =====
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // تطبيق أي migrations لم تُطبّق بعد
    var db = services.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    await SeedData.InitializeAsync(roleManager, userManager);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

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

app.MapControllerRoute(
    name: "courtcases",
    pattern: "CourtCases/{action=Index}/{id?}",
    defaults: new { controller = "CourtCases" });

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

app.Run();
