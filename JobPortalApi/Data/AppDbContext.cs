using Microsoft.EntityFrameworkCore;
using JobPortalApi.Models;

namespace JobPortalApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Job> Jobs { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<JobCategory> JobCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Job entity
        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.JobId);

            entity.Property(e => e.Title).IsRequired().HasMaxLength(180);
            entity.Property(e => e.JobType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.WorkMode).HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(150);

            entity.Property(e => e.SalaryMin).HasPrecision(12, 2);
            entity.Property(e => e.SalaryMax).HasPrecision(12, 2);

            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Responsibilities);
            entity.Property(e => e.Requirements);
            entity.Property(e => e.Benefits);

            entity.Property(e => e.Deadline);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(30).HasDefaultValue("Open");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            entity.Property(e => e.UpdatedAt);

            entity.HasCheckConstraint("CK_Jobs_JobType", "JobType IN ('Full-time', 'Part-time', 'Contract', 'Internship', 'Remote')");
            entity.HasCheckConstraint("CK_Jobs_WorkMode", "WorkMode IN ('On-site', 'Hybrid', 'Remote')");
            entity.HasCheckConstraint("CK_Jobs_Status", "Status IN ('Draft', 'Open', 'Closed', 'Paused')");

            // Relationship with Company
            entity.HasOne(e => e.Company)
                  .WithMany()
                  .HasForeignKey(e => e.CompanyId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(30);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            entity.HasCheckConstraint("CK_Users_Role", "Role IN ('Candidate', 'Employer', 'Admin')");
        });

        // Configure Company entity
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(180);
            entity.Property(e => e.Industry).HasMaxLength(100);
            entity.Property(e => e.Website).HasMaxLength(255);
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
            entity.Property(e => e.Description);
            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
        });

        // Seed some initial data
        modelBuilder.Entity<Company>().HasData(
            new Company
            {
                CompanyId = 1,
                CompanyName = "Tech Solutions Inc.",
                Industry = "Software",
                Website = "https://www.techsolutions.com",
                LogoUrl = "https://www.techsolutions.com/logo.png",
                Description = "A leading technology company specializing in software development.",
                Address = "123 Tech Drive",
                City = "Seattle",
                Country = "USA",
                CreatedAt = DateTime.UtcNow
            },
            new Company
            {
                CompanyId = 2,
                CompanyName = "Digital Innovations LLC",
                Industry = "Digital Services",
                Website = "https://www.digitalinnovations.com",
                LogoUrl = "https://www.digitalinnovations.com/logo.png",
                Description = "Innovative digital solutions for modern businesses.",
                Address = "456 Innovation Way",
                City = "Austin",
                Country = "USA",
                CreatedAt = DateTime.UtcNow
            }
        );

        modelBuilder.Entity<Job>().HasData(
            new Job
            {
                JobId = 1,
                JobType = "Full-time",
                Title = "Senior Software Developer",
                Description = "We are looking for an experienced software developer to join our team.",
                SalaryMin = 90000.00m,
                SalaryMax = 110000.00m,
                CompanyId = 1,
                PostedByUserId = 1
            },
            new Job
            {
                JobId = 2,
                JobType = "Part-time",
                Title = "UI/UX Designer",
                Description = "Creative designer needed for web and mobile applications.",
                SalaryMin = 40000.00m,
                SalaryMax = 50000.00m,
                CompanyId = 2,
                PostedByUserId = 1
            }
        );
    }
}
