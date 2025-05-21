using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QualityControlApp.Models.Entities;


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
        public DbSet<CompanyQuestionAssignedUsers> CompanyQuestionAssignedUsers { get; set; }

        public DbSet<SiteInfo> SiteInfo { get; set; }
        public DbSet<SiteState> SiteState { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<BookingAppointment> BookingAppointment { get; set; }
        public object Company { get; internal set; }

        public DbSet<AirPortRequest> AirPortRequests { get; set; }
        public DbSet<Landing> Landing { get; set; }
        public DbSet<AirPortRequestFiles> AirPortRequestFiles { get; set; }
        public DbSet<CompanyTypeCategoryAvailable> CompanyTypeCategoryAvailable { get; set; }
        public DbSet<FileType> FileType { get; set; }
        public DbSet<CompanyType> CompanyType { get; set; }

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
            modelBuilder.Entity<Landing>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<AirPortRequestFiles >().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<FileType >().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<BookingAppointment >().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Location>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<CompanyType>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<CompanyTypeCategoryAvailable>().Property(x => x.Id).HasDefaultValueSql("NEWID()");

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



            modelBuilder.Entity<BookingAppointment>()
              .HasOne(a => a.Company)
              .WithMany(b => b.BookingAppointment)
              .HasForeignKey(a => a.CompanyId)
              .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AirPortRequest>()
    .HasMany(r => r.RequestFiles)
    .WithOne(f => f.AirPortRequest)
    .HasForeignKey(f => f.AirPortRequestId)
    .OnDelete(DeleteBehavior.Cascade);

 //           modelBuilder.Entity<Landing>()
 //.HasMany(r => r.RequestFiles)
 //.WithOne(f => f.Landing)
 //.HasForeignKey(f => f.AirPortRequestId)
 //.OnDelete(DeleteBehavior.Cascade);

            // تكوين العلاقة بين AirPortRequest و ApplicationUser
            modelBuilder.Entity<AirPortRequest>()
                .HasOne(r => r.ApplicationUser)
                .WithMany(u => u.AirportRequest)
                .HasForeignKey(r => r.ApproverUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);



            modelBuilder.Entity<CompanyTypeCategoryAvailable>()
                           .HasOne(cta => cta.CompanyType)
                           .WithMany(ct => ct.AvailableCategories)
                           .HasForeignKey(cta => cta.CompanyTypeId)
                           .OnDelete(DeleteBehavior.Cascade);

          


            modelBuilder.Entity<Company>()
       .HasOne(c => c.CompanyType)
       .WithMany(ct => ct.Company)
       .HasForeignKey(c => c.CompanyTypeId)
       .OnDelete(DeleteBehavior.SetNull); // أو DeleteBehavior.Restrict حسب ما يناسبك



            modelBuilder.Entity<CompanyQuestionAssignedUsers>()
    .HasKey(x => new { x.AssignedCompanyQuestionsId, x.AssignedUsersId });

            modelBuilder.Entity<CompanyQuestionAssignedUsers>()
                .HasOne(x => x.CompanyQuestion)
                .WithMany() // أو .WithMany(x => x.AssignedUsersLink) لو أضفت navigation
                .HasForeignKey(x => x.AssignedCompanyQuestionsId);

            modelBuilder.Entity<CompanyQuestionAssignedUsers>()
                .HasOne(x => x.User)
                .WithMany() // أو .WithMany(x => x.AssignedCompanyQuestionsLink)
                .HasForeignKey(x => x.AssignedUsersId);


        //    modelBuilder.Entity<CompanyQuestion>()
        //.HasOne(q => q.Creator)
        //.WithMany(u => u.CreatedCompanyQuestions)
        //.HasForeignKey(q => q.CreatorId) // <-- استخدم اسم الخاصية المعدل
        //.OnDelete(DeleteBehavior.Restrict);









        }

    }


}
