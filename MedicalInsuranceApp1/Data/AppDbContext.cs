using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Models.ViewModels.Identity;
namespace MedicalInsuranceApp1.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ===== جداول النظام الأصلية =====
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Court> Courts { get; set; }
       // public DbSet<CourtCase> CourtCases { get; set; }
        //public DbSet<CourtFile> CourtFiles { get; set; }
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
        public DbSet<RolePermission> RolePermissions { get; set; }

        // ===== التسويات =====
        public DbSet<ReservationSettlement> ReservationSettlements { get; set; }
        public DbSet<CaseFeeSettlement> CaseFeeSettlements { get; set; }
        public DbSet<PrivateLawyerSettlement> PrivateLawyerSettlements { get; set; }

        // ===== المصارف وفروعها =====
        public DbSet<Bank> Banks { get; set; }
        public DbSet<BankBranch> BankBranches { get; set; }

        // ===== الجزء المالي =====
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<BankTransaction> BankTransactions { get; set; }
        public DbSet<Commission> Commissions { get; set; }

        // ===== وحدة الاكتتاب =====
        public DbSet<InsuranceContract>      InsuranceContracts      { get; set; }
        public DbSet<SupplyOrder>            SupplyOrders            { get; set; }
        public DbSet<ReceiptVoucher>         ReceiptVouchers         { get; set; }
        public DbSet<PaymentAuthorization>   PaymentAuthorizations   { get; set; }

        // ===== وحدة المسوقين =====
        public DbSet<Marketer>                   Marketers                   { get; set; }
        public DbSet<MarketerCommissionRecord>   MarketerCommissionRecords   { get; set; }
        public DbSet<MarketerCommissionItem>     MarketerCommissionItems     { get; set; }

        // ===== وحدة الموردين =====
        public DbSet<Supplier> Suppliers { get; set; }

        // ===== كشف المؤمن عليهم =====
        public DbSet<InsuredEmployee> InsuredEmployees { get; set; }

        // ===== قسيمة الإيداع المصرفي =====
        public DbSet<BankDepositSlip> BankDepositSlips { get; set; }

        // ===== القيود المحاسبية =====
        public DbSet<AccountingEntry>     AccountingEntries     { get; set; }
        public DbSet<AccountingEntryLine> AccountingEntryLines  { get; set; }

        // ===== اليومية =====
        public DbSet<DailyJournal>     DailyJournals     { get; set; }
        public DbSet<DailyJournalItem> DailyJournalItems { get; set; }

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
            //builder.Entity<CourtCase>()
            //    .HasIndex(x => new { x.Year, x.Num })
            //    .IsUnique()
            //    .HasDatabaseName("IX_CourtCases_Year_Num");

            // ===== إعدادات الجزء المالي =====

            builder.Entity<BankAccount>(e =>
            {
                e.Property(x => x.OpeningBalance).HasColumnType("decimal(18,3)");
                e.Property(x => x.CurrentBalance).HasColumnType("decimal(18,3)");
            });

            builder.Entity<BankTransaction>(e =>
            {
                e.Property(x => x.DebitAmount).HasColumnType("decimal(18,3)");
                e.Property(x => x.CreditAmount).HasColumnType("decimal(18,3)");
                e.Property(x => x.RunningBalance).HasColumnType("decimal(18,3)");

                // منع cascade على الطرفين المتعددة (FriendlyClaim, OutterClaim)
                e.HasOne(x => x.FriendlyClaim)
                 .WithMany()
                 .HasForeignKey(x => x.FriendlyClaimId)
                 .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(x => x.OutterClaim)
                 .WithMany()
                 .HasForeignKey(x => x.OutterClaimId)
                 .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(x => x.Commission)
                 .WithMany(c => c.Transactions)
                 .HasForeignKey(x => x.CommissionId)
                 .OnDelete(DeleteBehavior.SetNull);

                // index على التاريخ لتسريع الاستعلامات الشهرية
                e.HasIndex(x => x.TransactionDate)
                 .HasDatabaseName("IX_BankTransactions_Date");

                e.HasIndex(x => x.BankAccountId)
                 .HasDatabaseName("IX_BankTransactions_AccountId");
            });

            builder.Entity<Commission>(e =>
            {
                e.Property(x => x.PremiumAmount).HasColumnType("decimal(18,3)");
                e.Property(x => x.CommissionRate).HasColumnType("decimal(5,2)");
                e.Property(x => x.CommissionAmount).HasColumnType("decimal(18,3)");
            });

            // ===== إعدادات وحدة الاكتتاب =====

            builder.Entity<InsuranceContract>(e =>
            {
                e.Property(x => x.PrivatePremium).HasColumnType("decimal(18,3)");
                e.Property(x => x.PublicPremium).HasColumnType("decimal(18,3)");
                e.Property(x => x.SupervisionFee).HasColumnType("decimal(18,3)");
                e.Property(x => x.PrepaidAmount).HasColumnType("decimal(18,3)");
                e.Property(x => x.UnderCollectionAmount).HasColumnType("decimal(18,3)");
                e.Property(x => x.TreasuryAmount).HasColumnType("decimal(18,3)");
            });

            builder.Entity<SupplyOrder>(e =>
            {
                e.Property(x => x.Amount).HasColumnType("decimal(18,3)");
                e.HasOne(x => x.Contract)
                 .WithMany(c => c.SupplyOrders)
                 .HasForeignKey(x => x.ContractId)
                 .OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.Receipt)
                 .WithOne(r => r.SupplyOrder)
                 .HasForeignKey<ReceiptVoucher>(r => r.SupplyOrderId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<ReceiptVoucher>(e =>
            {
                e.Property(x => x.DinarAmount).HasColumnType("decimal(18,3)");
                e.Property(x => x.DirhamAmount).HasColumnType("decimal(5,3)");
                e.Property(x => x.TotalAmount).HasColumnType("decimal(18,3)");
                e.Property(x => x.SupervisionFee).HasColumnType("decimal(18,3)");
                e.Property(x => x.TreasuryAmount).HasColumnType("decimal(18,3)");
                e.HasOne(x => x.Contract)
                 .WithMany(c => c.Receipts)
                 .HasForeignKey(x => x.ContractId)
                 .OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.BankAccount)
                 .WithMany()
                 .HasForeignKey(x => x.BankAccountId)
                 .OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.LinkedTransaction)
                 .WithMany()
                 .HasForeignKey(x => x.LinkedTransactionId)
                 .OnDelete(DeleteBehavior.NoAction);
                e.HasIndex(x => x.ReceiptDate)
                 .HasDatabaseName("IX_ReceiptVouchers_Date");
            });

            builder.Entity<PaymentAuthorization>(e =>
            {
                e.Property(x => x.Amount).HasColumnType("decimal(18,3)");
                e.HasOne(x => x.BankAccount)
                 .WithMany()
                 .HasForeignKey(x => x.BankAccountId)
                 .OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.Commission)
                 .WithMany()
                 .HasForeignKey(x => x.CommissionId)
                 .OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.LinkedTransaction)
                 .WithMany()
                 .HasForeignKey(x => x.LinkedTransactionId)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            // ===== كشف المؤمن عليهم =====
            builder.Entity<InsuredEmployee>(e =>
            {
                e.Property(x => x.MonthlySalary).HasColumnType("decimal(18,3)");
                e.Property(x => x.EntityRate).HasColumnType("decimal(5,4)");
                e.Property(x => x.EmployeeRate).HasColumnType("decimal(5,4)");
                e.Property(x => x.EntityShare).HasColumnType("decimal(18,3)");
                e.Property(x => x.EmployeeShare).HasColumnType("decimal(18,3)");
                e.Property(x => x.TotalPremium).HasColumnType("decimal(18,3)");
                e.HasOne(x => x.Contract)
                 .WithMany(c => c.InsuredEmployees)
                 .HasForeignKey(x => x.ContractId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasIndex(x => x.ContractId)
                 .HasDatabaseName("IX_InsuredEmployees_ContractId");
            });

            // ===== قسيمة الإيداع المصرفي =====
            builder.Entity<BankDepositSlip>(e =>
            {
                e.Property(x => x.Amount).HasColumnType("decimal(18,3)");
                e.HasOne(x => x.BankAccount)
                 .WithMany()
                 .HasForeignKey(x => x.BankAccountId)
                 .OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.ReceiptVoucher)
                 .WithMany()
                 .HasForeignKey(x => x.ReceiptVoucherId)
                 .OnDelete(DeleteBehavior.SetNull);
                e.HasIndex(x => x.DepositDate)
                 .HasDatabaseName("IX_BankDepositSlips_Date");
            });

            // ===== القيود المحاسبية =====
            builder.Entity<AccountingEntry>(e =>
            {
                e.Property(x => x.DebitAmount).HasColumnType("decimal(18,3)");
                e.Property(x => x.BankDebitAmount).HasColumnType("decimal(18,3)");
                e.HasOne(x => x.SupplyOrder)
                 .WithMany()
                 .HasForeignKey(x => x.SupplyOrderId)
                 .OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.BankDepositSlip)
                 .WithMany()
                 .HasForeignKey(x => x.BankDepositSlipId)
                 .OnDelete(DeleteBehavior.SetNull);
                e.HasIndex(x => x.EntryDate)
                 .HasDatabaseName("IX_AccountingEntries_Date");
            });

            // ===== بنود القيود المحاسبية =====
            builder.Entity<AccountingEntryLine>(e =>
            {
                e.Property(x => x.Debit).HasColumnType("decimal(18,3)");
                e.Property(x => x.Credit).HasColumnType("decimal(18,3)");
                e.HasOne(x => x.Entry)
                 .WithMany(a => a.Lines)
                 .HasForeignKey(x => x.EntryId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== اليومية =====
            builder.Entity<DailyJournal>(e =>
            {
                e.Property(x => x.TotalAmount).HasColumnType("decimal(18,3)");
                e.HasIndex(x => x.JournalDate)
                 .HasDatabaseName("IX_DailyJournals_Date");
            });

            builder.Entity<DailyJournalItem>(e =>
            {
                e.Property(x => x.Amount).HasColumnType("decimal(18,3)");
                e.HasOne(x => x.Journal)
                 .WithMany(j => j.Items)
                 .HasForeignKey(x => x.JournalId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.SupplyOrder)
                 .WithMany()
                 .HasForeignKey(x => x.SupplyOrderId)
                 .OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.ReceiptVoucher)
                 .WithMany()
                 .HasForeignKey(x => x.ReceiptVoucherId)
                 .OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.BankDepositSlip)
                 .WithMany()
                 .HasForeignKey(x => x.BankDepositSlipId)
                 .OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.AccountingEntry)
                 .WithMany()
                 .HasForeignKey(x => x.AccountingEntryId)
                 .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}