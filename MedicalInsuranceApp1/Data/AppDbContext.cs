using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Models.Identity;
using MedicalInsuranceApp1.Models.Entities;
namespace MedicalInsuranceApp1.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ===== جداول النظام الأصلية =====
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Court> Courts { get; set; }
        public DbSet<CourtCase> CourtCases { get; set; }
        public DbSet<CourtFile> CourtFiles { get; set; }
        public DbSet<FriendlyClaim> FriendlyClaims { get; set; }
        public DbSet<FriendlyFile> FriendlyFiles { get; set; }
        public DbSet<OutterClaim> OutterClaims { get; set; }
        public DbSet<OutterFile> OutterFiles { get; set; }
        public DbSet<PlaintiffName> PlaintiffNames { get; set; }
        public DbSet<IncidentPlace> IncidentPlaces { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseType> ExpenseTypes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<AuditEvent> AuditEvents { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // ← مهم جداً لا تحذفيه

            // ===== تخصيص أسماء جداول Identity =====
            builder.Entity<ApplicationUser>().ToTable("AppUsers");
            builder.Entity<ApplicationRole>().ToTable("AppRoles");
            builder.Entity<IdentityUserRole<string>>().ToTable("AppUserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("AppUserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("AppUserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("AppRoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("AppUserTokens");

            // حل مشكلة decimal
            builder.Entity<Expense>()
                .Property(x => x.Amount)
                .HasColumnType("decimal(18,2)");


            // ===== إعدادات Branch =====
            builder.Entity<Branch>(e => {
                e.ToTable("TblBranch");
                e.HasKey(x => x.Id);
                e.Property(x => x.BranchName).HasMaxLength(500).IsRequired();
            });

            // ===== إعدادات Court =====
            builder.Entity<Court>(e => {
                e.ToTable("TblCourt1");
                e.HasKey(x => x.Id);
                e.Property(x => x.CourtName).HasMaxLength(500).IsRequired();
            });

            // ===== Soft Delete Filter =====
            // Branch: no filter — Del values may be NULL from legacy data
            builder.Entity<PlaintiffName>()
                .HasQueryFilter(x => x.Del != true);
            builder.Entity<IncidentPlace>()
                .HasQueryFilter(x => x.Del != true);

            // ===== Indexes =====
            builder.Entity<ApplicationUser>()
                .HasIndex(x => x.OldUserId)
                .HasDatabaseName("IX_AppUsers_OldUserId");
            builder.Entity<CourtCase>()
                .HasIndex(x => new { x.Year, x.Num })
                .IsUnique()
                .HasDatabaseName("IX_CourtCases_Year_Num");
        }
    }
}