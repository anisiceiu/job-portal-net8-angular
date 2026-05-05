namespace JobPortalApi.Models;

public class Job
{
    public int JobId { get; set; }

    // Foreign keys
    public int CompanyId { get; set; }
    public Company? Company { get; set; }
    public int PostedByUserId { get; set; }
    public int? CategoryId { get; set; }

    // Core fields
    public string Title { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty; // e.g., Full-time
    public string? WorkMode { get; set; } // On-site, Hybrid, Remote
    public string? Location { get; set; }

    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }

    public string Description { get; set; } = string.Empty;
    public string? Responsibilities { get; set; }
    public string? Requirements { get; set; }
    public string? Benefits { get; set; }

    public DateTime? Deadline { get; set; }
    public string Status { get; set; } = "Open";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
