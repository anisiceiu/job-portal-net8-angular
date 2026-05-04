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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Job entity
        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.JobId);
            entity.Property(e => e.JobTitle).IsRequired().HasMaxLength(200);
            entity.Property(e => e.JobType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.JobDescription).IsRequired();
            entity.Property(e => e.Salary).HasPrecision(18, 2);

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
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CompanyDescription).IsRequired();
            entity.Property(e => e.CompanyEmail).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CompanyContactPhone).IsRequired().HasMaxLength(20);
        });

        // Seed some initial data
        modelBuilder.Entity<Company>().HasData(
            new Company
            {
                CompanyId = 1,
                CompanyName = "Tech Solutions Inc.",
                CompanyDescription = "A leading technology company specializing in software development.",
                CompanyEmail = "info@techsolutions.com",
                CompanyContactPhone = "+1-555-0100"
            },
            new Company
            {
                CompanyId = 2,
                CompanyName = "Digital Innovations LLC",
                CompanyDescription = "Innovative digital solutions for modern businesses.",
                CompanyEmail = "contact@digitalinnovations.com",
                CompanyContactPhone = "+1-555-0200"
            }
        );

        modelBuilder.Entity<Job>().HasData(
            new Job
            {
                JobId = 1,
                JobType = "Full-time",
                JobTitle = "Senior Software Developer",
                JobDescription = "We are looking for an experienced software developer to join our team.",
                Salary = 95000.00m,
                CompanyId = 1
            },
            new Job
            {
                JobId = 2,
                JobType = "Part-time",
                JobTitle = "UI/UX Designer",
                JobDescription = "Creative designer needed for web and mobile applications.",
                Salary = 45000.00m,
                CompanyId = 2
            }
        );
    }
}
