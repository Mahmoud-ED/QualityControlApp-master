using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models;


namespace QualityControlApp.Models
{
    //public class DataContext : DbContext
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        private readonly IServiceProvider _serviceProvider;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options
                                    , IServiceProvider serviceProvider) : base(options)
        {
            _serviceProvider = serviceProvider;
        }


        public DbSet<Contact> Contact { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<QuestionType> QuestionType { get; set; }
        public DbSet<CompanyQuestion> CompanyQuestion { get; set; }
        public DbSet<CompanyQuestionContent> CompanyQuestionContent { get; set; }
        public DbSet<QuestionCategoryType> QuestionCategoryType { get; set; }

        public DbSet<SiteInfo> SiteInfo { get; set; }
        public DbSet<SiteState> SiteState { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public object Company { get; internal set; }

        public DbSet<AirPortRequest> AirPortRequests { get; set; }
        public DbSet<AirPortRequestFiles> AirPortRequestFiles { get; set; }
        public DbSet<FileType> FileType { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contact>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<SiteInfo>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<SiteState>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<UserProfile>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Employee>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<QuestionType>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Question>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<CompanyQuestion>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<CompanyQuestionContent>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<QuestionCategoryType>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<AirPortRequest >().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<AirPortRequestFiles >().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<FileType >().Property(x => x.Id).HasDefaultValueSql("NEWID()");

            modelBuilder.Seed(_serviceProvider);

            //modelBuilder.Entity<UserProfile>()
            //    .HasOne(up => up.ApplicationUser )
            //    .WithMany()
            //    .HasForeignKey(up => up.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.Employee)
                .WithOne(b => b.ApplicationUser)
                .HasForeignKey<Employee>(b => b.UserId)
                .OnDelete(DeleteBehavior.NoAction);  // منع الحذف التلقائي

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.CompanyQuestion)
                .WithOne(b => b.ApplicationUser)
                .HasForeignKey<CompanyQuestion>(b => b.UserId)
                .OnDelete(DeleteBehavior.NoAction);  // منع الحذف التلقائي

            modelBuilder.Entity<CompanyQuestion>()
                .HasOne(a => a.Company)
                .WithMany(b => b.CompanyQuestions)
                .HasForeignKey(a => a.CompanyId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AirPortRequest>()
    .HasMany(r => r.RequestFiles)
    .WithOne(f => f.AirPortRequest)
    .HasForeignKey(f => f.AirPortRequestId)
    .OnDelete(DeleteBehavior.Cascade);

            // تكوين العلاقة بين AirPortRequest و ApplicationUser
            modelBuilder.Entity<AirPortRequest>()
                .HasOne(r => r.ApplicationUser)
                .WithMany(u => u.AirportRequest)
                .HasForeignKey(r => r.ApproverUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);


        }
    }
}
